namespace MyPersonalizedTodos.API
{
    public class AppConfig
    {
        public string CorsPolicyName { get; init; }
        public string ConnectionString { get; init; }
        public string JwtIssuer { get; init; }
        public string JwtAudience { get; init; }
        public string JwtKey { get; init; }
        public int JwtExpireHours { get; init; }
        public string TokenCookieName { get; init; }
        public string AppUrls { get; init; }
    }
}
