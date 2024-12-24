using FFhub_backend.Database;
using FFhub_backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel(options =>
{
    options.Listen(IPAddress.Any, 4500, listenOptions =>
    {
    });
});
// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalHost",
        builder =>
        {
            builder.WithOrigins("http://localhost")
                   .AllowAnyHeader()
                   .AllowAnyMethod().AllowCredentials();
        });
    options.AddPolicy("AllowAnyOrigin",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });

    options.AddPolicy("AllowOVerflowOrigin",
        builder =>
        {
            builder.WithOrigins("https://overflowapp.xyz")
                   .AllowAnyHeader()
                   .AllowAnyMethod().AllowCredentials();
        });

});

builder.Services.AddTransient<DataService>();
builder.Services.AddTransient<CommentService>();
var saPassword = Environment.GetEnvironmentVariable("SA_PASSWORD");

builder.Services.AddDbContext<FFDbContext>(options =>
    options.UseSqlServer($"Server=10.244.17.97,1433;Database=FfhubDB;User Id=sa;Password={saPassword};TrustServerCertificate=True"));

var app = builder.Build();

app.UseCors("AllowAnyOrigin");
// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
