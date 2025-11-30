using CliniPlus.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Movil.Services.Contrato
{
    public interface IAuthService
    {
        Task<AuthLoginResponseDTO?> LoginAsync(AuthLoginRequest req, bool recordar);

        Task LogoutAsync();

        Task<AuthMeResponse?> MeAsync();   

        Task<bool> PingAsync();

        Task<bool> RecuperarPasswordAsync(string email);

    }
}
