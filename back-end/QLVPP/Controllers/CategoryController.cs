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
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<List<CategoryRes>>> GetAll()
        {
            try
            {
                var categories = await _service.GetAll();
                return Ok(
                    ApiResponse<List<CategoryRes>>.SuccessResponse(
                        categories,
                        "Fetched categories successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("GetAllActivated")]
        public async Task<ActionResult<List<CategoryRes>>> GetAllActivated()
        {
            try
            {
                var categories = await _service.GetAllActivated();
                return Ok(
                    ApiResponse<List<CategoryRes>>.SuccessResponse(
                        categories,
                        "Fetched categories successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("GetById/{id:long}")]
        public async Task<ActionResult<CategoryRes>> GetById(long id)
        {
            try
            {
                var category = await _service.GetById(id);
                if (category == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Category not found"));

                return Ok(
                    ApiResponse<CategoryRes>.SuccessResponse(
                        category,
                        "Fetched category successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponse<CategoryRes>>> Create(
            [FromBody] CategoryReq request
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
                    ApiResponse<CategoryRes>.SuccessResponse(
                        created,
                        "Created category successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("Update/{id:long}")]
        public async Task<ActionResult<CategoryRes>> Update(long id, [FromBody] CategoryReq request)
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
                    return NotFound(ApiResponse<string>.ErrorResponse("Category not found"));

                return Ok(
                    ApiResponse<CategoryRes>.SuccessResponse(
                        updated,
                        "Updated category successfully"
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
