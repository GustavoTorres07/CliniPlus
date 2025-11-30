using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class ConsultaDiagnosticoItemDTO
    {
        [Required, MaxLength(10)]
        public string CIE10Codigo { get; set; } = null!;

        [MaxLength(500)]
        public string? Comentario { get; set; }

        public bool Principal { get; set; } = false;
    }
}
