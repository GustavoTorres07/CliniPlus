using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class TurnoSlotDTO
    {
        public int IdTurno { get; set; }              // 0 = no hay turno generado (solo hueco/bloqueo)
        public DateTime ScheduledAtUtc { get; set; }  // inicio del slot
        public int DuracionMin { get; set; }

        public bool EsReservable { get; set; }        // true = paciente puede reservar
        public string Estado { get; set; } = "";      // "Disponible", "Reservado", "Bloqueado", etc.

        // Info de contexto para mostrar en la UI
        public string HoraLocalTexto { get; set; } = ""; // ej "09:30"
        public string? TipoTurnoNombre { get; set; }
    }
}
