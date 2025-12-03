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

        [HttpPost("{requisitionId:long}/approve")]
        public async Task<IActionResult> Approve(
            [FromRoute] long requisitionId,
            [FromBody] ApproveReq request
        )
        {
            if (requisitionId != request.RequisitionId)
                return BadRequest(
                    ApiResponse<string>.ErrorResponse(
                        "RequisitionId in route and body do not match"
                    )
                );

            try
            {
                await _service.Approve(request);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("{requisitionId:long}/delegate")]
        public async Task<IActionResult> Delegate(
            [FromRoute] long requisitionId,
            [FromBody] DelegateReq request
        )
        {
            if (requisitionId != request.RequisitionId)
                return BadRequest(
                    ApiResponse<string>.ErrorResponse(
                        "RequisitionId in route and body do not match"
                    )
                );

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
                await _service.Delegate(request);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("{requisitionId:long}/reject")]
        public async Task<IActionResult> Reject(
            [FromRoute] long requisitionId,
            [FromBody] RejectReq request
        )
        {
            if (requisitionId != request.RequisitionId)
            {
                return BadRequest(
                    ApiResponse<string>.ErrorResponse(
                        "RequisitionId in route and body do not match"
                    )
                );
            }

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
                await _service.Reject(request);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }
    }
}
