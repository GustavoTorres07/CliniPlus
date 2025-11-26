using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class MedicoListadoDTO
    {
        public int IdMedico { get; set; }
        public int UsuarioId { get; set; }   // 👈 importante para navegación y reglas de negocio

        public string NombreCompleto { get; set; } = "";  // 👈 No null!, evita errores
        public string? Especialidad { get; set; }          // 👈 Puede ser null
        public string? Email { get; set; }                 // 👈 Puede ser null
        public bool IsActive { get; set; }

        public string? FotoUrl { get; set; }
    }
}
