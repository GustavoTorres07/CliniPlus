using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.Models
{
    [Table("MedicoEspecialidad")]
    public class MedicoEspecialidad
    {
        [Column("MedicoId")]
        public int MedicoId { get; set; }

        [Column("EspecialidadId")]
        public int EspecialidadId { get; set; }

        // Navs
        public virtual Medico Medico { get; set; } = null!;
        public virtual Especialidad Especialidad { get; set; } = null!;
    }
}
