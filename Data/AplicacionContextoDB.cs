using Microsoft.EntityFrameworkCore;
using Calendario.Modelos;

namespace Calendario.Data
{
    public class AplicacionContextoDB : DbContext
    {
        public AplicacionContextoDB(DbContextOptions<AplicacionContextoDB> options)
            : base(options)
        {
        }

        //Tables prova tecnica
        public DbSet<DefinicionCalendario> Calendarios { get; set; }
        public DbSet<ReglaCalendario> Reglas { get; set; }
        public DbSet<ExcepcionRegla> Excepciones { get; set; }
    }
}