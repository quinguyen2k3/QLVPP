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
            try
            {
                var products = await _service.GetAll();
                return Ok(
                    ApiResponse<List<ProductRes>>.SuccessResponse(
                        products,
                        "Fetched products successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("GetAllActivated")]
        public async Task<ActionResult<List<ProductRes>>> GetAllActivated()
        {
            try
            {
                var products = await _service.GetAllActivated();
                return Ok(
                    ApiResponse<List<ProductRes>>.SuccessResponse(
                        products,
                        "Fetched products successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("GetByWareHouse")]
        public async Task<ActionResult<List<ProductRes>>> GetAllByWarehouse()
        {
            try
            {
                var products = await _service.GetByWarehouse();
                return Ok(
                    ApiResponse<List<ProductRes>>.SuccessResponse(
                        products,
                        "Fetched products successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("GetById/{id:long}")]
        public async Task<ActionResult<ProductRes>> GetById(long id)
        {
            try
            {
                var product = await _service.GetById(id);
                if (product == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Product not found"));

                return Ok(
                    ApiResponse<ProductRes>.SuccessResponse(product, "Fetched product successfully")
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
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

            try
            {
                var created = await _service.Create(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.Id },
                    ApiResponse<ProductRes>.SuccessResponse(created, "Created product successfully")
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("Update/{id:long}")]
        public async Task<ActionResult<ProductRes>> Update(long id, [FromBody] ProductReq request)
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
                    return NotFound(ApiResponse<string>.ErrorResponse("Product not found"));

                return Ok(
                    ApiResponse<ProductRes>.SuccessResponse(updated, "Updated product successfully")
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }
    }
}
