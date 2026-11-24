using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Oficina.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Oficina.Infrastructure.Auth
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;

        public TokenService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public string GerarToken(string usuarioId, string email, string nome, IEnumerable<string> roles)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, usuarioId),
                new(JwtRegisteredClaimNames.Email, email),
                new(JwtRegisteredClaimNames.Name, nome),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.ChaveSecreta));
            var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Emissor,
                audience: _jwtSettings.PublicoAlvo,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiracaoMinutos),
                signingCredentials: credenciais
            );

            return new JwtSecurityTokenHandler().WriteToken(token);


        }
    }
}
