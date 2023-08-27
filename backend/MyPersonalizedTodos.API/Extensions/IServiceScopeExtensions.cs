namespace MyPersonalizedTodos.API.Extensions
{
    public static class IServiceScopeExtensions
    {
        public static TService Get<TService>(this IServiceScope serviceScope)
        {
            return serviceScope.ServiceProvider.GetService<TService>();
        }
    }
}
