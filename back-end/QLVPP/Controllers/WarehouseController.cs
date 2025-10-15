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
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _service;

        public WarehouseController(IWarehouseService service)
        {
            _service = service;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<List<WarehouseRes>>> GetAll()
        {
            var warehouses = await _service.GetAll();
            return Ok(
                ApiResponse<List<WarehouseRes>>.SuccessResponse(
                    warehouses,
                    "Fetched warehouses successfully"
                )
            );
        }

        [HttpGet("GetAllActivated")]
        public async Task<ActionResult<List<WarehouseRes>>> GetAllActivated()
        {
            var warehouses = await _service.GetAllActivated();
            return Ok(
                ApiResponse<List<WarehouseRes>>.SuccessResponse(
                    warehouses,
                    "Fetched warehouses successfully"
                )
            );
        }

        [HttpGet("GetById/{id:long}")]
        public async Task<ActionResult<WarehouseRes>> GetById(long id)
        {
            var warehouse = await _service.GetById(id);
            if (warehouse == null)
                return NotFound(new { message = "Warehouse not found" });

            return Ok(
                ApiResponse<WarehouseRes>.SuccessResponse(
                    warehouse,
                    "Fetched warehouse successfully"
                )
            );
        }

        [HttpPost("Create")]
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

            var created = await _service.Create(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                ApiResponse<WarehouseRes>.SuccessResponse(created, "Created warehouse successfully")
            );
        }

        [HttpPut("Update/{id:long}")]
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

            var updated = await _service.Update(id, request);
            if (updated == null)
                return NotFound(new { message = "Warehouse not found" });

            return Ok(
                ApiResponse<WarehouseRes>.SuccessResponse(updated, "Updated warehouse successfully")
            );
        }
    }
}
