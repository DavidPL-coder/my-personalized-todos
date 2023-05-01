using AutoMapper;
using MyPersonalizedTodos.API.Database.Entities;
using MyPersonalizedTodos.API.Enums;

namespace MyPersonalizedTodos.API.DTOs
{
    [AutoMap(typeof(UserSettings), ReverseMap = true)]
    public class UpdateUserSettingsDto
    {
        public bool Italic { get; init; }
        public bool Bold { get; init; }
        public bool Uppercase { get; init; }
        public string TextColor { get; init; }
        public string BackgroundColor { get; init; }
        public string HeaderColor { get; init; }
        public FontSize FontSize { get; init; }
    }
}
