using System.ComponentModel.DataAnnotations;

namespace Calendario.Modelos
{
    public class Evento
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; } = string.Empty;

        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }

        public string Color { get; set; } = "#f39121";
    }
}