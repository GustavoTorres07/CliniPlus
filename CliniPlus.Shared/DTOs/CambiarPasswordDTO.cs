using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class CambiarPasswordDTO
    {
        public string PasswordActual { get; set; } = "";
        public string PasswordNueva { get; set; } = "";
    }
}
