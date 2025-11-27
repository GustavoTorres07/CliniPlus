using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class TurnoFiltroDTO
    {
        public int? MedicoId { get; set; }
        public int? PacienteId { get; set; }
        public TurnoEstadoDTO? Estado { get; set; }

        public DateTime? DesdeUtc { get; set; }
        public DateTime? HastaUtc { get; set; }

        public int? TipoTurnoId { get; set; }

        // Paginación
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
