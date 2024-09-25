using System;

namespace DemoAppAdo.Models
{
    public class SalesRecord
    {
        public int Id { get; set; }
        public string SalesmanId { get; set; }
        public string CarModelId { get; set; }
        public int NumberOfCarsSold { get; set; }
        public int Class { get; set; }
        public int Brand { get; set; }


    }

    public class SalesmanCommissionReport
    {
        public string SalesmanName { get; set; }
        public int NumberOfCarsSold { get; set; }
        public decimal TotalCommission { get; set; }
        public List<string> Brands { get; set; } // List of brands sold
        public List<string> Classes { get; set; } // List of classes sold

        public SalesmanCommissionReport()
        {
            Brands = new List<string>();
            Classes = new List<string>();
        }
    }


    public class SalesmanSalesFigures
    {
        public string CId { get; set; }
        public string SalesmanName { get; set; }
        public decimal LastYearTotalSales { get; set; }
    }


   

    public class CommissionReport
    {
        public string SalesmanName { get; set; }
        public int ClassA { get; set; }
        public int ClassB { get; set; }
        public int ClassC { get; set; }
        public string BrandName { get; set; }
    }

    public class CommissionRate
    {
        public int Id { get; set; }
        public int BrandId { get; set; }
        public decimal FixedCommission { get; set; }
        public decimal? ClassACommission { get; set; }
        public decimal? ClassBCommission { get; set; }
        public decimal? ClassCCommission { get; set; }
    }
}
