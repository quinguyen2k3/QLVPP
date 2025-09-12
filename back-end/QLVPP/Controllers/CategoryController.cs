using Microsoft.AspNetCore.Mvc;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace QLVPP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<CategoryRes>>> GetAll()
        {
            var categories = await _service.GetAll();
            return Ok(categories);
        }

        [HttpGet("GetById/{id:long}")]
        public async Task<ActionResult<CategoryRes>> GetById(long id)
        {
            var category = await _service.GetById(id);
            if (category == null)
                return NotFound(new { message = "Category not found" });

            return Ok(category);
        }

        [HttpPost("Create")]
        public async Task<ActionResult<CategoryRes>> Create([FromBody] CategoryReq request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _service.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("Update/{id:long}")]
        public async Task<ActionResult<CategoryRes>> Update(long id, [FromBody] CategoryReq request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _service.Update(id, request);
            if (updated == null)
                return NotFound(new { message = "Category not found" });

            return Ok(updated);
        }
    }
}
