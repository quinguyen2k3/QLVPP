using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLVPP.Constants.Status;
using QLVPP.Constants.Types;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;
using QLVPP.Services;

namespace QLVPP.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class StockOutController : ControllerBase
    {
        private readonly IStockOutService _stockOutService;
        private readonly ICurrentUserService _currentUserService;

        public StockOutController(IStockOutService service, ICurrentUserService currentUserService)
        {
            _stockOutService = service;
            _currentUserService = currentUserService;
        }

        [HttpGet("warehouse/pending")]
        public async Task<ActionResult<List<StockOutRes>>> GetPendingStockOut()
        {
            try
            {
                var warehouseId = _currentUserService.GetWarehouseId();
                var stockOuts = await _stockOutService.GetByConditions(
                    new StockOutFilterReq
                    {
                        Statuses = new List<string> { StockOutStatus.Pending },
                        FromWarehouseId = warehouseId,
                        IsActivated = true,
                        OrderByDesc = true,
                    }
                );

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
                var stockOuts = await _stockOutService.GetByConditions(
                    new StockOutFilterReq
                    {
                        Statuses = new List<string> { StockOutStatus.Approved },
                        DepartmentId = _currentUserService.GetDepartmentId(),
                        IsActivated = true,
                        OrderByDesc = true,
                    }
                );

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

        [HttpGet("department/received")]
        public async Task<ActionResult<List<StockOutRes>>> GetReceivedForDepartment()
        {
            try
            {
                var stockOuts = await _stockOutService.GetByConditions(
                    new StockOutFilterReq
                    {
                        Statuses = new List<string> { StockOutStatus.Received },
                        DepartmentId = _currentUserService.GetDepartmentId(),
                        IsActivated = true,
                        OrderByDesc = true,
                    }
                );

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
                var stockOuts = await _stockOutService.GetByConditions(
                    new StockOutFilterReq
                    {
                        FromWarehouseId = _currentUserService.GetWarehouseId(),
                        IsActivated = true,
                        OrderByDesc = true,
                    }
                );

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
                var stockOuts = await _stockOutService.GetByConditions(
                    new StockOutFilterReq
                    {
                        CreatedBy = _currentUserService.GetUserAccount(),
                        IsActivated = true,
                        OrderByDesc = true,
                    }
                );
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
                var stockOut = await _stockOutService.GetById(id);
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
                var created = await _stockOutService.Create(request);

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
                var updated = await _stockOutService.Update(id, request);
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
                var updated = await _stockOutService.Approve(id);
                if (!updated)
                    return NotFound(ApiResponse<string>.ErrorResponse("Stock out not found"));

                return Ok(
                    ApiResponse<bool>.SuccessResponse(updated, "Approve stock out successfully")
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("receive/{id:long}")]
        public async Task<ActionResult<bool>> Received(long id)
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
                var updated = await _stockOutService.Receive(id);
                if (!updated)
                    return NotFound(ApiResponse<string>.ErrorResponse("Delivery not found"));

                return Ok(
                    ApiResponse<bool>.SuccessResponse(updated, "Received supply successfully")
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
                var deleted = await _stockOutService.Cancel(id);

                if (deleted == false)
                    return NotFound(ApiResponse<string>.ErrorResponse("Delivery not found"));

                return Ok(
                    ApiResponse<bool>.SuccessResponse(deleted, "Cancel delivery successfully")
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
                var deleted = await _stockOutService.Delete(id);

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

        [HttpGet("transfer/received")]
        public async Task<IActionResult> GetReceivedTransfers()
        {
            try
            {
                var myWarehouseId = _currentUserService.GetWarehouseId();
                var result = await _stockOutService.GetByConditions(
                    new StockOutFilterReq
                    {
                        Type = StockOutType.Transfer,
                        ToWarehouseId = myWarehouseId,
                        Statuses = new List<string> { StockOutStatus.Received },
                        IsActivated = true,
                        OrderByDesc = true,
                    }
                );
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("usage/received")]
        public async Task<IActionResult> GetReceivedUsage()
        {
            try
            {
                var myWarehouseId = _currentUserService.GetWarehouseId();
                var result = await _stockOutService.GetByConditions(
                    new StockOutFilterReq
                    {
                        Type = StockOutType.Usage,
                        ToWarehouseId = myWarehouseId,
                        Statuses = new List<string> { StockOutStatus.Received },
                        IsActivated = true,
                        OrderByDesc = true,
                    }
                );
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("transfer/incoming")]
        public async Task<ActionResult<ApiResponse<List<StockOutRes>>>> GetIncomingTransfers()
        {
            try
            {
                var myWarehouseId = _currentUserService.GetWarehouseId();
                var transfers = await _stockOutService.GetByConditions(
                    new StockOutFilterReq
                    {
                        Type = StockOutType.Transfer,
                        ToWarehouseId = myWarehouseId,
                        Statuses = new List<string> { StockOutStatus.Approved },
                        IsActivated = true,
                        OrderByDesc = true,
                    }
                );

                return Ok(
                    ApiResponse<List<StockOutRes>>.SuccessResponse(
                        transfers,
                        "Fetched incoming transfers successfully"
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
