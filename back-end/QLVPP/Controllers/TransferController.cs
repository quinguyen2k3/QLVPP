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
    public class TransferController : ControllerBase
    {
        private readonly ITransferService _service;

        public TransferController(ITransferService service)
        {
            _service = service;
        }

        [HttpGet("my")]
        public async Task<ActionResult<List<TransferRes>>> GetAllByMyself()
        {
            try
            {
                var transfers = await _service.GetAllByMyself();
                return Ok(
                    ApiResponse<List<TransferRes>>.SuccessResponse(
                        transfers,
                        "Fetched transfer successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<TransferRes>> GetById(long id)
        {
            try
            {
                var transfer = await _service.GetById(id);
                if (transfer == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Transfer not found"));

                return Ok(
                    ApiResponse<TransferRes>.SuccessResponse(
                        transfer,
                        "Fetched transfer successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<TransferRes>> Create([FromBody] TransferReq request)
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
                    ApiResponse<TransferRes>.SuccessResponse(
                        created,
                        "Created transfer successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("approve/{id:long}")]
        public async Task<ActionResult<TransferRes>> Approve(
            long id,
            [FromBody] TransferReq request
        )
        {
            try
            {
                var transfer = await _service.Approve(id, request);
                if (transfer == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Transfer not found"));

                return Ok(
                    ApiResponse<TransferRes>.SuccessResponse(
                        transfer,
                        "Approve transfer successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("receive/{id:long}")]
        public async Task<ActionResult<TransferRes>> Receive(
            long id,
            [FromBody] TransferReq request
        )
        {
            try
            {
                var transfer = await _service.Receive(id, request);
                if (transfer == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Transfer not found"));

                return Ok(
                    ApiResponse<TransferRes>.SuccessResponse(
                        transfer,
                        "Received transfer successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("from-warehouse/pending")]
        public async Task<ActionResult<List<TransferRes>>> GetPendingOfWarehouse()
        {
            try
            {
                var transfers = await _service.GetPendingFromWarehouse();
                return Ok(
                    ApiResponse<List<TransferRes>>.SuccessResponse(
                        transfers,
                        "Fetched transfer successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("to-warehouse/approved")]
        public async Task<ActionResult<List<TransferRes>>> GetApprovedForWarehouse()
        {
            try
            {
                var transfers = await _service.GetApprovedForWarehouse();
                return Ok(
                    ApiResponse<List<TransferRes>>.SuccessResponse(
                        transfers,
                        "Fetched transfer successfully"
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
