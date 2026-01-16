using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Calendario.Modelos
{
    // 1. Agrupador de reglas (El "Contenedor" o tipo de calendario)
    public class CalendarioDefinition
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; } = "General";
        public string Color { get; set; } = "#2196F3";

        // Relación: Un calendario tiene muchas reglas
        public List<ReglaCalendario> Reglas { get; set; } = new();
    }

    // 2. Definición de la Regla (Evento o patrón recurrente)
    public class ReglaCalendario
    {
        [Key]
        public int Id { get; set; }
        public int CalendarioId { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        public string Titulo { get; set; } = string.Empty;

        // Nuevos campos para personalización (Requisito UI/UX)
        public string? Categoria { get; set; } = "Personal";
        public string? Color { get; set; } // Color específico para esta regla

        // Tipo de regla ampliado
        public TipoRegla Tipo { get; set; }

        // Rango de validez
        public DateTime FechaInicio { get; set; } = DateTime.Today;
        public DateTime FechaFin { get; set; } = DateTime.Today.AddYears(1);

        // Horario
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }

        // Días de la semana (JSON para que EF lo guarde fácil)
        public List<int> DiasSemana { get; set; } = new();

        // Relación con excepciones
        public List<ExcepcionRegla> Excepciones { get; set; } = new();
    }

    // 3. Excepciones (Modificaciones a ocurrencias específicas)
    public class ExcepcionRegla
    {
        [Key]
        public int Id { get; set; }
        public int ReglaCalendarioId { get; set; }

        public DateTime FechaOriginal { get; set; }
        public TipoExcepcion Tipo { get; set; }

        public TimeSpan? NuevaHoraInicio { get; set; }
        public TimeSpan? NuevaHoraFin { get; set; }
        public string? NuevoTitulo { get; set; }
    }

    public enum TipoRegla
    {
        Puntual,
        Rango,
        Semanal,
        Mensual,
        Anual
    }

    public enum TipoExcepcion
    {
        Modificar,
        Eliminar
    }
}