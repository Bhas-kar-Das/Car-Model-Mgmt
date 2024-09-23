using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using DemoAppAdo.Data;
using DemoAppAdo.DTOs;
using System.Text.RegularExpressions;

namespace DemoAppAdo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarModelsController : ControllerBase
    {
        private readonly CarModelRepository _carModelRepository;

        public CarModelsController(CarModelRepository carModelRepository)
        {
            _carModelRepository = carModelRepository;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCarModel([FromForm] CreateCarModelDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate ModelCode length and format
            if (model.ModelCode.Length != 10 || !Regex.IsMatch(model.ModelCode, @"^[a-zA-Z0-9]+$"))
            {
                return BadRequest("Model Code must be 10 alphanumeric characters.");
            }

            // Validate images
            if (model.Images == null || model.Images.Count == 0)
            {
                return BadRequest("At least one image is required.");
            }

            var imagePaths = new List<string>();
            var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), $"images/{model.ModelCode}");

            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            foreach (var image in model.Images)
            {
                if (image.Length > 5 * 1024 * 1024)
                {
                    return BadRequest("Image size must be less than 5MB.");
                }

                var validImageTypes = new[] { "image/jpeg", "image/png", "image/jpg" };
                if (!validImageTypes.Contains(image.ContentType))
                {
                    return BadRequest("Only JPEG ,jpg and PNG images are allowed.");
                }


                var uniqueFileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                var imagePath = Path.Combine(uploadDirectory, uniqueFileName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                imagePaths.Add(imagePath);
            }

            try
            {
                await _carModelRepository.CreateCarModel(model);

                // Call SaveImagePaths with the entire list of image paths
                await _carModelRepository.SaveImagePaths(model.ModelCode, imagePaths);

                // Create car model in the database
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            return CreatedAtAction(nameof(GetCarModel), new { id = model.ModelCode }, model);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCarModel(string id)
        {
            var model = await _carModelRepository.GetCarModelById(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpGet("list")]
        public async Task<IActionResult> ListCarModels(string ? search = null, string ? orderBy = null)
        {
            var models = await _carModelRepository.GetAllCarModels(search, orderBy);
            return Ok(models);
        }
    }
}
