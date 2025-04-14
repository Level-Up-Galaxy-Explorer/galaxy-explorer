using System.Security.Claims;
using galaxy_api.Models;
using galaxy_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace galaxy_api.Controllers
{

    [ApiController]
    [Route("api/auth/google")]
    public class GoogleAuthController : ControllerBase
    {
        private readonly IGoogleAuthProvider _googleAuthProvider;
        private readonly IUserService _userService;

        public GoogleAuthController(IGoogleAuthProvider googleAuthProvider, IUserService userService)
        {
            _googleAuthProvider = googleAuthProvider;
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate([FromBody] GoogleAuthRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var idToken = await _googleAuthProvider.ExchangeCodeForIdTokenAsync(request.Code, request.CodeVerifier, request.RedirectUri);

            if (idToken == null)
            {
                return BadRequest("Failed to exchange code or retrieve ID token from Google.");
            }

            var principal = await _googleAuthProvider.ValidateGoogleIdTokenAsync(idToken);
            if (principal == null)
            {
                return Unauthorized("Invalid Google ID token.");
            }

            string? userGoogleId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            Users? user = await _userService.GetUserByGoogleIdAsync(userGoogleId);

            if (user == null)
            {
                user = new Users
                {
                    Full_Name = $"{principal.FindFirstValue(ClaimTypes.GivenName)} {principal.FindFirstValue(ClaimTypes.Surname)}",
                    Email_Address = principal.FindFirstValue(ClaimTypes.Email) ?? "",
                    Google_Id = userGoogleId,
                    Rank_Id = 5,
                    Is_Active = true,
                    Created_At = DateTime.Now
                };
                await _userService.AddUserAsync(user);
            }
            else
            {
                // user exists
            }

            return Ok(new { token = getJTW(principal, idToken) });
        }

        public string getJTW(ClaimsPrincipal principal, string idToken)
        {
            // Create our own JWT with custom claims?
            // Return google token for now.
            return idToken;
        }
    }
}