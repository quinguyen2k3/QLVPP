using QLVPP.DTOs;
using QLVPP.Models;

namespace QLVPP.Services
{
    public interface IJwtService
    {
        Task<string> GenerateAccessTokenAsync(Employee employee, HttpResponse response);
        Task<string> RenewAccessTokenAsync(HttpRequest request, HttpResponse response);
        Task RevokeTokenAsync(string accessToken, HttpRequest request, HttpResponse response);
    }
}
