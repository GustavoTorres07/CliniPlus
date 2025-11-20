using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class MedicoCrearDTO
    {
        [Required]
        public int UsuarioId { get; set; } // ya creado el usuario con rol Médico

        [MaxLength(1000)]
        public string? Bio { get; set; }

        [MaxLength(300)]
        public string? FotoUrl { get; set; }

        public int DefaultSlotMin { get; set; } = 30;

        /// <summary>Ids de especialidades</summary>
        public List<int> Especialidades { get; set; } = new();
    }
}
