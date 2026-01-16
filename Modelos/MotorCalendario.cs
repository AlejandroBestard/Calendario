using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Calendario.Modelos
{
    // 1. Agrupador regles
    public class CalendarioDefinition
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; } = "General";
        public string Color { get; set; } = "#2196F3";

        // Relaciio de clanedari
        public List<ReglaCalendario> Reglas { get; set; } = new();
    }

    // 2. Regla definicio events
    public class ReglaCalendario
    {
        [Key]
        public int Id { get; set; }
        public int CalendarioId { get; set; }

        public string Titulo { get; set; } = string.Empty;

        // Tipo de regla: 0 = Puntual (un día), 1 = Rango, 2 = Recurrente Semanal
        public TipoRegla Tipo { get; set; }

        // Rango de validez de la regla
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        // Horario (Ej: de 13:00 a 15:00)
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }

        // Para recurrencia semanal: ¿Qué días aplica? (Lunes=1, Martes=2...)
        public List<int> DiasSemana { get; set; } = new();

        // Relación con excepciones
        public List<ExcepcionRegla> Excepciones { get; set; } = new();
    }

    // 3. LA EXCEPCIÓN (Cambios puntuales a una regla)
    public class ExcepcionRegla
    {
        [Key]
        public int Id { get; set; }
        public int ReglaCalendarioId { get; set; }

        // ¿Qué fecha concreta de la regla estamos cambiando?
        public DateTime FechaOriginal { get; set; }

        // Tipo: 0 = Modificar horario, 1 = Eliminar evento
        public TipoExcepcion Tipo { get; set; }

        // Si es modificar, aquí van los nuevos datos
        public TimeSpan? NuevaHoraInicio { get; set; }
        public TimeSpan? NuevaHoraFin { get; set; }
        public string? NuevoTitulo { get; set; }
    }

    public enum TipoRegla { Puntual, Rango, Semanal }
    public enum TipoExcepcion { Modificar, Eliminar }
}