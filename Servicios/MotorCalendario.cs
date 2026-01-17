using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Calendario.Modelos;

namespace Calendario.Servicios
{
    // DTO per resposta API
    public class ResultadoConsulta
    {
        public bool Coincide { get; set; }
        public List<string> Calendarios { get; set; } = new();
    }

    public class MotorCalendario
    {


        // api mirar si cau en dates calendari
        public ResultadoConsulta IsDateInCalendars(DateTime fechaConsulta, List<DefinicionCalendario> todosLosCalendarios)
        {
            var resultado = new ResultadoConsulta { Coincide = false };

            foreach (var cal in todosLosCalendarios)
            {
                if (cal.Reglas != null)
                {
                    bool coincide = cal.Reglas.Any(r => VerificarReglaCompleta(fechaConsulta, r, estricto: true));

                    if (coincide)
                    {
                        resultado.Coincide = true;
                        resultado.Calendarios.Add(cal.Nombre);
                    }
                }
            }
            return resultado;
        }


        public bool VerificarReglaIndividual(DateTime fecha, ReglaCalendario regla)
        {
            return VerificarReglaCompleta(fecha, regla, estricto: false);
        }

        private bool VerificarReglaCompleta(DateTime fechaHora, ReglaCalendario regla, bool estricto)
        {
            var excepcion = regla.Excepciones?.FirstOrDefault(e => e.FechaOriginal.Date == fechaHora.Date);

            if (excepcion != null)
            {
                if (excepcion.Tipo == TipoExcepcion.Eliminar) return false;

                if (excepcion.Tipo == TipoExcepcion.Modificar)
                {
                    if (!estricto) return true;
                    return VerificarHorario(fechaHora, excepcion.NuevaHoraInicio, excepcion.NuevaHoraFin);
                }
            }


            if (fechaHora.Date < regla.FechaInicio.Date) return false;
            if (regla.FechaFin.HasValue && fechaHora.Date > regla.FechaFin.Value.Date) return false;

            bool patronValido = VerificarPatronFecha(fechaHora, regla);

            if (patronValido)
            {

                if (!estricto) return true;

                return VerificarHorario(fechaHora, regla.HoraInicio, regla.HoraFin);
            }

            return false;
        }

        private bool VerificarHorario(DateTime fechaHora, TimeSpan? inicio, TimeSpan? fin)
        {
            if (inicio == null || fin == null || (inicio == TimeSpan.Zero && fin == TimeSpan.Zero))
                return true;

            TimeSpan horaConsulta = fechaHora.TimeOfDay;
            return horaConsulta >= inicio && horaConsulta < fin;
        }

        private bool VerificarPatronFecha(DateTime fecha, ReglaCalendario regla)
        {
            switch (regla.Tipo)
            {
                case TipoRegla.Puntual:
                    return fecha.Date == regla.FechaInicio.Date;
                case TipoRegla.Semanal:
                    return fecha.DayOfWeek == regla.FechaInicio.DayOfWeek;
                case TipoRegla.Mensual:
                    return fecha.Day == regla.FechaInicio.Day;
                case TipoRegla.Anual:
                    return fecha.Day == regla.FechaInicio.Day && fecha.Month == regla.FechaInicio.Month;
                case TipoRegla.LunesViernes:
                    return fecha.DayOfWeek != DayOfWeek.Saturday && fecha.DayOfWeek != DayOfWeek.Sunday;
                default:
                    return false;
            }
        }


        // IMPORTAR ICAL
        public List<ReglaCalendario> ParsearIcal(string contenido)
        {
            var reglas = new List<ReglaCalendario>();
            var lineas = contenido.Replace("\r\n", "\n").Split('\n');
            ReglaCalendario? reglaActual = null;
            bool dentroEvento = false;

            foreach (var linea in lineas)
            {
                var lineaLimpia = linea.Trim();
                if (lineaLimpia == "BEGIN:VEVENT")
                {
                    dentroEvento = true;
                    reglaActual = new ReglaCalendario { Tipo = TipoRegla.Puntual, Color = "#FF9800", Categoria = "Importado" };
                }
                else if (lineaLimpia == "END:VEVENT")
                {
                    if (reglaActual != null)
                    {
                        if (reglaActual.Tipo == TipoRegla.Puntual && !reglaActual.FechaFin.HasValue)
                            reglaActual.FechaFin = reglaActual.FechaInicio;

                        reglaActual.HoraInicio = reglaActual.FechaInicio.TimeOfDay;
                        if (reglaActual.FechaFin.HasValue)
                            reglaActual.HoraFin = reglaActual.FechaFin.Value.TimeOfDay;

                        reglas.Add(reglaActual);
                    }
                    dentroEvento = false;
                    reglaActual = null;
                }
                else if (dentroEvento && reglaActual != null)
                {
                    if (lineaLimpia.StartsWith("SUMMARY:")) reglaActual.Titulo = lineaLimpia.Substring(8);
                    else if (lineaLimpia.StartsWith("DTSTART"))
                    {
                        var val = ObtenerValorFecha(lineaLimpia);
                        if (val != DateTime.MinValue) reglaActual.FechaInicio = val;
                    }
                    else if (lineaLimpia.StartsWith("DTEND"))
                    {
                        var val = ObtenerValorFecha(lineaLimpia);
                        if (val != DateTime.MinValue) reglaActual.FechaFin = val;
                    }
                    else if (lineaLimpia.StartsWith("RRULE:")) AnalizarRrule(lineaLimpia.Substring(6), reglaActual);
                }
            }
            return reglas;
        }

