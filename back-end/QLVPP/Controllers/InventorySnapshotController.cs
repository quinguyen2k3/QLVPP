using Microsoft.AspNetCore.Mvc;
using QLVPP.DTOs.Response;
using QLVPP.Services;

namespace QLVPP.Controllers
{
    [Route("api/[controller]")]
    public class SnapshotController : ControllerBase
    {
        private readonly IInventorySnapshotService _service;

        public SnapshotController(IInventorySnapshotService service)
        {
            _service = service;
        }

        [HttpGet("my-warehouse")]
        public async Task<ActionResult<List<InventorySnapshotRes>>> GetByWarehouse()
        {
            try
            {
                var snapshots = await _service.GetByWarehouse();
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

        [HttpGet("{id:long}")]
        public async Task<ActionResult<InventorySnapshotRes>> GetById(long id)
        {
            try
            {
                var snapshot = await _service.GetById(id);
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

        [HttpPost]
        public async Task<ActionResult<InventorySnapshotRes>> Create()
        {
            try
            {
                var created = await _service.Create();
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

        [HttpPut("{id:long}/closing")]
        public async Task<ActionResult<InventorySnapshotRes>> Close(long id)
        {
            try
            {
                var closed = await _service.Close(id);
                if (closed == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Snapshot not found"));

                return Ok(
                    ApiResponse<InventorySnapshotRes>.SuccessResponse(
                        closed,
                        "Close snapshot successfully"
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
