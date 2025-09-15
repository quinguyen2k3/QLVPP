using QLVPP.Data;
using QLVPP.DTOs;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;
using QLVPP.Repositories;
using QLVPP.Security;

namespace QLVPP.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;

        public AuthService(IUnitOfWork unitOfWork, IJwtService jwtService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
        }

        public async Task<AuthRes> AuthenticateAsync(AuthReq request)
        {
            Employee employee = await _unitOfWork.Employee.GetByAccount(request.Account);
            if (employee == null)
            {
                return new AuthRes
                {
                    Authenticated = false
                };
            }

            if (!employee.IsActived)
            {
                return new AuthRes
                {
                    Authenticated = false,
                    IsActived = false
                };
            }

            if (!PasswordHasher.VerifyPassword(request.Password, employee.Password))
            {
                return new AuthRes 
                { 
                    Authenticated = false,
                    IsActived = false 
                };
            }
            var token = await _jwtService.GenerateAccessTokenAsync(employee);

            return new AuthRes
            {
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken,
                IsActived = true,
                Authenticated = true
            };
        }

        public async Task LogoutAsync(TokenDto request)
        {
            if (request == null)
                throw new Exception("Request is invalid");
            await _jwtService.RevokeTokenAsync(request);
        }
    }
}
