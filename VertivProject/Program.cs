
using VertivProject.Interfaces;
using VertivProject.Models;
using VertivProject.Services;

namespace VertivProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
            builder.Services.AddScoped<OtpService>();
            builder.Services.AddScoped<IEncriptionService, EncriptionService>();

            // Add CORS services to the DI container
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Use CORS policy
            app.UseCors("AllowLocalhost");

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
