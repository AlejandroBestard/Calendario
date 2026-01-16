using Microsoft.EntityFrameworkCore;
using Calendario.Modelos;

namespace Calendario.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Evento> Eventos { get; set; }

        //Tables prova tecnica
        public DbSet<CalendarioDefinition> Calendarios { get; set; }
        public DbSet<ReglaCalendario> Reglas { get; set; }
        public DbSet<ExcepcionRegla> Excepciones { get; set; }
    }
}