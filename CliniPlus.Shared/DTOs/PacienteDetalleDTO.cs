using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class PacienteDetalleDTO
    {
        public int IdPaciente { get; set; }

        // Principal identificador de persona
        public string DNI { get; set; } = null!;

        // Si tiene cuenta vinculada
        public int? UsuarioId { get; set; }
        public string? NombreCompleto { get; set; }

        public bool IsProvisional { get; set; }
        public bool IsActive { get; set; }

        // Datos de contacto
        public string? Telefono { get; set; }
        public string? Email { get; set; }

        // Obra social
        public int? ObraSocialId { get; set; }
        public string? ObraSocialNombre { get; set; }
        public string? NumeroAfiliado { get; set; }
    }
}
