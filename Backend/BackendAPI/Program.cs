using BackendAPI.Data; // Import the namespace for AppDBContext
using Microsoft.EntityFrameworkCore;
using Accessors;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure the database context with the connection string.
builder.Services.AddDbContext<AppDBContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        Console.WriteLine("[ERROR] Connection string 'DefaultConnection' is not configured.");
        throw new InvalidOperationException("Connection string 'DefaultConnection' must be provided in appsettings.json.");
    }

    // Log SQL queries and parameters (for development only, disable in production).
    options.UseSqlServer(connectionString)
           .EnableSensitiveDataLogging()  // Logs query parameters (e.g., username, password).
           .LogTo(Console.WriteLine);     // Logs raw SQL queries to the console.
});

// Register UserAccessor as a scoped service.
builder.Services.AddScoped<UserAccessor>();
builder.Services.AddScoped<LeaderboardAccessor>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Add CORS policy to allow requests from React frontend.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// Apply the CORS policy.
app.UseCors("AllowReactApp");

// Middleware pipeline.
app.UseHttpsRedirection(); // Redirect HTTP to HTTPS.
app.UseAuthorization();    // Enable authorization for protected endpoints.
app.MapControllers();      // Map API controllers to endpoints.

app.Run(); // Run the application.
