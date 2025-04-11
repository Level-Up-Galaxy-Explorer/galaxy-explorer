using galaxy_api.Models;

namespace galaxy_api.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<Users>> GetAllUserAsync();
        Task<Users?> GetUserByIdAsync(int id);
        Task AddUserAsync(Users users);
        Task UpdateUserDetailsAsync(int id, Users users);
        Task AssignUserAsync(int id, Users users);
        Task DeactivateUserAsync(int id, Users users);

    }
}