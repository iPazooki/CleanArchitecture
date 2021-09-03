using Ca.Data;
using Ca.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using System;

namespace Ca.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Clean Architecture WebApi",
                    Description = "ASP.NET Core Web API",
                    Version = "v1",
                    TermsOfService = new Uri("https://github.com/iPazooki/CleanArchitecture"),
                    Contact = new OpenApiContact
                    {
                        Name = "Clean Architecture Web API",
                        Email = "iPazooki@gmail.com",
                        Url = new Uri("https://github.com/iPazooki/CleanArchitecture"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MRP",
                        Url = new Uri("https://github.com/iPazooki/CleanArchitecture"),
                    }
                });
            });

            services.AddDbContext<AppDbContext>
                (option => option.UseSqlServer(Configuration["ConnectionStrings:Default"]));

            services.RegisterAllServices(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ca.WebApi v1"));
            }

            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}