using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPersonalizedTodos.API.Database;
using MyPersonalizedTodos.API.Enums;
using MyPersonalizedTodos.API.Services;
using System.Security.Claims;

namespace MyPersonalizedTodos.API.Controllers
{
    [Authorize]
    public class AccountController : BaseApiController
    {
        private readonly IAuthorizedUserProvider _authorizedUserProvider;
        private readonly AppDbContext _context;

        public AccountController(IAuthorizedUserProvider authorizedUserProvider, AppDbContext context)
        {
            _authorizedUserProvider = authorizedUserProvider;
            _context = context;
        }

        [HttpGet("username")]
        public async Task<IActionResult> GetAuthorizedUserName()
        {
            var user = await _authorizedUserProvider.GetAuthUser(mustIncludeRelatedData: false);
            return Ok(new { name = user.Name });
        }

        [HttpGet("role")]
        public async Task<IActionResult> GetAuthorizedUserRole()
        {
            var user = await _authorizedUserProvider.GetAuthUser();
            var role = Enum.GetName(user.Role.UserRole);
            return Ok(new { role });
        }

        [AllowAnonymous]
        [HttpGet("exist-state/{username}")]
        public IActionResult CheckIfAccountExist(string username)
        {
            if (!_context.Users.Any(u => u.Name == username))
                return NotFound(new { message = "An account with the given username doesn't exist." });

            return Ok();
        }
    }
}
