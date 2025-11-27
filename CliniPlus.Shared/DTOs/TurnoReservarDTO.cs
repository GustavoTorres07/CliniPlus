using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class TurnoReservarDTO
    {
        [Required]
        public int TurnoId { get; set; }

        [Required]
        public int TipoTurnoId { get; set; }
    }
}
