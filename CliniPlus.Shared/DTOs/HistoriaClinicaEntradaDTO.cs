using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class HistoriaClinicaEntradaDTO
    {
        public int IdEntrada { get; set; }
        public int PacienteId { get; set; }
        public int MedicoId { get; set; }
        public string MedicoNombre { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public string CIE10Codigo { get; set; } = null!;
        public string CIE10Descripcion { get; set; } = null!;
        public string? Notas { get; set; }
        public int? TurnoId { get; set; }
        public int? ConsultaId { get; set; }
    }
}
