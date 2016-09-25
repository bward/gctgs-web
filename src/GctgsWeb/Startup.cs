using System;
using System.Reflection;
using BJW.Raven;
using GctgsWeb.Authorisation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.FileProviders;

namespace GctgsWeb
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            Configuration = builder.Build();
            CurrentEnvironment = env;
        }

        private IHostingEnvironment CurrentEnvironment { get; set; }
        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddMemoryCache();
            services.AddDbContext<GctgsContext>(options => options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));
            services.AddSingleton(RavenClientProvider);
            var ravenAssembly = Assembly.Load(new AssemblyName("BJW.Raven"));
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.FileProviders.Add(new EmbeddedFileProvider(ravenAssembly));
            });
            services.AddMvc().AddApplicationPart(ravenAssembly);
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RegisteredUser",
                    policy => policy.Requirements.Add(new RegisteredUserRequirement()));
                options.AddPolicy("Admin",
                    policy => policy.Requirements.Add(new AdminRequirement()));
            });
            services.AddSingleton<IAuthorizationHandler, RegisteredUserHandler>();
            services.AddSingleton<IAuthorizationHandler, AdminHandler>();
        }

        public WebAuthClient RavenClientProvider(IServiceProvider provider)
        {
            return CurrentEnvironment.IsDevelopment() ? new DemoWebAuthClient(Configuration["URL"]) : new WebAuthClient(Configuration["URL"]);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookieAuthentication(CookieAuthentication.DefaultOptions);

            var context = app.ApplicationServices.GetService<GctgsContext>();
            context.Database.Migrate();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=BoardGames}/{action=Index}/{id?}");
            });

        }
    }
}
