namespace Calendario.Modelos
{
    public class FestivosDto
    {
        public DateTime Date { get; set; }

        public string LocalName { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string CountryCode { get; set; } = string.Empty;
    }
}