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
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrderController(IOrderService service)
        {
            _service = service;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<List<OrderRes>>> GetAll()
        {
            var orders = await _service.GetAll();
            return Ok(
                ApiResponse<List<OrderRes>>.SuccessResponse(orders, "Fetched orders successfully")
            );
        }

        [HttpGet("GetAllActivated")]
        public async Task<ActionResult<List<OrderRes>>> GetAllActivated()
        {
            var orders = await _service.GetAllActivated();
            return Ok(
                ApiResponse<List<OrderRes>>.SuccessResponse(orders, "Fetched orders successfully")
            );
        }

        [HttpGet("GetById/{id:long}")]
        public async Task<ActionResult<OrderRes>> GetById(long id)
        {
            var order = await _service.GetById(id);
            if (order == null)
                return NotFound(new { message = "Order not found" });

            return Ok(ApiResponse<OrderRes>.SuccessResponse(order, "Fetched order successfully"));
        }

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponse<OrderRes>>> Create([FromBody] OrderReq request)
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
                ApiResponse<OrderRes>.SuccessResponse(created, "Created order successfully")
            );
        }

        [HttpPut("Update/{id:long}")]
        public async Task<ActionResult<OrderRes>> Update(long id, [FromBody] OrderReq request)
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
                return NotFound(new { message = "Order not found" });

            return Ok(ApiResponse<OrderRes>.SuccessResponse(updated, "Updated order successfully"));
        }

        [HttpPut("Received/{id:long}")]
        public async Task<ActionResult<OrderRes>> Received(long id, [FromBody] OrderReq request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<string>.ErrorResponse("Validation failed", errors));
            }

            var updated = await _service.Received(id, request);
            if (updated == null)
                return NotFound(new { message = "Order not found" });

            return Ok(
                ApiResponse<OrderRes>.SuccessResponse(updated, "Received order successfully")
            );
        }
    }
}
