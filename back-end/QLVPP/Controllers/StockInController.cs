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
    public class StockInController : ControllerBase
    {
        private readonly IStockInService _service;

        public StockInController(IStockInService service)
        {
            _service = service;
        }

        [HttpGet("warehouse/pending")]
        public async Task<ActionResult<List<StockInRes>>> GetPendingByWarehouse()
        {
            try
            {
                var stockIns = await _service.GetPendingByWarehouse();

                return Ok(
                    ApiResponse<List<StockInRes>>.SuccessResponse(
                        stockIns,
                        "Fetched orders successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("warehouse/all")]
        public async Task<ActionResult<List<StockInRes>>> GetByWarehouse()
        {
            try
            {
                var stockIns = await _service.GetByWarehouse();

                return Ok(
                    ApiResponse<List<StockInRes>>.SuccessResponse(
                        stockIns,
                        "Fetched orders successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("my")]
        public async Task<ActionResult<StockInRes>> GetAllByMyself()
        {
            try
            {
                var stockIns = await _service.GetAllByMyself();
                return Ok(
                    ApiResponse<List<StockInRes>>.SuccessResponse(
                        stockIns,
                        "Fetched orders successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<StockInRes>> GetById(long id)
        {
            try
            {
                var stockIn = await _service.GetById(id);
                if (stockIn == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Order not found"));

                return Ok(
                    ApiResponse<StockInRes>.SuccessResponse(stockIn, "Fetched order successfully")
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<StockInRes>>> Create(
            [FromBody] StockInReq request
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
                    ApiResponse<StockInRes>.SuccessResponse(created, "Created order successfully")
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id:long}")]
        public async Task<ActionResult<StockInRes>> Update(long id, [FromBody] StockInReq request)
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
                    ApiResponse<StockInRes>.SuccessResponse(updated, "Updated order successfully")
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("approve/{id:long}")]
        public async Task<ActionResult<StockInRes>> Receive(long id, [FromBody] StockInReq request)
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
                var updated = await _service.Approve(id, request);
                if (updated == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Order not found"));

                return Ok(
                    ApiResponse<StockInRes>.SuccessResponse(updated, "Received order successfully")
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
                    return NotFound(ApiResponse<string>.ErrorResponse("Delivery not found"));

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
