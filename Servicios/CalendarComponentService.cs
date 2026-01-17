using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Calendario.Modelos;
using Calendario.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Forms; // Para IBrowserFile

namespace Calendario.Servicios
{
    // DTO para el resultado (según tu requisito isDateInCalendars)
    public class CalendarMatchResult
    {
        public bool Matches { get; set; }
        public List<string> Calendars { get; set; } = new();
    }

    public class CalendarComponentService
    {
        private readonly CalendarEngine _engine;
        private readonly ApplicationDbContext _context;

        // Estado interno (caché de calendarios cargados)
        private List<CalendarioDefinition> _internalCalendars = new();

        public CalendarComponentService(CalendarEngine engine, ApplicationDbContext context)
        {
            _engine = engine;
            _context = context;
        }

        // ---------------------------------------------------------
        // 1. loadCalendars(calendars: CalendarDefinition[]): void
        // ---------------------------------------------------------
        // Carga una lista de calendarios en la memoria del componente
        public void LoadCalendars(List<CalendarioDefinition> calendars)
        {
            _internalCalendars = calendars;
        }

        // Opción B: Si "load" significa cargar de la BD
        public async Task LoadCalendarsFromDbAsync()
        {
            _internalCalendars = await _context.Calendarios
                .Include(c => c.Reglas)
                .ThenInclude(r => r.Excepciones)
                .AsNoTracking()
                .ToListAsync();
        }

        // ---------------------------------------------------------
        // 2. getCalendars(): CalendarDefinition[]
        // ---------------------------------------------------------
        // Devuelve los calendarios actuales
        public List<CalendarioDefinition> GetCalendars()
        {
            return _internalCalendars;
        }

        // ---------------------------------------------------------
        // 3. isDateInCalendars(date: Date): CalendarMatchResult
        // ---------------------------------------------------------
        // Usa nuestro motor para comprobar la fecha contra los calendarios cargados
        public CalendarMatchResult IsDateInCalendars(DateTime date)
        {
            // Reutilizamos la lógica robusta que ya creamos en CalendarEngine
            // Nota: El Engine devuelve su propio DTO, lo mapeamos al de este servicio si es necesario
            // o usamos el mismo. Aquí asumimos que usamos el del Engine.

            var resultadoEngine = _engine.IsDateInCalendars(date, _internalCalendars);

            return new CalendarMatchResult
            {
                Matches = resultadoEngine.Matches,
                Calendars = resultadoEngine.Calendars
            };
        }

        // ---------------------------------------------------------
        // 4. importFromICal(file: File): Promise<void>
        // ---------------------------------------------------------
        // En C# "File" suele ser un Stream o IBrowserFile. 
        // Devuelve Task (Promise) porque leer ficheros es asíncrono.
        public async Task ImportFromICal(IBrowserFile file)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            using var stream = file.OpenReadStream(maxAllowedSize: 1024 * 1024 * 5); // Limite 5MB
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();

            // Usamos el parser del Engine
            var reglas = _engine.ParsearIcal(content);

            // Creamos el calendario nuevo
            var nuevoCal = new CalendarioDefinition
            {
                Nombre = $"Importado: {file.Name}",
                Color = "#9C27B0", // Morado por defecto
                Reglas = reglas
            };

            // Guardamos en BD
            _context.Calendarios.Add(nuevoCal);
            await _context.SaveChangesAsync();

            // Recargamos la lista interna
            await LoadCalendarsFromDbAsync();
        }

        // ---------------------------------------------------------
        // 5. exportToICal(calendarName: string): string
        // ---------------------------------------------------------
        public string ExportToICal(string calendarName)
        {
            // Buscamos el calendario en la lista interna por nombre
            var calendario = _internalCalendars
                .FirstOrDefault(c => c.Nombre.Equals(calendarName, StringComparison.OrdinalIgnoreCase));

            if (calendario == null)
            {
                throw new Exception($"Calendario '{calendarName}' no encontrado.");
            }

            // Usamos el Engine para generar el string
            return _engine.ExportToICal(calendario.Reglas);
        }
    }
}