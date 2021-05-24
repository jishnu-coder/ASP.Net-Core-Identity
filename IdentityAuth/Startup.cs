using IdentityAuth.Entity;
using IdentityAuth.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityAuth
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
            services.AddControllersWithViews();
            services.AddDbContextPool<AppDbContext>(
              options => options.UseSqlServer(Configuration.GetConnectionString("UserDBConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();
            services.ConfigureApplicationCookie(options => options.LoginPath = "/Home/Login");

            services.AddAuthorization(
                options =>
                {
                    options.AddPolicy("DeleteRolePolicy", Policy => Policy.RequireClaim("Delete Role","True"));
                    options.AddPolicy("EditRolePolicy", Policy => Policy.RequireClaim("Edit Role", "True"));
                    options.AddPolicy("CreateRolePolicy", Policy => Policy.RequireClaim("Create Role", "True"));
                });
            services.AddAuthorization(
                options =>
                {
                    options.AddPolicy("AdminPolicy", Policy => Policy.RequireRole("Admin"));
                    
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error/CustomError");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();


            app.UseAuthorization();
            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
