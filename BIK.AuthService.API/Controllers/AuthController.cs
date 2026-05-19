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

namespace BIK.AuthService.API.Controllers
{
    /// <summary>
    /// Controlador principal para la gestión de autenticación, registro de credenciales y cambio de contraseñas.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repository;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Inicializa una nueva instancia del controlador de autenticación con sus dependencias necesarias.
        /// </summary>
        /// <param name="repository">Repositorio de acceso a datos de credenciales.</param>
        /// <param name="configuration">Proveedor de configuraciones globales de la aplicación.</param>
        public AuthController(IAuthRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        /// <summary>
        /// Procesa la solicitud de inicio de sesión, valida contraseñas con hash de BCrypt y genera un token JWT.
        /// </summary>
        /// <param name="request">Payload con el identificador del usuario y contraseña.</param>
        /// <returns>Objeto conteniendo el estado del login, token JWT y rol asociado.</returns>
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
            
            await _repository.CreateLoginNotificationAsync(user.Id);

            return Ok(new { status = "success", token, rol = user.Rol });
        }

        /// <summary>
        /// Registra las credenciales iniciales de acceso para un usuario verificado.
        /// </summary>
        /// <param name="request">Payload conteniendo datos de identificación, contacto y contraseña en plano a hashear.</param>
        /// <returns>Respuesta HTTP indicando éxito o fracaso en el registro de credenciales.</returns>
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

        /// <summary>
        /// Permite actualizar la contraseña de un usuario validando la clave actual previamente.
        /// </summary>
        /// <param name="request">Payload conteniendo ID de usuario, contraseña actual y nueva contraseña.</param>
        /// <returns>Resultado de la actualización de contraseña.</returns>
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

        /// <summary>
        /// Genera de forma segura un token JSON Web Token (JWT) firmado con algoritmo HS256 para la sesión activa.
        /// </summary>
        /// <param name="user">Entidad del usuario de autenticación conteniendo ID y Rol.</param>
        /// <returns>Token JWT serializado en formato string.</returns>
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