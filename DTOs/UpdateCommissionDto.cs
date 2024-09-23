using System.ComponentModel.DataAnnotations;

namespace DemoAppAdo.DTOs
{
    public class UpdateCommissionDto
    {
        // Unique identifier for the salesman
        [Required(ErrorMessage = "CId is required.")]
        public string CId { get; set; }

        // Last year's sales figure
        [Required(ErrorMessage = "LastYearSales is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "LastYearSales must be a non-negative value.")]
        public decimal LastYearSales { get; set; }
    }
}
