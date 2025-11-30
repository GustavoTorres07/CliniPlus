using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class PacienteActivarCuentaDTO
    {
        [Required]
        public int PacienteId { get; set; }

        [Required]
        public int UsuarioId { get; set; }   // usuario YA creado

        [Required]
        public string DNI { get; set; } = "";

        public string? Email { get; set; }
        public string? Telefono { get; set; }
    }
}

