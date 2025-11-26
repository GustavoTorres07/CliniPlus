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
        // Usuario ya creado con Rol = "Medico"
        [Required]
        public int UsuarioId { get; set; }

        public string? Bio { get; set; }
        public string? FotoUrl { get; set; }

        public int DefaultSlotMin { get; set; } = 30;

        // Puede quedar sin especialidad al crearlo
        public int? EspecialidadId { get; set; }
    }
}
