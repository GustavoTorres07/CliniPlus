using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class TurnoReservarSlotDTO
    {
        public int MedicoId { get; set; }

        public DateTime ScheduledAtUtc { get; set; }

        public int TipoTurnoId { get; set; }
    }
}
