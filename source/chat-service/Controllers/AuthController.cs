using System.Threading.Tasks;
using LemVic.Services.Chat.DataAccess.Models;
using LemVic.Services.Chat.Protocol;
using LemVic.Services.Chat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LemVic.Services.Chat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<ChatUser> SignInManager;
        private readonly UserManager<ChatUser>   UserManager;
        private readonly ITokenService           TokenService;

        public AuthController(SignInManager<ChatUser> signInManager,
                              UserManager<ChatUser>   userManager,
                              ITokenService           tokenService)
        {
            SignInManager = signInManager;
            UserManager   = userManager;
            TokenService  = tokenService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthParams authParams)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var signInResult = await SignInManager.PasswordSignInAsync(authParams.Name,
                                                                       authParams.Password,
                                                                       true,
                                                                       false);

            if (signInResult.Succeeded)
            {
                var userResult = await UserManager.FindByNameAsync(authParams.Name);
                var token      = await TokenService.Create(authParams.Name, userResult.Alias);
                return Ok(new ChatUserAuthDescription {Alias = userResult.Alias, Token = token});
            }

            return Unauthorized();
        }

        [AllowAnonymous]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateParams createParams)
        {
            var chatUser = new ChatUser {Alias = createParams.Alias, UserName = createParams.Name};
            var createResult = await UserManager.CreateAsync(chatUser,
                                                             createParams.Password);


            if (createResult.Succeeded)
            {
                await SignInManager.SignInAsync(chatUser, isPersistent: true);
                var token = await TokenService.Create(createParams.Name, createParams.Alias);
                return Ok(new ChatUserAuthDescription {Alias = createParams.Alias, Token = token});
            }

            foreach (var error in createResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Conflict();
        }
    }
}
