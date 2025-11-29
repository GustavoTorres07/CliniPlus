using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class AuthLoginResponseDTO
    {
        public int UsuarioId { get; set; }
        public string NombreCompleto { get; set; } = null!;
        public string Rol { get; set; } = null!;
        public string Token { get; set; } = null!;

        public bool DebeCambiarPassword { get; set; }

    }
}
