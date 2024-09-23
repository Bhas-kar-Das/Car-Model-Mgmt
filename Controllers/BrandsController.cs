using DemoAppAdo.Data;
using DemoAppAdo.Models;
using Microsoft.AspNetCore.Mvc;

namespace DemoAppAdo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandsController : ControllerBase
    {
        private readonly BrandRepository _brandRepository;

        public BrandsController(BrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetBrands()
        {
            var brands = await _brandRepository.GetAllBrands();
            return Ok(brands);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrand(int id)
        {
            var brand = await _brandRepository.GetBrandById(id);
            return brand != null ? Ok(brand) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateBrand([FromBody] Brand brand)
        {
            await _brandRepository.AddBrand(brand);
            return CreatedAtAction(nameof(GetBrand), new { id = brand.Id }, brand);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBrand(int id, [FromBody] Brand brand)
        {
            if (id != brand.Id) return BadRequest();
            await _brandRepository.UpdateBrand(brand);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            await _brandRepository.DeleteBrand(id);
            return NoContent();
        }
    }

}
