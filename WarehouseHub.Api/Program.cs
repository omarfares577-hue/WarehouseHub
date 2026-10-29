using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WarehouseHub.Core.Contracts;
using WarehouseHub.Core.Errors;
using WarehouseHub.Core.Mapping;
using WarehouseHub.Domain.Entities;
using WarehouseHub.Infrastructure.Data;
using WarehouseHub.Infrastructure.Data.Seed.SeedData;
using WarehouseHub.Infrastructure.Data.Seed;
using WarehouseHub.Infrastructure.Repositories;
using WarehouseHub.Infrastructure.Services;

namespace WarehouseHub.Api
{
    public class ApiMarker

    { }
    public partial class Program
    {
     
        public static async Task Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                });
            builder.Services.AddHostedService<AutoArchiveService>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<IdentityContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));
            builder.Services.AddDbContext<WarehouseContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:ValidIssuer"],
                    ValidAudience = builder.Configuration["Jwt:ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"]!))
                };
            });

            builder.Services.AddScoped<ITokenServices, TokenServices>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped(typeof(IGenericrepository<>), typeof(GenericRepository<>));

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState
                        .Where(e => e.Value!.Errors.Count > 0)
                        .SelectMany(x => x.Value!.Errors)
                        .Select(x => x.ErrorMessage).ToArray();

                    var errorResponse = new ApiValidationError
                    {
                        ErrorMessages = errors
                    };

                    return new BadRequestObjectResult(errorResponse);
                };
            });
            builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(WarehouseProfile).Assembly));

            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
            }).AddEntityFrameworkStores<IdentityContext>();

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {
                    var context = services.GetRequiredService<WarehouseContext>();

                    // التصحيح هنا: تخطي الـ MigrateAsync في بيئة الـ Testing لأنها خاصة بـ SQL Server
                    if (!app.Environment.IsEnvironment("Testing"))
                    {
                        await context.Database.MigrateAsync();
                    }

                    await SeedContext.SeedAsync(context);
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "An error occurred during migration or seeding the database.");
                }
            }
            // Configure the HTTP request pipeline.
            app.UseSwagger();

            app.UseSwaggerUI(); 


            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}