using System.ComponentModel.DataAnnotations;

namespace DemoAppAdo.DTOs
{
    public class SalesRecordDto
    {
        [Required(ErrorMessage = "SalesmanId is required.")]
        public string SalesmanId { get; set; }

        [Required(ErrorMessage = "CarModelId is required.")]
        public string CarModelId { get; set; }

        [Required(ErrorMessage = "NumberOfCarsSold is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "NumberOfCarsSold must be a positive integer.")]
        public int NumberOfCarsSold { get; set; }

        [Required(ErrorMessage = "Brand is required.")]
        public int Brand { get; set; }

        [Required(ErrorMessage = "Class is required.")]
        public int Class { get; set; }

   
    }
}
