using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RestAPI.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RestAPI.Services
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;
        private readonly FlightsDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(IConfiguration configuration, FlightsDbContext context, PasswordHasher<User> passwordHasher)
        {
            _configuration = configuration;
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(
                               Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                               issuer: _configuration["Jwt:Issuer"],
                                              audience: _configuration["Jwt:Issuer"],
                                                             claims: claims,
                                                                            expires: DateTime.Now.AddMinutes(30),
                                                                                           signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool RegisterUser(UserModel registerModel)
        {
            if (_context.Users.Any(u => u.Username == registerModel.Username))
            {
                return false;
            }

            var user = new User
            { 
                Username = registerModel.Username
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, registerModel.Password);
            _context.Users.Add(user);
            _context.SaveChangesAsync();
            return true;
        }

        public User? ValidateUser(UserModel loginModel)
        {
            var user = _context.Users
                .SingleOrDefault(u => u.Username == loginModel.Username);

            if (user != null)
            {
                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginModel.Password);
                if (result == PasswordVerificationResult.Success)
                {
                    return user;
                }
            }
            return null;
        }

        public class UserModel
        {
            [Required(ErrorMessage = "Username is required.")]
            [StringLength(50, ErrorMessage = "Username cannot be longer than 50 characters.")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required.")]
            [StringLength(100, ErrorMessage = "Password cannot be longer than 100 characters.")]
            public string Password { get; set; } = string.Empty;
        }
    }
}
