using AutoMapper;
using DiveLog.API.Controllers;
using DiveLog.API.HangfireExtensions;
using DiveLog.API.Helpers;
using DiveLog.DAL;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;

namespace DiveLog.API
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
            services.AddEntityFrameworkSqlServer()
                .AddDbContext<DiveLogContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DiveLogDB"), b => b.MigrationsAssembly("DiveLog.DAL")));

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
			var hangfireDB = Configuration.GetConnectionString("HangfireDB");
			services.AddHangfire(x => x.UseSqlServerStorage(hangfireDB));
            services.AddMemoryCache();
						
            services.AddSession(options =>
            {
                options.IdleTimeout = System.TimeSpan.FromHours(1);
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Dive Log API", Version = "v1" });
            });

            SetupDI(services);
        }

		private void SetupHangfireJobs()
		{
			var recurringJobs = Hangfire.JobStorage.Current.GetConnection().GetRecurringJobs();
			recurringJobs.ForEach(x => RecurringJob.RemoveIfExists(x.Id));

			RecurringJob.AddOrUpdate<IBackgroundJobs>(x => x.DeriveDiveLogStatisics(), "0 */5 * ? * *");
		}

		private void SetupDI(IServiceCollection services)
        {
            services.AddScoped<DiveLogContext>();
			services.AddScoped<IBackgroundJobs, BackgroundJobs>();
			services.AddScoped<IDiveLogStatHelper, DiveLogStatHelper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSession();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();                
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

			app.UseHangfireServer();
			var options = new DashboardOptions
			{
				Authorization = new[] { new DivelogHangfireAuthorizationFilter() }
			};
			app.UseHangfireDashboard("/hangfire", options);

            app.UseSwagger(setupAction: null);
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dive Log API V1");
            });

            app.UseMiddleware<GzipRequestMiddleware>();
            app.UseMvc();

			SetupHangfireJobs();
		}
    }
}
