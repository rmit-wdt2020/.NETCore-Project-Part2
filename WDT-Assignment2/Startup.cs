using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WDT_Assignment2.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Globalization;

namespace WDT_Assignment2
{
    public class Startup
    {
        // Dependency Injection 
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            

            services.AddDbContext<NwbaContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString(nameof(NwbaContext)));

                // Enable lazy loading.
                options.UseLazyLoadingProxies();
            });

            services.AddSession(options =>
            {
                // Make the session cookie essential. 
                options.Cookie.IsEssential = true;

                // Set user session time out to 10 minutes
                options.IdleTimeout = TimeSpan.FromMinutes(10);
            });

            services.AddControllersWithViews();

            // Quick hack to get "$" symbol working
            var cultureInfo = new CultureInfo("en-AU");
            cultureInfo.NumberFormat.CurrencySymbol = "$";
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            services.AddDbContext<NwbaContext>(options => options.UseSqlServer(Configuration.GetConnectionString("NwbaContext")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Middlewares: various implementations that are present in-built and are executed in an order before the whole page is loaded

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //set up a middleware to handle the request in the pipeline
            app.UseStatusCodePagesWithReExecute("/StatusCode/{0}");

            // Adds support for reading files from static folders 
            app.UseStaticFiles();

            // Creating the pattern for URL 
            app.UseRouting();

            app.UseSession(); 

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {

                endpoints.MapDefaultControllerRoute();
                //endpoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
