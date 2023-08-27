using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyPersonalizedTodos.API.Database.Entities;
using MyPersonalizedTodos.API.Enums;
using System.Text.Json;

namespace MyPersonalizedTodos.API.Database;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; init; }
    public DbSet<ToDo> ToDos { get; init; }
    public DbSet<UserSettings> UsersSettings { get; init; }
    public DbSet<Role> Roles { get; init; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().Property(user => user.Name).IsRequired();
        modelBuilder.Entity<User>().HasIndex(user => user.Name).IsUnique();
        modelBuilder.Entity<User>().Property(user => user.PasswordHash).IsRequired();
        modelBuilder.Entity<User>().Property(user => user.Purposes).IsRequired();

        modelBuilder.Entity<User>().Property(user => user.Purposes)
            .HasConversion(
                value => JsonSerializer.Serialize(value, (JsonSerializerOptions)null),
                value => JsonSerializer.Deserialize<List<Purpose>>(value, (JsonSerializerOptions)null)
                );

        modelBuilder.Entity<ToDo>().Property(todo => todo.Title).IsRequired();

        modelBuilder.Entity<UserSettings>().Property(settings => settings.TextColor).IsRequired();
        modelBuilder.Entity<UserSettings>().Property(settings => settings.BackgroundColor).IsRequired();
        modelBuilder.Entity<UserSettings>().Property(settings => settings.HeaderColor).IsRequired();

        modelBuilder.Entity<Role>().HasIndex(role => role.UserRole).IsUnique();
    }
}
