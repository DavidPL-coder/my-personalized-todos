using Microsoft.AspNetCore.Identity;
using MyPersonalizedTodos.API.Database.Entities;
using MyPersonalizedTodos.API.Enums;

namespace MyPersonalizedTodos.API.Database
{
    public class DbSeeder
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly AppConfig _appConfig;
        private readonly ILogger<DbSeeder> _logger;

        public DbSeeder(AppDbContext context, IPasswordHasher<User> passwordHasher, AppConfig appConfig, ILogger<DbSeeder> logger)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _appConfig = appConfig;
            _logger = logger;
        }

        public void Seed() 
        { 
            SeedRoles();
            SeedAdmins();
        }

        private void SeedRoles()
        {
            if (!_context.Roles.Any())
            {
                _context.Roles.AddRange(new[]
                { 
                    new Role { UserRole = UserRole.User },
                    new Role { UserRole = UserRole.Admin }
                });
                _context.SaveChanges();
                _logger.LogInformation("# Database created all user roles succesfully.");
            }
        }

        // TODO: Add possibilty to create many admins.
        private void SeedAdmins()
        {
            if (_context.Users.Any(u => u.Name == _appConfig.MPT_ADMIN_LOGIN))
            {
                _logger.LogInformation("# Database just has an account '{name}'. Use other login if you want to create an another admin account.", _appConfig.MPT_ADMIN_LOGIN);
                return;
            }

            var adminUser = GetAdmin();
            _context.Users.Add(adminUser);
            _context.SaveChanges();
            _logger.LogInformation("# Database created an admin account ({name}) succesfully.", adminUser.Name);
        }

        private User GetAdmin()
        {
            var adminRole = _context.Roles.First(r => r.UserRole == UserRole.Admin);
            var adminUser = new User // TODO: Some of this data should be optional to provide.
            {
                Name = _appConfig.MPT_ADMIN_LOGIN,
                Role = adminRole,

                Age = 21,
                DateOfBirth = DateTime.Now,
                Gender = UserGender.Male,
                Nationality = UserNationality.Poland,
                Purposes = new() { Purpose.Other },
                Settings = new(),
            };

            adminUser.PasswordHash = _passwordHasher.HashPassword(adminUser, _appConfig.MPT_ADMIN_PASSWORD);
            return adminUser;
        }
    }
}
