namespace MyPersonalizedTodos.API.DTOs;

public class RegisterUserDTO
{
    // some properties are strings, because it allow do some valid checks by fluent validation.
    public string Login { get; init; }
    public string Password { get; init; }
    public string ConfirmPassword { get; init; }
    public int Age { get; init; }
    public DateTime DateOfBirth { get; init; }
    public string Gender { get; init; }
    public List<string> Purposes { get; init; }
    public string Nationality { get; init; }
    public string Description { get; init; }
}