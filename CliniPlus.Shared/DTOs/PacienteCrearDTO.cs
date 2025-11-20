using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class PacienteCrearDTO
    {
        [Required, MaxLength(8)]
        public string DNI { get; set; } = null!;

        [Phone, MaxLength(30)]
        public string? Telefono { get; set; }

        [EmailAddress, MaxLength(150)]
        public string? Email { get; set; }

        public int? ObraSocialId { get; set; }

        [MaxLength(50)]
        public string? NumeroAfiliado { get; set; }

        /// <summary>Si es reserva como invitado</summary>
        public bool IsProvisional { get; set; } = true;
    }
}
