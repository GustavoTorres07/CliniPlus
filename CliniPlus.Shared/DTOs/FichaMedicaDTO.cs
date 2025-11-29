using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class FichaMedicaDTO
    {
        public int PacienteId { get; set; }
        public string? GrupoSanguineo { get; set; }
        public string? Alergias { get; set; }
        public string? EnfermedadesCronicas { get; set; }
        public string? MedicacionHabitual { get; set; }
        public string? Observaciones { get; set; }
    }
}
