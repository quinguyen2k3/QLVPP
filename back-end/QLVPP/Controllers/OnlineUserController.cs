using Microsoft.AspNetCore.Mvc;
using QLVPP.DTOs.Response;
using QLVPP.Services;

namespace QLVPP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnlineUserController : ControllerBase
    {
        private readonly IOnlineUserService _onlineUserService;
        private readonly ICurrentUserService _currentUserService;

        public OnlineUserController(
            IOnlineUserService onlineUserService,
            ICurrentUserService currentUserService
        )
        {
            _onlineUserService = onlineUserService;
            _currentUserService = currentUserService;
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
