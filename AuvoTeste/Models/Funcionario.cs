namespace AuvoTeste.Models
{
    public class Funcionario
    {
        public string? Nome { get; set; }
        public int Codigo { get; set; }
        public double TotalReceber => CalcularTotalRecebido();
        public double HorasExtras => CalcularHorasExtras();
        public double HorasDebito => CalcularHorasDebito();

        public int DiasFalta { get; set; }
        public int DiasExtras { get; set; }
        public int DiasTrabalhados { get; set; }
        internal double ValorHora { get; set; }

        private double totalRecebido = 0;
        private double horasExtrasAcumuladas = 0;
        private double horasDebitoAcumuladas = 0;

        private double CalcularTotalRecebido()
        {
            return Math.Round(totalRecebido, 1);
        }

        private double CalcularHorasExtras()
        {
            return Math.Round(horasExtrasAcumuladas, 2);
        }

        private double CalcularHorasDebito()
        {
            return Math.Round(horasDebitoAcumuladas, 2);
        }
        public void AdicionarValorRecebido(double valor)
        {
            totalRecebido += valor;
        }

        public void AdicionarHorasExtras(double valor)
        {
            horasExtrasAcumuladas += valor;
        }

        public void AdicionarHorasDebito(double valor)
        {
            horasDebitoAcumuladas += valor;
        }
    }
}
