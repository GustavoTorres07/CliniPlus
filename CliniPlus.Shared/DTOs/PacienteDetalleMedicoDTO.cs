using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class PacienteDetalleMedicoDTO
    {
        public int PacienteId { get; set; }
        public string NombreCompleto { get; set; } = "";
        public string DNI { get; set; } = "";
        public string? Email { get; set; }
        public string? Telefono { get; set; }

        public string? ObraSocialNombre { get; set; }
        public string? NumeroAfiliado { get; set; }

        // Ficha médica resumida
        public FichaMedicaDTO? Ficha { get; set; }
    }
}
