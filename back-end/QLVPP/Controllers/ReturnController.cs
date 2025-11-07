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
    public class ReturnController : ControllerBase
    {
        private readonly IReturnService _service;

        public ReturnController(IReturnService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<ReturnRes>>> GetReturns()
        {
            try
            {
                var returnNotes = await _service.GetByWarehouse();

                return Ok(
                    ApiResponse<List<ReturnRes>>.SuccessResponse(
                        returnNotes,
                        "Fetched return successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("my")]
        public async Task<ActionResult<RequisitionRes>> GetAllByMyself()
        {
            try
            {
                var returnNotes = await _service.GetAllByMyself();
                return Ok(
                    ApiResponse<List<ReturnRes>>.SuccessResponse(
                        returnNotes,
                        "Fetched return successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<ReturnRes>> GetById(long id)
        {
            try
            {
                var returnNote = await _service.GetById(id);
                if (returnNote == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Return note not found"));

                return Ok(
                    ApiResponse<ReturnRes>.SuccessResponse(
                        returnNote,
                        "Fetched return note successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ReturnRes>>> Create([FromBody] ReturnReq request)
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
                    ApiResponse<ReturnRes>.SuccessResponse(created, "Created return successfully")
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id:long}")]
        public async Task<ActionResult<ReturnRes>> Update(long id, [FromBody] ReturnReq request)
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
                    return NotFound(ApiResponse<string>.ErrorResponse("Return note not found"));

                return Ok(
                    ApiResponse<ReturnRes>.SuccessResponse(
                        updated,
                        "Return note update successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id:long}/confirm")]
        public async Task<ActionResult<ReturnRes>> Returned(long id, [FromBody] ReturnReq request)
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
                var updated = await _service.Returned(id, request);
                if (updated == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Return note not found"));

                return Ok(ApiResponse<ReturnRes>.SuccessResponse(updated, "Return note complete"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{id:long}")]
        public async Task<ActionResult<bool>> Delete(long id)
        {
            try
            {
                var deleted = await _service.Delete(id);

                if (deleted == false)
                    return NotFound(ApiResponse<string>.ErrorResponse("Return not found"));

                return Ok(ApiResponse<bool>.SuccessResponse(deleted, "Delete return successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }
    }
}
