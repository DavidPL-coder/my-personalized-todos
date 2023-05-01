using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPersonalizedTodos.API.Database;
using MyPersonalizedTodos.API.Database.Entities;
using MyPersonalizedTodos.API.DTOs;
using MyPersonalizedTodos.API.Services;
using System.Security.Claims;

namespace MyPersonalizedTodos.API.Controllers
{
    // TODO: Add authorization
    public class UsersController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly AppDbContext _context;
        private readonly IAuthorizedUserProvider _authorizedUserProvider;
        private readonly IUsersToDosService _usersToDosService;

        public UsersController(IMapper mapper, IPasswordHasher<User> passwordHasher, AppDbContext context, IAuthorizedUserProvider authorizedUserProvider, IUsersToDosService usersToDosService)
        {
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _context = context;
            _authorizedUserProvider = authorizedUserProvider;
            _usersToDosService = usersToDosService;
        }

        // [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.Users.Include(u => u.ToDos).Include(u => u.Settings).ToListAsync());
        }

        [HttpGet("{nameOrId}")]
        public async Task<IActionResult> GetByNameOrId([FromRoute] string nameOrId)
        {
            var user = int.TryParse(nameOrId, out int id) 
                ? await _context.Users.FindAsync(id) 
                : await _context.Users.FirstOrDefaultAsync(user => user.Name == nameOrId);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RegisterUserDTO dto)
        {
            var user = _mapper.Map<User>(dto);

            var passwordHash = _passwordHasher.HashPassword(user, dto.Password);
            user.PasswordHash = passwordHash;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var userToDelete = await _context.Users.FindAsync(id);
            if (userToDelete == null)
                return NotFound();

            _context.Users.Remove(userToDelete);
            await _context.SaveChangesAsync();

            return Ok();
        }

        /// ToDos actions:

        // [Authorize]
        [HttpGet("{name}/ToDos")]
        public async Task<IActionResult> GetToDos([FromRoute] string name)
        {
            var user = await _context.Users.Include(u => u.ToDos).FirstOrDefaultAsync(u => u.Name == name);
            return Ok(user.ToDos);
        }

        // TODO: Add validation of body data.
        [Authorize]
        [HttpPost("{username}/ToDos")]
        public async Task<IActionResult> CreateToDo([FromRoute] string username, [FromBody] CreateToDoDTO dto)
        {
            var user = await _authorizedUserProvider.GetAuthUser();
            if (user.Name != username)
                return Forbid();

            var toDo = _mapper.Map<ToDo>(dto);
            await _usersToDosService.AddToDo(user, toDo);

            return Ok();
        }

        [Authorize]
        [HttpDelete("{username}/ToDos/{todoTitle}")]
        public async Task<IActionResult> DeleteToDo([FromRoute] string username, [FromRoute] string todoTitle)
        {
            var user = await _authorizedUserProvider.GetAuthUser();
            if (user.Name != username)
                return Forbid();

            var toDo = user.ToDos.FirstOrDefault(t => t.Title == todoTitle);
            await _usersToDosService.DeleteToDo(user, toDo);

            return Ok();
        }

        [Authorize]
        [HttpPut("{username}/ToDos/{todoTitle}")]
        public async Task<IActionResult> UpdateToDo([FromRoute] string username, [FromRoute] string todoTitle, [FromBody] UpdateToDoDto dto)
        {
            var user = await _authorizedUserProvider.GetAuthUser();
            if (user.Name != username)
                return Forbid();

            var toDoIndex = user.ToDos.FindIndex(t => t.Title == todoTitle);
            if (toDoIndex == -1)
                return NotFound();

            var toDo = _mapper.Map<ToDo>(dto);
            toDo.Id = user.ToDos.First(t => t.Title == todoTitle).Id;
            await _usersToDosService.UpdateToDo(user, toDoIndex, toDo);

            return Ok();
        }

        /// Settings actions:

        [HttpGet("{username}/Settings")]
        public async Task<IActionResult> GetSettings([FromRoute] string username)
        {
            var user = await _context.Users.Include(u => u.Settings).FirstOrDefaultAsync(u => u.Name == username);
            return Ok(user.Settings);
        }

        [Authorize]
        [HttpPut("{username}/Settings")]
        public async Task<IActionResult> UpdateSettings([FromRoute] string username, [FromBody] UpdateUserSettingsDto dto)
        {
            var user = await _authorizedUserProvider.GetAuthUser();
            if (user.Name != username)
                return Forbid();

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
