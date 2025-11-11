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
    public class StockOutController : ControllerBase
    {
        private readonly IStockOutService _service;

        public StockOutController(IStockOutService service)
        {
            _service = service;
        }

        [HttpGet("warehouse/pending")]
        public async Task<ActionResult<List<StockOutRes>>> GetPendingDeliveries()
        {
            try
            {
                var stockOuts = await _service.GetPendingByWarehouse();

                return Ok(
                    ApiResponse<List<StockOutRes>>.SuccessResponse(
                        stockOuts,
                        "Fetched stock out successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("department/approved")]
        public async Task<ActionResult<List<StockOutRes>>> GetApprovedForDepartment()
        {
            try
            {
                var stockOuts = await _service.GetApprovedForDepartment();

                return Ok(
                    ApiResponse<List<StockOutRes>>.SuccessResponse(
                        stockOuts,
                        "Fetched stock out successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("warehouse/all")]
        public async Task<ActionResult<List<StockOutRes>>> GetByWarehouse()
        {
            try
            {
                var stockOuts = await _service.GetByWarehouse();

                return Ok(
                    ApiResponse<List<StockOutRes>>.SuccessResponse(
                        stockOuts,
                        "Fetched stock outs successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("my")]
        public async Task<ActionResult<StockOutRes>> GetAllByMyself()
        {
            try
            {
                var stockOuts = await _service.GetAllByMyself();
                return Ok(
                    ApiResponse<List<StockOutRes>>.SuccessResponse(
                        stockOuts,
                        "Fetched stock outs successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<StockOutRes>> GetById(long id)
        {
            try
            {
                var stockOut = await _service.GetById(id);
                if (stockOut == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Delivery not found"));

                return Ok(
                    ApiResponse<StockOutRes>.SuccessResponse(
                        stockOut,
                        "Fetched stock out successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<StockOutRes>>> Create(
            [FromBody] StockOutReq request
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
                    ApiResponse<StockOutRes>.SuccessResponse(
                        created,
                        "Created stock out successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id:long}")]
        public async Task<ActionResult<StockOutRes>> Update(long id, [FromBody] StockOutReq request)
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
                    return NotFound(ApiResponse<string>.ErrorResponse("Delivery not found"));

                return Ok(
                    ApiResponse<StockOutRes>.SuccessResponse(updated, "Delivery order successfully")
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("approve/{id:long}")]
        public async Task<ActionResult<StockOutRes>> Approve(
            long id,
            [FromBody] StockOutReq request
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
                var updated = await _service.Approve(id, request);
                if (updated == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Stock out not found"));

                return Ok(
                    ApiResponse<StockOutRes>.SuccessResponse(
                        updated,
                        "Approve stock out successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("receive/{id:long}")]
        public async Task<ActionResult<StockOutRes>> Received(long id)
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
                var updated = await _service.Receive(id);
                if (updated == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Delivery not found"));

                return Ok(
                    ApiResponse<StockOutRes>.SuccessResponse(
                        updated,
                        "Received supply successfully"
                    )
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
