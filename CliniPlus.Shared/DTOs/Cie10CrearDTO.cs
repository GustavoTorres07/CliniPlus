using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class Cie10CrearDTO
    {
        [Required]
        [MaxLength(10)]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        [MaxLength(300)]
        public string Descripcion { get; set; } = string.Empty;
    }
}
