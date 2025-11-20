using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class TurnoCrearDTO
    {
        [Required]
        public int MedicoId { get; set; }

        public int? PacienteId { get; set; }      // null si se publica como disponible
        public int? TipoTurnoId { get; set; }

        [Required]
        public DateTime ScheduledAtUtc { get; set; }

        [Required]
        public int DuracionMin { get; set; }
    }
}
