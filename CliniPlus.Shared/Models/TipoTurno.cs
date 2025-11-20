using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.Models
{
    [Table("TipoTurno")]
    public class TipoTurno
    {
        [Key]
        [Column("IdTipoTurno")]
        public int IdTipoTurno { get; set; }

        [Required, MaxLength(120)]
        public string Nombre { get; set; } = null!;

        [Required]
        public int DuracionMin { get; set; }

        [MaxLength(200)]
        public string? Descripcion { get; set; }

        [Required]
        [Column("Activo")]
        public bool Activo { get; set; } = true;

        public virtual ICollection<Turno> Turnos { get; set; } = new List<Turno>();
    }
}
