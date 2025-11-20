using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.Models
{
    [Table("FichaMedica")]
    public class FichaMedica
    {
        [Key]
        [Column("IdFicha")]
        public int IdFicha { get; set; }

        [Required]
        [Column("PacienteId")]
        public int PacienteId { get; set; }

        [MaxLength(5)]
        public string? GrupoSanguineo { get; set; }

        [MaxLength(500)]
        public string? Alergias { get; set; }

        [MaxLength(500)]
        public string? EnfermedadesCronicas { get; set; }

        [MaxLength(500)]
        public string? MedicacionHabitual { get; set; }

        [MaxLength(1000)]
        public string? Observaciones { get; set; }

        // Navs
        [ForeignKey(nameof(PacienteId))]
        public virtual Paciente Paciente { get; set; } = null!;
    }
}
