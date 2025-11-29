using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.Models
{
    [Table("Turno")]
    public class Turno
    {
        [Key]
        [Column("IdTurno")]
        public int IdTurno { get; set; }

        [Required]
        [Column("MedicoId")]
        public int MedicoId { get; set; }

        [Column("PacienteId")]
        public int? PacienteId { get; set; }

        [Column("TipoTurnoId")]
        public int? TipoTurnoId { get; set; }

        [Required]
        public DateTime ScheduledAtUtc { get; set; }

        [Required]
        public int DuracionMin { get; set; }

        [Required, MaxLength(20)]
        public string Estado { get; set; } = "Disponible";

        [Required]
        public bool IsActive { get; set; } = true;

        [ForeignKey(nameof(MedicoId))]
        public virtual Medico Medico { get; set; } = null!;

        [ForeignKey(nameof(PacienteId))]
        public virtual Paciente? Paciente { get; set; }

        [ForeignKey(nameof(TipoTurnoId))]
        public virtual TipoTurno? TipoTurno { get; set; }

        public virtual Consulta? Consulta { get; set; }
        public virtual ICollection<HistoriaClinicaEntrada> HistoriaClinica { get; set; } = new List<HistoriaClinicaEntrada>();
    }
}
