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
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _service;

        public SupplierController(ISupplierService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<SupplierRes>>> GetSuppliers([FromQuery] bool? activated)
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

                var suppliers =
                    activated == true ? await _service.GetAllActivated() : await _service.GetAll();

                return Ok(
                    ApiResponse<List<SupplierRes>>.SuccessResponse(
                        suppliers,
                        "Fetched suppliers successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<SupplierRes>> GetById(long id)
        {
            try
            {
                var supplier = await _service.GetById(id);
                if (supplier == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Supplier not found"));

                return Ok(
                    ApiResponse<SupplierRes>.SuccessResponse(
                        supplier,
                        "Fetched supplier successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<SupplierRes>>> Create(
            [FromBody] SupplierReq request
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
                    ApiResponse<SupplierRes>.SuccessResponse(
                        created,
                        "Created supplier successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id:long}")]
        public async Task<ActionResult<SupplierRes>> Update(long id, [FromBody] SupplierReq request)
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
                    return NotFound(ApiResponse<string>.ErrorResponse("Supplier not found"));

                return Ok(
                    ApiResponse<SupplierRes>.SuccessResponse(
                        updated,
                        "Updated supplier successfully"
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
