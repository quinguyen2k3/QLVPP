using Microsoft.AspNetCore.Mvc;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace QLVPP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _service;

        public SupplierController(ISupplierService service)
        {
            _service = service;
        }
        [HttpGet("GetAll")]
        public async Task<ActionResult<List<SupplierRes>>> GetAll()
        {
            var suppliers = await _service.GetAll();
            return Ok(ApiResponse<List<SupplierRes>>.SuccessResponse(
                suppliers,
                "Fetched suppliers successfully"
            ));
        }

        [HttpGet("GetAllActived")]
        public async Task<ActionResult<List<CategoryRes>>> GetAllActived()
        {
            var suppliers = await _service.GetAllActived();
            return Ok(ApiResponse<List<SupplierRes>>.SuccessResponse(
                 suppliers,
                 "Fetched suppliers successfully"
             ));
        }

        [HttpGet("GetById/{id:long}")]
        public async Task<ActionResult<SupplierRes>> GetById(long id)
        {
            var supplier = await _service.GetById(id);
            if (supplier == null)
                return NotFound(new { message = "Supplier not found" });

            return Ok(ApiResponse<SupplierRes>.SuccessResponse(
                supplier,
                "Fetched categroy successfully"
            ));
        }

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponse<SupplierRes>>> Create([FromBody] SupplierReq request)
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
                ApiResponse<SupplierRes>.SuccessResponse(
                    created,
                    "Created category successfully"
                )
            );
        }


        [HttpPut("Update/{id:long}")]
        public async Task<ActionResult<SupplierRes>> Update(long id, [FromBody] SupplierReq request)
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

            return Ok(ApiResponse<SupplierRes>.SuccessResponse(
                updated,
                "Updated categroy successfully"
            ));
        }
    }
}
