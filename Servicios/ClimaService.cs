using System.Net.Http.Json;
using Calendario.Modelos;

namespace Calendario.Servicios
{
    public class ClimaService
    {
        private readonly HttpClient _http;

        public ClimaService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<DiaClima>> ObtenerPronostico(double lat, double lon)
        {
            try
            {
                //Open meteo en teoria es free
                //revisar VisualCrossing, Watherapi.com tambe pot funcionar millor
                string latStr = lat.ToString(System.Globalization.CultureInfo.InvariantCulture);
                string lonStr = lon.ToString(System.Globalization.CultureInfo.InvariantCulture);

                string url = $"https://api.open-meteo.com/v1/forecast?latitude={latStr}&longitude={lonStr}&daily=weather_code,temperature_2m_max,temperature_2m_min&timezone=auto";

                var respuesta = await _http.GetFromJsonAsync<ClimaResponse>(url);


                if (respuesta?.Daily == null || respuesta.Daily.Time == null)
                    return new List<DiaClima>();

                // Reunir ses dades en un arraybid
                var dias = new List<DiaClima>();

                //  Retorn API de forma paralela
                for (int i = 0; i < respuesta.Daily.Time.Count; i++)
                {
                    dias.Add(new DiaClima
                    {
                        Fecha = DateTime.Parse(respuesta.Daily.Time[i]),
                        CodigoClima = respuesta.Daily.WeatherCode[i],
                        Max = respuesta.Daily.TemperatureMax[i],
                        Min = respuesta.Daily.TemperatureMin[i]
                    });
                }
                return dias;
            }
            catch (Exception ex)
            {
                //Si falla o no internet return i pa casa
                Console.WriteLine($"Error Clima: {ex.Message}");
                return new List<DiaClima>();
            }
        }
        // ... (Tu función ObtenerPronostico sigue aquí arriba) ...

        // NUEVA FUNCIÓN: Traduce "Madrid" -> Coordenadas
        public async Task<CiudadResultado?> BuscarCiudad(string nombreCiudad)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombreCiudad)) return null;

                var url = $"https://geocoding-api.open-meteo.com/v1/search?name={nombreCiudad}&count=1&language=es&format=json";

                var respuesta = await _http.GetFromJsonAsync<GeoResponse>(url);

                return respuesta?.Results.FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
        // ... (Tus otras funciones siguen aquí arriba) ...

        // NUEVA FUNCIÓN: Traduce Coordenadas -> "Madrid, España"
        public async Task<string> ObtenerNombreUbicacion(double lat, double lon)
        {
            try
            {
                // Usamos latitud y longitud con punto (culture invariant)
                string latStr = lat.ToString(System.Globalization.CultureInfo.InvariantCulture);
                string lonStr = lon.ToString(System.Globalization.CultureInfo.InvariantCulture);

                var url = $"https://nominatim.openstreetmap.org/reverse?format=json&lat={latStr}&lon={lonStr}";

                // IMPORTANTE: Nominatim requiere un User-Agent o da error 403
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("User-Agent", "CalendarioApp/1.0");

                var response = await _http.SendAsync(request);
                var data = await response.Content.ReadFromJsonAsync<ReverseGeoResponse>();

                if (data?.Address != null)
                {
                    // Intentamos coger el nombre más preciso disponible
                    string ciudad = data.Address.City
                                    ?? data.Address.Town
                                    ?? data.Address.Village
                                    ?? data.Address.State
                                    ?? "Ubicación desconocida";

                    return $"{ciudad}, {data.Address.Country}";
                }

                return "Ubicación detectada";
            }
            catch
            {
                return "Tu Ubicación Actual";
            }
        }
    }


}