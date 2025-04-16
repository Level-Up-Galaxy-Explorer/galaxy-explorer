using galaxy_cli.Models;

namespace galaxy_cli.Services;

public interface IUserService
{
    Task<List<Users>> GetAllUsersAsync();
}