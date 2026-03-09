using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLVPP.Constants.Status;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Services;

namespace QLVPP.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class MaterialRequestController : ControllerBase
    {
        private readonly IMaterialRequestService _materialRequestService;
        private readonly ICurrentUserService _currentUserService;

        public MaterialRequestController(
            IMaterialRequestService materialRequestService,
            ICurrentUserService currentUserService
        )
        {
            _currentUserService = currentUserService;
            _materialRequestService = materialRequestService;
        }

        [HttpGet("my-requests")]
        public async Task<ActionResult<List<MaterialRequestRes>>> GetMyRequest()
        {
            try
            {
                var materialRequests = await _materialRequestService.GetByConditions(
                    new MaterialRequestFilterReq
                    {
                        CreatedBy = _currentUserService.GetUserAccount(),
                        OrderByDesc = true,
                    }
                );

                return Ok(
                    ApiResponse<List<MaterialRequestRes>>.SuccessResponse(
                        materialRequests,
                        "Fetched material requests successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("department/pending")]
        public async Task<ActionResult<List<MaterialRequestRes>>> GetPendingByDepartment()
        {
            try
            {
                var materialRequests = await _materialRequestService.GetByConditions(
                    new MaterialRequestFilterReq
                    {
                        Statuses = new List<string> { MaterialRequestStatus.Pending_Department },
                        ApproverId = _currentUserService.GetUserId(),
                        OrderByDesc = true,
                    }
                );

                return Ok(
                    ApiResponse<List<MaterialRequestRes>>.SuccessResponse(
                        materialRequests,
                        "Fetched material requests successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("warehouse/pending")]
        public async Task<ActionResult<List<MaterialRequestRes>>> GetPendingByWarehouse()
        {
            try
            {
                var materialRequests = await _materialRequestService.GetByConditions(
                    new MaterialRequestFilterReq
                    {
                        Statuses = new List<string> { MaterialRequestStatus.Pending_Warehouse },
                        WarehouseId = _currentUserService.GetWarehouseId(),
                        OrderByDesc = true,
                    }
                );

                return Ok(
                    ApiResponse<List<MaterialRequestRes>>.SuccessResponse(
                        materialRequests,
                        "Fetched material requests successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<MaterialRequestRes>> GetById(long id)
        {
            try
            {
                var materialRequest = await _materialRequestService.GetById(id);
                if (materialRequest == null)
                    return NotFound(
                        ApiResponse<string>.ErrorResponse("Material request not found")
                    );

                return Ok(
                    ApiResponse<MaterialRequestRes>.SuccessResponse(
                        materialRequest,
                        "Fetched material request successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<MaterialRequestRes>>> Create(
            [FromBody] MaterialRequestReq request
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
                var created = await _materialRequestService.Create(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.Id },
                    ApiResponse<MaterialRequestRes>.SuccessResponse(
                        created,
                        "Created request successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id:long}")]
        public async Task<ActionResult<MaterialRequestRes>> Update(
            long id,
            [FromBody] MaterialRequestReq request
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
                var updated = await _materialRequestService.Update(id, request);
                if (updated == null)
                    return NotFound(
                        ApiResponse<string>.ErrorResponse("Material request not found")
                    );

                return Ok(
                    ApiResponse<MaterialRequestRes>.SuccessResponse(
                        updated,
                        "Updated material request successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("approve")]
        public async Task<ActionResult<bool>> Approve([FromBody] ApproveReq request)
        {
            try
            {
                var approved = await _materialRequestService.Approve(request);
                if (approved == false)
                    return NotFound(
                        ApiResponse<string>.ErrorResponse("Material request not found")
                    );

                return Ok(
                    ApiResponse<bool>.SuccessResponse(
                        approved,
                        "Approved material request successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("reject")]
        public async Task<ActionResult<bool>> Reject([FromBody] RejectReq request)
        {
            try
            {
                var rejected = await _materialRequestService.Reject(request);
                if (rejected == false)
                    return NotFound(
                        ApiResponse<string>.ErrorResponse("Material request not found")
                    );

                return Ok(
                    ApiResponse<bool>.SuccessResponse(
                        rejected,
                        "Rejected material request successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("delegate")]
        public async Task<ActionResult<bool>> Delegate([FromBody] DelegateReq request)
        {
            try
            {
                var delegated = await _materialRequestService.Delegate(request);
                if (delegated == false)
                    return NotFound(
                        ApiResponse<string>.ErrorResponse("Material request not found")
                    );

                return Ok(
                    ApiResponse<bool>.SuccessResponse(
                        delegated,
                        "Delegated material request successfully"
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
                var deleted = await _materialRequestService.Delete(id);
                if (deleted == false)
                    return NotFound(
                        ApiResponse<string>.ErrorResponse("Material request not found")
                    );

                return Ok(
                    ApiResponse<bool>.SuccessResponse(
                        deleted,
                        "Deleted material request successfully"
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
