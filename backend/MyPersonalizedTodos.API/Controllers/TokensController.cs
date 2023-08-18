using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyPersonalizedTodos.API.Database;
using MyPersonalizedTodos.API.Database.Entities;
using MyPersonalizedTodos.API.DTOs;
using MyPersonalizedTodos.API.Services;

namespace MyPersonalizedTodos.API.Controllers
{
    // TODO: Let user decide where save token (cookie or nowhere)
    public class TokensController : BaseApiController
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ITokensService _tokensService;

        public TokensController(AppDbContext context, IPasswordHasher<User> passwordHasher, ITokensService tokensService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _tokensService = tokensService;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginDTO dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Name == dto.Login);
            if (user == null)
                return BadRequest("Invalid login or password");

            var checkingResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (checkingResult == PasswordVerificationResult.Failed)
                return BadRequest("Invalid login or password");

            var token = _tokensService.GenerateJwtToken(user);
            _tokensService.SaveTokenToCookie(token);

            return Ok();
        }

        [Authorize]
        [HttpDelete] // TODO: maybe using post would be a great idea
        public IActionResult Logout() 
        {
            _tokensService.DeleteCookieWithToken();
            return Ok();
        }
    }
}
