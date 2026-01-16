namespace Calendario.Modelos
{
    public class FeriadoDto
    {
        public DateTime Date { get; set; }

        // Añadimos "= string.Empty;" para que C# sepa que no será nulo
        public string LocalName { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string CountryCode { get; set; } = string.Empty;
    }
}