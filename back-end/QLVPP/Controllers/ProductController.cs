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
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<List<ProductRes>>> GetAll()
        {
            var products = await _service.GetAll();
            return Ok(
                ApiResponse<List<ProductRes>>.SuccessResponse(
                    products,
                    "Fetched products successfully"
                )
            );
        }

        [HttpGet("GetAllActivated")]
        public async Task<ActionResult<List<ProductRes>>> GetAllActivated()
        {
            var products = await _service.GetAllActivated();
            return Ok(
                ApiResponse<List<ProductRes>>.SuccessResponse(
                    products,
                    "Fetched products successfully"
                )
            );
        }

        [HttpGet("GetByWareHouse")]
        public async Task<ActionResult<List<ProductRes>>> GetAllByWarehouse()
        {
            var products = await _service.GetByWarehouse();
            return Ok(
                ApiResponse<List<ProductRes>>.SuccessResponse(
                    products,
                    "Fetched products successfully"
                )
            );
        }

        [HttpGet("GetById/{id:long}")]
        public async Task<ActionResult<ProductRes>> GetById(long id)
        {
            var product = await _service.GetById(id);
            if (product == null)
                return NotFound(new { message = "Product not found" });

            return Ok(
                ApiResponse<ProductRes>.SuccessResponse(product, "Fetched product successfully")
            );
        }

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponse<ProductRes>>> Create(
            [FromBody] ProductReq request
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

            var created = await _service.Create(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                ApiResponse<ProductRes>.SuccessResponse(created, "Created product successfully")
            );
        }

        [HttpPut("Update/{id:long}")]
        public async Task<ActionResult<DepartmentRes>> Update(
            long id,
            [FromBody] ProductReq request
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

            var updated = await _service.Update(id, request);
            if (updated == null)
                return NotFound(new { message = "Product not found" });

            return Ok(
                ApiResponse<ProductRes>.SuccessResponse(updated, "Updated product successfully")
            );
        }
    }
}
