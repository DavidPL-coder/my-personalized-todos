using MyPersonalizedTodos.API.Enums;

namespace MyPersonalizedTodos.API.Database.Entities
{
    public class UserSettings
    {
        public int Id { get; set; }
        public bool Italic { get; init; }
        public bool Bold { get; init; }
        public bool Uppercase { get; init; }
        public string TextColor { get; init; }
        public string BackgroundColor { get; init; }
        public string HeaderColor { get; init; }
        public FontSize FontSize { get; init; }

        public UserSettings()
        {
            // TODO: Take it from config
            Italic = false;
            Bold = false;
            Uppercase = false;
            TextColor = "#ffffff";
            BackgroundColor = "#222930";
            HeaderColor = "#3199e3";
            FontSize = FontSize.Small;
        }
    }
}
