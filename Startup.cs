using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json.Serialization;
using WebApi.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebApi.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

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

//             services.AddAuthentication(auth =>
//             {
//                 auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//                 auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//             }).AddJwtBearer(options =>
// {
//     options.SaveToken = true;
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuer = true,
//         ValidateAudience = true,
//         ValidateLifetime = true,
//         ValidateIssuerSigningKey = true,
//         ValidIssuer = Configuration["Jwt:Issuer"],
//         ValidAudience = Configuration["Jwt:Issuer"],
//         IssuerSigningKey = new
//         SymmetricSecurityKey
//         (Encoding.UTF8.GetBytes
//         (Configuration["Jwt:Key"]))
//     };
// });


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

            // app.UseSession();

            // app.Use(async (context, next) =>
            // {
            //     var token = context.Session.GetString("Token");
            //     if (!string.IsNullOrEmpty(token))
            //     {
            //         context.Request.Headers.Add("Authorization", "Bearer " + token);
            //     }
            //     await next();
            // });

            // app.UseAuthentication();
            // app.UseAuthorization();
            // app.UseStatusCodePages();
            // //app.UseDefaultFiles(); // so index.html is not required
            // //app.UseStaticFiles();

            // app.UseEndpoints(endpoints =>
            // {
            //     endpoints.MapControllers();
            // });

        }
    }
}