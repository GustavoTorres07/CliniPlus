using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class MedicoDisponiblePacienteDTO
    {
        public int IdMedico { get; set; }

        public string NombreCompleto { get; set; } = string.Empty;

        public string? EspecialidadNombre { get; set; }

        public string? Bio { get; set; }

        public string? FotoUrl { get; set; }

        public bool TieneAgenda { get; set; }

        public DateTime? ProximoTurnoUtc { get; set; }
    }
}
