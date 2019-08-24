using System;
using System.Text;
using System.Threading.Tasks;
using LemVic.Services.Chat.DataAccess;
using LemVic.Services.Chat.DataAccess.Models;
using LemVic.Services.Chat.Hubs;
using LemVic.Services.Chat.Services;
using LemVic.Services.Chat.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace LemVic.Services.Chat
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });

            services.AddDbContext<ChatDbContext>(options => { options.UseInMemoryDatabase("__chat__"); });
            services.AddIdentity<UserAuth, IdentityRole>()
                    .AddEntityFrameworkStores<ChatDbContext>()
                    .AddDefaultTokenProviders();

            // Authentication (from https://docs.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz?view=aspnetcore-2.2)
            services.AddAuthentication(options => {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(options => {
                        var securitySettings = Configuration.GetSection("Security").Get<SecuritySettings>();

                        options.TokenValidationParameters = new TokenValidationParameters {
                            LifetimeValidator = (before, expires, token, param) => expires > DateTime.UtcNow,
                            ValidateAudience  = false,
                            ValidateIssuer    = false,
                            ValidateActor     = false,
                            ValidateLifetime  = true,
                            IssuerSigningKey =
                                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securitySettings.SecretKey))
                        };

                        options.Events = new JwtBearerEvents {
                            OnMessageReceived = context => {
                                var accessToken = context.Request.Query["access_token"];
                                var hubPath     = context.HttpContext.Request.Path;
                                if (!string.IsNullOrEmpty(accessToken) && hubPath.StartsWithSegments("/chat"))
                                {
                                    context.Token = accessToken;
                                }

                                return Task.CompletedTask;
                            }
                        };
                    });

            services.AddSingleton<IUserIdProvider, NameUserIdProvider>()
                    .AddScoped<IAuthService, AuthService>();

            // Add SignalR server component.
            services.AddSignalR();

            services.Configure<SecuritySettings>(Configuration.GetSection("Security"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection()
               .UseStaticFiles()
               .UseSpaStaticFiles();

            app.UseAuthentication();

            app.UseSignalR(routes => { routes.MapHub<ChatHub>("/chat"); });

            app.UseMvc(routes => {
                routes.MapRoute("default", "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa => {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer("start");
                }
            });
        }
    }
}
