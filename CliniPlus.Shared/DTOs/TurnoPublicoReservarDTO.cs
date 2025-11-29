using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class TurnoPublicoReservarDTO
    {
        [Required]
        public string DNI { get; set; } = "";

        [Required]
        public string Nombre { get; set; } = "";

        [Required]
        public string Apellido { get; set; } = "";

        public int? ObraSocialId { get; set; }
        public string? NumeroAfiliado { get; set; }

        [Required]
        public int MedicoId { get; set; }

        [Required]
        public int TipoTurnoId { get; set; }

        [Required]
        public int TurnoId { get; set; }
    }
}
