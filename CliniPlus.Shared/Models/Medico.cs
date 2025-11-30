using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.Models
{
    [Table("Medico")]
    public class Medico
    {
        [Key]
        [Column("IdMedico")]
        public int IdMedico { get; set; }

        [Required]
        [Column("UsuarioId")]
        public int UsuarioId { get; set; }

        [MaxLength(1000)]
        public string? Bio { get; set; }

        [MaxLength(300)]
        public string? FotoUrl { get; set; }

        [Required]
        public int DefaultSlotMin { get; set; } = 30;

        [Required]
        public bool IsActive { get; set; } = true;

        public int? EspecialidadId { get; set; }

        public Especialidad? Especialidad { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public virtual Usuario Usuario { get; set; } = null!;

        public virtual ICollection<MedicoHorario> Horarios { get; set; } = new List<MedicoHorario>();

        public virtual ICollection<MedicoBloqueo> Bloqueos { get; set; } = new List<MedicoBloqueo>();

        public virtual ICollection<Turno> Turnos { get; set; } = new List<Turno>();

        public virtual ICollection<Consulta> Consultas { get; set; } = new List<Consulta>();

        public virtual ICollection<HistoriaClinicaEntrada> HistoriaClinica { get; set; } = new List<HistoriaClinicaEntrada>();
    }
}
