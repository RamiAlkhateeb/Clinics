using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json.Serialization;
using WebApi.Helpers;
using WebApi.Authorization;
using WebApi.Services;
using Microsoft.Extensions.Configuration;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // add services to the DI container
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<DataContext>();
            services.AddCors();
            services.AddControllers().AddJsonOptions(x =>
            {
                // serialize enums as strings in api responses (e.g. Role)
                x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                // ignore omitted parameters on models to enable optional params (e.g. User update)
                x.JsonSerializerOptions.IgnoreNullValues = true;
            });
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            // configure DI for application services
            services.AddScoped<IJwtUtils, JwtUtils>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISlotService, SlotService>();

        }

        // configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            // global error handler
            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseMiddleware<JwtMiddleware>();


            app.UseEndpoints(x => x.MapControllers());

            

        }
    }
}