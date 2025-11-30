using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.DTOs
{
    public class AuthLoginRequest
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato valido")]
        [MaxLength(150, ErrorMessage = "El email no puede exceder los 150 caracteres")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        [MaxLength(200, ErrorMessage = "La contraseña no puede exceder los 200 caracteres")]
        public string Password { get; set; } = null!;
    }
}
