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

        public StockTakeController(IStockTakeService service)
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

        [HttpPut("approve/{id:long}")]
        public async Task<ActionResult<bool>> Approve(long id)
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
                var updated = await _service.Approve(id);
                if (!updated)
                    return NotFound(ApiResponse<string>.ErrorResponse("Stock take not found"));

                return Ok(
                    ApiResponse<bool>.SuccessResponse(updated, "Approve stock take successfully")
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("cancel/{id:long}")]
        public async Task<ActionResult<bool>> Cancel(long id)
        {
            try
            {
                var deleted = await _service.Cancel(id);

                if (deleted == false)
                    return NotFound(ApiResponse<string>.ErrorResponse("Stock take not found"));

                return Ok(
                    ApiResponse<bool>.SuccessResponse(deleted, "Cancel stock take successfully")
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{id:long}")]
        public async Task<ActionResult<bool>> Delete(long id)
        {
            try
            {
                var deleted = await _service.Delete(id);

                if (deleted == false)
                    return NotFound(ApiResponse<string>.ErrorResponse("Stock take not found"));

                return Ok(
                    ApiResponse<bool>.SuccessResponse(deleted, "Delete delivery successfully")
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }
    }
}
