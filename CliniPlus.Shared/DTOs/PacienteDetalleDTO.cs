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
        public string DNI { get; set; } = null!;
        public bool IsProvisional { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public int? ObraSocialId { get; set; }
        public string? ObraSocialNombre { get; set; }
        public string? NumeroAfiliado { get; set; }
        public bool IsActive { get; set; }
    }
}
