using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Movil.Services.Contrato
{
    public interface ITokenService
    {
        Task GuardarAsync(string token, bool recordar);
        Task<string?> ObtenerAsync();
        Task BorrarAsync();
    }
}
