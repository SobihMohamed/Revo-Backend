using Revo.API.Extentions;
using Revo.API.GlobalHandler;
using Revo.Application;
using Revo.Infrastructure;
using Revo.Infrastructure.Implementations.Notifications;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Services Registration
builder.Services.AddControllers(); 
builder.Services.AddInfrastructureDependencies(builder.Configuration);
builder.Services.AddApplicationDependencies();
builder.Services.AddOpenApi();

// Add the global exception handler and problem details services
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000") 
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
var app = builder.Build();

await Revo.Infrastructure.Database.Seeding.AdminSeeder.SeedAsync(app.Services);

app.UseExceptionHandler();
app.UseCustomStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Revo API Documentation");
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowFrontend");

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications"); 

app.Run();