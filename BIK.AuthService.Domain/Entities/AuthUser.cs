namespace BIK.AuthService.Domain.Entities
{
    public class AuthUser
    {
        public string Id { get; set; } = string.Empty;
        public string Dpi { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Rol { get; set; } = "Cliente";
        public string Estado { get; set; } = "En Verificacion";
    }
}