namespace AuvoTeste.Models
{
    public static class DadosTempDepartamento
    {
        public static List<Departamento>? Departamento { get; set; }
        public static string? NomeArquivo { get; set; }
        public static double QuantidadeArquivo { get; set; }
        public static double PorcentagemAtual => QuantidadeArquivo > 0 ? (Departamento?.Count ?? 0) / QuantidadeArquivo * 100 : 0;

    }
}
