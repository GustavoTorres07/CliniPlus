using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class TurnoDetalleDTO
    {
        public int IdTurno { get; set; }

        public int MedicoId { get; set; }
        public string MedicoNombre { get; set; } = "";

        public int? PacienteId { get; set; }
        public string? PacienteNombre { get; set; }

        public DateTime ScheduledAtUtc { get; set; }
        public int DuracionMin { get; set; }

        public TurnoEstadoDTO Estado { get; set; }

        public int? TipoTurnoId { get; set; }
        public string? TipoTurnoNombre { get; set; }

        public bool IsActive { get; set; }
    }
}
