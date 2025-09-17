using Microsoft.AspNetCore.Mvc;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Services;

namespace QLVPP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _service;
        public EmployeeController(IEmployeeService service)
        {
            _service = service;
        }
        [HttpGet("GetAll")]
        public async Task<ActionResult<List<EmployeeRes>>> GetAll()
        {
            var employees = await _service.GetAll();
            return Ok(ApiResponse<List<EmployeeRes>>.SuccessResponse(
                employees,
                "Fetched employees successfully"
            ));
        }

        [HttpGet("GetAllActived")]
        public async Task<ActionResult<List<EmployeeRes>>> GetAllActived()
        {
            var employees = await _service.GetAllActived();
            return Ok(ApiResponse<List<EmployeeRes>>.SuccessResponse(
                 employees,
                 "Fetched categories successfully"
             ));
        }

        [HttpGet("GetById/{id:long}")]
        public async Task<ActionResult<EmployeeRes>> GetById(long id)
        {
            var employee = await _service.GetById(id);
            if (employee == null)
                return NotFound(new { message = "Employee not found" });

            return Ok(ApiResponse<EmployeeRes>.SuccessResponse(
                employee,
                "Fetched employee successfully"
            ));
        }

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponse<EmployeeRes>>> Create([FromBody] EmployeeReq request)
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
                new { id = created.Id},
                ApiResponse<EmployeeRes>.SuccessResponse(
                    created,
                    "Created employee successfully"
                )
            );
        }


        [HttpPut("Update/{id:long}")]
        public async Task<ActionResult<EmployeeRes>> Update(long id, [FromBody] EmployeeReq request)
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
                return NotFound(new { message = "Employee not found" });

            return Ok(ApiResponse<EmployeeRes>.SuccessResponse(
                updated,
                "Updated categroy successfully"
            ));
        }
    }
}
