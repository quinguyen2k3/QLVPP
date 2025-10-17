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
            try
            {
                var employees = await _service.GetAll();
                return Ok(
                    ApiResponse<List<EmployeeRes>>.SuccessResponse(
                        employees,
                        "Fetched employees successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("GetAllActivated")]
        public async Task<ActionResult<List<EmployeeRes>>> GetAllActivated()
        {
            try
            {
                var employees = await _service.GetAllActivated();
                return Ok(
                    ApiResponse<List<EmployeeRes>>.SuccessResponse(
                        employees,
                        "Fetched employees successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("GetById/{id:long}")]
        public async Task<ActionResult<EmployeeRes>> GetById(long id)
        {
            try
            {
                var employee = await _service.GetById(id);
                if (employee == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Employee not found"));

                return Ok(
                    ApiResponse<EmployeeRes>.SuccessResponse(
                        employee,
                        "Fetched employee successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponse<EmployeeRes>>> Create(
            [FromBody] EmployeeReq request
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
                    ApiResponse<EmployeeRes>.SuccessResponse(
                        created,
                        "Created employee successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("Update/{id:long}")]
        public async Task<ActionResult<EmployeeRes>> Update(long id, [FromBody] EmployeeReq request)
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
                    return NotFound(ApiResponse<string>.ErrorResponse("Employee not found"));

                return Ok(
                    ApiResponse<EmployeeRes>.SuccessResponse(
                        updated,
                        "Updated employee successfully"
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
