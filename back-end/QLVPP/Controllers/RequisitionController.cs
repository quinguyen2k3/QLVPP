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
    public class RequisitionController : ControllerBase
    {
        private readonly IRequisitionService _service;

        public RequisitionController(IRequisitionService service)
        {
            _service = service;
        }

        [HttpGet("pending-approval")]
        public async Task<ActionResult<List<RequisitionRes>>> GetRequisitionsPendingApproval()
        {
            try
            {
                var requisitions = await _service.GetPendingForMyApproval();

                return Ok(
                    ApiResponse<List<RequisitionRes>>.SuccessResponse(
                        requisitions,
                        "Fetched requisitions successfully"
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
                var requisitions = await _service.GetAllByMyself();
                return Ok(
                    ApiResponse<List<RequisitionRes>>.SuccessResponse(
                        requisitions,
                        "Fetched requisitions successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<RequisitionRes>> GetById(long id)
        {
            try
            {
                var requisition = await _service.GetById(id);
                if (requisition == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Requisition not found"));

                return Ok(
                    ApiResponse<RequisitionRes>.SuccessResponse(
                        requisition,
                        "Fetched requisition successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<RequisitionRes>>> Create(
            [FromBody] RequisitionReq request
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
                var created = await _service.Create(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.Id },
                    ApiResponse<RequisitionRes>.SuccessResponse(
                        created,
                        "Created requisition successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id:long}")]
        public async Task<ActionResult<RequisitionRes>> Update(long id, [FromQuery] string status)
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
                var updated = await _service.Update(id, status);
                if (updated == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Requisition not found"));

                return Ok(
                    ApiResponse<RequisitionRes>.SuccessResponse(
                        updated,
                        "Updated requisition successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("forward/{id:long}")]
        public async Task<ActionResult<RequisitionRes>> Forward(
            long id,
            [FromBody] ForwardReq request
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
                var updated = await _service.Forward(id, request);
                if (updated == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Requisition not found"));

                return Ok(
                    ApiResponse<RequisitionRes>.SuccessResponse(
                        updated,
                        "Forward requisition successfully"
                    )
                );
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
