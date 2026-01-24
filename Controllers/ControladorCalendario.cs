using Microsoft.AspNetCore.Mvc;
using Calendario.Modelos;
using Calendario.Servicios;

[ApiController]
[Route("api/[controller]")]
public class ControladorCalendario : ControllerBase
{
    private readonly MotorCalendario _motor;
    private readonly RepositorioCalendario _repositorio;

    public ControladorCalendario(MotorCalendario motor, RepositorioCalendario repositorio)
    {
        _motor = motor;
        _repositorio = repositorio;
    }

    [HttpGet("verificar")]
    public async Task<IActionResult> VerificarFecha([FromQuery] DateTime fecha, [FromQuery] int calendarioId, [FromQuery] TimeSpan? hora = null)
    {
        var calendario = await _repositorio.ObtenerCalendarioPorId(calendarioId);

        if (calendario == null) return NotFound("Calendario no encontrado");


        var eventos = calendario.Reglas
            .Where(r => _motor.VerificarReglaIndividual(fecha, r))
            .Select(r => new { r.Titulo, r.HoraInicio, r.Color, r.Categoria })
            .ToList();

        // Si se proporcionó una hora, filtrar eventos que ocurren en esa hora
        if (hora.HasValue)
        {
            eventos = eventos
                .Where(e => e.HoraInicio.Hours == hora.Value.Hours &&
                           e.HoraInicio.Minutes == hora.Value.Minutes)
                .ToList();
        }

        return Ok(new
        {
            fecha = fecha.ToShortDateString(),
            hora = hora?.ToString(@"hh\:mm"),
            tieneEventos = eventos.Any(),
            eventos = eventos
        });
    }

    // endpoint api/calendario/exportar/1
    [HttpGet("exportar/{id}")]
    public async Task<IActionResult> ExportarIcal(int id)
    {
        var calendario = await _repositorio.ObtenerCalendarioPorId(id);
        if (calendario == null) return NotFound();

        var ics = _motor.ExportToICal(calendario.Reglas);
        return File(System.Text.Encoding.UTF8.GetBytes(ics), "text/calendar", $"{calendario.Nombre}.ics");
    }
}