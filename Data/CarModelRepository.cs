using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using DemoAppAdo.Models;
using DemoAppAdo.DTOs;

namespace DemoAppAdo.Data
{
    public class CarModelRepository
    {
        private readonly string _connectionString;
        private readonly ClassRepository _classRepository;
        private readonly BrandRepository _brandRepository;

        public CarModelRepository(string connectionString, ClassRepository classRepository, BrandRepository brandRepository)
        {
            _connectionString = connectionString;
            _classRepository = classRepository;
            _brandRepository = brandRepository;
        }

        public async Task<IEnumerable<CarModel>> GetAllCarModels(string? search = null, string? orderBy = null)
        {
            var carModels = new List<CarModel>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("GetAllCarModels", connection) { CommandType = CommandType.StoredProcedure })
                    {
                        command.Parameters.AddWithValue("@Search", (object)search ?? DBNull.Value);
                        command.Parameters.AddWithValue("@OrderBy", (object)orderBy ?? DBNull.Value);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var carModel = new CarModel
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Brand = reader.GetInt32(reader.GetOrdinal("Brand")),
                                    Class = reader.GetInt32(reader.GetOrdinal("Class")),
                                    BrandName = await _brandRepository.GetBrandNameById(reader.GetInt32(reader.GetOrdinal("Brand"))),
                                    ClassName = await _classRepository.GetClassNameById(reader.GetInt32(reader.GetOrdinal("Class"))),
                                    ModelName = reader.GetString(reader.GetOrdinal("ModelName")),
                                    ModelCode = reader.GetString(reader.GetOrdinal("ModelCode")),
                                    Description = reader.GetString(reader.GetOrdinal("Description")),
                                    Features = reader.GetString(reader.GetOrdinal("Features")),
                                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                    DateOfManufacturing = reader.GetDateTime(reader.GetOrdinal("DateOfManufacturing")),
                                    Active = reader.GetBoolean(reader.GetOrdinal("Active")), // Directly retrieve as bool
                                    SortOrder = reader.GetInt32(reader.GetOrdinal("SortOrder")),
                                    ImageUrls = await GetCarModelImageUrls(reader.GetString(reader.GetOrdinal("ModelCode")))
                                };

                                carModels.Add(carModel);
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                
                Console.WriteLine($"SQL Error: {sqlEx.Message}");
                throw; 
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }

            return carModels;
        }



        public async Task CreateCarModel(CreateCarModelDto model)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("CreateCarModel", connection) { CommandType = CommandType.StoredProcedure })
                    {
                        command.Parameters.AddWithValue("@Brand", model.BrandId);
                        command.Parameters.AddWithValue("@Class", model.ClassId);
                        command.Parameters.AddWithValue("@ModelName", model.ModelName);
                        command.Parameters.AddWithValue("@ModelCode", model.ModelCode);
                        command.Parameters.AddWithValue("@Description", model.Description);
                        command.Parameters.AddWithValue("@Features", model.Features);
                        command.Parameters.AddWithValue("@Price", model.Price);
                        command.Parameters.AddWithValue("@DateOfManufacturing", model.DateOfManufacturing);
                        command.Parameters.AddWithValue("@SortOrder", model.SortOrder);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException sqlEx)
            {
               
                Console.WriteLine($"SQL Error: {sqlEx.Message}");
                throw; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }


        public async Task<CarModel> GetCarModelById(string modelCode)
        {
            CarModel carModel = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("GetCarModelById", connection) { CommandType = CommandType.StoredProcedure })
                {
                    command.Parameters.AddWithValue("@ModelCode", modelCode);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            carModel = new CarModel
                            {
                                Id = reader.GetInt32(0),
                                Brand = reader.GetInt32(1),
                                Class = reader.GetInt32(2),
                                BrandName = await _brandRepository.GetBrandNameById(reader.GetInt32(1)), 
                                ClassName = await _classRepository.GetClassNameById(reader.GetInt32(2)), 
                                ModelName = reader.GetString(3),
                                ModelCode = reader.GetString(4),
                                Description = reader.GetString(5),
                                Features = reader.GetString(6),
                                Price = reader.GetDecimal(7),
                                DateOfManufacturing = reader.GetDateTime(8),
                                Active = reader.GetBoolean(9),
                                SortOrder = reader.GetInt32(10)
                            };
                        }
                    }
                }
            }
            return carModel;
        }
        public async Task<List<string>> GetCarModelImageUrls(string carModelId)
        {
            var photoUrls = new List<string>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("GetCarModelImages", connection) { CommandType = CommandType.StoredProcedure })
                {
                    command.Parameters.AddWithValue("@CarModelId", carModelId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            photoUrls.Add(reader.GetString(0)); 
                        }
                    }
                }
            }
            return photoUrls; 
        }

        public async Task SaveImagePaths(string carModelId, IEnumerable<string> photoUrls)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                foreach (var photoUrl in photoUrls)
                {
                    using (var command = new SqlCommand("InsertCarModelImages", connection) { CommandType = CommandType.StoredProcedure })
                    {
                        command.Parameters.AddWithValue("@CarModelId", carModelId);
                        command.Parameters.AddWithValue("@PhotoUrl", photoUrl);
                        command.Parameters.AddWithValue("@CreatedBy", "System"); 
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
        }

    }
}
