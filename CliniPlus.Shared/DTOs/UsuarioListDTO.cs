using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class UsuarioListDTO
    {
        public int IdUsuario { get; set; }

        public string NombreCompleto { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Rol { get; set; } = null!;

        public bool IsActive { get; set; }
    }
}
