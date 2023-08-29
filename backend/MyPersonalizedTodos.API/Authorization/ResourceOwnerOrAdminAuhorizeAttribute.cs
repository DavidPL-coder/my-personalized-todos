using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using MyPersonalizedTodos.API.Database;
using Microsoft.AspNetCore.Mvc;
using MyPersonalizedTodos.API.Database.Entities;

namespace MyPersonalizedTodos.API.Authorization
{
    public class ResourceOwnerOrAdminAuhorizeAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
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
            var userWithGivenUsername = await GetUserFromQuery(context, dbContext);

            if (userWithGivenUsername is null)
            {
                // TODO: Let select how to do in this case.
                context.Result = new BadRequestObjectResult(new { message = "The given user who was specified in url doesn't exist." });
                return;
            }

            var ownerId = userWithGivenUsername.Id.ToString();
            var loggedUser = await dbContext.Users.Include(u => u.Role).FirstAsync(user => user.Id.ToString() == loggedUserId);
            if (loggedUserId != ownerId && !loggedUser.IsAdmin)
            {
                context.Result = new ForbidResult();
                var appLogger = context.HttpContext.RequestServices.GetService<ILogger<ResourceOwnerOrAdminAuhorizeAttribute>>();
                appLogger.LogWarning("# Server returns 403 for '{username}' user (id: {userId}) in endpoint: {path}/{query} (only resource owner and admin access)", 
                    loggedUser.Name, loggedUserId, context.HttpContext.Request.Path, context.HttpContext.Request.QueryString);
                return;
            }
        }

        private async static Task<User> GetUserFromQuery(AuthorizationFilterContext context, AppDbContext dbContext)
        {
            string usernameOrId = null;
            if (context.HttpContext.Request.RouteValues.TryGetValue("username", out var usernameFromQuery))
                usernameOrId = usernameFromQuery.ToString();
            else
                usernameOrId = context.HttpContext.Request.RouteValues["usernameOrId"].ToString();

            var user = int.TryParse(usernameOrId, out int id)
                ? await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id)
                : await dbContext.Users.FirstOrDefaultAsync(u => u.Name == usernameOrId);

            return user;
        }
    }
}
