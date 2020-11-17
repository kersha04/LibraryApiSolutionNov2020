using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using LibraryAPI.Data;
using LibraryAPI.Profiles;
using LibraryAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LibraryAPI
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

            services.AddTransient<IProvideServerStatusInformation, HealthMonitoringApiServerStatus>();

            services.AddDbContext<LibraryDataContext>(options =>
            {
                // TODONE: Get rid of this hard-coded connection string before an infrastructure person burns down my house.
                options.UseSqlServer(Configuration.GetConnectionString("library"));
            });

            var mapperConfiguration = new MapperConfiguration(config =>
            {
                config.AddProfile<BooksProfile>();
            });

            IMapper mapper = mapperConfiguration.CreateMapper();

            services.AddSingleton<MapperConfiguration>(mapperConfiguration);
            services.AddSingleton<IMapper>(mapper);

            services.AddScoped<ILookupBooks, EfSqlBooks>();
            services.AddScoped<IBookCommands, EfSqlBooks>();
            services.AddScoped<ILookupOnCallDevelopers, RedisOnCallLookup>();

            services.AddHostedService<CachePrimer>();
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = Configuration.GetConnectionString("redis");
            });

            services.AddSwaggerGen(c =>
            {
               c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
               {
                   Title = "Library API for BES 100",
                   Version = "1.0",
                   Contact = new Microsoft.OpenApi.Models.OpenApiContact
                   {
                       Name = "Jeff Gonzalez",
                       Email = "jeff@hypertheory.com"
                   },
                   Description = "For Training"
               });
               var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
               var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
               c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library API");
                c.RoutePrefix = "";
            });
        }
    }
}
