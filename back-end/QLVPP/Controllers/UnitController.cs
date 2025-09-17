using Microsoft.AspNetCore.Mvc;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace QLVPP.Controllers
{
    [Route("api/[controller]")]
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
            var units = await _service.GetAll();
            return Ok(ApiResponse<List<UnitRes>>.SuccessResponse(
                units,
                "Fetched units successfully"
            ));
        }

        [HttpGet("GetAllActived")]
        public async Task<ActionResult<List<UnitRes>>> GetAllActived()
        {
            var units = await _service.GetAllActived();
            return Ok(ApiResponse<List<UnitRes>>.SuccessResponse(
                 units,
                 "Fetched units successfully"
             ));
        }

        [HttpGet("GetById/{id:long}")]
        public async Task<ActionResult<UnitRes>> GetById(long id)
        {
            var unit = await _service.GetById(id);
            if (unit == null)
                return NotFound(new { message = "Unit not found" });

            return Ok(ApiResponse<UnitRes>.SuccessResponse(
                unit,
                "Fetched categroy successfully"
            ));
        }

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponse<UnitRes>>> Create([FromBody] UnitReq request)
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
                ApiResponse<UnitRes>.SuccessResponse(
                    created,
                    "Created category successfully"
                )
            );
        }


        [HttpPut("Update/{id:long}")]
        public async Task<ActionResult<UnitRes>> Update(long id, [FromBody] UnitReq request)
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
                return NotFound(new { message = "Category not found" });

            return Ok(ApiResponse<UnitRes>.SuccessResponse(
                updated,
                "Updated categroy successfully"
            ));
        }
    }
}
