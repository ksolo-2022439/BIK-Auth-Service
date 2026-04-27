namespace BIK.AuthService.Application.DTOs
{
    public class LoginRequest
    {
        public string Identificador { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}