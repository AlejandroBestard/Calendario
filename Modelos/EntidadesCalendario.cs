using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Calendario.Modelos
{
    public class DefinicionCalendario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = "General";

        public string Color { get; set; } = "#2196F3";

        public List<ReglaCalendario> Reglas { get; set; } = new();
    }

    public class ReglaCalendario
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("DefinicionCalendario")]
        public int CalendarioId { get; set; }

        [JsonIgnore]
        public DefinicionCalendario? DefinicionCalendario { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        public string Titulo { get; set; } = string.Empty;

        public string? Categoria { get; set; } = "Personal";
        public string? Color { get; set; }

        public TipoRegla Tipo { get; set; }

        public DateTime FechaInicio { get; set; } = DateTime.Today;

        public DateTime? FechaFin { get; set; }

        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }

        public List<int> DiasSemana { get; set; } = new();

        public List<ExcepcionRegla> Excepciones { get; set; } = new();
    }


    public class ExcepcionRegla
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Regla")]
        public int ReglaCalendarioId { get; set; }

        [JsonIgnore]
        public ReglaCalendario? Regla { get; set; }

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
        Anual,
        LunesViernes
    }

    public enum TipoExcepcion
    {
        Modificar,
        Eliminar
    }
}