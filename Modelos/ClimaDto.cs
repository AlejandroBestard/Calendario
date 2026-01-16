using System.Text.Json.Serialization;

namespace Calendario.Modelos
{
    // Clase principal
    public class ClimaResponse
    {
        [JsonPropertyName("daily")]
        // new DailyData() per evitar es null que tira error (vorem si va be revisar)
        public DailyData Daily { get; set; } = new DailyData();
    }

    // Dates diaris temp etc.
    public class DailyData
    {
        [JsonPropertyName("time")]
        public List<string> Time { get; set; } = new List<string>();

        [JsonPropertyName("weather_code")]
        public List<int> WeatherCode { get; set; } = new List<int>();

        [JsonPropertyName("temperature_2m_max")]
        public List<double> TemperatureMax { get; set; } = new List<double>();

        [JsonPropertyName("temperature_2m_min")]
        public List<double> TemperatureMin { get; set; } = new List<double>();
    }

    // Clase auxiliar per ajudar sa impresso pantalla
    public class DiaClima
    {
        public DateTime Fecha { get; set; }
        public int CodigoClima { get; set; }
        public double Max { get; set; }
        public double Min { get; set; }
    }
    // --- NUEVAS CLASES PARA EL BUSCADOR ---
    public class GeoResponse
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

    // --- NUEVAS CLASES PARA EL NOMBRE DE LA UBICACIÓN (GPS) ---
    public class ReverseGeoResponse
    {
        [JsonPropertyName("address")]
        public Direccion Address { get; set; } = new Direccion();
    }

    public class Direccion
    {
        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("town")]
        public string? Town { get; set; }

        [JsonPropertyName("village")]
        public string? Village { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }
    }
}