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
        [MaxLength(1000)]
        public string? Bio { get; set; }

        [MaxLength(300)]
        public string? FotoUrl { get; set; }

        public int? DefaultSlotMin { get; set; }
        public bool? IsActive { get; set; }

        /// <summary>Reemplazo total opcional</summary>
        public List<int>? Especialidades { get; set; }
    }
}
