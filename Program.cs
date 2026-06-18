using CommerceSystemAPI.Repositories;
using CommerceSystemAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
namespace CommerceSystemAPI

{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        "Logs/log-.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog();
            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            ///register services for dependency injection
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"));
            });
           
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

            builder.Services.AddScoped<PasswordService>();
            builder.Services.AddScoped<JwtService>();
            builder.Services.AddScoped<UserRepository>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<EmailService>();
            builder.Services.AddScoped<ProductRepository>();
            builder.Services.AddScoped<ProductService>();
            builder.Services.AddScoped<ReviewRepository>();
            builder.Services.AddScoped<ReviewService>();
            builder.Services.AddScoped<OrderProductsRepository>();
            builder.Services.AddScoped<OrderProductsService>();
            builder.Services.AddScoped<OrderRepository>();
            builder.Services.AddScoped<OrderService>();
            builder.Services.AddScoped<CategoryRepository>();
            builder.Services.AddScoped<CategoryService>();


            var jwtKey = builder.Configuration["Jwt:Key"]!;
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
.AddJwtBearer(options =>
{
    options.TokenValidationParameters =
        new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer =
                builder.Configuration["Jwt:Issuer"],

            ValidAudience =
                builder.Configuration["Jwt:Audience"],

            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey)),

            ClockSkew = TimeSpan.Zero
        }; 

        });
            builder.Services.AddAuthorization();



            // configure Swagger/OpenAPI for API documentation and testing
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using Bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
            });

            var app = builder.Build();
            app.UseMiddleware<CommerceSystemAPI.Middleware.ExceptionMiddleware>();
            // Configure the HTTP request pipeline.

            //running phase
            // Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
