using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLVPP.DTOs.Response;
using QLVPP.Services;

namespace QVLPP.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _service;

        public ReportController(IReportService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<DeptUsageRes>>> GetReports(
            [FromQuery] DateOnly startDate,
            [FromQuery] DateOnly endDate
        )
        {
            try
            {
                var reportData = await _service.GetReportUsage(startDate, endDate);

                return Ok(ApiResponse<List<DeptUsageRes>>.SuccessResponse(reportData));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    ApiResponse<string>.ErrorResponse(
                        $"An internal server error occurred: {ex.Message}"
                    )
                );
            }
        }
    }
}
