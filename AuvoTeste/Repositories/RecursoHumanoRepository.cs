using AuvoTeste.Models;
using AuvoTeste.Repositories.Interface;
using System.Text;

namespace AuvoTeste.Repositories
{
    public class RecursoHumanoRepository : IRecursoHumanoRepository
    {
        public async Task<string> ProcessarArquivos(UploadArquivo model)
        {
            string caminhoPasta = await Task.Run(() => CriarPastaEArmazenarArquivos(model));
            await Task.Run(() => CarregarPasta(caminhoPasta));

            return "Processamento de arquivos concluído.";
        }

        public string CriarPastaEArmazenarArquivos(UploadArquivo model)
        {
            int contadorPasta = 1;
            string diretorioAtual = Directory.GetCurrentDirectory();
            string caminhoDestino = Path.Combine(diretorioAtual, "ArquivosCvsTemporarios");
            string dataHoraAtual = DateTime.Now.ToString("yyyyMMdd");
            string caminhoCompleto = Path.Combine(caminhoDestino, $"{contadorPasta}_{dataHoraAtual}");

            while (Directory.Exists(caminhoCompleto))
            {
                contadorPasta++;
                caminhoCompleto = Path.Combine(caminhoDestino, $"{contadorPasta}_{dataHoraAtual}");
            }

            Directory.CreateDirectory(caminhoCompleto);

            if (model.Arquivos != null)
            {
                foreach (var arquivo in model.Arquivos)
                {
                    string nomeArquivo = Path.GetFileName(arquivo.FileName);
                    DadosTempDepartamento.NomeArquivo = arquivo.FileName.Split("/")[0];

                    var caminhoCompletoArquivo = Path.Combine(caminhoCompleto, nomeArquivo);

                    using var stream = new FileStream(caminhoCompletoArquivo, FileMode.Create);
                    arquivo.CopyTo(stream);
                }
            }
            return caminhoCompleto;
        }
        public async Task CarregarPasta(string caminhoPasta)
        {
            DirectoryInfo Dir = new(caminhoPasta);
            FileInfo[] Files = Dir.GetFiles("*", SearchOption.AllDirectories);
            DadosTempDepartamento.Departamento = new List<Departamento>();
            DadosTempDepartamento.QuantidadeArquivo = Files.Length;

            var tasks = Files.Select(file => CarregarDados(file));
            await Task.WhenAll(tasks);

        }
        public async Task CarregarDados(FileInfo file)
        {
            Departamento departamentoModel = new();
            string fileName = file.Name;
            var fileNameArray = fileName.Split(".")[0].Split("-");
            departamentoModel.Departamentos = fileNameArray[0];
            departamentoModel.MesVigencia = fileNameArray[1];
            departamentoModel.AnoVigencia = int.Parse(fileNameArray[2]);

            List<FuncionarioCsvDados> funcionarioCsvModel = new List<FuncionarioCsvDados>();

            using (StreamReader reader = new StreamReader(file.FullName, Encoding.Latin1, true))
            {
                var linha = reader.ReadLine();
                while (true)
                {
                    linha = reader.ReadLine();
                    if (linha == null) break;
                    funcionarioCsvModel.Add(new FuncionarioCsvDados(linha));
                }
            }

            var funcionariosViewModel = await Task.Run(() => CalcularDados(funcionarioCsvModel));

            departamentoModel.Funcionarios = funcionariosViewModel;

            DadosTempDepartamento.Departamento?.Add(departamentoModel);
        }

        public List<Funcionario> CalcularDados(List<FuncionarioCsvDados> funcionarios)
        {

            DateTime dataInicial = DateTime.Parse(funcionarios[0].Data);
            dataInicial = dataInicial.AddDays(1 - dataInicial.Day);
            int diasUteis = 0;
            int mes = dataInicial.Month;

            while (dataInicial.Month == mes)
            {
                if (dataInicial.DayOfWeek != DayOfWeek.Saturday && dataInicial.DayOfWeek != DayOfWeek.Sunday)
                    diasUteis++;
                dataInicial = dataInicial.AddDays(1);
            }

            var funcionariosAgrupados = funcionarios.GroupBy(f => f.Codigo);
            var funcionariosListaModel = new List<Funcionario>();

            foreach (var grupoFuncionarios in funcionariosAgrupados)
            {
                var primeiroFuncionario = grupoFuncionarios.First();

                if (string.IsNullOrEmpty(primeiroFuncionario.Nome))
                    continue;

                var valorHoraStr = primeiroFuncionario.ValorHora.Replace("R$", "").Replace(" ", "");
                double valorHora = double.Parse(valorHoraStr);

                var funcionarioModel = new Funcionario
                {
                    Nome = primeiroFuncionario.Nome,
                    Codigo = int.Parse(primeiroFuncionario.Codigo),
                    ValorHora = valorHora
                };

                foreach (var linhaCSV in grupoFuncionarios)
                {
                    DateTime data = DateTime.Parse(linhaCSV.Data);
                    string diaSemana = data.DayOfWeek.ToString();

                    funcionarioModel.DiasTrabalhados++;

                    TimeSpan entrada = TimeSpan.Parse(linhaCSV.Entrada);
                    TimeSpan saida = TimeSpan.Parse(linhaCSV.Saida);

                    var almoco = linhaCSV.Almoco.Replace(" ", "").Split("-");

                    TimeSpan entradaAlmoco = TimeSpan.Parse(almoco[0]);
                    TimeSpan saidaAlmoco = TimeSpan.Parse(almoco[1]);

                    int horasTrabalhadas = saida.Hours - entrada.Hours - (saidaAlmoco.Hours - entradaAlmoco.Hours);

                    funcionarioModel.AdicionarValorRecebido(horasTrabalhadas * valorHora);

                    if (diaSemana == "Saturday" || diaSemana == "Sunday")
                    {
                        funcionarioModel.DiasExtras++;
                        funcionarioModel.AdicionarHorasExtras(horasTrabalhadas);
                    }
                    else
                    {
                        if (horasTrabalhadas > 8)
                            funcionarioModel.AdicionarHorasExtras(horasTrabalhadas - 8);
                        else if (horasTrabalhadas < 8)
                            funcionarioModel.AdicionarHorasDebito(8 - horasTrabalhadas);
                    }
                }

                var diasFalta = diasUteis - (funcionarioModel.DiasTrabalhados - funcionarioModel.DiasExtras);

                if (diasFalta > 0)
                {
                    funcionarioModel.DiasFalta = diasFalta;
                    funcionarioModel.AdicionarHorasDebito(diasFalta * 8);
                }

                funcionariosListaModel.Add(funcionarioModel);
            }

            return funcionariosListaModel.OrderBy(f => f.Codigo).ToList();

        }
    }
}