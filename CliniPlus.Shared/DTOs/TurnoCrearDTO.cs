using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class TurnoCrearDTO
    {
        public int MedicoId { get; set; }
        public int? PacienteId { get; set; } 
        public int? TipoTurnoId { get; set; }
        public DateTime ScheduledAtUtc { get; set; }
        public int DuracionMin { get; set; }
        public string Estado { get; set; } = "Disponible";
    }
}
