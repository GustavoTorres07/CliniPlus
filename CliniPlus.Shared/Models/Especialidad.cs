using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.Models
{
    [Table("Especialidad")]
    public class Especialidad
    {
        [Key]
        [Column("IdEspecialidad")]
        public int IdEspecialidad { get; set; }

        [Required, MaxLength(120)]
        public string Nombre { get; set; } = null!;

        [MaxLength(200)]
        public string? Descripcion { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;
        public virtual ICollection<Medico> Medicos { get; set; } = new List<Medico>();
    }
}
