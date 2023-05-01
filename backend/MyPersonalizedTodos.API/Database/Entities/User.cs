using MyPersonalizedTodos.API.Enums;

namespace MyPersonalizedTodos.API.Database.Entities;

// TODO: change Id type to Guid
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string PasswordHash { get; set; }
    public int Age { get; set; }
    public DateTime DateOfBirth { get; set; }
    public UserGender Gender { get; set; }
    public List<Purpose> Purposes { get; set; }
    public UserNationality Nationality { get; set; }
    public string Description { get; set; }
    public List<ToDo> ToDos { get; set; }
    public UserSettings Settings { get; set; }
}