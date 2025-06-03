using Microsoft.AspNetCore.Mvc;
using RestAPI.Services;
using static RestAPI.Services.AuthService;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly AuthService _authService;
        public AuthController(AuthService authService)
        {
           _authService = authService;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserModel login)
        {
            var user = _authService.ValidateUser(login);
            if (user == null)
            {
                return Unauthorized();
            }
            var token = _authService.GenerateJwtToken(user);
            return Ok(new { token });
        }

        [HttpPost("register")]
        public IActionResult Register()
        { 
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserModel registerModel)
        {
            var result = _authService.RegisterUser(registerModel);
            if (!result)
            {
                return BadRequest("Username already exists.");
            }
            return Ok("User registered successfully.");
        }

        

    }
}
