using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class TurnoAgendaMedicoDTO
    {
        public int IdTurno { get; set; }

        public DateTime ScheduledAtUtc { get; set; }

        public int DuracionMin { get; set; }

        public string Estado { get; set; } = "";

        public string? PacienteNombre { get; set; }

        public bool EsPacienteActual { get; set; }

        public int? PacienteId { get; set; }

        public string? TipoTurnoNombre { get; set; }
    }
}
