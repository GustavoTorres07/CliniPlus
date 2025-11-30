using CliniPlus.Api.Repositories.Contrato;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CliniPlus.Api.Repositories.Implementa
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IConfiguration _config;

        public TokenRepository(IConfiguration config) => _config = config;

        public string EmitirToken(int usuarioId, string nombre, string apellido, string email, string rol, int horasValidez = 8)
        {
            var jwt = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new("sub", usuarioId.ToString()),
            new("name", $"{nombre} {apellido}"),
            new("email", email),
            new("role", rol),

            new(ClaimTypes.NameIdentifier, usuarioId.ToString()),
            new(ClaimTypes.Name, $"{nombre} {apellido}"),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Role, rol)
        };

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(horasValidez),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
