namespace AuvoTeste.Models
{
    public class FuncionarioCsvDados
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public string ValorHora { get; set; }
        public string Data { get; set; }
        public string Entrada { get; set; }
        public string Saida { get; set; }
        public string Almoco { get; set; }

        public FuncionarioCsvDados(string linha)
        {
            var dadosLinha = linha.Split(';');
            Codigo = dadosLinha[0];
            Nome = dadosLinha[1];
            ValorHora = dadosLinha[2];
            Data = dadosLinha[3];
            Entrada = dadosLinha[4];
            Saida = dadosLinha[5];
            Almoco = dadosLinha[6];
        }
    }
}
