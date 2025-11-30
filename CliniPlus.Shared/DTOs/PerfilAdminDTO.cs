using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class PerfilAdminDTO
    {
        public int UsuarioId { get; set; }

        public string Nombre { get; set; } = "";

        public string Apellido { get; set; } = "";

        public string Email { get; set; } = "";

        public string Rol { get; set; } = "Administrador";

        public DateTime FechaRegistro { get; set; }
    }
}
