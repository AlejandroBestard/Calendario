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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar la relación entre DefinicionCalendario y ReglaCalendario
            modelBuilder.Entity<DefinicionCalendario>()
                .HasMany(c => c.Reglas)
                .WithOne(r => r.DefinicionCalendario)
                .HasForeignKey(r => r.CalendarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configurar la relación entre ReglaCalendario y ExcepcionRegla
            modelBuilder.Entity<ReglaCalendario>()
                .HasMany(r => r.Excepciones)
                .WithOne(e => e.Regla)
                .HasForeignKey(e => e.ReglaCalendarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}