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

        [HttpGet("GetAll")]
        public async Task<ActionResult<List<ReturnRes>>> GetAll()
        {
            var returnNotes = await _service.GetAll();
            return Ok(ApiResponse<List<ReturnRes>>.SuccessResponse(
                returnNotes,
                "Fetched return successfully"
             ));
        }

        [HttpGet("GetAllActivated")]
        public async Task<ActionResult<List<ReturnRes>>> GetAllActivated()
        {
            var returnNotes = await _service.GetAllActivated();
            return Ok(ApiResponse<List<ReturnRes>>.SuccessResponse(
                returnNotes,
                "Fetched return successfully"
             ));
        }

        [HttpGet("GetById/{id:long}")]
        public async Task<ActionResult<ReturnRes>> GetById(long id)
        {
            var returnNote = await _service.GetById(id);
            if (returnNote == null)
                return NotFound(new { message = "Return note not found" });

            return Ok(ApiResponse<ReturnRes>.SuccessResponse(
                returnNote,
                "Fetched return note successfully"
            ));
        }

         [HttpPost("Create")]
        public async Task<ActionResult<ApiResponse<ReturnRes>>> Create([FromBody] ReturnReq request)
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
                ApiResponse<ReturnRes>.SuccessResponse(
                    created,
                    "Created return successfully"
                )
            );
        }

        [HttpPut("Update/{id:long}")]
        public async Task<ActionResult<ReturnRes>> Update(long id, [FromBody] ReturnReq request)
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

            var updated = await _service.Update(id, request);
            if (updated == null)
                return NotFound(new { message = "Return note not found" });

            return Ok(ApiResponse<ReturnRes>.SuccessResponse(
                updated,
                "Return note update successfully"
            ));
        }

        [HttpPut("Returned/{id:long}")]
        public async Task<ActionResult<ReturnRes>> Returned(long id, [FromBody] ReturnReq request)
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

            var updated = await _service.Returned(id, request);
            if (updated == null)
                return NotFound(new { message = "Return note not found" });

            return Ok(ApiResponse<ReturnRes>.SuccessResponse(
                updated,
                "Return note complete"
            ));
        }
    }
}