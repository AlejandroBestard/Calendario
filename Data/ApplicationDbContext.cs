using Microsoft.EntityFrameworkCore;
using Calendario.Modelos;

namespace Calendario.Data;


public class ApplicationDbContext : DbContext
{

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }


    public DbSet<Evento> Eventos { get; set; }
}