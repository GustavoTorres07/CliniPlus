using CliniPlus.Movil.Services.Contrato;
using CliniPlus.Shared.DTOs;
using System.Net.Http.Json;

namespace CliniPlus.Movil.Services.Implementa
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IHttpClientFactory _http;

        public UsuarioService(IHttpClientFactory http)
        {
            _http = http;
        }

        public async Task<List<UsuarioListDTO>?> ListarAsync()
        {
            var cli = _http.CreateClient("ApiCliniPlus");
            var res = await cli.GetAsync("api/usuarios/listar");
            return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<List<UsuarioListDTO>>() : null;
        }

        public async Task<UsuarioDetalleDTO?> ObtenerAsync(int id)
        {
            var cli = _http.CreateClient("ApiCliniPlus");
            var res = await cli.GetAsync($"api/usuarios/{id}");
            return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<UsuarioDetalleDTO>() : null;
        }

        public async Task<UsuarioDetalleDTO?> CrearAsync(UsuarioCrearDTO dto)
        {
            var cli = _http.CreateClient("ApiCliniPlus");
            var res = await cli.PostAsJsonAsync("api/usuarios/crear", dto);
            return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<UsuarioDetalleDTO>() : null;
        }

        public async Task<UsuarioDetalleDTO?> EditarAsync(int id, UsuarioEditarDTO dto)
        {
            var cli = _http.CreateClient("ApiCliniPlus");
            var res = await cli.PutAsJsonAsync($"api/usuarios/editar/{id}", dto);
            return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<UsuarioDetalleDTO>() : null;
        }

        public async Task<bool> CambiarEstadoAsync(int id, bool activo)
        {
            var cli = _http.CreateClient("ApiCliniPlus");
            var body = new UsuarioEstadoDTO { IsActive = activo };
            var res = await cli.PatchAsJsonAsync($"api/usuarios/estado/{id}", body);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> CambiarPasswordAdminAsync(int idUsuario, string nuevaPassword)
        {
            var cli = _http.CreateClient("ApiCliniPlus");

            var body = new { NuevaPassword = nuevaPassword };

            var res = await cli.PatchAsJsonAsync($"api/usuarios/password/{idUsuario}", body);

            return res.IsSuccessStatusCode;
        }
    }
}
