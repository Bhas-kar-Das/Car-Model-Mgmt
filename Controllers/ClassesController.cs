using DemoAppAdo.Data;
using DemoAppAdo.Models;
using Microsoft.AspNetCore.Mvc;

namespace DemoAppAdo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassesController : ControllerBase
    {
        private readonly ClassRepository _classRepository;

        public ClassesController(ClassRepository classRepository)
        {
            _classRepository = classRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetClasses()
        {
            var classes = await _classRepository.GetAllClasses();
            return Ok(classes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClass(int id)
        {
            var carClass = await _classRepository.GetClassById(id);
            return carClass != null ? Ok(carClass) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateClass([FromBody] Class carClass)
        {
            await _classRepository.AddClass(carClass);
            return CreatedAtAction(nameof(GetClass), new { id = carClass.Id }, carClass);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClass(int id, [FromBody] Class carClass)
        {
            if (id != carClass.Id) return BadRequest();
            await _classRepository.UpdateClass(carClass);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClass(int id)
        {
            await _classRepository.DeleteClass(id);
            return NoContent();
        }
    }

}
