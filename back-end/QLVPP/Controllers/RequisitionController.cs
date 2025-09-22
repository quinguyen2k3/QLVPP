using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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

        [HttpGet("GetAll")]
        public async Task<ActionResult<List<RequisitionRes>>> GetAll()
        {
            var requisitions = await _service.GetAll();
            return Ok(ApiResponse<List<RequisitionRes>>.SuccessResponse(
                requisitions,
                "Fetched requisitions successfully"
            ));
        }

        [HttpGet("GetAllActived")]
        public async Task<ActionResult<List<RequisitionRes>>> GetAllActived()
        {
            var requisitions = await _service.GetAllActived();
            return Ok(ApiResponse<List<RequisitionRes>>.SuccessResponse(
                 requisitions,
                 "Fetched requisitions successfully"
             ));
        }

        [HttpGet("GetById/{id:long}")]
        public async Task<ActionResult<RequisitionRes>> GetById(long id)
        {
            var requisition = await _service.GetById(id);
            if (requisition == null)
                return NotFound(new { message = "Requisition not found" });

            return Ok(ApiResponse<RequisitionRes>.SuccessResponse(
                requisition,
                "Fetched requisition successfully"
            ));
        }

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponse<RequisitionRes>>> Create([FromBody] RequisitionReq request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<string>.ErrorResponse(
                    "Validation failed",
                    errors
                ));
            }

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

        [HttpPut("Update/{id:long}")]
        public async Task<ActionResult<RequisitionRes>> Update(long id,  [FromQuery] string status)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<string>.ErrorResponse(
                    "Validation failed",
                    errors
                ));
            }

            var updated = await _service.Update(id, status);
            if (updated == null)
                return NotFound(new { message = "Category not found" });

            return Ok(ApiResponse<RequisitionRes>.SuccessResponse(
                updated,
                "Updated categroy successfully"
            ));
        }
    }
}
