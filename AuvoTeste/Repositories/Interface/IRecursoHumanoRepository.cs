using AuvoTeste.Models;

namespace AuvoTeste.Repositories.Interface
{
    public interface IRecursoHumanoRepository
    {
        Task<string> ProcessarArquivos(UploadArquivo model);
        string? CriarPastaEArmazenarArquivos(UploadArquivo model);
        Task CarregarPasta(string pathFolder);
        Task CarregarDados(FileInfo file);
        List<Funcionario> CalcularDados(List<FuncionarioCsvDados> funcionarios);
    }
}
