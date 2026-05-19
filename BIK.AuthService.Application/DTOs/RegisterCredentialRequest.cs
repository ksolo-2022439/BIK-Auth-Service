namespace BIK.AuthService.Application.DTOs
{
    /// <summary>
    /// Objeto de Transferencia de Datos (DTO) que representa la solicitud para registrar credenciales de acceso iniciales.
    /// </summary>
    public class RegisterCredentialRequest
    {
        /// <summary>
        /// Obtiene o establece el identificador único del usuario en el sistema.
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Obtiene o establece el Documento Personal de Identificación (DPI) del usuario.
        /// </summary>
        public string Dpi { get; set; } = string.Empty;

        /// <summary>
        /// Obtiene o establece la dirección de correo electrónico vinculada a la cuenta.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Obtiene o establece el número de teléfono móvil para contacto y autenticación.
        /// </summary>
        public string Telefono { get; set; } = string.Empty;

        /// <summary>
        /// Obtiene o establece la contraseña en texto plano que será hasheada e inicializada.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Obtiene o establece el rol que tendrá el usuario en la plataforma (por defecto "Cliente").
        /// </summary>
        public string Rol { get; set; } = "Cliente";
    }
}