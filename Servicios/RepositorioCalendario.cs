using Calendario.Modelos;
using System.Text.Json;

namespace Calendario.Servicios
{
    public class RepositorioCalendario
    {
        private List<DefinicionCalendario> _calendarios = new();
        private readonly string _rutaArchivo;
        private int _siguienteIdCalendario = 1;
        private int _siguienteIdRegla = 1;
        private int _siguienteIdExcepcion = 1;

        public RepositorioCalendario()
        {
            _rutaArchivo = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Calendario", "datos.json");
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                var directorio = Path.GetDirectoryName(_rutaArchivo);
                if (!string.IsNullOrEmpty(directorio) && !Directory.Exists(directorio))
                {
                    Directory.CreateDirectory(directorio);
                }

                if (File.Exists(_rutaArchivo))
                {
                    var json = File.ReadAllText(_rutaArchivo);
                    _calendarios = JsonSerializer.Deserialize<List<DefinicionCalendario>>(json) ?? new();

                    // Recalcular los IDs máximos
                    if (_calendarios.Any())
                    {
                        _siguienteIdCalendario = _calendarios.Max(c => c.Id) + 1;

                        var todasLasReglas = _calendarios.SelectMany(c => c.Reglas).ToList();
                        if (todasLasReglas.Any())
                        {
                            _siguienteIdRegla = todasLasReglas.Max(r => r.Id) + 1;

                            var todasLasExcepciones = todasLasReglas.SelectMany(r => r.Excepciones).ToList();
                            if (todasLasExcepciones.Any())
                            {
                                _siguienteIdExcepcion = todasLasExcepciones.Max(e => e.Id) + 1;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar datos: {ex.Message}");
                _calendarios = new();
            }
        }

        private async Task GuardarDatos()
        {
            try
            {
                var directorio = Path.GetDirectoryName(_rutaArchivo);
                if (!string.IsNullOrEmpty(directorio) && !Directory.Exists(directorio))
                {
                    Directory.CreateDirectory(directorio);
                }

                var opciones = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                var json = JsonSerializer.Serialize(_calendarios, opciones);
                await File.WriteAllTextAsync(_rutaArchivo, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar datos: {ex.Message}");
            }
        }

        // Métodos para Calendarios
        public async Task<List<DefinicionCalendario>> ObtenerTodosLosCalendarios()
        {
            return await Task.FromResult(_calendarios.ToList());
        }

        public async Task<DefinicionCalendario?> ObtenerCalendarioPorId(int id)
        {
            return await Task.FromResult(_calendarios.FirstOrDefault(c => c.Id == id));
        }

        public async Task<DefinicionCalendario> AgregarCalendario(DefinicionCalendario calendario)
        {
            calendario.Id = _siguienteIdCalendario++;
            _calendarios.Add(calendario);
            await GuardarDatos();
            return calendario;
        }

        public async Task<DefinicionCalendario?> ActualizarCalendario(DefinicionCalendario calendario)
        {
            var existente = _calendarios.FirstOrDefault(c => c.Id == calendario.Id);
            if (existente == null) return null;

            existente.Nombre = calendario.Nombre;
            existente.Color = calendario.Color;
            await GuardarDatos();
            return existente;
        }

        public async Task<bool> EliminarCalendario(int id)
        {
            var calendario = _calendarios.FirstOrDefault(c => c.Id == id);
            if (calendario == null) return false;

            _calendarios.Remove(calendario);
            await GuardarDatos();
            return true;
        }

        // Métodos para Reglas
        public async Task<List<ReglaCalendario>> ObtenerReglasDeCalendario(int calendarioId)
        {
            var calendario = _calendarios.FirstOrDefault(c => c.Id == calendarioId);
            return await Task.FromResult(calendario?.Reglas ?? new List<ReglaCalendario>());
        }

        public async Task<ReglaCalendario?> ObtenerReglaPorId(int reglaId)
        {
            var regla = _calendarios
                .SelectMany(c => c.Reglas)
                .FirstOrDefault(r => r.Id == reglaId);
            return await Task.FromResult(regla);
        }

        public async Task<ReglaCalendario> AgregarRegla(int calendarioId, ReglaCalendario regla)
        {
            var calendario = _calendarios.FirstOrDefault(c => c.Id == calendarioId);
            if (calendario == null) throw new InvalidOperationException("Calendario no encontrado");

            regla.Id = _siguienteIdRegla++;
            regla.CalendarioId = calendarioId;
            calendario.Reglas.Add(regla);
            await GuardarDatos();
            return regla;
        }

        public async Task<ReglaCalendario?> ActualizarRegla(ReglaCalendario regla)
        {
            var calendario = _calendarios.FirstOrDefault(c => c.Id == regla.CalendarioId);
            if (calendario == null) return null;

            var existente = calendario.Reglas.FirstOrDefault(r => r.Id == regla.Id);
            if (existente == null) return null;

            existente.Titulo = regla.Titulo;
            existente.Categoria = regla.Categoria;
            existente.Color = regla.Color;
            existente.Tipo = regla.Tipo;
            existente.FechaInicio = regla.FechaInicio;
            existente.FechaFin = regla.FechaFin;
            existente.HoraInicio = regla.HoraInicio;
            existente.HoraFin = regla.HoraFin;
            existente.DiasSemana = regla.DiasSemana;
            await GuardarDatos();
            return existente;
        }

        public async Task<bool> EliminarRegla(int reglaId)
        {
            foreach (var calendario in _calendarios)
            {
                var regla = calendario.Reglas.FirstOrDefault(r => r.Id == reglaId);
                if (regla != null)
                {
                    calendario.Reglas.Remove(regla);
                    await GuardarDatos();
                    return true;
                }
            }
            return false;
        }

        // Métodos para Excepciones
        public async Task<ExcepcionRegla> AgregarExcepcion(int reglaId, ExcepcionRegla excepcion)
        {
            var regla = _calendarios
                .SelectMany(c => c.Reglas)
                .FirstOrDefault(r => r.Id == reglaId);

            if (regla == null) throw new InvalidOperationException("Regla no encontrada");

            excepcion.Id = _siguienteIdExcepcion++;
            excepcion.ReglaCalendarioId = reglaId;
            regla.Excepciones.Add(excepcion);
            await GuardarDatos();
            return excepcion;
        }

        public async Task<bool> EliminarExcepcion(int excepcionId)
        {
            foreach (var calendario in _calendarios)
            {
                foreach (var regla in calendario.Reglas)
                {
                    var excepcion = regla.Excepciones.FirstOrDefault(e => e.Id == excepcionId);
                    if (excepcion != null)
                    {
                        regla.Excepciones.Remove(excepcion);
                        await GuardarDatos();
                        return true;
                    }
                }
            }
            return false;
        }

        // Método auxiliar para obtener calendario con todas sus relaciones
        public async Task<DefinicionCalendario?> ObtenerCalendarioCompleto(int id)
        {
            // En este caso, como todo está en memoria, no hay diferencia
            // pero mantenemos el método por compatibilidad con el patrón EF
            return await ObtenerCalendarioPorId(id);
        }
    }
}
