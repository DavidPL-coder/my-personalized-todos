using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MyPersonalizedTodos.API;
using MyPersonalizedTodos.API.Authorization;
using MyPersonalizedTodos.API.Database;
using MyPersonalizedTodos.API.Database.Entities;
using MyPersonalizedTodos.API.Extensions;
using MyPersonalizedTodos.API.Initialization;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

// TODO: The file is too big. Move some code to another files.
// TODO: write unit tests.
// TODO: Configure JsonConverter

var builder = WebApplication.CreateBuilder(args);
var appConfig = builder.Configuration.Get<AppConfig>();

builder.Services.AddSingleton(appConfig);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<DbSeeder>();
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); // it is added for disable object cycles error. 

var isCorsPolicyEnable = !string.IsNullOrWhiteSpace(appConfig.MPT_CORS_POLICY_NAME) && !string.IsNullOrWhiteSpace(appConfig.MPT_CORS_ALLOWED_URL);
if (isCorsPolicyEnable)
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(appConfig.MPT_CORS_POLICY_NAME, policyBuilder => policyBuilder.WithOrigins(appConfig.MPT_CORS_ALLOWED_URL)
            .AllowAnyMethod().AllowAnyHeader().AllowCredentials());
    });
}

builder.Services.AddDbContext<AppDbContext>(optionsBuilder => optionsBuilder.UseSqlServer(appConfig.MPT_CONNECTION_STRING));

// builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(@"/root/.aspnet/DataProtection-Keys"));

builder.Services.AddAuthentication(options => 
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(cfg => 
{
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = appConfig.MPT_JWT_ISSUER,
        ValidAudience = appConfig.MPT_JWT_AUDIENCE,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appConfig.MPT_JWT_KEY))
    };
    cfg.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey(appConfig.MPT_TOKEN_COOKIE_NAME))
                context.Token = context.Request.Cookies[appConfig.MPT_TOKEN_COOKIE_NAME];

            return Task.CompletedTask;
        }
    }; 
});

builder.Services.Scan(scan => scan.FromCallingAssembly().AddClasses(publicOnly: false)
    .AsImplementedInterfaces()
    .WithScopedLifetime());

var app = builder.Build();
using var serviceScope = app.Services.CreateScope();

if (!DbMigrator.TryToMigrate(app, serviceScope))
{
    app.Logger.LogCritical("The limit waiting for connection ({ConnectionLimit}s) has been passed. Can't connect to db.", appConfig.MPT_DB_CONNECTION_LIMIT/1000);
    return;
}

serviceScope.Get<DbSeeder>().Seed();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (isCorsPolicyEnable)
{
    app.Logger.LogInformation("# The CORS Policy '{policyName}' is enabled.", appConfig.MPT_CORS_POLICY_NAME);
    app.UseCors(appConfig.MPT_CORS_POLICY_NAME);
}

app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
