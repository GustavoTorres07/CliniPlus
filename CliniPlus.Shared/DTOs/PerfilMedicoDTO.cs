using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class PerfilMedicoDTO
    {
        // Usuario
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = "";
        public string Apellido { get; set; } = "";
        public string Email { get; set; } = "";

        // Medico
        public int IdMedico { get; set; }
        public string? Bio { get; set; }
        public string? FotoUrl { get; set; }

        // Solo lectura
        public List<SimpleEspecialidadDTO> Especialidades { get; set; } = new();
    }

    public class SimpleEspecialidadDTO
    {
        public int IdEspecialidad { get; set; }
        public string Nombre { get; set; } = "";
    }
}
