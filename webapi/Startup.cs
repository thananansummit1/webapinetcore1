using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using webapi.Attributes;
using webapi.Core;
using webapi.Service;
using webapi.Service.Interface;

namespace webapi
{
    
    public class Startup
    {
        private readonly string EnvironmentName;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            this.EnvironmentName = env.EnvironmentName;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddScoped<ActionFilter>();

            services.AddTransient<IProductService, ProductService>();

            // Get latest version
            JWT.Version = GetType().Assembly.GetName().Version.ToString();

            // Swagger
            const string buildNumber = "build_number";

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Test API",
                    Version = "v1",
                    Description = "Test API blueprint<br>" +
                        "Version: v" + GetType().Assembly.GetName().Version.ToString() + "." + buildNumber + "<br>" +
                        "Environment: " + this.EnvironmentName,
                    Contact = new OpenApiContact
                    {
                        Name = "test test",
                        Email = "test@test.com",
                    }
                });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.Last());
                //c.DocumentFilter<SwaggerFilter>();

                var securityScheme = new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter into field the word 'Bearer' following by space and JWT",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                };
                c.AddSecurityDefinition("Bearer", securityScheme);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
   {
                     new OpenApiSecurityScheme
                     {
                       Reference = new OpenApiReference
                       {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                       }
                      },
                      Array.Empty<string>()
                    }
                  });

                // csv
                c.MapType<FileContentResult>(() => new OpenApiSchema
                {
                    Type = "file"
                });
            });

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors("AllowOrigin");

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                c.InjectJavascript("/js/jquery-3.4.1.min.js");
                c.InjectJavascript("/js/swagger-custom.js");
                c.InjectStylesheet("/css/swagger-custom.css");
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot"
                ))
            });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
