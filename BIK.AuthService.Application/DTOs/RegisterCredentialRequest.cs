namespace BIK.AuthService.Application.DTOs
{
    public class RegisterCredentialRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string Dpi { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Rol { get; set; } = "Cliente";
    }
}