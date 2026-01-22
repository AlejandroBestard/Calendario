using System.Net.Http.Json;
using Calendario.Modelos;

namespace Calendario.Servicios
{
    public class FestivosService
    {
        private readonly HttpClient _http;

        public FestivosService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<FestivosDto>> ObtenerFestivos(int anio, string codigoPais)
        {
            try
            {
                var url = $"https://date.nager.at/api/v3/publicholidays/{anio}/{codigoPais}";
                var resultado = await _http.GetFromJsonAsync<List<FestivosDto>>(url);
                return resultado ?? new List<FestivosDto>();
            }
            catch (Exception)
            {
                return new List<FestivosDto>();
            }
        }
    }
}