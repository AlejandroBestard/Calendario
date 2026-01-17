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

                string latStr = lat.ToString(System.Globalization.CultureInfo.InvariantCulture);
                string lonStr = lon.ToString(System.Globalization.CultureInfo.InvariantCulture);

                string url = $"https://api.open-meteo.com/v1/forecast?latitude={latStr}&longitude={lonStr}&daily=weather_code,temperature_2m_max,temperature_2m_min&timezone=auto";

                var respuesta = await _http.GetFromJsonAsync<RespuestaClima>(url);


                if (respuesta?.Daily == null || respuesta.Daily.Tiempo == null)
                    return new List<DiaClima>();

                // Reunir ses dades en un arraybid
                var dias = new List<DiaClima>();

                //  Retorn API de forma paralela
                for (int i = 0; i < respuesta.Daily.Tiempo.Count; i++)
                {
                    dias.Add(new DiaClima
                    {
                        Fecha = DateTime.Parse(respuesta.Daily.Tiempo[i]),
                        CodigoClima = respuesta.Daily.CodigoClima[i],
                        Max = respuesta.Daily.TemperaturaMax[i],
                        Min = respuesta.Daily.TemperaturaMin[i]
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

        public async Task<CiudadResultado?> BuscarCiudad(string nombreCiudad)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombreCiudad)) return null;

                var url = $"https://geocoding-api.open-meteo.com/v1/search?name={nombreCiudad}&count=1&language=es&format=json";

                var respuesta = await _http.GetFromJsonAsync<GeoRepuesta>(url);

                return respuesta?.Results.FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> ObtenerNombreUbicacion(double lat, double lon)
        {
            try
            {
                string latStr = lat.ToString(System.Globalization.CultureInfo.InvariantCulture);
                string lonStr = lon.ToString(System.Globalization.CultureInfo.InvariantCulture);

                var url = $"https://nominatim.openstreetmap.org/reverse?format=json&lat={latStr}&lon={lonStr}";

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("User-Agent", "CalendarioApp/1.0");

                var response = await _http.SendAsync(request);
                var data = await response.Content.ReadFromJsonAsync<RevGeoRespuesta>();

                if (data?.Address != null)
                {
                    // Intentar agafar el nom mes exacte
                    string ciudad = data.Address.Ciudad
                                    ?? data.Address.Pueblo
                                    ?? data.Address.Villa
                                    ?? data.Address.Comunidad
                                    ?? "Ubicación desconocida";

                    return $"{ciudad}, {data.Address.Pais}";
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