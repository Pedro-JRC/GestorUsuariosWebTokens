using System.Security.Claims;
using System.Text;
using GestorUsuariosWebTokens.Helpers;
using GestorUsuariosWebTokens.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;
using GestorUsuariosWebTokens.Data;
using Microsoft.Extensions.Logging;

namespace GestorUsuariosWebTokens.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AppDbContext context, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        // POST: api/auth/login
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var hashedPassword = SecurityHelper.ComputeSha256Hash(loginRequest.Password);

            var user = _context.Usuarios.FirstOrDefault(u =>
                u.NombreUsuario == loginRequest.NombreUsuario && u.PasswordHash == hashedPassword);

            if (user == null)
                return Unauthorized(new { message = "Credenciales inválidas" });

            var tokenResponse = GenerateJwtToken(user);
            return Ok(tokenResponse);
        }

        // POST: api/auth/refresh
        [Authorize]
        [HttpPost("refresh")]
        public IActionResult Refresh()
        {
            // Extrae el claim "id" que contiene el ID del usuario
            var idClaim = User.FindFirst("id");
            var userIdString = idClaim?.Value?.Trim();

            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized(new { message = "Token inválido" });

            var user = _context.Usuarios.FirstOrDefault(u => u.Id.ToString() == userIdString);
            if (user == null)
                return Unauthorized(new { message = "Usuario no encontrado" });

            var tokenResponse = GenerateJwtToken(user);
            return Ok(tokenResponse);
        }



        private AuthResponse GenerateJwtToken(Usuario user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings.GetValue<string>("SecretKey");
            var issuer = jwtSettings.GetValue<string>("Issuer");
            var audience = jwtSettings.GetValue<string>("Audience");
            var expiryInMinutes = jwtSettings.GetValue<int>("ExpiryInMinutes");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                // "sub" contendrá el nombre de usuario
                new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub, user.NombreUsuario),
                new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                // Agrega un claim personalizado "id" para el ID del usuario
                new Claim("id", user.Id.ToString())
            };

            var expiration = DateTime.UtcNow.AddMinutes(expiryInMinutes);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Subject = new ClaimsIdentity(claims),
                Expires = expiration,
                SigningCredentials = creds
            };

            var tokenHandler = new Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler();
            var tokenString = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthResponse
            {
                Token = tokenString,
                Expiration = expiration
            };
        }


    }
}
