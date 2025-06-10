using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using PersonManagement.Application.Interfaces;
using PersonManagement.Application.Mappings;
using PersonManagement.Application.Services;
using PersonManagement.Domain.Interfaces;
using PersonManagement.Infrastructure.Data;
using PersonManagement.Infrastructure.Repositories;
using PersonManagement.API.Middleware;
using PersonManagement.API.Filters;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Person Management API", Version = "v1" });
});

// Configure Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Register repositories
builder.Services.AddScoped<IPersonRepository, PersonRepository>();

// Register services
builder.Services.AddScoped<IPersonService, PersonService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNextJS",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});


var app = builder.Build();
// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Person Management API v1");
        c.RoutePrefix = string.Empty; // Makes Swagger UI available at root
    });
}
app.UseGlobalErrorHandling();
app.UseHttpsRedirection();
app.UseCors("AllowNextJS");
app.UseAuthorization();
app.MapControllers();

// Apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}
app.Run();