        //  EXPORTAR ICAL
        public string ExportToICal(IEnumerable<ReglaCalendario> reglas)
        {
            var sb = new StringBuilder();
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("PRODID:-//MiCalendarioApp//ES");

            foreach (var regla in reglas)
            {
                sb.AppendLine("BEGIN:VEVENT");
                sb.AppendLine($"UID:{Guid.NewGuid()}@micalendario.app");
                sb.AppendLine($"DTSTAMP:{FormatoFechaIcal(DateTime.UtcNow)}");

                var dtStart = regla.FechaInicio.Date.Add(regla.HoraInicio);
                sb.AppendLine($"DTSTART:{FormatoFechaIcal(dtStart)}");

                DateTime dtEnd;
                if (regla.FechaFin.HasValue) dtEnd = regla.FechaFin.Value.Date.Add(regla.HoraFin);
                else dtEnd = dtStart.AddHours(1);

                sb.AppendLine($"DTEND:{FormatoFechaIcal(dtEnd)}");
                sb.AppendLine($"SUMMARY:{EscapeIcal(regla.Titulo)}");

                string rrule = GenerarRRule(regla);
                if (!string.IsNullOrEmpty(rrule)) sb.AppendLine(rrule);

                sb.AppendLine("END:VEVENT");
            }
            sb.AppendLine("END:VCALENDAR");
            return sb.ToString();
        }

        //  -auxiliars
        private DateTime ObtenerValorFecha(string linea)
        {
            try
            {
                var partes = linea.Split(':');
                if (partes.Length < 2) return DateTime.MinValue;
                string f = partes[1].Trim();
                if (f.Length == 8 && int.TryParse(f, out _))
                {
                    int y = int.Parse(f.Substring(0, 4)), m = int.Parse(f.Substring(4, 2)), d = int.Parse(f.Substring(6, 2));
                    return new DateTime(y, m, d, 0, 0, 0, DateTimeKind.Utc);
                }
                if (f.EndsWith("Z") && f.Length >= 16)
                {
                    int y = int.Parse(f.Substring(0, 4)), m = int.Parse(f.Substring(4, 2)), d = int.Parse(f.Substring(6, 2));
                    int h = int.Parse(f.Substring(9, 2)), mn = int.Parse(f.Substring(11, 2)), s = int.Parse(f.Substring(13, 2));
                    return new DateTime(y, m, d, h, mn, s, DateTimeKind.Utc);
                }
            }
            catch { }
            return DateTime.MinValue;
        }

        private void AnalizarRrule(string rrule, ReglaCalendario regla)
        {
            if (rrule.Contains("FREQ=WEEKLY")) regla.Tipo = rrule.Contains("MO,TU,WE,TH,FR") ? TipoRegla.LunesViernes : TipoRegla.Semanal;
            else if (rrule.Contains("FREQ=MONTHLY")) regla.Tipo = TipoRegla.Mensual;
            else if (rrule.Contains("FREQ=YEARLY")) regla.Tipo = TipoRegla.Anual;

            var m = Regex.Match(rrule, @"UNTIL=([0-9TZ]+)");
            if (m.Success)
            {
                var fin = ObtenerValorFecha(":" + m.Groups[1].Value);
                if (fin != DateTime.MinValue) regla.FechaFin = fin;
            }
        }

        private string FormatoFechaIcal(DateTime dt) => dt.ToUniversalTime().ToString("yyyyMMddTHHmmssZ");
        private string EscapeIcal(string t) => string.IsNullOrEmpty(t) ? "" : t.Replace(",", "\\,").Replace(";", "\\;");
        private string GenerarRRule(ReglaCalendario r) => r.Tipo switch
        {
            TipoRegla.Semanal => "RRULE:FREQ=WEEKLY",
            TipoRegla.Mensual => "RRULE:FREQ=MONTHLY",
            TipoRegla.Anual => "RRULE:FREQ=YEARLY",
            TipoRegla.LunesViernes => "RRULE:FREQ=WEEKLY;BYDAY=MO,TU,WE,TH,FR",
            _ => ""
        };
    }
}