using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Services;
using QLVPP.Services.Implementations;

namespace QLVPP.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class StockTakeController : ControllerBase
    {
        public readonly IStockTakeService _service;

        public StockTakeController(StockTakeService service)
        {
            _service = service;
        }

        [HttpGet("warehouse/all")]
        public async Task<ActionResult<List<StockTakeRes>>> GetByWarehouse()
        {
            try
            {
                var stockTakes = await _service.GetByWarehouse();

                return Ok(
                    ApiResponse<List<StockTakeRes>>.SuccessResponse(
                        stockTakes,
                        "Fetched stock take successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<StockTakeRes>> GetById(long id)
        {
            try
            {
                var stockTake = await _service.GetById(id);
                if (stockTake == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Stock take not found"));

                return Ok(
                    ApiResponse<StockTakeRes>.SuccessResponse(
                        stockTake,
                        "Fetched stock take successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<StockInRes>>> Create(
            [FromBody] StockTakeReq request
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
                    ApiResponse<StockTakeRes>.SuccessResponse(created, "Created order successfully")
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id:long}")]
        public async Task<ActionResult<StockTakeRes>> Update(
            long id,
            [FromBody] StockTakeReq request
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
                    return NotFound(ApiResponse<string>.ErrorResponse("Order not found"));

                return Ok(
                    ApiResponse<StockTakeRes>.SuccessResponse(
                        updated,
                        "Updated stock take successfully"
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
