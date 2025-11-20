using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class MedicoBloqueoDTO
    {
        public int IdBloqueo { get; set; }
        public int MedicoId { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public string? Motivo { get; set; }
    }
}
