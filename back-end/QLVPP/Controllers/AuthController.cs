using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLVPP.DTOs;
using QLVPP.DTOs.Response;
using QLVPP.DTOs.Request;
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
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                return BadRequest(ApiResponse<AuthRes>.ErrorResponse("Invalid request data", errors));
            }

            try
            {
                var result = await _authService.AuthenticateAsync(request);

                if (!result.Authenticated)
                {
                    if (result.IsActived == false)
                    {
                        return Unauthorized(ApiResponse<AuthRes>.ErrorResponse(
                            "NotActive"
                        ));
                    }
                    return Unauthorized(ApiResponse<AuthRes>.ErrorResponse(
                        "Unauthenticated"
                    ));
                }

                return Ok(ApiResponse<AuthRes>.SuccessResponse(result, "Login successful"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<AuthRes>.ErrorResponse("Internal Server Error", new[] { ex.Message }));
            }
        }

        [HttpPost("Refresh-Token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] TokenDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.AccessToken) || string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest(new { message = "AccessToken and RefreshToken are required." });
            }

            try
            {
                var tokenDto = await _jwtService.RenewAccessTokenAsync(request);
                return Ok(tokenDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] TokenDto request)
        {
            try
            {
                await _authService.LogoutAsync(request);

                return Ok(new { message = "Logout successful." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
