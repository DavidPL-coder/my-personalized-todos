using MyPersonalizedTodos.API.Enums;

namespace MyPersonalizedTodos.API.Database.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public UserRole UserRole { get; set; }
    }
}
