using Calendario.Modelos;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Calendario.Servicios
{
    public class CalendarEngine
    {
        public bool IsDateInCalendars(DateTime date, List<CalendarioDefinition> calendars)
{
    foreach (var cal in calendars)
    {
        if (cal.Reglas == null) continue;
        foreach (var regla in cal.Reglas)
        {
            // 1. Verificar rango de validez
            if (date.Date < regla.FechaInicio.Date || date.Date > regla.FechaFin.Date) continue;

            // 2. Lógica por tipo de regla
            bool aplica = regla.Tipo switch
            {
                TipoRegla.Puntual => date.Date == regla.FechaInicio.Date,
                TipoRegla.Semanal => regla.DiasSemana.Contains(GetIsoDayOfWeek(date)),
                TipoRegla.Mensual => date.Day == regla.FechaInicio.Day,
                TipoRegla.Anual   => date.Day == regla.FechaInicio.Day && date.Month == regla.FechaInicio.Month,
                TipoRegla.Rango   => true,
                _ => false
            };

            if (aplica) return true;
        }
    }
    return false;
}

private int GetIsoDayOfWeek(DateTime date) => date.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)date.DayOfWeek;

        private bool VerificarRegla(DateTime fecha, ReglaCalendario regla)
        {
            if (fecha.Date < regla.FechaInicio.Date || fecha.Date > regla.FechaFin.Date) return false;

            switch (regla.Tipo)
            {
                case TipoRegla.Puntual: return fecha.Date == regla.FechaInicio.Date;
                case TipoRegla.Rango: return true;
                case TipoRegla.Semanal:
                    int dia = (int)fecha.DayOfWeek;
                    if (dia == 0) dia = 7;
                    return regla.DiasSemana?.Contains(dia) ?? false;
                default: return false;
            }
        }

        public string ExportToICal(CalendarioDefinition? calendario)
        {
            var ical = new Ical.Net.Calendar();
            if (calendario?.Reglas == null) return new CalendarSerializer().SerializeToString(ical) ?? "";

            foreach (var regla in calendario.Reglas)
            {
                var vEvent = new CalendarEvent
                {
                    Summary = regla.Titulo,
                    Start = new CalDateTime(regla.FechaInicio.Date.Add(regla.HoraInicio)),
                    End = new CalDateTime(regla.FechaFin.Date.Add(regla.HoraFin))
                };
                ical.Events.Add(vEvent);
            }
            return new CalendarSerializer().SerializeToString(ical) ?? "";
        }

        public List<ReglaCalendario> ImportFromICal(string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return new List<ReglaCalendario>();
            var ical = Ical.Net.Calendar.Load(content);
            var nuevasReglas = new List<ReglaCalendario>();

            if (ical?.Events == null) return nuevasReglas;

            foreach (var vEvent in ical.Events)
            {
                if (vEvent.Start == null || vEvent.End == null) continue;
                DateTime inicio = vEvent.Start.Value;
                DateTime fin = vEvent.End.Value;

                nuevasReglas.Add(new ReglaCalendario
                {
                    Titulo = vEvent.Summary ?? "Evento Importado",
                    Tipo = TipoRegla.Puntual,
                    FechaInicio = inicio,
                    FechaFin = fin,
                    HoraInicio = inicio.TimeOfDay,
                    HoraFin = fin.TimeOfDay,
                    DiasSemana = new List<int>()
                });
            }
            return nuevasReglas;
        }
        public List<ReglaCalendario> ParsearIcal(string contenidoIcs)
{
    var reglas = new List<ReglaCalendario>();
    var lineas = contenidoIcs.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
    ReglaCalendario? actual = null;

    foreach (var linea in lineas)
    {
        if (linea.StartsWith("BEGIN:VEVENT")) actual = new ReglaCalendario { Tipo = TipoRegla.Puntual };
        else if (linea.StartsWith("SUMMARY:") && actual != null) actual.Titulo = linea.Replace("SUMMARY:", "").Trim();
        else if (linea.StartsWith("DTSTART:") && actual != null) {
            var fechaStr = linea.Replace("DTSTART:", "").Trim(); // Formato YYYYMMDDTHHMMSS
            if (DateTime.TryParseExact(fechaStr.Substring(0, 8), "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var f))
                actual.FechaInicio = f;
            if (fechaStr.Contains("T")) actual.HoraInicio = TimeSpan.ParseExact(fechaStr.Substring(9, 4), "hhmm", null);
        }
        else if (linea.StartsWith("END:VEVENT") && actual != null) {
            actual.FechaFin = actual.FechaInicio;
            actual.HoraFin = actual.HoraInicio.Add(TimeSpan.FromHours(1));
            reglas.Add(actual);
            actual = null;
        }
    }
    return reglas;
}
    }

    
}