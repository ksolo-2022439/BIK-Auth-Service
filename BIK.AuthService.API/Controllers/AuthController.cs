using BIK.AuthService.Application.DTOs;
using BIK.AuthService.Application.Interfaces;
using BIK.AuthService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;

namespace BIK.AuthService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repository;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _repository.GetUserByIdentifierAsync(request.Identificador);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized(new { status = "error", message = "Credenciales inválidas" });
            }

            if (user.Estado != "Activo")
            {
                return StatusCode(403, new { status = "error", message = "Cuenta suspendida o en verificación." });
            }

            var token = GenerateJwtToken(user);
            
            // Crear notificación de inicio de sesión
            await _repository.CreateLoginNotificationAsync(user.Id);

            return Ok(new { status = "success", token, rol = user.Rol });
        }

        [HttpPost("register-credentials")]
        public async Task<IActionResult> RegisterCredentials([FromBody] RegisterCredentialRequest request)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newUser = new AuthUser
            {
                Id = request.UserId,
                Dpi = request.Dpi,
                Email = request.Email,
                Telefono = request.Telefono,
                PasswordHash = hashedPassword,
                Rol = request.Rol,
                Estado = "En Verificacion"
            };

            await _repository.CreateCredentialsAsync(newUser);
            return StatusCode(201, new { status = "success", message = "Credenciales registradas exitosamente" });
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var user = await _repository.GetUserByIdentifierAsync(request.UserId);

            if (user == null)
            {
                return NotFound(new { status = "error", message = "Usuario no encontrado" });
            }

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            {
                return BadRequest(new { status = "error", message = "La contraseña actual es incorrecta" });
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _repository.UpdateUserAsync(user);

            return Ok(new { status = "success", message = "Contraseña actualizada exitosamente" });
        }

        private string GenerateJwtToken(AuthUser user)
        {
            var jwtSecret = _configuration["JwtSettings:Secret"] ?? "TuClaveSecretaSuperSeguraParaValidarTokensDeCSharp"; 
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("uid", user.Id),
                new Claim("rol", user.Rol)
            };

            var token = new JwtSecurityToken(
                issuer: "BIK-AuthService",
                audience: "BIK-Clients",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}