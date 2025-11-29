using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class PacienteEditarDTO
    {
        [Required]
        [StringLength(8)]
        public string DNI { get; set; } = null!;
        public bool IsProvisional { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public int? ObraSocialId { get; set; }
        public string? NumeroAfiliado { get; set; }
        public int? UsuarioId { get; set; }  
    }
}
