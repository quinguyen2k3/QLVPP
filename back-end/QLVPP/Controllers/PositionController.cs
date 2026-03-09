using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLVPP.Attributes;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Services;

namespace QLVPP.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PositionController : ControllerBase
    {
        private readonly IPositionService _positionService;

        public PositionController(IPositionService positionService)
        {
            _positionService = positionService;
        }

        [HttpGet]
        [AllowCommonAccess]
        public async Task<ActionResult<ApiResponse<List<PositionRes>>>> GetPosition()
        {
            try
            {
                var position = await _positionService.GetAll();
                return Ok(
                    ApiResponse<List<PositionRes>>.SuccessResponse(
                        position,
                        "Fetched positions successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<ApiResponse<PositionRes>>> GetById(long id)
        {
            try
            {
                var position = await _positionService.GetById(id);
                return Ok(
                    ApiResponse<PositionRes>.SuccessResponse(
                        position,
                        "Fetched position successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<PositionRes>>> Create(PositionReq request)
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
                var createdPosition = await _positionService.Create(request);
                return Ok(
                    ApiResponse<PositionRes>.SuccessResponse(
                        createdPosition,
                        "Created position successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id:long}")]
        public async Task<ActionResult<ApiResponse<PositionRes>>> Update(
            long id,
            PositionReq request
        )
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
                var created = await _positionService.Update(id, request);
                return Ok(
                    ApiResponse<PositionRes>.SuccessResponse(
                        created,
                        "Updated position successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }
    }
}
