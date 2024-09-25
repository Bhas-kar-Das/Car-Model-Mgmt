using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DemoAppAdo.DTOs;
using DemoAppAdo.Models;

namespace DemoAppAdo.Data
{
    public class SalesRecordRepository
    {
        private readonly string _connectionString;

        public SalesRecordRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task InsertSalesmanAsync(Salesman salesman)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("INSERT INTO Salesmen (CId, Name, LastYearSales, CreatedOn, CreatedBy, Status) VALUES (@CId, @Name, @LastYearSales, @CreatedOn, @CreatedBy, @Status)", connection))
                {
                    command.Parameters.AddWithValue("@CId", salesman.CId);
                    command.Parameters.AddWithValue("@Name", salesman.Name);
                    command.Parameters.AddWithValue("@LastYearSales", 0m);
                    command.Parameters.AddWithValue("@CreatedOn", DateTime.UtcNow);
                    command.Parameters.AddWithValue("@CreatedBy", "system");
                    command.Parameters.AddWithValue("@Status", true);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task InsertSalesRecord(SalesRecord salesRecord)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("InsertSalesRecord", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@SalesmanId", salesRecord.SalesmanId);
                    command.Parameters.AddWithValue("@CarModelId", salesRecord.CarModelId);
                    command.Parameters.AddWithValue("@NumberOfCarsSold", salesRecord.NumberOfCarsSold);
                    command.Parameters.AddWithValue("@Brand", salesRecord.Brand);
                    command.Parameters.AddWithValue("@Class", salesRecord.Class);
                    command.Parameters.AddWithValue("@CreatedBy", "system");

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<IEnumerable<SalesmanCommissionReport>> GetSalesmanCommissionReportAsync()
        {
            var reportDict = new Dictionary<string, SalesmanCommissionReport>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var salesFigures = await GetLastYearSalesFiguresAsync(connection);
                var salesRecords = await GetSalesRecordsAsync(connection);
                var brands = await GetBrandsAsync(connection);
                var Classes = await GetClassAsync(connection);
                var commissionRates = await GetCommissionRatesAsync(connection);

                foreach (var salesRecord in salesRecords)
                {
                    var salesman = salesFigures.FirstOrDefault(sf => sf.CId == salesRecord.SalesmanId);
                    var brand = brands.FirstOrDefault(b => b.Id == salesRecord.Brand);
                    var carClass = Classes.FirstOrDefault(c => c.Id == salesRecord.Class);
                    var commissionRate = commissionRates.FirstOrDefault(cr => cr.BrandId == brand.Id);

                    if (salesman != null && commissionRate != null)
                    {
                        decimal totalCommission = commissionRate.FixedCommission;
                        totalCommission += GetClassCommission(salesRecord.Class, commissionRate);

                        if (salesman.LastYearTotalSales > 500000 && salesRecord.Class == 1) // Class A
                        {
                            totalCommission += salesman.LastYearTotalSales * 0.02m; // 2% additional
                        }

                        if (reportDict.ContainsKey(salesman.SalesmanName))
                        {
                            reportDict[salesman.SalesmanName].NumberOfCarsSold += salesRecord.NumberOfCarsSold;
                            reportDict[salesman.SalesmanName].TotalCommission += totalCommission;

                            if (!reportDict[salesman.SalesmanName].Brands.Contains(brand.Name))
                            {
                                reportDict[salesman.SalesmanName].Brands.Add(brand.Name);
                            }

                            if (!reportDict[salesman.SalesmanName].Classes.Contains(carClass.Name))
                            {
                                reportDict[salesman.SalesmanName].Classes.Add(carClass.Name);
                            }
                        }
                        else
                        {
                            reportDict[salesman.SalesmanName] = new SalesmanCommissionReport
                            {
                                SalesmanName = salesman.SalesmanName,
                                NumberOfCarsSold = salesRecord.NumberOfCarsSold,
                                TotalCommission = totalCommission,
                                Brands = new List<string> { brand.Name },
                                Classes = new List<string> { carClass.Name }
                            };
                        }
                    }
                }
            }

            return reportDict.Values.ToList();
        }

        private async Task<List<SalesmanSalesFigures>> GetLastYearSalesFiguresAsync(SqlConnection connection)
        {
            var figures = new List<SalesmanSalesFigures>();
            var command = new SqlCommand("SELECT CId, SalesmanName, LastYearTotalSales FROM SalesmanSalesFigures", connection);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    figures.Add(new SalesmanSalesFigures
                    {
                        CId = reader["CId"].ToString(),
                        SalesmanName = reader["SalesmanName"].ToString(),
                        LastYearTotalSales = reader.GetDecimal(reader.GetOrdinal("LastYearTotalSales"))
                    });
                }
            }

