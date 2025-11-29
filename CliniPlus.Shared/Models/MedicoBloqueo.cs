using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.Models
{
    [Table("MedicoBloqueo")]
    public class MedicoBloqueo
    {
        [Key]
        [Column("IdBloqueo")]
        public int IdBloqueo { get; set; }

        [Required]
        [Column("MedicoId")]
        public int MedicoId { get; set; }

        [Required]
        public DateTime Desde { get; set; }

        [Required]
        public DateTime Hasta { get; set; }

        [MaxLength(200)]
        public string? Motivo { get; set; }

        [ForeignKey(nameof(MedicoId))]
        public virtual Medico Medico { get; set; } = null!;
    }
}
