using System.ComponentModel.DataAnnotations;

namespace DemoAppAdo.Models
{
   
        public class CarModel
        {
            public int Id { get; set; }
            public int Brand { get; set; }
            public int Class { get; set; }
        public string? BrandName { get; set; }
        public string? ClassName { get; set; }
        public string ModelName { get; set; }
            public string ModelCode { get; set; }
            public string Description { get; set; }
            public string Features { get; set; }
            public decimal Price { get; set; }
            public DateTime DateOfManufacturing { get; set; }
            public bool Active { get; set; }
            public int SortOrder { get; set; }
        public List<string>? ImageUrls { get; set; } // This can be null if there are no images

        public IList<IFormFile> Images { get; set; }  // Add this line for images

    }



}
