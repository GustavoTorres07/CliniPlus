using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class HistorialRapidoItemDTO
    {
        public DateTime Fecha { get; set; }
        public string CIE10Codigo { get; set; } = "";
        public string CIE10Descripcion { get; set; } = "";
    }
}
