using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LemVic.Services.Chat.DataAccess;
using LemVic.Services.Chat.DataAccess.Models;
using LemVic.Services.Chat.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LemVic.Services.Chat.Services
{
    public class AuthService : IAuthService
    {
        private readonly ChatDbContext              ChatDbContext;
        private readonly IOptions<SecuritySettings> SecuritySettings;

        public AuthService(ChatDbContext chatDbContext, IOptions<SecuritySettings> securitySettings)
        {
            ChatDbContext    = chatDbContext;
            SecuritySettings = securitySettings;
        }

        public async Task<(ChatUser User, string Token)> Authenticate(string userName, string password)
        {
            var userAuth = await ChatDbContext.Auths.FirstOrDefaultAsync(auth => auth.Login == userName
                                                                                 && auth.Password == password);

            if (userAuth == null)
            {
                throw new AuthenticationException();
            }

            var token = GenerateToken(userAuth);

            return (userAuth.User, token);
        }

        public async Task<(ChatUser User, string Token)> Create(string userName, string password, string alias)
        {
            var existingAuth = await ChatDbContext.Auths.FirstOrDefaultAsync(auth => auth.Login == userName
                                                                                     && auth.Password == password);

            if (existingAuth != null)
            {
                throw new DuplicateNameException();
            }

            var newAuth = new UserAuth {
                Login = userName, Password = password, User = new ChatUser {Alias = alias}, UserName = userName
            };
            await ChatDbContext.Auths.AddAsync(newAuth);
            await ChatDbContext.SaveChangesAsync();

            return (newAuth.User, GenerateToken(newAuth));
        }

        private string GenerateToken(UserAuth userAuth)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey    = Encoding.UTF8.GetBytes(SecuritySettings.Value.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new[] {new Claim(ClaimTypes.Name, userAuth.Login)}),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
