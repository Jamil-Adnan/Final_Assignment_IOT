
using IT_SegmentApi.Data;
using IT_SegmentApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace IT_SegmentApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<IOTFinalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            }); ;

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddHostedService<MqttProcessedListener>();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "IT API", Version = "v1" });
            });

            var app = builder.Build();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "IT API v1");
            });

            // Configure the HTTP request pipeline.


            app.UseHttpsRedirection();
            app.MapControllers();
            app.Urls.Add("https://localhost:5001");
            app.Urls.Add("http://localhost:5000");
            app.Run();
        }
    }
}
