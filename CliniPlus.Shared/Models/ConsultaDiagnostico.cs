using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.Models
{
    [Table("ConsultaDiagnostico")]
    public class ConsultaDiagnostico
    {
        [Required] 
        [Column("ConsultaId")]
        public int ConsultaId { get; set; }

        [Required, MaxLength(10)] 
        [Column("CIE10Codigo")]
        public string CIE10Codigo { get; set; } = null!;

        [MaxLength(500)]
        public string? Comentario { get; set; }

        [Required]
        public bool Principal { get; set; } = false;

        public virtual Consulta Consulta { get; set; } = null!;
        public virtual CIE10 CIE10 { get; set; } = null!;
    }
}
