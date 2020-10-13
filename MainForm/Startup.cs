using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayers.Class;
using DataAccessLayers.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MainForm.Data;
using MainForm.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modals.CustomValidation;
using Modals.Models;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

namespace MainForm
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContextPool<ApplicationDbContext>(options => options.UseSqlServer(connectionString));


            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequiredLength = 2;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredUniqueChars = 0;
                    options.Password.RequireNonAlphanumeric = false;
                    options.SignIn.RequireConfirmedEmail = true;



                }).AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("DeleteRolePolicy", policy=>policy.RequireClaim("Delete Role"));
                options.AddPolicy("EditRolePolicy", policy => policy.RequireAssertion(context =>
                    context.User.IsInRole("admin") &&
                    context.User.HasClaim(claim => claim.Type is "Edit Role" && claim.Value is "true") ||
                    context.User.IsInRole("SuperAdmin")));
                options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin"));

            });

            

            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                options.Filters.Add(new AuthorizeFilter(policy));

            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Adminstration/AccessDenied");

            });


            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = "883309166444-kef7vbouheaba0coivl9tklp2lj559ep.apps.googleusercontent.com";
                    options.ClientSecret = "XKtI2P2WyXXG8Dx47iHA70eg";
                })
                .AddFacebook(options =>
                {
                    options.AppId = "753472335525117";
                    options.AppSecret = "11880a4dc58380405f0b4804bc2df7bf";
                });
                

            services.AddSingleton<IEmployeeRepository, EmployeeRepository>();
            services.AddSingleton<DataProtectionPurposeStrings>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();
            app.UseMvc();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Employee}/{action=Index}/{id?}");
            });
        }
    }
}
