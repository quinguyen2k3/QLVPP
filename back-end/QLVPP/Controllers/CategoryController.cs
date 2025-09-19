using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
            var categories = await _service.GetAll();
            return Ok(ApiResponse<List<CategoryRes>>.SuccessResponse(
                categories,
                "Fetched categories successfully"
            ));
        }

        [HttpGet("GetAllActived")]
        public async Task<ActionResult<List<CategoryRes>>> GetAllActived()
        {
            var categories = await _service.GetAllActived();
            return Ok(ApiResponse<List<CategoryRes>>.SuccessResponse(
                 categories,
                 "Fetched categories successfully"
             ));
        }

        [HttpGet("GetById/{id:long}")]
        public async Task<ActionResult<CategoryRes>> GetById(long id)
        {
            var category = await _service.GetById(id);
            if (category == null)
                return NotFound(new { message = "Category not found" });

            return Ok(ApiResponse<CategoryRes>.SuccessResponse(
                category,
                "Fetched categroy successfully"
            ));
        }

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponse<CategoryRes>>> Create([FromBody] CategoryReq request)
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
                ApiResponse<CategoryRes>.SuccessResponse(
                    created,
                    "Created category successfully"
                )
            );
        }


        [HttpPut("Update/{id:long}")]
        public async Task<ActionResult<CategoryRes>> Update(long id, [FromBody] CategoryReq request)
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

            return Ok(ApiResponse<CategoryRes>.SuccessResponse(
                updated,
                "Updated categroy successfully"
            ));
        }
    }
}
