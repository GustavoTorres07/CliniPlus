using CliniPlus.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Movil.Services.Contrato
{
    internal interface IAuthService
    {
        Task<AuthLoginResponse?> LoginAsync(AuthLoginRequest req, bool recordar);
        Task LogoutAsync();
        Task<bool> PingAsync(); // opcional
    }
}
