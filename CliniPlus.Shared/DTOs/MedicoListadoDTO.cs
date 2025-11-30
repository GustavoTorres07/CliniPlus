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

        public int UsuarioId { get; set; }   

        public string NombreCompleto { get; set; } = "";  

        public string? Especialidad { get; set; }
        
        public string? Email { get; set; }    
        
        public bool IsActive { get; set; }

        public string? FotoUrl { get; set; }
    }
}
