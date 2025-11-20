using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.Models
{
    [Table("MedicoHorario")]
    public class MedicoHorario
    {
        [Key]
        [Column("IdHorario")]
        public int IdHorario { get; set; }

        [Required]
        [Column("MedicoId")]
        public int MedicoId { get; set; }

        /// <summary>0=Dom .. 6=Sab</summary>
        [Required]
        public byte DiaSemana { get; set; }

        [Required]
        public TimeSpan HoraInicio { get; set; }

        [Required]
        public TimeSpan HoraFin { get; set; }

        public int? SlotMinOverride { get; set; }

        [Required]
        [Column("Activo")]
        public bool Activo { get; set; } = true;

        // Nav
        [ForeignKey(nameof(MedicoId))]
        public virtual Medico Medico { get; set; } = null!;
    }
}
