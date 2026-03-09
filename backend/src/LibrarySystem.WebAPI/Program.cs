using System.Text;
using System.Reflection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using LibrarySystem.Application;
using LibrarySystem.Infrastructure;
using LibrarySystem.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(
    Assembly.Load("LibrarySystem.Application")));

// Add FluentValidation
builder.Services.AddValidatorsFromAssembly(
    Assembly.Load("LibrarySystem.Application"));

// Database
builder.Services.AddDbContext<LibrarySystem.Infrastructure.Persistence.ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<LibrarySystem.Application.Interfaces.IDbContext>(provider =>
    provider.GetRequiredService<LibrarySystem.Infrastructure.Persistence.ApplicationDbContext>());

// Repositories
builder.Services.AddScoped<LibrarySystem.Application.Interfaces.IBookRepository,
    LibrarySystem.Infrastructure.Repositories.BookRepository>();
builder.Services.AddScoped<LibrarySystem.Application.Interfaces.IAuthorRepository,
    LibrarySystem.Infrastructure.Repositories.AuthorRepository>();
builder.Services.AddScoped<LibrarySystem.Application.Interfaces.IUnitOfWork,
    LibrarySystem.Infrastructure.Repositories.UnitOfWork>();

// Authentication
builder.Services.AddScoped<LibrarySystem.Application.Interfaces.IJwtTokenGenerator,
    LibrarySystem.Infrastructure.Authentication.JwtTokenGenerator>();
builder.Services.AddScoped<LibrarySystem.Application.Interfaces.IPasswordHasher,
    LibrarySystem.Infrastructure.Authentication.PasswordHasher>();

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audence not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        await DatabaseSeeder.SeedAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();

