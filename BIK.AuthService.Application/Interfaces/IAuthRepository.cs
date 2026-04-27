using BIK.AuthService.Domain.Entities;
using System.Threading.Tasks;

namespace BIK.AuthService.Application.Interfaces
{
    public interface IAuthRepository
    {
        Task<AuthUser?> GetUserByIdentifierAsync(string identifier);
        Task CreateCredentialsAsync(AuthUser user);
    }
}