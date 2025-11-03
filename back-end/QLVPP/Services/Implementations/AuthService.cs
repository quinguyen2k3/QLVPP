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
        private readonly IOnlineUserService _onlineUserService;

        public AuthService(
            IUnitOfWork unitOfWork,
            IJwtService jwtService,
            ICurrentUserService currentUserService,
            IOnlineUserService onlineUserService
        )
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _currentUserService = currentUserService;
            _onlineUserService = onlineUserService;
        }

        public async Task<AuthRes> AuthenticateAsync(AuthReq request, HttpResponse httpResponse)
        {
            var employee = await _unitOfWork.Employee.GetByAccount(request.Account);

            if (employee == null || !employee.IsActivated)
            {
                return new AuthRes
                {
                    Authenticated = false,
                    IsActivated = employee?.IsActivated ?? false,
                };
            }

            if (!PasswordHasher.VerifyPassword(request.Password, employee.Password))
            {
                return new AuthRes { Authenticated = false, IsActivated = employee.IsActivated };
            }

            var token = await _jwtService.GenerateAccessTokenAsync(employee, httpResponse);

            await _onlineUserService.AddUser(employee.Id.ToString());

            return new AuthRes
            {
                AccessToken = token,
                IsActivated = true,
                Authenticated = true,
            };
        }

        public async Task LogoutAsync(
            LogoutReq request,
            HttpRequest httpRequest,
            HttpResponse httpResponse
        )
        {
            if (request == null)
                throw new Exception("Request is invalid");
            await _onlineUserService.AddUser(_currentUserService.GetUserId().ToString());
            await _jwtService.RevokeTokenAsync(request.AccessToken, httpRequest, httpResponse);
        }

        public async Task ChangePasswordAsync(ChangePassReq request)
        {
            var userId = _currentUserService.GetUserId();

            var user = await _unitOfWork.Employee.GetById(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            user.Password = PasswordHasher.HashPassword(request.ConfirmPassword);

            await _unitOfWork.Employee.Update(user);
            await _unitOfWork.SaveChanges();
        }
    }
}
