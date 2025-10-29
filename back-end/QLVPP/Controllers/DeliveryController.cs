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
    public class DeliveryController : ControllerBase
    {
        private readonly IDeliveryService _service;

        public DeliveryController(IDeliveryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<DeliveryRes>>> GetDeliveries(
            [FromQuery] bool? activated
        )
        {
            try
            {
                if (activated.HasValue && !activated.Value)
                {
                    return BadRequest(
                        ApiResponse<string>.ErrorResponse(
                            "Invalid query: activated cannot be false."
                        )
                    );
                }

                var deliveries =
                    activated == true ? await _service.GetAllActivated() : await _service.GetAll();

                return Ok(
                    ApiResponse<List<DeliveryRes>>.SuccessResponse(
                        deliveries,
                        "Fetched deliveries successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("my")]
        public async Task<ActionResult<DeliveryRes>> GetAllByMyself()
        {
            try
            {
                var deliveries = await _service.GetAllByMyself();
                return Ok(
                    ApiResponse<List<DeliveryRes>>.SuccessResponse(
                        deliveries,
                        "Fetched deliveries successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<DeliveryRes>> GetById(long id)
        {
            try
            {
                var delivery = await _service.GetById(id);
                if (delivery == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Delivery not found"));

                return Ok(
                    ApiResponse<DeliveryRes>.SuccessResponse(
                        delivery,
                        "Fetched delivery successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<DeliveryRes>>> Create(
            [FromBody] DeliveryReq request
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
                    ApiResponse<DeliveryRes>.SuccessResponse(
                        created,
                        "Created delivery successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id:long}")]
        public async Task<ActionResult<DeliveryRes>> Update(long id, [FromBody] DeliveryReq request)
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
                    ApiResponse<DeliveryRes>.SuccessResponse(updated, "Delivery order successfully")
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("dispatch/{id:long}")]
        public async Task<ActionResult<DeliveryRes>> Received(
            long id,
            [FromBody] DeliveryReq request
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
                var updated = await _service.Dispatch(id, request);
                if (updated == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Delivery not found"));

                return Ok(
                    ApiResponse<DeliveryRes>.SuccessResponse(
                        updated,
                        "Dispatch supply successfully"
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
