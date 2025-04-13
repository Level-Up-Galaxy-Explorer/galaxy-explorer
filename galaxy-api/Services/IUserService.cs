using galaxy_api.Models;

namespace galaxy_api.Services
{
    public interface IUserService
    {
        Task<IEnumerable<Users>> GetAllUserAsync();
        Task<Users?> GetUserByIdAsync(int id);
        Task<Users?> GetUserByGoogleIdAsync(string id);
        Task AddUserAsync(Users users);
        Task UpdateUserDetailsAsync(int id, Users users);
        Task AssignUserAsync(int id, Users users);
        Task DeactivateUserAsync(int id, Users users);

    }
}