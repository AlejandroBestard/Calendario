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
        public bool IsDateInCalendars(DateTime fecha, List<CalendarioDefinition>? calendarios)
        {
            if (calendarios == null) return false;

            foreach (var cal in calendarios)
            {
                if (cal.Reglas == null) continue;
                foreach (var regla in cal.Reglas)
                {
                    if (VerificarRegla(fecha, regla))
                    {
                        var eliminado = regla.Excepciones?.Any(e => e.FechaOriginal.Date == fecha.Date && e.Tipo == TipoExcepcion.Eliminar) ?? false;
                        if (eliminado) continue;

                        var modif = regla.Excepciones?.FirstOrDefault(e => e.FechaOriginal.Date == fecha.Date && e.Tipo == TipoExcepcion.Modificar);
                        if (modif != null)
                        {
                            var hora = fecha.TimeOfDay;
                            return hora >= (modif.NuevaHoraInicio ?? TimeSpan.Zero) && hora <= (modif.NuevaHoraFin ?? TimeSpan.Zero);
                        }

                        var horaRegla = fecha.TimeOfDay;
                        return horaRegla >= regla.HoraInicio && horaRegla <= regla.HoraFin;
                    }
                }
            }
            return false;
        }

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
    }
}