            return figures;
        }

        private async Task<List<SalesRecord>> GetSalesRecordsAsync(SqlConnection connection)
        {
            var records = new List<SalesRecord>();
            var command = new SqlCommand("SELECT SalesmanId, CarModelId, NumberOfCarsSold, Brand, Class FROM SalesRecords", connection);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    records.Add(new SalesRecord
                    {
                        SalesmanId = reader["SalesmanId"].ToString(),
                        CarModelId = reader["CarModelId"].ToString(),
                        NumberOfCarsSold = reader.GetInt32(reader.GetOrdinal("NumberOfCarsSold")),
                        Brand = reader.GetInt32(reader.GetOrdinal("Brand")),
                        Class = reader.GetInt32(reader.GetOrdinal("Class"))
                    });
                }
            }

            return records;
        }
        private async Task<List<Class>> GetClassAsync(SqlConnection connection)
        {
            var Classes = new List<Class>();
            var command = new SqlCommand("SELECT Id, Name FROM Classes", connection);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    Classes.Add(new Class
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader["Name"].ToString()
                    });
                }
            }

            return Classes;
        }
       
        private async Task<List<Brand>> GetBrandsAsync(SqlConnection connection)
        {
            var brands = new List<Brand>();
            var command = new SqlCommand("SELECT Id, Name FROM Brands", connection);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    brands.Add(new Brand
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader["Name"].ToString()
                    });
                }
            }

            return brands;
        }

        private async Task<List<CommissionRate>> GetCommissionRatesAsync(SqlConnection connection)
        {
            var rates = new List<CommissionRate>();
            var command = new SqlCommand("SELECT Id, BrandId, FixedCommission, ClassACommission, ClassBCommission, ClassCCommission FROM CommissionRates", connection);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    rates.Add(new CommissionRate
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        BrandId = reader.GetInt32(reader.GetOrdinal("BrandId")),
                        FixedCommission = reader.GetDecimal(reader.GetOrdinal("FixedCommission")),
                        ClassACommission = reader.IsDBNull(reader.GetOrdinal("ClassACommission")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("ClassACommission")),
                        ClassBCommission = reader.IsDBNull(reader.GetOrdinal("ClassBCommission")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("ClassBCommission")),
                        ClassCCommission = reader.IsDBNull(reader.GetOrdinal("ClassCCommission")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("ClassCCommission"))
                    });
                }
            }

            return rates;
        }

        private decimal GetClassCommission(int carClass, CommissionRate rate)
        {
            return carClass switch
            {
                1 => rate.ClassACommission ?? 0,
                2 => rate.ClassBCommission ?? 0,
                3 => rate.ClassCCommission ?? 0,
                _ => 0
            };
        }

        public async Task UpdateCommissionAsync(UpdateCommissionDto updateCommissionDto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("UPDATE Salesmen SET LastYearSales = @LastYearSales WHERE CId = @CId", connection);
                command.Parameters.AddWithValue("@LastYearSales", updateCommissionDto.LastYearSales);
                command.Parameters.AddWithValue("@CId", updateCommissionDto.CId);

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}