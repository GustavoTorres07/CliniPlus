using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.Models
{
    [Table("HistoriaClinicaEntrada")]
    public class HistoriaClinicaEntrada
    {
        [Key]
        [Column("IdEntrada")]
        public int IdEntrada { get; set; }

        [Required]
        [Column("PacienteId")]
        public int PacienteId { get; set; }

        [Required]
        [Column("MedicoId")]
        public int MedicoId { get; set; }

        [Required]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        [Required, MaxLength(10)]
        [Column("CIE10Codigo")]
        public string CIE10Codigo { get; set; } = null!;

        [MaxLength(2000)]
        public string? Notas { get; set; }

        [Column("TurnoId")]
        public int? TurnoId { get; set; }

        [Column("ConsultaId")]
        public int? ConsultaId { get; set; }

        // Navs
        [ForeignKey(nameof(PacienteId))]
        public virtual Paciente Paciente { get; set; } = null!;

        [ForeignKey(nameof(MedicoId))]
        public virtual Medico Medico { get; set; } = null!;

        [ForeignKey(nameof(CIE10Codigo))]
        public virtual CIE10 CIE10 { get; set; } = null!;

        [ForeignKey(nameof(TurnoId))]
        public virtual Turno? Turno { get; set; }

        [ForeignKey(nameof(ConsultaId))]
        public virtual Consulta? Consulta { get; set; }
    }
}
