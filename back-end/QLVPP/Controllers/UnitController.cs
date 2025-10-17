using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Services;

namespace QLVPP.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UnitController : ControllerBase
    {
        private readonly IUnitService _service;

        public UnitController(IUnitService service)
        {
            _service = service;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<List<UnitRes>>> GetAll()
        {
            try
            {
                var units = await _service.GetAll();
                return Ok(
                    ApiResponse<List<UnitRes>>.SuccessResponse(units, "Fetched units successfully")
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("GetAllActivated")]
        public async Task<ActionResult<List<UnitRes>>> GetAllActivated()
        {
            try
            {
                var units = await _service.GetAllActivated();
                return Ok(
                    ApiResponse<List<UnitRes>>.SuccessResponse(units, "Fetched units successfully")
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("GetById/{id:long}")]
        public async Task<ActionResult<UnitRes>> GetById(long id)
        {
            try
            {
                var unit = await _service.GetById(id);
                if (unit == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Unit not found"));

                return Ok(ApiResponse<UnitRes>.SuccessResponse(unit, "Fetched unit successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponse<UnitRes>>> Create([FromBody] UnitReq request)
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
                var created = await _service.Create(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.Id },
                    ApiResponse<UnitRes>.SuccessResponse(created, "Created unit successfully")
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("Update/{id:long}")]
        public async Task<ActionResult<UnitRes>> Update(long id, [FromBody] UnitReq request)
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
                var updated = await _service.Update(id, request);
                if (updated == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Unit not found"));

                return Ok(
                    ApiResponse<UnitRes>.SuccessResponse(updated, "Updated unit successfully")
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }
    }
}
