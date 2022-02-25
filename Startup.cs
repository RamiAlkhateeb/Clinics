using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json.Serialization;
using WebApi.Helpers;
using WebApi.Authorization;
using WebApi.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNet.OData.Extensions;
using System.Reflection;
using System.IO;
using System.Linq;
using Microsoft.OpenApi.Models;
using OData.Swagger.Services;

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
                //x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                // ignore omitted parameters on models to enable optional params (e.g. User update)
                x.JsonSerializerOptions.IgnoreNullValues = true;
            });
            services.AddControllers().AddNewtonsoftJson();
            services.AddOData();

            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "V1",
                    Title = "Clincs API",
                    Description = "API for Clincs"
                });
                //s.ResolveConflictingActions(apiDescriptions => apiDescriptions.First()); //This line

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                s.IncludeXmlComments(xmlPath);

            });
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            // configure DI for application services
            services.AddScoped<IJwtUtils, JwtUtils>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISlotService, SlotService>();
            //services.AddSwaggerGen();
            services.AddOdataSwaggerSupport();



        }

        // configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            //             app.UseSwagger(c =>
            //    {
            //        c.SerializeAsV2 = true;
            //    });
            app.UseDeveloperExceptionPage();

            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.)
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("./v1/swagger.json", "My API V1");
                //c.RoutePrefix = string.Empty;
            });
            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            // global error handler
            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseMiddleware<JwtMiddleware>();


            app.UseEndpoints(x =>
            {
                x.EnableDependencyInjection();
                x.Select().Count().Filter().OrderBy().MaxTop(100).SkipToken().Expand();
                x.MapControllers();
            });



        }
    }
}