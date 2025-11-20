using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.Models
{
    [Table("CIE10")]
    public class CIE10
    {
        [Key]
        [MaxLength(10)]
        [Column("Codigo")]
        public string Codigo { get; set; } = null!;

        [Required, MaxLength(300)]
        public string Descripcion { get; set; } = null!;

        public virtual ICollection<ConsultaDiagnostico> ConsultaDiagnosticos { get; set; } = new List<ConsultaDiagnostico>();
        public virtual ICollection<HistoriaClinicaEntrada> HistoriaClinicaEntradas { get; set; } = new List<HistoriaClinicaEntrada>();
    }
}
