using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLVPP.Attributes;
using QLVPP.DTOs.Response;
using QLVPP.Services;

namespace QLVPP.Controlers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _service;

        public RoleController(IRoleService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowCommonAccess]
        public async Task<ActionResult<List<RoleRes>>> GetRoles()
        {
            try
            {
                var products = await _service.GetAll();
                return Ok(ApiResponse<List<RoleRes>>.SuccessResponse(products));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }
    }
}
