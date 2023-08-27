namespace MyPersonalizedTodos.API.Database.Entities;

public class ToDo
{
    public int Id { get; set; }
    public string Title { get; init; }
    public string Description { get; init; }
    public DateTime? TaskStart { get; init; }
    public DateTime? TaskEnd { get; init; }
    public User User { get; init; }
}
