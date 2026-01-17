using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Calendario.Data;
using Calendario.Modelos;
using Calendario.Servicios;
using System.Text;


[ApiController]
[Route("api/[controller]")]
public class ControladorCalendario : ControllerBase
{
    private readonly MotorCalendario _motor;
    private readonly AplicacionContextoDB _db;

    public ControladorCalendario(MotorCalendario motor, AplicacionContextoDB db)
    {
        _motor = motor;
        _db = db;
    }

    [HttpGet("verificar")]
    public async Task<IActionResult> VerificarFecha([FromQuery] DateTime fecha, [FromQuery] int calendarioId)
    {
        var calendario = await _db.Calendarios
            .Include(c => c.Reglas)
            .FirstOrDefaultAsync(c => c.Id == calendarioId);

        if (calendario == null) return NotFound("Calendario no encontrado");


        var eventos = calendario.Reglas
            .Where(r => _motor.VerificarReglaIndividual(fecha, r))
            .Select(r => new { r.Titulo, r.HoraInicio, r.Color, r.Categoria })
            .ToList();

        return Ok(new
        {
            fecha = fecha.ToShortDateString(),
            tieneEventos = eventos.Any(),
            eventos = eventos
        });
    }

    // endpoint api/calendario/exportar/1
    [HttpGet("exportar/{id}")]
    public async Task<IActionResult> ExportarIcal(int id)
    {
        var calendario = await _db.Calendarios.Include(c => c.Reglas).FirstOrDefaultAsync(c => c.Id == id);
        if (calendario == null) return NotFound();

        var ics = _motor.ExportToICal(calendario.Reglas);
        return File(System.Text.Encoding.UTF8.GetBytes(ics), "text/calendar", $"{calendario.Nombre}.ics");
    }
}