using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class TurnoCambiarEstadoDTO
    {
        [Required]
        public TurnoEstadoDTO Estado { get; set; }
    }
}
