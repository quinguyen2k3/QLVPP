using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Services;

namespace QLVPP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IJwtService _jwtService;
        private readonly IOnlineUserService _onlineUserService;

        public AuthController(
            IAuthService authService,
            IJwtService jwtService,
            IOnlineUserService onlineUserService,
            ICurrentUserService currentUserService
        )
        {
            _authService = authService;
            _jwtService = jwtService;
            _currentUserService = currentUserService;
            _onlineUserService = onlineUserService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AuthRes>>> Login([FromBody] AuthReq request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(
                    ApiResponse<AuthRes>.ErrorResponse("Invalid request data", errors)
                );
            }

            try
            {
                var result = await _authService.AuthenticateAsync(request, Response);

                if (!result.Authenticated)
                {
                    if (result.IsActivated == false)
                    {
                        return Unauthorized(ApiResponse<AuthRes>.ErrorResponse("NotActive"));
                    }
                    return Unauthorized(ApiResponse<AuthRes>.ErrorResponse("Unauthenticated"));
                }
                return Ok(ApiResponse<AuthRes>.SuccessResponse(result, "Login successful"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<AuthRes>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                var newAccessToken = await _jwtService.RenewAccessTokenAsync(Request, Response);
                return Ok(ApiResponse<string>.SuccessResponse(newAccessToken, "Token refreshed"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<string>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutReq request)
        {
            try
            {
                var userId = _currentUserService.GetUserId();

                await _authService.LogoutAsync(request, Request, Response);
                return Ok(ApiResponse<string>.SuccessResponse(string.Empty, "Logout successful."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassReq request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<string>.ErrorResponse("Validation failed", errors));
            }

            try
            {
                await _authService.ChangePasswordAsync(request);
                return Ok(
                    ApiResponse<string>.SuccessResponse(
                        string.Empty,
                        "Password changed successfully."
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("online-users")]
        public async Task<IActionResult> GetOnlineCount()
        {
            var count = await _onlineUserService.GetOnlineUserCount();
            return Ok(ApiResponse<int>.SuccessResponse(count, "Fetch online user successfully"));
        }

        [HttpPost("heartbeat")]
        public async Task<IActionResult> SendHeartbeat()
        {
            var userId = _currentUserService.GetUserId();

            await _onlineUserService.AddUser(userId);

            return Ok(ApiResponse<object>.SuccessResponse("Heartbeat received"));
        }
    }
}
