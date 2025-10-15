using QLVPP.DTOs;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;

namespace QLVPP.Services
{
    public interface IAuthService
    {
        Task<AuthRes> AuthenticateAsync(AuthReq request, HttpResponse httpResponse);
        Task LogoutAsync(LogoutReq request, HttpRequest httpRequest, HttpResponse httpResponse);
        Task ChangePasswordAsync(ChangePassReq request);
    }
}
