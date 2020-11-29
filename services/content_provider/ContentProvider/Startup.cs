using ContentProvider.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.Diagnostics;
using System.IO;

namespace ContentProvider
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers().AddNewtonsoftJson();
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "www";
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyMethod()
                    .AllowAnyHeader()
                    //.WithOrigins("http://localhost:3000")
                    .AllowCredentials()
                    .SetIsOriginAllowed(_ => true)
                );
            });

            services.AddScoped<IRoutesService, RoutesService>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            //app.UseHttpsRedirection();
            
            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); 
                //endpoints.MapFallbackToFile("/index.html");
            });

            app.UseStaticFiles();

            app.UseSpaStaticFiles();
            app.UseSpa(options => 
            {
                //options.Options.SourcePath = webContent;
            });

        }
    }
}
