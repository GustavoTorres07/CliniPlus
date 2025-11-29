using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class CambiarPasswordDTO
    {
        [Required]
        [MinLength(6)]
        [MaxLength(200)]
        public string PasswordActual { get; set; } = "";

        [Required]
        [MinLength(6)]
        [MaxLength(200)]
        public string PasswordNueva { get; set; } = "";
    }
}
