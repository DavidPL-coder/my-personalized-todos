using Microsoft.EntityFrameworkCore;
using MyPersonalizedTodos.API.Database;
using MyPersonalizedTodos.API.Database.Entities;

namespace MyPersonalizedTodos.API.Services
{
    public interface IUsersToDosService
    {
        Task AddToDo(User user, ToDo toDo);
        Task DeleteToDo(ToDo toDo);
        Task UpdateToDo(User user, int toDoIndex, ToDo toDo);
    }

    public class UsersToDosService : IUsersToDosService
    {
        private readonly AppDbContext _context;

        public UsersToDosService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddToDo(User user, ToDo toDo)
        {
            user.ToDos.Add(toDo);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteToDo(ToDo toDo) 
        {
            _context.ToDos.Remove(toDo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateToDo(User user, int toDoIndex, ToDo toDo)
        {
            _context.Entry(user.ToDos[toDoIndex]).State = EntityState.Detached;
            user.ToDos[toDoIndex] = toDo;
            _context.Entry(toDo).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }
    }
}
