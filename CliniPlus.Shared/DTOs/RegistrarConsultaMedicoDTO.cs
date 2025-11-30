using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class RegistrarConsultaMedicoDTO
    {
        public int TurnoId { get; set; }

        public string? Notas { get; set; }

        public List<RegistrarConsultaDiagnosticoDTO> Diagnosticos { get; set; } = new();

        public TurnoEstadoDTO EstadoFinal { get; set; } = TurnoEstadoDTO.Atendido;
    }

}
