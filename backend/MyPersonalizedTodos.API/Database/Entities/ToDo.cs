namespace MyPersonalizedTodos.API.Database.Entities;

public class ToDo
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime? TaskStart { get; set; }
    public DateTime? TaskEnd { get; set; }
}
