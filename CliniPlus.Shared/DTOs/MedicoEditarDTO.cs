using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class MedicoEditarDTO
    {
        public string? Bio { get; set; }
        public string? FotoUrl { get; set; }
        public int DefaultSlotMin { get; set; }
        public int? EspecialidadId { get; set; }
    }
}
