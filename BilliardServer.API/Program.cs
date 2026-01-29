using Billiard.Application;
using BilliardServer.Core.Abstractions;
using BilliardServer.DataAccess;
using BilliardServer.DataAccess.Repositories;
using BuilliardServer;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

Debug.WriteLine($"App started");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BilliardDbContext>(b =>
{
    var connectionString = builder.Configuration.GetConnectionString(nameof(BilliardDbContext));
    b.UseNpgsql(connectionString);
});

builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IUsersService, UsersService>();

builder.Services.AddSignalR();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(MyAllowSpecificOrigins);

app.MapHub<GameHub>("/gameHub");

//.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
