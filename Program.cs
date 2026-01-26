using Microsoft.EntityFrameworkCore;
using IncidentServiceAPI.Services.Interfaces;
using IncidentServiceAPI.Middleware;
using IncidentServiceAPI.Services;
using IncidentServiceAPI.Data;


var builder = WebApplication.CreateBuilder(args);



// ----------------------------
// 1. Add services to DI
// ----------------------------

// Controllers
builder.Services.AddControllers();

// DbContext with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddScoped<IIncidentService, IncidentService>();
builder.Services.AddScoped<IAccountService, AccountService>();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ----------------------------
// 2. Middleware pipeline
// ----------------------------

// Global exception handling
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();