using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using MyPersonalizedTodos.API.Database;
using System.Security.Claims;

namespace MyPersonalizedTodos.API.Authorization
{
    public class AdminAuthorizeAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var loggedUserId = context.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (loggedUserId is null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var dbContext = context.HttpContext.RequestServices.GetService<AppDbContext>();
            var loggedUser = await dbContext.Users.Include(u => u.Role).FirstAsync(user => user.Id.ToString() == loggedUserId);
            if (!loggedUser.IsAdmin()) 
            {
                context.Result = new ForbidResult();
                var appLogger = context.HttpContext.RequestServices.GetService<ILogger<AdminAuthorizeAttribute>>();
                appLogger.LogWarning("# Server returns 403 for '{username}' user (id: {userId}) in endpoint: {path}/{query} (only admin access)",
                    loggedUser.Name, loggedUserId, context.HttpContext.Request.Path, context.HttpContext.Request.QueryString);
                return;
            }
        }
    }
}
