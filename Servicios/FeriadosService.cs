using System.Net.Http.Json;
using Calendario.Modelos;

namespace Calendario.Servicios
{
    public class FeriadosService
    {
        private readonly HttpClient _http;

        public FeriadosService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<FeriadoDto>> ObtenerFeriados(int anio, string codigoPais)
        {
            try
            {
                // Llamamos a la API pública de Nager.Date
                var url = $"https://date.nager.at/api/v3/publicholidays/{anio}/{codigoPais}";
                var resultado = await _http.GetFromJsonAsync<List<FeriadoDto>>(url);
                return resultado ?? new List<FeriadoDto>();
            }
            catch (Exception)
            {
                // Si falla (ej. sin internet), devolvemos lista vacía
                return new List<FeriadoDto>();
            }
        }
    }
}