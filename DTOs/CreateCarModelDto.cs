using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DemoAppAdo.DTOs
{
    public class CreateCarModelDto
    {
        [Required(ErrorMessage = "Brand ID is required.")]
        public int BrandId { get; set; }  // Changed to int

        [Required(ErrorMessage = "Class ID is required.")]
        public int ClassId { get; set; }  // Changed to int

        [Required(ErrorMessage = "Model Name is required.")]
        [StringLength(100, ErrorMessage = "Model Name cannot be longer than 100 characters.")]
        public string ModelName { get; set; }

        [Required(ErrorMessage = "Model Code is required.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Model Code must be exactly 10 alphanumeric characters.")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Model Code must be alphanumeric.")]
        public string ModelCode { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Features are required.")]
        public string Features { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Date of Manufacturing is required.")]
        [DataType(DataType.Date)]
        public DateTime DateOfManufacturing { get; set; }

        public bool Active { get; set; } = true;  // Default to true if not provided

        [Required(ErrorMessage = "Sort Order is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Sort Order must be greater than zero.")]
        public int SortOrder { get; set; }

        [Required(ErrorMessage = "At least one image is required.")]
        public IList<IFormFile> Images { get; set; }
    }
}
