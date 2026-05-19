namespace BIK.AuthService.Application.DTOs
{
    /// <summary>
    /// Objeto de Transferencia de Datos (DTO) que representa la solicitud de inicio de sesión de un usuario.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Obtiene o establece el identificador del usuario (puede ser ID, DPI, Email o Teléfono).
        /// </summary>
        public string Identificador { get; set; } = string.Empty;

        /// <summary>
        /// Obtiene o establece la contraseña provista por el usuario para su verificación.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}