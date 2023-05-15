using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyPersonalizedTodos.API;
using MyPersonalizedTodos.API.Database;
using MyPersonalizedTodos.API.Database.Entities;
using MyPersonalizedTodos.API.Initialization;
using System.Reflection;
using System.Text;

// TODO: The file is too big. Move some code to another files.
// TODO: Take constants from config file, don't use them in code.
// TODO: write unit tests.
// TODO: Configure JsonConverter

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddHttpContextAccessor();

var appConfig = builder.Configuration.GetSection(nameof(AppConfig)).Get<AppConfig>();
builder.Services.AddSingleton(appConfig);

builder.Services.AddCors(options =>
{
    // TODO: Add the address of app on production 
    options.AddPolicy(appConfig.CorsPolicyName, policyBuilder => policyBuilder.WithOrigins("http://localhost", "http://mpt-frontend-container", "http://192.168.0.168")
        .AllowAnyMethod().AllowAnyHeader().AllowCredentials());
});

builder.Services.AddDbContext<AppDbContext>(optionsBuilder => optionsBuilder.UseSqlServer(appConfig.ConnectionString));

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
        ValidIssuer = appConfig.JwtIssuer,
        ValidAudience = appConfig.JwtAudience,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appConfig.JwtKey))
    };
    cfg.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey(appConfig.TokenCookieName))
                context.Token = context.Request.Cookies[appConfig.TokenCookieName];

            return Task.CompletedTask;
        }
    }; 
});

builder.Services.Scan(scan => scan.FromCallingAssembly().AddClasses(publicOnly: false)
    .AsImplementedInterfaces()
    .WithScopedLifetime());

builder.WebHost.UseUrls(appConfig.AppUrls);

var app = builder.Build();

if (!DbMigrator.TryToMigrate(app))
{
    app.Logger.LogCritical("The limit waiting for connection (30s) has been passed. Can't connect to db.");
    return;
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// TODO: Use HSTS

app.UseCors(appConfig.CorsPolicyName);

app.UseAuthentication();

// app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
