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
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _service;

        public WarehouseController(IWarehouseService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<WarehouseRes>>> GetWarehouses(
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

                var warehouses =
                    activated == true ? await _service.GetAllActivated() : await _service.GetAll();

                return Ok(
                    ApiResponse<List<WarehouseRes>>.SuccessResponse(
                        warehouses,
                        "Fetched warehouses successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<WarehouseRes>> GetById(long id)
        {
            try
            {
                var warehouse = await _service.GetById(id);
                if (warehouse == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Warehouse not found"));

                return Ok(
                    ApiResponse<WarehouseRes>.SuccessResponse(
                        warehouse,
                        "Fetched warehouse successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<WarehouseRes>>> Create(
            [FromBody] WarehouseReq request
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
                    ApiResponse<WarehouseRes>.SuccessResponse(
                        created,
                        "Created warehouse successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id:long}")]
        public async Task<ActionResult<WarehouseRes>> Update(
            long id,
            [FromBody] WarehouseReq request
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
                    return NotFound(ApiResponse<string>.ErrorResponse("Warehouse not found"));

                return Ok(
                    ApiResponse<WarehouseRes>.SuccessResponse(
                        updated,
                        "Updated warehouse successfully"
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
