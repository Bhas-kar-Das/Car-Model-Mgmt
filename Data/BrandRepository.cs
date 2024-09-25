using DemoAppAdo.Models;
using System.Data.SqlClient;

namespace DemoAppAdo.Data
{
    public class BrandRepository
    {
        private readonly string _connectionString;

        public BrandRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Brand>> GetAllBrands()
        {
            var brands = new List<Brand>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT * FROM Brands WHERE Status = 1", connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            brands.Add(new Brand
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),                              // Access by column name
                                Name = reader.GetString(reader.GetOrdinal("Name")),                          // Access by column name
                                CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn")),              // Access by column name
                                UpdatedOn = reader.IsDBNull(reader.GetOrdinal("UpdatedOn"))
                                    ? (DateTime?)null
                                    : reader.GetDateTime(reader.GetOrdinal("UpdatedOn")),                   // Access by column name
                                CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy")),                // Access by column name
                                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("UpdatedBy")),                      // Access by column name
                                Status = reader.GetBoolean(reader.GetOrdinal("Status"))                      // Access by column name
                            });
                        }

                    }
                }
            }
            return brands;
        }

        public async Task<Brand> GetBrandById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT * FROM Brands WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Brand
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            CreatedOn = reader.GetDateTime(2),
                            UpdatedOn = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                            CreatedBy = reader.GetString(4),
                            UpdatedBy = reader.IsDBNull(5) ? null : reader.GetString(5),
                            Status = reader.GetBoolean(6)
                        };
                    }
                    return null;
                }
            }
        }

        public async Task AddBrand(Brand brand)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("INSERT INTO Brands (Name, CreatedOn, CreatedBy, Status) VALUES (@Name, @CreatedOn, @CreatedBy, @Status)", connection);
                command.Parameters.AddWithValue("@Name", brand.Name);
                command.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                command.Parameters.AddWithValue("@CreatedBy", brand.CreatedBy);
                command.Parameters.AddWithValue("@Status", brand.Status);
                await command.ExecuteNonQueryAsync();
            }
        }
        public async Task<string> GetBrandNameById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT Name FROM Brands WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                var result = await command.ExecuteScalarAsync();
                return result?.ToString(); // Return the name or null if not found
            }
        }

        public async Task UpdateBrand(Brand brand)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("UPDATE Brands SET Name = @Name, UpdatedOn = @UpdatedOn, UpdatedBy = @UpdatedBy WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Name", brand.Name);
                command.Parameters.AddWithValue("@UpdatedOn", DateTime.Now);
                command.Parameters.AddWithValue("@UpdatedBy", brand.UpdatedBy);
                command.Parameters.AddWithValue("@Id", brand.Id);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteBrand(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("UPDATE Brands SET Status = 0 WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                await command.ExecuteNonQueryAsync();
            }
        }
    }

}
