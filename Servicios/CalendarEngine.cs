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
        // EL MÉTODO DEBE SER 'public' PARA QUE SE VEA DESDE LAS PÁGINAS
        public bool VerificarReglaIndividual(DateTime fecha, ReglaCalendario regla)
        {
            // 1. Validar rango de fechas
            if (fecha.Date < regla.FechaInicio.Date || fecha.Date > regla.FechaFin.Date)
                return false;

            // 2. Validar según el tipo
            return regla.Tipo switch
            {
                TipoRegla.Puntual => fecha.Date == regla.FechaInicio.Date,
                
                TipoRegla.Semanal => regla.DiasSemana != null && 
                                     regla.DiasSemana.Contains(GetIsoDayOfWeek(fecha)),
                
                TipoRegla.Mensual => fecha.Day == regla.FechaInicio.Day,
                
                TipoRegla.Anual   => fecha.Day == regla.FechaInicio.Day && 
                                     fecha.Month == regla.FechaInicio.Month,
                
                TipoRegla.Rango   => true,
                _ => false
            };
        }

        // ESTE MÉTODO DEBE ESTAR FUERA DE CUALQUIER OTRO MÉTODO
        private int GetIsoDayOfWeek(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)date.DayOfWeek;
        }

        public List<ReglaCalendario> ParsearIcal(string contenidoIcs)
        {
            var reglas = new List<ReglaCalendario>();
            var lineas = contenidoIcs.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            ReglaCalendario? actual = null;

            foreach (var l in lineas)
            {
                var linea = l.Trim();
                if (linea.StartsWith("BEGIN:VEVENT")) 
                    actual = new ReglaCalendario { Tipo = TipoRegla.Puntual, DiasSemana = new List<int>() };
                
                else if (linea.StartsWith("SUMMARY:") && actual != null) 
                    actual.Titulo = linea.Substring(8).Trim();
                
                else if (linea.Contains("DTSTART") && actual != null)
                {
                    var match = Regex.Match(linea, @"(\d{8})");
                    if (match.Success && DateTime.TryParseExact(match.Value, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var fecha))
                    {
                        actual.FechaInicio = fecha;
                        actual.FechaFin = fecha;
                        actual.HoraInicio = new TimeSpan(12, 0, 0); 
                        actual.HoraFin = new TimeSpan(13, 0, 0);
                    }
                }
                else if (linea.StartsWith("END:VEVENT") && actual != null)
                {
                    reglas.Add(actual);
                    actual = null;
                }
            }
            return reglas;
        }

       public string ExportToICal(CalendarioDefinition cal)
{
    var sb = new System.Text.StringBuilder();
    sb.AppendLine("BEGIN:VCALENDAR");
    sb.AppendLine("VERSION:2.0");
    sb.AppendLine("PRODID:-//Calendario//ES");

    if (cal.Reglas != null)
    {
        foreach (var regla in cal.Reglas)
        {
            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine($"SUMMARY:{regla.Titulo}");
            // Formato iCal: YYYYMMDDTHHMMSS
            sb.AppendLine($"DTSTART:{regla.FechaInicio:yyyyMMdd}T{regla.HoraInicio:hhmm}00");
            sb.AppendLine($"DTEND:{regla.FechaInicio:yyyyMMdd}T{regla.HoraFin:hhmm}00");
            
            // Lógica de recurrencia
            if (regla.Tipo == TipoRegla.Semanal && regla.DiasSemana != null) {
                var dias = string.Join(",", regla.DiasSemana.Select(d => ConvertirDiaIcal(d)));
                sb.AppendLine($"RRULE:FREQ=WEEKLY;BYDAY={dias}");
            }
            else if (regla.Tipo == TipoRegla.Mensual) sb.AppendLine("RRULE:FREQ=MONTHLY");
            else if (regla.Tipo == TipoRegla.Anual) sb.AppendLine("RRULE:FREQ=YEARLY");

            sb.AppendLine("END:VEVENT");
        }
    }

    sb.AppendLine("END:VCALENDAR");
    return sb.ToString();
}

private string ConvertirDiaIcal(int d) => d switch { 1=>"MO", 2=>"TU", 3=>"WE", 4=>"TH", 5=>"FR", 6=>"SA", 7=>"SU", _=>"" };
    }
}