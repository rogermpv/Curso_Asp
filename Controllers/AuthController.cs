using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApiDia2.Models;
using WebApiDia2.NewFolder;
using WebApiDia2.Services;

namespace WebApiDia2.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;

        public AuthController(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            LoginServices logServ = new LoginServices();

            var user = logServ.AuthenticateUser(login);

            if (user == null)
                return Unauthorized();

            var tokenString = logServ.GetToken(user, login.ClientType, _jwtSettings);


            return Ok(new { Token = tokenString });

        }

    }
}
