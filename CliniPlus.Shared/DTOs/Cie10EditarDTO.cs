using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class Cie10EditarDTO
    {
        [Required]
        [MaxLength(300)]
        public string Descripcion { get; set; } = string.Empty;
    }
}
