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
        public string NombreCompleto { get; set; } = null!;
        public string? Bio { get; set; }
        public string? FotoUrl { get; set; }
        public int DefaultSlotMin { get; set; }
        public bool IsActive { get; set; }
        public List<EspecialidadDTO> Especialidades { get; set; } = new();
    }
}
