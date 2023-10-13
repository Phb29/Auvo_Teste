namespace AuvoTeste.Models
{
    public class Departamento
    {
        public Departamento() => Funcionarios = new List<Funcionario>();
        public string? Departamentos { get; set; }
        public string? MesVigencia { get; set; }
        public int AnoVigencia { get; set; }

        public double TotalPagar => ObterTotalPagar();
        public double TotalDescontos => ObterTotalDescontos();
        public double TotalExtras => ObterTotalExtras();

        private double ObterTotalPagar()
        {
            return Math.Round(Funcionarios.Sum(f => f.TotalReceber), 1);
        }

        private double ObterTotalDescontos()
        {
            return Math.Round(Funcionarios.Sum(f => f.ValorHora * f.HorasDebito), 1);
        }

        private double ObterTotalExtras()
        {
            return Math.Round(Funcionarios.Sum(f => f.ValorHora * f.HorasExtras), 1);
        }
        public List<Funcionario> Funcionarios { get; set; }
    }
}
