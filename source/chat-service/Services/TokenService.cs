using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LemVic.Services.Chat.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LemVic.Services.Chat.Services
{
    public class TokenService : ITokenService
    {
        private readonly IOptions<SecuritySettings> SecuritySettings;

        public TokenService(IOptions<SecuritySettings> securitySettings)
        {
            SecuritySettings = securitySettings;
        }

        public Task<string> Create(string userLogin, string userAlias)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey    = Encoding.UTF8.GetBytes(SecuritySettings.Value.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, userLogin),
                    new Claim(ClaimTypes.NameIdentifier, userAlias)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Task.FromResult(tokenHandler.WriteToken(token));
        }
    }
}
