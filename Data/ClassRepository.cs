using DemoAppAdo.Models;
using System.Data.SqlClient;

namespace DemoAppAdo.Data
{
    public class ClassRepository
    {
        private readonly string _connectionString;

        public ClassRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Class>> GetAllClasses()
        {
            var classes = new List<Class>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT * FROM Classes WHERE Status = 'Active'", connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        classes.Add(new Class
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            CreatedOn = reader.GetDateTime(2),
                            UpdatedOn = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                            CreatedBy = reader.GetString(4),
                            UpdatedBy = reader.IsDBNull(5) ? null : reader.GetString(5),
                            Status = reader.GetString(6)
                        });
                    }
                }
            }
            return classes;
        }
        public async Task<string> GetClassNameById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT Name FROM Classes WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                var result = await command.ExecuteScalarAsync();
                return result?.ToString(); 
            }
        }


        public async Task<Class> GetClassById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT * FROM Classes WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Class
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            CreatedOn = reader.GetDateTime(2),
                            UpdatedOn = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                            CreatedBy = reader.GetString(4),
                            UpdatedBy = reader.IsDBNull(5) ? null : reader.GetString(5),
                            Status = reader.GetString(6)
                        };
                    }
                    return null;
                }
            }
        }

        public async Task AddClass(Class carClass)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("INSERT INTO Classes (Name, CreatedOn, CreatedBy, Status) VALUES (@Name, @CreatedOn, @CreatedBy, @Status)", connection);
                command.Parameters.AddWithValue("@Name", carClass.Name);
                command.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                command.Parameters.AddWithValue("@CreatedBy", carClass.CreatedBy);
                command.Parameters.AddWithValue("@Status", carClass.Status);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateClass(Class carClass)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("UPDATE Classes SET Name = @Name, UpdatedOn = @UpdatedOn, UpdatedBy = @UpdatedBy WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Name", carClass.Name);
                command.Parameters.AddWithValue("@UpdatedOn", DateTime.Now);
                command.Parameters.AddWithValue("@UpdatedBy", carClass.UpdatedBy);
                command.Parameters.AddWithValue("@Id", carClass.Id);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteClass(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("UPDATE Classes SET Status = 'Inactive' WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                await command.ExecuteNonQueryAsync();
            }
        }
    }

}
