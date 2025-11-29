using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class PacienteListDTO
    {
        public int IdPaciente { get; set; }
        public string DNI { get; set; } = null!;
        public string? NombreCompleto { get; set; }
        public string? Email { get; set; }
        public bool IsProvisional { get; set; }
        public bool IsActive { get; set; }
        public string? ObraSocialNombre { get; set; }
    }
}
