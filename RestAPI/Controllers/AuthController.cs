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

        public class RefreshTokenRequest
        {
            public string Token { get; set; } = string.Empty;       
            public string RefreshToken { get; set; } = string.Empty;  
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        public IActionResult Login(UserModel login)
        {
            var user = _authService.ValidateUser(login);
            if (user == null)
            {
                return Unauthorized();
            }
            var token = _authService.GenerateTokens(user);

            _authService.SaveRefreshToken(user, token.RefreshToken);
            return Ok(new { token });
        }

        [HttpGet("register")]
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

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var principal = _authService.GetPrincipalFromExpiredToken(request.Token);
            if (principal == null)
            {
                return BadRequest("Invalid token.");
            }

            string username = principal.Identity?.Name ?? "";
            var user = _authService.GetUserByUsername(username);
            if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return BadRequest("Invalid refresh token.");
            }

            var newTokens = _authService.GenerateTokens(user);
            _authService.SaveRefreshToken(user, newTokens.RefreshToken);

            return Ok(newTokens);
        }

        [HttpPost("revoke-refresh-token")]
        public IActionResult RevokeRefreshToken([FromBody] RefreshTokenRequest request)
        {
            var user = _authService.GetUserByRefreshToken(request.RefreshToken);
            if (user == null)
            {
                return NotFound("Refresh token not found.");
            }

            _authService.RevokeRefreshToken(user.Username);
            return Ok("Refresh token revoked.");
        }

    }
}
