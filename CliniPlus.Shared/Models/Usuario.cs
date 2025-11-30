using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.Models
{
    [Table("Usuario")]
    public class Usuario
    {
        [Key]
        [Column("IdUsuario")]
        public int IdUsuario { get; set; }

        [Required, MaxLength(100)]
        public string Nombre { get; set; } = null!;

        [Required, MaxLength(100)]
        public string Apellido { get; set; } = null!;

        [Required, MaxLength(150)]
        public string Email { get; set; } = null!;

        [Required, MaxLength(200)]
        public string PasswordHash { get; set; } = null!;

        [Required, MaxLength(20)]
        public string Rol { get; set; } = null!;

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        public virtual Medico? Medico { get; set; }

        public virtual Paciente? Paciente { get; set; }

        public bool RecuperarContrasena { get; set; } = false;

    }
}