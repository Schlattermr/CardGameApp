using Backend.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

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

var app = builder.Build();

// Apply the CORS policy.
app.UseCors("AllowReactApp");

// Middleware pipeline.
app.UseHttpsRedirection(); // Redirect HTTP to HTTPS.
app.UseAuthorization();    // Enable authorization for protected endpoints.
app.MapControllers();      // Map API controllers to endpoints.

app.Run(); // Run the application.
