using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Calendario.Data;
using Calendario.Servicios;

[ApiController]
[Route("api/[controller]")]
public class CalendarioController : ControllerBase
{
    private readonly CalendarEngine _engine;
    private readonly ApplicationDbContext _db;

    public CalendarioController(CalendarEngine engine, ApplicationDbContext db)
    {
        _engine = engine;
        _db = db;
    }

    [HttpGet("verificar")]
    public async Task<IActionResult> VerificarFecha([FromQuery] DateTime fecha, [FromQuery] int calendarioId)
    {
        var calendario = await _db.Calendarios
            .Include(c => c.Reglas)
            .FirstOrDefaultAsync(c => c.Id == calendarioId);

        if (calendario == null) return NotFound("Calendario no encontrado");

        // Usamos la API lógica que ya desarrollamos
        var eventos = calendario.Reglas
            .Where(r => _engine.VerificarReglaIndividual(fecha, r))
            .Select(r => new { r.Titulo, r.HoraInicio, r.Color, r.Categoria })
            .ToList();

        return Ok(new { 
            fecha = fecha.ToShortDateString(), 
            tieneEventos = eventos.Any(),
            eventos = eventos 
        });
    }

    // ENDPOINT: api/calendario/exportar/1
    [HttpGet("exportar/{id}")]
    public async Task<IActionResult> ExportarIcal(int id)
    {
        var calendario = await _db.Calendarios.Include(c => c.Reglas).FirstOrDefaultAsync(c => c.Id == id);
        if (calendario == null) return NotFound();

        var ics = _engine.ExportToICal(calendario);
        return File(System.Text.Encoding.UTF8.GetBytes(ics), "text/calendar", $"{calendario.Nombre}.ics");
    }
}