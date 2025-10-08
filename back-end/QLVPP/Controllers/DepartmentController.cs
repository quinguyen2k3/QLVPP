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
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _service;

        public DepartmentController(IDepartmentService service)
        {
            _service = service;
        }
        [HttpGet("GetAll")]
        public async Task<ActionResult<List<DepartmentRes>>> GetAll()
        {
            var departments = await _service.GetAll();
            return Ok(ApiResponse<List<DepartmentRes>>.SuccessResponse(
                departments,
                "Fetched departments successfully"
            ));
        }

        [HttpGet("GetAllActivated")]
        public async Task<ActionResult<List<DepartmentRes>>> GetAllActivated()
        {
            var departments = await _service.GetAllActivated();
            return Ok(ApiResponse<List<DepartmentRes>>.SuccessResponse(
                 departments,
                 "Fetched departments successfully"
             ));
        }

        [HttpGet("GetById/{id:long}")]
        public async Task<ActionResult<DepartmentRes>> GetById(long id)
        {
            var department = await _service.GetById(id);
            if (department == null)
                return NotFound(new { message = "Department not found" });

            return Ok(ApiResponse<DepartmentRes>.SuccessResponse(
                department,
                "Fetched department successfully"
            ));
        }

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponse<DepartmentRes>>> Create([FromBody] DepartmentReq request)
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
                ApiResponse<DepartmentRes>.SuccessResponse(
                    created,
                    "Created employee successfully"
                )
            );
        }


        [HttpPut("Update/{id:long}")]
        public async Task<ActionResult<DepartmentRes>> Update(long id, [FromBody] DepartmentReq request)
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
                return NotFound(new { message = "Department not found" });

            return Ok(ApiResponse<DepartmentRes>.SuccessResponse(
                updated,
                "Updated categroy successfully"
            ));
        }
    }
}
