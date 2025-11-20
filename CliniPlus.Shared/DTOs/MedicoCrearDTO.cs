using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class MedicoCrearDTO
    {
        // Usuario
        [Required]
        public string Nombre { get; set; } = null!;

        [Required]
        public string Apellido { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        // Médico
        public string? Bio { get; set; }
        public string? FotoUrl { get; set; }
        public int DefaultSlotMin { get; set; } = 30;

        // UNA sola especialidad
        [Required]
        public int EspecialidadId { get; set; }
    }
}
