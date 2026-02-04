using Billiard.Application;
using BilliardServer.API.Controllers;
using BilliardServer.Application.Features.Users;
using BilliardServer.Core.Abstractions;
using BilliardServer.Infrastructure;
using BilliardServer.Infrastructure;
using BilliardServer.Infrastructure.Entities;
using BilliardServer.Infrastructure.Repositories;
using BuilliardServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(LoginByEmailCommand).Assembly);
        });

        #region Swagger

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(services =>
            {
                services.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                services.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Введите ваш JWT токен.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });

                services.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()   // если нужны scopes — пишем их сюда
            }
                });
            });

        #endregion

        #region DbContext and Identity

        builder.Services
            .AddDbContext<BilliardDbContext>(options =>
                options
                    .UseNpgsql(builder.Configuration.GetConnectionString(nameof(BilliardDbContext)))
                    .LogTo(Console.WriteLine, LogLevel.Information)
                    .EnableDetailedErrors()
                );

        builder.Services
            .AddIdentityCore<UserEntity>(options =>
            {
                options.Password.RequiredLength = 3;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = false;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
            })
            .AddSignInManager()
            .AddRoles<UserRole>()
            .AddEntityFrameworkStores<BilliardDbContext>()
            .AddDefaultTokenProviders();

        #endregion

        #region Authentication and Authorization

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,          // строгое время
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                };
            });

        builder.Services
                .AddAuthentication()
                .AddYandex(options =>
                {
                    options.ClientId = "4c33f8ee634d44af8beba946bec58ebb";
                    options.ClientSecret = "3d06a2c3a3b24170954d6f38d517288c";
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                    //options.SaveTokens = false;   // Если только логин, то достаточно false, если далее будем работать с апи yandex, то true
                    //options.CallbackPath = "/signin-yandex";
                    //options.Events = new OAuthEvents
                    //{
                    //    OnCreatingTicket = async context =>
                    //    {
                    //        Debug.WriteLine("Yandex OnCreatingTicket called");
                    //        // Здесь можно дообогатить claims, если нужно
                    //        // Например, запросить дополнительные данные по access_token

                    //        //var request = new HttpRequestMessage(HttpMethod.Get, "https://login.yandex.ru/info?format=json");
                    //        //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                    //        //var response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
                    //        //response.EnsureSuccessStatusCode();

                    //        //var userJson = await response.Content.ReadAsStringAsync();
                    //        //var user = System.Text.Json.JsonDocument.Parse(userJson);

                    //        //var id = user.RootElement.GetProperty("id").GetString();
                    //        //context.Identity?.AddClaim(new Claim(ClaimTypes.NameIdentifier, id));

                    //        //if (user.RootElement.TryGetProperty("display_name", out var name))
                    //        //{
                    //        //    context.Identity?.AddClaim(new Claim(ClaimTypes.Name, name.GetString() ?? ""));
                    //        //}

                    //        //if (user.RootElement.TryGetProperty("default_email", out var email))
                    //        //{
                    //        //    context.Identity?.AddClaim(new Claim(ClaimTypes.Email, email.GetString() ?? ""));
                    //        //}

                    //        // и т.д.
                    //    },

                    //    // Опционально: обработка ошибок
                    //    OnRemoteFailure = context =>
                    //    {
                    //        Debug.WriteLine($"Yandex OnRemoteFailure called{Uri.EscapeDataString(context.Failure?.Message ?? "Unknown error")}");
                    //        //context.Response.Redirect("/error?message=" + Uri.EscapeDataString(context.Failure?.Message ?? "Unknown error"));
                    //        //context.HandleResponse();
                    //        return Task.CompletedTask;
                    //    }
                    //};
                })
                .AddIdentityCookies();

        #endregion


        builder.Services.AddScoped<TokenService>();
        builder.Services.AddScoped<IUsersRepository, UsersRepository>();
        builder.Services.AddScoped<IUsersService, UsersService>();
        builder.Services.AddScoped<IAuthService, AuthService>();

        builder.Services.AddSignalR();

        var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: MyAllowSpecificOrigins,
                policy =>
                {
                    policy.WithOrigins(
                        "http://localhost",
                        "https://localhost"
                        )
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


        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapHub<GameHub>("/gameHub");
        app.MapControllers();

        app.Run();
    }
}

#region Swagger

#endregion
