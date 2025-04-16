using galaxy_cli.Models;

using galaxy_cli.Services;

using galaxy_cli.DTO;

namespace galaxy_cli.Services;

public interface IUserService
{
    Task<IEnumerable<UserDTO>> GetUsersAsync();
    Task<UserDTO?> GetUserByIdAsync(int id);
    Task<bool> AssignRankAsync(int userId, int rankId);
    Task<bool> DeactivateUserAsync(int userId);
    Task<bool> UpdateUserAsync(int id, UserDTO user);
}
