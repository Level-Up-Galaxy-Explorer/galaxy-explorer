using galaxy_api.Models;
using galaxy_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace galaxy_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Users>>> GetAllUsers()
        {
            var users = await _userService.GetAllUserAsync();
            return Ok(users);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Users>> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult> AddUser([FromBody] Users user)
        {
            user.Is_Active = true;
            user.Created_At = DateTime.Now;

            await _userService.AddUserAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.User_Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUserDetails(int id, [FromBody] Users user)
        {
            var existing = await _userService.GetUserByIdAsync(id);
            if (existing == null)
                return NotFound();

            await _userService.UpdateUserDetailsAsync(id, user);
           return NoContent();
        }

        [HttpPatch("{id}/assign")]
        public async Task<ActionResult> AssignUser(int id, [FromBody] Users user)
        {
            var existing = await _userService.GetUserByIdAsync(id);
            if (existing == null)
                return NotFound();

            await _userService.AssignUserAsync(id, user);
            return NoContent();
        }

        [HttpPatch("{id}/deactivate")]
        public async Task<ActionResult> DeactivateUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            user.Is_Active = false;
            await _userService.DeactivateUserAsync(id, user);
            return NoContent();
        }
    }
}
