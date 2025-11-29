using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class ConsultaCrearDTO
    {
        [Required]
        public int TurnoId { get; set; }

        [Required]
        public int MedicoId { get; set; }

        public DateTime? FechaHora { get; set; } 

        [MaxLength(2000)]
        public string? Notas { get; set; }

        public List<ConsultaDiagnosticoItemDTO> Diagnosticos { get; set; } = new();
    }
}
