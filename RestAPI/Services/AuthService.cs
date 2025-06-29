using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RestAPI.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RestAPI.Services
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;
        private readonly FlightsDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthService(IConfiguration configuration, FlightsDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _configuration = configuration;
            _context = context;
            _passwordHasher = passwordHasher;
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

        public class AuthResponse
        {
            public string AccessToken { get; set; } = string.Empty;
            public string RefreshToken { get; set; } = string.Empty;
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

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber);
        }

        public AuthResponse GenerateTokens(User user)
        {
            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7); // Valid for 7 days

            _context.Users.Update(user);
            _context.SaveChanges();

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
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
            _context.SaveChanges();
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

        public User? ValidateRefreshToken(string refreshToken)
        {
            var user = _context.Users.SingleOrDefault(u => u.RefreshToken == refreshToken);

            if (user == null) return null;

            if (user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return null;
            }

            return user;
        }

        public bool RevokeRefreshToken(string username)
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == username);
            if (user == null || string.IsNullOrEmpty(user.RefreshToken))
                return false;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            _context.Users.Update(user);
            _context.SaveChanges();
            return true;
        }

        public void SaveRefreshToken(User user, string refreshToken)
        {
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public User? GetUserByUsername(string username)
        {
            return _context.Users.SingleOrDefault(u => u.Username == username);
        }

        public User? GetUserByRefreshToken(string refreshToken)
        {
            return _context.Users.SingleOrDefault(u => u.RefreshToken == refreshToken);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
