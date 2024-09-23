using Microsoft.AspNetCore.Mvc;
using DemoAppAdo.DTOs;
using DemoAppAdo.Models;
using DemoAppAdo.Repositories;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using DemoAppAdo.Data;
using DemoAppAdo.DTOs.DemoAppAdo.DTOs;

namespace DemoAppAdo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly SalesRecordRepository _salesmanRepository;

        public UsersController(IUserRepository userRepository, SalesRecordRepository salesmanRepository)
        {
            _userRepository = userRepository;
            _salesmanRepository = salesmanRepository; 
        }

        [HttpGet("roles")] 
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var roles = await _userRepository.GetRolesAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userRepository.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/users/create
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!IsStrongPassword(createUserDto.PasswordHash))
            {
                return BadRequest("Password must be at least 8 characters long, contain upper and lower case letters, a number, and a special character.");
            }

            string hashedPassword = HashPassword(createUserDto.PasswordHash);
            string newCid = DateTime.Now.ToString("ddMMyyHHmmssfff");

            var user = new User
            {
                Username = createUserDto.Username,
                PasswordHash = hashedPassword,
                Email = createUserDto.Email,
                RoleId = createUserDto.RoleId,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "System",
                UpdatedOn = DateTime.UtcNow,
                UpdatedBy = "System",
                CId = newCid
            };

            try
            {
                await _userRepository.CreateUserAsync(user);

                // Check if the role is 2 (Salesman)
                if (user.RoleId == 2)
                {
                    var salesman = new Salesman
                    {
                        CId = user.CId,//copany Id For New user
                        Name = user.Username // or another property if applicable
                    };
                    await _salesmanRepository.InsertSalesmanAsync(salesman);
                }
                return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private bool IsStrongPassword(string password)
        {
            if (password.Length < 8) return false;

            bool hasUpperCase = false, hasLowerCase = false, hasDigit = false, hasSpecialChar = false;

            foreach (var ch in password)
            {
                if (char.IsUpper(ch)) hasUpperCase = true;
                else if (char.IsLower(ch)) hasLowerCase = true;
                else if (char.IsDigit(ch)) hasDigit = true;
                else if (!char.IsLetterOrDigit(ch)) hasSpecialChar = true;
            }

            return hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;
        }
    }
}
