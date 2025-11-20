using CliniPlus.Movil.Services.Contrato;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Movil.Services.Implementa
{
    public class TokenService : ITokenService
    {
        private const string KEY = "jwt";

        public async Task GuardarAsync(string token, bool recordar)
        {
            // En MAUI, SecureStorage ya persiste entre sesiones.
            // Si no querés persistir cuando NO recuerda, lo guardamos igual
            // y en Logout lo limpiamos. Alternativamente, podrías usar Preferences.
            await SecureStorage.SetAsync(KEY, token);
        }

        public async Task<string?> ObtenerAsync() => await SecureStorage.GetAsync(KEY);

        public Task BorrarAsync()
        {
            SecureStorage.Remove(KEY);
            return Task.CompletedTask;
        }
    }
}
