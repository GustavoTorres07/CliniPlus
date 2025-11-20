using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class ConsultaDetalleDTO
    {
        public int IdConsulta { get; set; }
        public int TurnoId { get; set; }
        public int MedicoId { get; set; }
        public string MedicoNombre { get; set; } = null!;
        public DateTime FechaHora { get; set; }
        public string? Notas { get; set; }
        public List<ConsultaDiagnosticoItemDTO> Diagnosticos { get; set; } = new();
    }
}
