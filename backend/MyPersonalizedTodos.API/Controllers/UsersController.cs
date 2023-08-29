using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPersonalizedTodos.API.Authorization;
using MyPersonalizedTodos.API.Database;
using MyPersonalizedTodos.API.Database.Entities;
using MyPersonalizedTodos.API.DTOs;
using MyPersonalizedTodos.API.Enums;
using MyPersonalizedTodos.API.Services;
using System.Security.Claims;

namespace MyPersonalizedTodos.API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly AppDbContext _context;
        private readonly IAuthorizedUserProvider _authorizedUserProvider;
        private readonly IUsersToDosService _usersToDosService;
        private readonly ITokensService _tokensService;

        public UsersController(IMapper mapper, IPasswordHasher<User> passwordHasher, AppDbContext context, IAuthorizedUserProvider authorizedUserProvider, IUsersToDosService usersToDosService, ITokensService tokensService)
        {
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _context = context;
            _authorizedUserProvider = authorizedUserProvider;
            _usersToDosService = usersToDosService;
            _tokensService = tokensService;
        }

        [AdminAuthorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {   
            var users = await _context.Users.AsNoTracking().ToListAsync();
            return Ok(users);
        }

        [ResourceOwnerOrAdminAuhorize]
        [HttpGet("{usernameOrId}")]
        public async Task<IActionResult> GetByNameOrId([FromRoute] string usernameOrId)
        {
            var getUsersQuery = _context.Users.AsNoTracking()
                .Include(u => u.ToDos)
                .Include(u => u.Settings)
                .Include(u => u.Role);

            var user = int.TryParse(usernameOrId, out int id) 
                ? await getUsersQuery.FirstAsync(user => user.Id == id) 
                : await getUsersQuery.FirstAsync(user => user.Name == usernameOrId);

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] RegisterUserDTO dto)
        {
            var user = _mapper.Map<User>(dto);
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
            user.Role = await _context.Roles.FirstAsync(role => role.UserRole == UserRole.User);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [AdminAuthorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            var userToDelete = await _context.Users.FirstAsync(u => u.Id == id);
            if (userToDelete is null)
                return NotFound();

            // TODO: Logout deleting user even if it is not logged user.
            var loggedUser = await _authorizedUserProvider.GetAuthUser();
            if (loggedUser.Id == userToDelete.Id)
                _tokensService.DeleteCookieWithToken();

            _context.Users.Remove(userToDelete);
            await _context.SaveChangesAsync();

            return Ok();
        }

        /// ToDos actions:

        [ResourceOwnerOrAdminAuhorize]
        [HttpGet("{username}/ToDos")]
        public async Task<IActionResult> GetToDos([FromRoute] string username)
        {
            var toDos = await _context.ToDos.AsNoTracking().Include(todo => todo.User)
                .Where(todo => todo.User.Name == username)
                .ToListAsync();

            return Ok(toDos);
        }

        // TODO: Add validation of body data.
        [ResourceOwnerOrAdminAuhorize]
        [HttpPost("{username}/ToDos")]
        public async Task<IActionResult> CreateToDo([FromRoute] string username, [FromBody] CreateToDoDTO dto)
        {
            var toDo = _mapper.Map<ToDo>(dto);
            var user = await _context.Users
                .Include(u => u.ToDos)
                .FirstAsync(u => u.Name == username);

            await _usersToDosService.AddToDo(user, toDo);
            return Ok();
        }

        [ResourceOwnerOrAdminAuhorize]
        [HttpDelete("{username}/ToDos/{todoTitle}")]
        public async Task<IActionResult> DeleteToDo([FromRoute] string username, [FromRoute] string todoTitle)
        {
            var toDo = await _context.ToDos.Include(t => t.User)
                .FirstAsync(t => t.Title == todoTitle && t.User.Name == username);

            await _usersToDosService.DeleteToDo(toDo);
            return Ok();
        }

        // TODO: Refactor it.
        [ResourceOwnerOrAdminAuhorize]
        [HttpPut("{username}/ToDos/{todoTitle}")]
        public async Task<IActionResult> UpdateToDo([FromRoute] string username, [FromRoute] string todoTitle, [FromBody] UpdateToDoDto dto)
        {
            var user = await _context.Users.Include(u => u.ToDos).FirstAsync(u => u.Name == username);

            var toDoIndex = user.ToDos.FindIndex(t => t.Title == todoTitle);
            if (toDoIndex == -1)
                return NotFound();

            var toDo = _mapper.Map<ToDo>(dto);
            toDo.Id = user.ToDos[toDoIndex].Id;
            await _usersToDosService.UpdateToDo(user, toDoIndex, toDo);

            return Ok();
        }

        /// Settings actions:

        [ResourceOwnerOrAdminAuhorize]
        [HttpGet("{username}/Settings")]
        public async Task<IActionResult> GetSettings([FromRoute] string username)
        {
            var settings = await _context.UsersSettings.AsNoTracking().Include(s => s.User)
                .FirstAsync(s => s.User.Name == username);

            return Ok(settings);
        }

        // TODO: Refactor it.
        [ResourceOwnerOrAdminAuhorize]
        [HttpPut("{username}/Settings")]
        public async Task<IActionResult> UpdateSettings([FromRoute] string username, [FromBody] UpdateUserSettingsDto dto)
        {
            var user = await _context.Users.Include(u => u.Settings).FirstAsync(u => u.Name == username);
            var settings = _mapper.Map<UserSettings>(dto);
            settings.Id = user.Settings.Id;

            _context.Entry(user.Settings).State = EntityState.Detached;
            user.Settings = settings;
            _context.Entry(settings).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
