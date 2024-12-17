using FFhub_backend.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
var saPassword = Environment.GetEnvironmentVariable("SA_PASSWORD");

builder.Services.AddDbContext<FFDbContext>(options =>
    options.UseSqlServer($"Server=10.244.17.97,1433;Database=OverflowDB;User Id=sa;Password={saPassword};TrustServerCertificate=True"));

var app = builder.Build();


// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
