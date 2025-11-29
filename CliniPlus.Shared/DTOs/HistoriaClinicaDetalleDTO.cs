using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class HistoriaClinicaDetalleDTO
    {
        public int IdEntrada { get; set; }
        public DateTime Fecha { get; set; }

        public string CIE10Codigo { get; set; } = "";
        public string CIE10Descripcion { get; set; } = "";
        public string? MedicoNombre { get; set; }

        public string? Notas { get; set; }

        // Relación con turno
        public int? TurnoId { get; set; }
        public DateTime? TurnoFechaHora { get; set; }
        public string? TipoTurnoNombre { get; set; }
        public string? EstadoTurno { get; set; }

        // Relación con consulta
        public int? ConsultaId { get; set; }
        public DateTime? ConsultaFechaHora { get; set; }
    }
}
