using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class PerfilPacienteDTO
    {
        public string Nombre { get; set; } = "";

        public string Apellido { get; set; } = "";

        public string Email { get; set; } = "";

        public string Telefono { get; set; } = "";

        public string DNI { get; set; } = "";

        public int? ObraSocialId { get; set; }

        public string? ObraSocialNombre { get; set; }

        public string NumeroAfiliado { get; set; } = "";

        public DateTime FechaRegistro { get; set; }
    }
}
