using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class PacienteListadoMedicoDTO
    {
        public int PacienteId { get; set; }

        public string NombreCompleto { get; set; } = "";

        public string DNI { get; set; } = "";

        public string? Email { get; set; }

        public string? Telefono { get; set; }

        public int TurnosTotales { get; set; }

        public DateTime? UltimoTurnoUtc { get; set; }
    }
}
