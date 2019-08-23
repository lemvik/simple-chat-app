using System.Threading.Tasks;
using LemVic.Services.Chat.Protocol;
using LemVic.Services.Chat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LemVic.Services.Chat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService AuthService;

        public AuthController(IAuthService authService)
        {
            AuthService = authService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthParams authParams)
        {
            var (user, token) = await AuthService.Authenticate(authParams.Name, authParams.Password);
            return Ok(new ChatUserAuthDescription {Alias = user.Alias, Token = token});
        }

        [AllowAnonymous]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateParams createParams)
        {
            var (user, token) = await AuthService.Create(createParams.Name, createParams.Password, createParams.Alias);
            return Ok(new ChatUserAuthDescription {Alias = user.Alias, Token = token});
        }
    }
}
