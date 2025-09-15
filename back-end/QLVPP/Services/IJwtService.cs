using QLVPP.DTOs;
using QLVPP.Models;

namespace QLVPP.Services
{
    public interface IJwtService
    {
        Task<TokenDto> GenerateAccessTokenAsync(Employee employee);
        Task<TokenDto> RenewAccessTokenAsync(TokenDto request);
        Task RevokeTokenAsync(TokenDto request);
    }
}
