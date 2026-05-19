namespace BIK.AuthService.Domain.Entities
{
    /// <summary>
    /// Entidad de dominio que representa los datos de credenciales y seguridad de un usuario en el microservicio.
    /// </summary>
    public class AuthUser
    {
        /// <summary>
        /// Obtiene o establece el identificador único del usuario (mapeado al ObjectId de MongoDB).
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Obtiene o establece el Documento Personal de Identificación (DPI).
        /// </summary>
        public string Dpi { get; set; } = string.Empty;

        /// <summary>
        /// Obtiene o establece la dirección de correo electrónico del usuario.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Obtiene o establece el número de teléfono móvil del usuario.
        /// </summary>
        public string Telefono { get; set; } = string.Empty;

        /// <summary>
        /// Obtiene o establece el hash de la contraseña (generado con BCrypt).
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Obtiene o establece el rol asignado al usuario (ej. Cliente, Administrador).
        /// </summary>
        public string Rol { get; set; } = "Cliente";

        /// <summary>
        /// Obtiene o establece el estado administrativo de la cuenta (ej. Activo, En Verificacion, Suspendido).
        /// </summary>
        public string Estado { get; set; } = "En Verificacion";
    }
}