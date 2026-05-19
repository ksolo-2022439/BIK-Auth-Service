using BIK.AuthService.Domain.Entities;
using System.Threading.Tasks;

namespace BIK.AuthService.Application.Interfaces
{
    /// <summary>
    /// Interfaz que define los contratos de persistencia para la gestión de credenciales y auditoría de seguridad.
    /// </summary>
    public interface IAuthRepository
    {
        /// <summary>
        /// Busca de forma asíncrona a un usuario mediante sus datos de identificación únicos (DPI, Email, Teléfono o ID).
        /// </summary>
        /// <param name="identifier">Identificador único del usuario.</param>
        /// <returns>La entidad AuthUser si se encuentra; de lo contrario, null.</returns>
        Task<AuthUser?> GetUserByIdentifierAsync(string identifier);

        /// <summary>
        /// Persiste las credenciales y el hash de contraseña asignado a un usuario en base de datos.
        /// </summary>
        /// <param name="user">Entidad de usuario que contiene la información de credenciales.</param>
        Task CreateCredentialsAsync(AuthUser user);

        /// <summary>
        /// Registra una alerta de auditoría por inicio de sesión exitoso.
        /// </summary>
        /// <param name="userId">Identificador único del usuario que inicia sesión.</param>
        Task CreateLoginNotificationAsync(string userId);

        /// <summary>
        /// Actualiza de forma asíncrona los datos de perfil y seguridad del usuario.
        /// </summary>
        /// <param name="user">Entidad de usuario con la información actualizada.</param>
        Task UpdateUserAsync(AuthUser user);
    }
}