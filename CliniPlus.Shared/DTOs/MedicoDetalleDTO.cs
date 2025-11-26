using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class MedicoDetalleDTO
    {
        public int IdMedico { get; set; }
        public int UsuarioId { get; set; }

        public string NombreCompleto { get; set; } = "";
        public string? Email { get; set; }

        public string? Bio { get; set; }
        public string? FotoUrl { get; set; }

        public int DefaultSlotMin { get; set; }
        public bool IsActive { get; set; }

        public int? EspecialidadId { get; set; }
        public string? EspecialidadNombre { get; set; }
    }
}
