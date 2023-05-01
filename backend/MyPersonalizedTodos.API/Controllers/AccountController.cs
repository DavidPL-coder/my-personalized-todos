using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPersonalizedTodos.API.Database;
using MyPersonalizedTodos.API.Services;
using System.Security.Claims;

namespace MyPersonalizedTodos.API.Controllers
{
    [Authorize]
    public class AccountController : BaseApiController
    {
        private readonly IAuthorizedUserProvider _authorizedUserProvider;

        public AccountController(IAuthorizedUserProvider authorizedUserProvider)
        {
            _authorizedUserProvider = authorizedUserProvider;
        }

        [HttpGet("username")]
        public async Task<IActionResult> GetAuthorizedUserName()
        {
            var user = await _authorizedUserProvider.GetAuthUser(mustIncludeRelatedData: false);
            return Ok(new { name = user.Name });
        }
    }
}
