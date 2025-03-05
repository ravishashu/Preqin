
using Microsoft.Extensions.Configuration;
using Preqin.Infrastructure;
using Preqin.Infrastructure.Data;
using Preqin.WebAPI.Filters;
using Prequin.Service;
using Prequin.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace Preqin.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Load Configuration
            var configuration = builder.Configuration;
            // Add services to the container.
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp",
                    policy => policy.WithOrigins("http://localhost:3001")
                                    .AllowAnyMethod()
                                    .AllowAnyHeader());
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Repository
            builder.Services.AddScoped<IInvestorRepository, InvestorRepository>();
            builder.Services.AddScoped<IInvestorService, InvestorService>();

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ExceptionHandlingFilter>(); // Global Exception Handling
            }).
            AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                options.JsonSerializerOptions.WriteIndented = true;
            });

            // Configure SQLite Database
            builder.Services.AddDbContext<PreqinDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));


            // Register filters in DI
            builder.Services.AddScoped<LoggingActionFilter>();
            builder.Services.AddScoped<ExceptionHandlingFilter>();



            var app = builder.Build();
            app.UseCors("AllowReactApp"); // Apply CORS policy
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<PreqinDbContext>();
                dbContext.Database.Migrate(); // Apply Migrations
                DbInitializer.Seed(dbContext); // Seed Data from CSV
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
