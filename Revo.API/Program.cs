using Revo.API.GlobalHandler;
using Revo.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Services Registration
builder.Services.AddControllers(); 
builder.Services.AddInfrastructureDependencies(builder.Configuration);
builder.Services.AddOpenApi();
// Add the global exception handler and problem details services
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// 2. HTTP Request Pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Revo API Documentation");
    });
}

app.UseHttpsRedirection();

app.UseExceptionHandler(); 

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); 
app.Run();