using AutoMapper;
using MyPersonalizedTodos.API.Database.Entities;

namespace MyPersonalizedTodos.API.DTOs
{
    [AutoMap(typeof(ToDo), ReverseMap = true)]
    public class UpdateToDoDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? TaskStart { get; set; }
        public DateTime? TaskEnd { get; set; }
    }
}
