using QLVPP.DTOs;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;

namespace QLVPP.Services
{
    public interface IAuthService
    {
        Task<AuthRes> AuthenticateAsync(AuthReq request);
        Task LogoutAsync(TokenDto request);
        Task ChangePasswordAsync(ChangePassReq request);
    }
}
