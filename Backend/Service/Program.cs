using Backend.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR();

// Add CORS policy to allow requests from React frontend.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Apply the CORS policy.
app.UseCors("AllowReactApp");

// Middleware pipeline.
app.UseHttpsRedirection(); // Redirect HTTP to HTTPS.
app.UseAuthorization();    // Enable authorization for protected endpoints.
app.MapControllers();      // Map API controllers to endpoints.
app.MapHub<WarGameHub>("/wargamehub");

await app.RunAsync();
