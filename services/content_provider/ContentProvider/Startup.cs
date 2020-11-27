using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            var webContent = Configuration["WebContent"];

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = webContent;
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyMethod()
                    .AllowAnyHeader()
                    //.WithOrigins("http://localhost:3000")
                    .AllowCredentials()
                );
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseSpa(options => 
            { 
  
            });
            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();
            app.UseRouting();


        }
    }
}
