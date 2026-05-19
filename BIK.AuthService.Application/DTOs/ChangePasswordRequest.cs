namespace BIK.AuthService.Application.DTOs
{
    /// <summary>
    /// Objeto de Transferencia de Datos (DTO) que representa la solicitud para realizar un cambio de contraseña.
    /// </summary>
    public class ChangePasswordRequest
    {
        /// <summary>
        /// Obtiene o establece el identificador único del usuario que desea cambiar su contraseña.
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Obtiene o establece la contraseña actual del usuario para su validación previa.
        /// </summary>
        public string CurrentPassword { get; set; } = string.Empty;

        /// <summary>
        /// Obtiene o establece la nueva contraseña propuesta por el usuario.
        /// </summary>
        public string NewPassword { get; set; } = string.Empty;
    }
}
