using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLVPP.DTOs;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace QLVPP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;

        public AuthController(IAuthService authService, IJwtService jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AuthRes>>> Login([FromBody] AuthReq request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

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
                return StatusCode(
                    500,
                    ApiResponse<AuthRes>.ErrorResponse(
                        "Internal Server Error",
                        new[] { ex.Message }
                    )
                );
            }
        }

        [HttpPost("Refresh-Token")]
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
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutReq request)
        {
            try
            {
                await _authService.LogoutAsync(request, Request, Response);

                return Ok(new { message = "Logout successful." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("Change-Password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassReq request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _authService.ChangePasswordAsync(request);
                return Ok(new { success = true, message = "Password changed successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        success = false,
                        message = "An error occurred.",
                        error = ex.Message,
                    }
                );
            }
        }
    }
}
