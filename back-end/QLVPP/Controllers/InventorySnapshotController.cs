using Microsoft.AspNetCore.Mvc;
using QLVPP.DTOs.Response;
using QLVPP.Services;

namespace QLVPP.Controllers
{
    [Route("api/[controller]")]
    public class InventorySnapshotController : ControllerBase
    {
        private readonly IInventorySnapshotService _service;

        public InventorySnapshotController(IInventorySnapshotService service)
        {
            _service = service;
        }

        [HttpGet("GetByWarehouse")]
        public async Task<ActionResult<List<InventorySnapshotRes>>> GetByWarehouse()
        {
            var snapshots = await _service.GetByWarehouse();
            return Ok(
                ApiResponse<List<InventorySnapshotRes>>.SuccessResponse(
                    snapshots,
                    "Fetched snapshot successfully"
                )
            );
        }

        [HttpGet("GetById/{id:long}")]
        public async Task<ActionResult<InventorySnapshotRes>> GetById(long id)
        {
            var snapshot = await _service.GetById(id);
            if (snapshot == null)
                return NotFound(new { message = "Snapshot not found" });

            return Ok(
                ApiResponse<InventorySnapshotRes>.SuccessResponse(
                    snapshot,
                    "Fetched snapshot successfully"
                )
            );
        }

        [HttpPost("Create")]
        public async Task<ActionResult<InventorySnapshotRes>> Create()
        {
            var created = await _service.Create();
            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                ApiResponse<InventorySnapshotRes>.SuccessResponse(
                    created,
                    "Created product successfully"
                )
            );
        }

        [HttpPut("Close/{id:long}")]
        public async Task<ActionResult<InventorySnapshotRes>> Close(long id)
        {
            var closed = await _service.Close(id);
            if (closed == null)
                return NotFound(new { message = "Snapshot not found" });

            return Ok(
                ApiResponse<InventorySnapshotRes>.SuccessResponse(
                    closed,
                    "Close snapshot successfully"
                )
            );
        }
    }
}
