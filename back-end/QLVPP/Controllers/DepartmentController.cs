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
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _service;

        public DepartmentController(IDepartmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<DepartmentRes>>> GetDepartments(
            [FromQuery] bool? activated
        )
        {
            try
            {
                if (activated.HasValue && !activated.Value)
                {
                    return BadRequest(
                        ApiResponse<string>.ErrorResponse(
                            "Invalid query: activated cannot be false."
                        )
                    );
                }

                var departments =
                    activated == true ? await _service.GetAllActivated() : await _service.GetAll();

                return Ok(
                    ApiResponse<List<DepartmentRes>>.SuccessResponse(
                        departments,
                        "Fetched departments successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<DepartmentRes>> GetById(long id)
        {
            try
            {
                var department = await _service.GetById(id);
                if (department == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Department not found"));

                return Ok(
                    ApiResponse<DepartmentRes>.SuccessResponse(
                        department,
                        "Fetched department successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<DepartmentRes>>> Create(
            [FromBody] DepartmentReq request
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
                    ApiResponse<DepartmentRes>.SuccessResponse(
                        created,
                        "Created department successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id:long}")]
        public async Task<ActionResult<DepartmentRes>> Update(
            long id,
            [FromBody] DepartmentReq request
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
                var updated = await _service.Update(id, request);
                if (updated == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Department not found"));

                return Ok(
                    ApiResponse<DepartmentRes>.SuccessResponse(
                        updated,
                        "Updated department successfully"
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
