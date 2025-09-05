namespace PrototipoApi.Models.GammaAI
{
    public sealed class RentabilidadResponse
    {
        public double Ingresos { get; init; }
        public double Gastos { get; init; }
        public double Rentabilidad { get; init; } // % (0..100)
    }
}
