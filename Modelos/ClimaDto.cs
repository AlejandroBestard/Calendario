using System.Text.Json.Serialization;

namespace Calendario.Modelos
{
    //  Jandrito, espabila deixa ses vari en angles
    public class RespuestaClima
    {
        [JsonPropertyName("daily")]
        // new DailyData() per evitar es null que tira error (vorem si va be revisar)
        public DataDiaria Daily { get; set; } = new DataDiaria();
    }

    // Dates diaris temp etc. deixa es noms igual
    public class DataDiaria
    {
        [JsonPropertyName("time")]
        public List<string> Tiempo { get; set; } = new List<string>();

        [JsonPropertyName("weather_code")]
        public List<int> CodigoClima { get; set; } = new List<int>();

        [JsonPropertyName("temperature_2m_max")]
        public List<double> TemperaturaMax { get; set; } = new List<double>();

        [JsonPropertyName("temperature_2m_min")]
        public List<double> TemperaturaMin { get; set; } = new List<double>();
    }

    // Clase auxiliar per ajudar sa impresso pantalla
    public class DiaClima
    {
        public DateTime Fecha { get; set; }
        public int CodigoClima { get; set; }
        public double Max { get; set; }
        public double Min { get; set; }
    }
    public class GeoRepuesta
    {
        [JsonPropertyName("results")]
        public List<CiudadResultado> Results { get; set; } = new List<CiudadResultado>();
    }

    public class CiudadResultado
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;
    }

    //  Nom ubicacio gps
    public class RevGeoRespuesta
    {
        [JsonPropertyName("address")]
        public Direccion Address { get; set; } = new Direccion();
    }

    public class Direccion
    {
        [JsonPropertyName("ciudad")]
        public string? Ciudad { get; set; }

        [JsonPropertyName("Publo")]
        public string? Pueblo { get; set; }

        [JsonPropertyName("villa")]
        public string? Villa { get; set; }

        [JsonPropertyName("estado")]
        public string? Comunidad { get; set; }

        [JsonPropertyName("pais")]
        public string? Pais { get; set; }
    }
}