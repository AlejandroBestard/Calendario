using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Calendario.Modelos;

namespace Calendario.Servicios
{
    public class CalendarEngine
    {
        // 1. Lógica de validación (Corrige el error previo CS1061 de image_3194eb.png)
        public bool VerificarReglaIndividual(DateTime fecha, ReglaCalendario regla)
        {
            if (fecha.Date < regla.FechaInicio.Date || fecha.Date > regla.FechaFin.Date)
                return false;

            return regla.Tipo switch
            {
                TipoRegla.Puntual => fecha.Date == regla.FechaInicio.Date,
                TipoRegla.Semanal => regla.DiasSemana != null && regla.DiasSemana.Contains(GetIsoDayOfWeek(fecha)),
                TipoRegla.Mensual => fecha.Day == regla.FechaInicio.Day,
                TipoRegla.Anual   => fecha.Day == regla.FechaInicio.Day && fecha.Month == regla.FechaInicio.Month,
                TipoRegla.Rango   => true,
                _ => false
            };
        }

        private int GetIsoDayOfWeek(DateTime date) => 
            date.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)date.DayOfWeek;

        // 2. MÉTODO DE IMPORTACIÓN (Corrige error en GestionCalendarios.razor:161)
       public List<ReglaCalendario> ParsearIcal(string contenidoIcs)
{
    var reglas = new List<ReglaCalendario>();
    // Dividimos por líneas de forma segura para Windows/Linux
    var lineas = contenidoIcs.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
    ReglaCalendario? actual = null;

    foreach (var l in lineas)
    {
        var linea = l.Trim();
        if (linea.StartsWith("BEGIN:VEVENT")) 
        {
            actual = new ReglaCalendario { 
                Titulo = "Evento Importado", 
                Tipo = TipoRegla.Puntual, 
                DiasSemana = new List<int>(),
                HoraInicio = new TimeSpan(9, 0, 0),
                HoraFin = new TimeSpan(10, 0, 0)
            };
        }
        else if (linea.StartsWith("SUMMARY:") && actual != null) 
        {
            actual.Titulo = linea.Substring(8).Trim();
        }
        else if (linea.Contains("DTSTART") && actual != null)
        {
            // Buscamos 8 dígitos seguidos (YYYYMMDD)
            var match = Regex.Match(linea, @"(\d{8})");
            if (match.Success && DateTime.TryParseExact(match.Value, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var fecha))
            {
                actual.FechaInicio = fecha;
                actual.FechaFin = fecha;
            }
        }
        else if (linea.StartsWith("END:VEVENT") && actual != null)
        {
            // Solo añadimos si tiene una fecha válida
            if (actual.FechaInicio != default)
            {
                reglas.Add(actual);
            }
            actual = null;
        }
    }
    return reglas;
}

        // 3. MÉTODO DE EXPORTACIÓN (Corrige errores en CalendarioController:48 y GestionCalendarios:134)
        public string ExportToICal(CalendarioDefinition cal)
        {
            var sb = new StringBuilder();
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("PRODID:-//Calendario//ES");

            if (cal.Reglas != null)
            {
                foreach (var regla in cal.Reglas)
                {
                    sb.AppendLine("BEGIN:VEVENT");
                    sb.AppendLine($"SUMMARY:{regla.Titulo}");
                    sb.AppendLine($"DTSTART:{regla.FechaInicio:yyyyMMdd}T090000");
                    sb.AppendLine($"DTEND:{regla.FechaInicio:yyyyMMdd}T100000");
                    sb.AppendLine("END:VEVENT");
                }
            }
            sb.AppendLine("END:VCALENDAR");
            return sb.ToString();
        }
    }
}