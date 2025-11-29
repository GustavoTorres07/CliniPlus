using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class RegistrarConsultaDiagnosticoDTO
    {
        public string CIE10Codigo { get; set; } = "";
        public string? Comentario { get; set; }
        public bool Principal { get; set; }
    }
}
