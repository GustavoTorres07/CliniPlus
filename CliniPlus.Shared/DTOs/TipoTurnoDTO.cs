using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class TipoTurnoDTO
    {
        public int IdTipoTurno { get; set; }

        public string Nombre { get; set; } = "";

        public int DuracionMin { get; set; }
    }
}
