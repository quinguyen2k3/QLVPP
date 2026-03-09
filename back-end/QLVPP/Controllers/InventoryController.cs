using Microsoft.AspNetCore.Mvc;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.DTOs.Result;
using QLVPP.Services;

namespace QLVPP.Controllers
{
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly ICurrentUserService _currentUserService;

        public InventoryController(
            IInventoryService inventoryService,
            ICurrentUserService currentUserService
        )
        {
            _inventoryService = inventoryService;
            _currentUserService = currentUserService;
        }

        [HttpGet("snapshot/my-warehouse")]
        public async Task<ActionResult<List<InventorySnapshotRes>>> GetByWarehouse()
        {
            try
            {
                var snapshots = await _inventoryService.GetListAsync(
                    new InventorySnapshotFilterReq
                    {
                        WarehouseId = _currentUserService.GetWarehouseId(),
                    }
                );
                return Ok(
                    ApiResponse<List<InventorySnapshotRes>>.SuccessResponse(
                        snapshots,
                        "Fetched snapshot successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("snapshot/{id:long}")]
        public async Task<ActionResult<InventorySnapshotRes>> GetById(long id)
        {
            try
            {
                var snapshot = await _inventoryService.GetById(id);
                if (snapshot == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Snapshot not found"));

                return Ok(
                    ApiResponse<InventorySnapshotRes>.SuccessResponse(
                        snapshot,
                        "Fetched snapshot successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("snapshot")]
        public async Task<ActionResult<InventorySnapshotRes>> Create(
            [FromBody] InventorySnapshotReq request
        )
        {
            try
            {
                var created = await _inventoryService.Create(request);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.Id },
                    ApiResponse<InventorySnapshotRes>.SuccessResponse(
                        created,
                        "Created snapshot successfully"
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
