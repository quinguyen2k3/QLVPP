﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Services;

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
            try
            {
                var orders = await _service.GetAll();
                return Ok(
                    ApiResponse<List<OrderRes>>.SuccessResponse(
                        orders,
                        "Fetched orders successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("GetAllActivated")]
        public async Task<ActionResult<List<OrderRes>>> GetAllActivated()
        {
            try
            {
                var orders = await _service.GetAllActivated();
                return Ok(
                    ApiResponse<List<OrderRes>>.SuccessResponse(
                        orders,
                        "Fetched orders successfully"
                    )
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("GetById/{id:long}")]
        public async Task<ActionResult<OrderRes>> GetById(long id)
        {
            try
            {
                var order = await _service.GetById(id);
                if (order == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Order not found"));

                return Ok(
                    ApiResponse<OrderRes>.SuccessResponse(order, "Fetched order successfully")
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
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

            try
            {
                var created = await _service.Create(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.Id },
                    ApiResponse<OrderRes>.SuccessResponse(created, "Created order successfully")
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
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

            try
            {
                var updated = await _service.Update(id, request);
                if (updated == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Order not found"));

                return Ok(
                    ApiResponse<OrderRes>.SuccessResponse(updated, "Updated order successfully")
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
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

            try
            {
                var updated = await _service.Received(id, request);
                if (updated == null)
                    return NotFound(ApiResponse<string>.ErrorResponse("Order not found"));

                return Ok(
                    ApiResponse<OrderRes>.SuccessResponse(updated, "Received order successfully")
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }
    }
}
