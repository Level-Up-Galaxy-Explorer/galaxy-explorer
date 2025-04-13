using galaxy_api.Models;
using galaxy_api.Repositories;

namespace galaxy_api.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Users>> GetAllUserAsync()
        {
            var missions = await _repository.GetAllUserAsync();

            return missions.OrderBy(u => u.User_Id);
        }

        public async Task<Users?> GetUserByIdAsync(int id)
        {
            return await _repository.GetUserByIdAsync(id);
        }

        public async Task AddUserAsync(Users users)
        {
            await _repository.AddUserAsync(users);
        }

        public async Task UpdateUserDetailsAsync(int id, Users users)
        {
            await _repository.UpdateUserDetailsAsync(id,users);
        }

        public async Task AssignUserAsync(int id, Users users)
        {
            await _repository.AssignUserAsync(id, users);
        }

        public async Task DeactivateUserAsync(int id, Users users)
        {
            await _repository.DeactivateUserAsync(id, users);
        }

    }
}
