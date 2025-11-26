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

        // 👇 ahora SON STRINGS para viajar por JSON
        public string HoraInicio { get; set; } = "";  // "08:00"
        public string HoraFin { get; set; } = "";     // "12:00"

        public int? SlotMinOverride { get; set; }
        public bool Activo { get; set; }
    }
}
