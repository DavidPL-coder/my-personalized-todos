// TODO: Use ConfigurationProperty to change names of the properties. It will be maybe a good idea.
namespace MyPersonalizedTodos.API
{
    public class AppConfig
    {
        public string MPT_CORS_POLICY_NAME { get; init; }
        public string MPT_CORS_ALLOWED_URL { get; init; }
        public string MPT_CONNECTION_STRING { get; init; }
        public string MPT_CONNECTION_STRING_FOR_CONNECTION_TEST { get; init; }
        public string MPT_DATABASE_NAME { get; init; }
        public int MPT_DB_CONNECTION_LIMIT { get; init; }
        public int MPT_DB_CONNECTION_WAITING_TIME { get; init; }
        public string MPT_JWT_ISSUER { get; init; }
        public string MPT_JWT_AUDIENCE { get; init; }
        public string MPT_JWT_KEY { get; init; }
        public int MPT_JWT_EXPIRE_HOURS { get; init; }
        public string MPT_TOKEN_COOKIE_NAME { get; init; }
        public int MPT_MIN_LOGIN_LENGTH { get; init; }
        public int MPT_MIN_PASSWORD_LENGTH { get; init; }
        public int MPT_MIN_AGE { get; init; }
        public int MPT_MAX_AGE { get; init; }
    }
}
