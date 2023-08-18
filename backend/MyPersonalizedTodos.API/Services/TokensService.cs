using Microsoft.IdentityModel.Tokens;
using MyPersonalizedTodos.API.Database.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyPersonalizedTodos.API.Services
{
    public interface ITokensService
    {
        string GenerateJwtToken(User user);
        void SaveTokenToCookie(string token);
        void DeleteCookieWithToken();
    }

    public class TokensService : ITokensService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly AppConfig _appConfig;

        public TokensService(IHttpContextAccessor contextAccessor, AppConfig appConfig)
        {
            _contextAccessor = contextAccessor;
            _appConfig = appConfig;
        }

        public string GenerateJwtToken(User user)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig.MPT_JWT_KEY));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(_appConfig.MPT_JWT_EXPIRE_HOURS);
                                                                                    
            var token = new JwtSecurityToken(_appConfig.MPT_JWT_ISSUER, _appConfig.MPT_JWT_AUDIENCE, claims, null, expires, credentials);
            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }

        public void SaveTokenToCookie(string token)
        {
            var cookies = _contextAccessor.HttpContext.Response.Cookies;
            cookies.Append(_appConfig.MPT_TOKEN_COOKIE_NAME, token, new CookieOptions
            {
                HttpOnly = false,
                SameSite = SameSiteMode.Lax,
                Secure = false,
                Path = "/",
                Expires = DateTime.Now.AddMinutes(_appConfig.MPT_JWT_EXPIRE_HOURS)
            });
        }

        public void DeleteCookieWithToken()
        {
            // TODO: Delete token too.
            var cookies = _contextAccessor.HttpContext.Response.Cookies;
            cookies.Delete(_appConfig.MPT_TOKEN_COOKIE_NAME);
        }
    }
}
