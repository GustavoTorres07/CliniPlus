using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.Models
{
    [Table("Consulta")]
    public class Consulta
    {
        [Key]
        [Column("IdConsulta")]
        public int IdConsulta { get; set; }

        [Required]
        [Column("TurnoId")]
        public int TurnoId { get; set; }

        [Required]
        [Column("MedicoId")]
        public int MedicoId { get; set; }

        [Required]
        public DateTime FechaHora { get; set; } = DateTime.UtcNow;

        [MaxLength(2000)]
        public string? Notas { get; set; }

        [ForeignKey(nameof(TurnoId))]
        public virtual Turno Turno { get; set; } = null!;

        [ForeignKey(nameof(MedicoId))]
        public virtual Medico Medico { get; set; } = null!;

        public virtual ICollection<ConsultaDiagnostico> Diagnosticos { get; set; } = new List<ConsultaDiagnostico>();
        public virtual ICollection<HistoriaClinicaEntrada> HistoriaClinica { get; set; } = new List<HistoriaClinicaEntrada>();
    }
}
