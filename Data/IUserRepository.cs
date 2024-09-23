using DemoAppAdo.Models;

namespace DemoAppAdo.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<Role>> GetRolesAsync();
        Task CreateUserAsync(User user);
        Task<User> GetUserByIdAsync(int userId);
        Task<IEnumerable<User>> GetAllUsersAsync();

    }
}