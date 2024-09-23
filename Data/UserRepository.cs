using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DemoAppAdo.Models;

namespace DemoAppAdo.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var users = new List<User>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT * FROM Users", connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var user = new User
                        {
                            UserId = (int)reader["UserId"],
                            Username = (string)reader["Username"],
                            Email = (string)reader["Email"],
                            RoleId = (int)reader["RoleId"],
                            CId= (string)reader["CId"],
                            // Populate other properties as necessary
                        };
                        users.Add(user);
                    }
                }
            }

            return users;
        }

        public async Task<IEnumerable<Role>> GetRolesAsync()
        {
            var roles = new List<Role>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand("SELECT RoleId, RoleName FROM RoleMaster", connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            roles.Add(new Role
                            {
                                RoleId = (int)reader["RoleId"],
                                RoleName = reader["RoleName"].ToString(),
                            });
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Log SQL exceptions
                Console.WriteLine($"SQL Error: {sqlEx.Message}");
                throw; // Re-throw the exception for outer handling
            }
            catch (Exception ex)
            {
                // Log general exceptions
                Console.WriteLine($"Error: {ex.Message}");
                throw; // Re-throw the exception for outer handling
            }
            return roles;
        }


        public async Task CreateUserAsync(User user)
        {
            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using (var command = new SqlCommand("InsertUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@RoleId", user.RoleId);
                    command.Parameters.AddWithValue("@CreatedOn", user.CreatedOn);
                    command.Parameters.AddWithValue("@CreatedBy", user.CreatedBy);
                    command.Parameters.AddWithValue("@UpdatedOn", user.UpdatedOn);
                    command.Parameters.AddWithValue("@UpdatedBy", user.UpdatedBy);
                    command.Parameters.AddWithValue("@CId", user.CId);

                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (SqlException ex)
            {
                // Log the error (consider using a logging framework)
                throw new Exception("An error occurred while creating the user.", ex);
            }
            finally
            {
                if (connection != null)
                {
                    await connection.CloseAsync();
                }
            }
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            User user = null;
            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using (var command = new SqlCommand("GetUserById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            user = new User
                            {
                                UserId = (int)reader["UserId"],
                                Username = (string)reader["Username"],
                                Email = (string)reader["Email"],
                                RoleId = (int)reader["RoleId"],
                                CreatedOn = (DateTime)reader["CreatedOn"],
                                UpdatedOn = (DateTime)reader["UpdatedOn"],
                                CId = (string)reader["CId"]
                            };
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log the error (consider using a logging framework)
                throw new Exception("An error occurred while retrieving the user.", ex);
            }
            finally
            {
                if (connection != null)
                {
                    await connection.CloseAsync();
                }
            }

            return user;
        }
    }
}
