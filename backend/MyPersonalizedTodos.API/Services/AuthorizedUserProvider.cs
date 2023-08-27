using Microsoft.EntityFrameworkCore;
using MyPersonalizedTodos.API.Database;
using MyPersonalizedTodos.API.Database.Entities;
using System.Security.Claims;

namespace MyPersonalizedTodos.API.Services
{
    public interface IAuthorizedUserProvider
    {
        Task<User> GetAuthUser(bool mustIncludeRelatedData = true);
    }

    public class AuthorizedUserProvider : IAuthorizedUserProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly AppDbContext _dbContext;

        public AuthorizedUserProvider(IHttpContextAccessor contextAccessor, AppDbContext dbContext)
        {
            _contextAccessor = contextAccessor;
            _dbContext = dbContext;
        }

        // TODO: Let select related data to load.
        public async Task<User> GetAuthUser(bool mustIncludeRelatedData = true)
        {
            IQueryable<User> users = mustIncludeRelatedData 
                ? _dbContext.Users.Include(u => u.ToDos).Include(u => u.Settings).Include(u => u.Role)
                : _dbContext.Users;

            var id = int.Parse(_contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            return await users.FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
