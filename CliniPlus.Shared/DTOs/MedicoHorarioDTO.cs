using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class MedicoHorarioDTO
    {
        public int IdHorario { get; set; }
        public int MedicoId { get; set; }
        public byte DiaSemana { get; set; } // 0..6
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public int? SlotMinOverride { get; set; }
        public bool Activo { get; set; }
    }
}
