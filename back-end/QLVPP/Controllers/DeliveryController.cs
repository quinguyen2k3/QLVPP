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
    public class DeliveryController : ControllerBase
    {
        private readonly IDeliveryService _service;

        public DeliveryController(IDeliveryService service)
        {
            _service = service;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<List<DeliveryRes>>> GetAll()
        {
            var deliveries = await _service.GetAll();
            return Ok(ApiResponse<List<DeliveryRes>>.SuccessResponse(
                deliveries,
                "Fetched deliveries successfully"
            ));
        }

        [HttpGet("GetAllActived")]
        public async Task<ActionResult<List<DeliveryRes>>> GetAllActived()
        {
            var deliveries = await _service.GetAllActived();
            return Ok(ApiResponse<List<DeliveryRes>>.SuccessResponse(
                 deliveries,
                 "Fetched deliveries successfully"
             ));
        }

        [HttpGet("GetById/{id:long}")]
        public async Task<ActionResult<OrderRes>> GetById(long id)
        {
            var delivery = await _service.GetById(id);
            if (delivery == null)
                return NotFound(new { message = "Delivery not found" });

            return Ok(ApiResponse<DeliveryRes>.SuccessResponse(
                delivery,
                "Fetched delivery successfully"
            ));
        }

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponse<DeliveryRes>>> Create([FromBody] DeliveryReq request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<string>.ErrorResponse(
                    "Validation failed",
                    errors
                ));
            }

            var created = await _service.Create(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                ApiResponse<DeliveryRes>.SuccessResponse(
                    created,
                    "Created delivery successfully"
                )
            );
        }

        [HttpPut("Update/{id:long}")]
        public async Task<ActionResult<DeliveryRes>> Update(long id, [FromBody] DeliveryReq request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<string>.ErrorResponse(
                    "Validation failed",
                    errors
                ));
            }

            var updated = await _service.Update(id, request);
            if (updated == null)
                return NotFound(new { message = "Delivery not found" });

            return Ok(ApiResponse<DeliveryRes>.SuccessResponse(
                updated,
                "Delivery order successfully"
            ));
        }

        [HttpPut("Dispatch/{id:long}")]
        public async Task<ActionResult<OrderRes>> Received(long id, [FromBody] DeliveryReq request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<string>.ErrorResponse(
                    "Validation failed",
                    errors
                ));
            }

            var updated = await _service.Dispatch(id, request);
            if (updated == null)
                return NotFound(new { message = "Delivery not found" });

            return Ok(ApiResponse<DeliveryRes>.SuccessResponse(
                updated,
                "Dispatch supply successfully"
            ));
        }
    }
}
