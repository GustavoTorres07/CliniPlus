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

        public byte DiaSemana { get; set; } 

        public string HoraInicio { get; set; } = "";  

        public string HoraFin { get; set; } = "";    
        
        public int? SlotMinOverride { get; set; }

        public bool Activo { get; set; }
    }
}
