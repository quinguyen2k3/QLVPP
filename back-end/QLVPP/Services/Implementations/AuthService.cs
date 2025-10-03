using Microsoft.EntityFrameworkCore;
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
        private readonly ICurrentUserService _currentUserService;

        public AuthService(IUnitOfWork unitOfWork, IJwtService jwtService, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _currentUserService = currentUserService;
        }

        public async Task<AuthRes> AuthenticateAsync(AuthReq request)
        {
            var employee = await _unitOfWork.Employee.GetByAccount(request.Account);

            if (employee == null || !employee.IsActived)
            {
                return new AuthRes
                {
                    Authenticated = false,
                    IsActived = employee?.IsActived ?? false
                };
            }

            if (!PasswordHasher.VerifyPassword(request.Password, employee.Password))
            {
                return new AuthRes
                {
                    Authenticated = false,
                    IsActived = employee.IsActived
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

        public async Task ChangePasswordAsync(ChangePassReq request)
        {
            var userId = _currentUserService.UserId;

            var user = await _unitOfWork.Employee.GetById(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            user.Password = PasswordHasher.HashPassword(request.ConfirmPassword);

            await _unitOfWork.Employee.Update(user);
            await _unitOfWork.SaveChanges();
        }
    }
}
