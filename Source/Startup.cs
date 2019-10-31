using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BancoVirtualEstudantilWeb.Data;
using BancoVirtualEstudantilWeb.Services.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;

namespace BancoVirtualEstudantilWeb
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<CookieTempDataProviderOptions>(options =>
            {
                options.Cookie.IsEssential = true;
            });

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                config.User.RequireUniqueEmail = true;    // уникальный email
                config.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 -._@+";
                config.SignIn.RequireConfirmedEmail = false;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            if (Configuration["Authentication:Facebook:IsEnabled"] == "true")
            {
                services.AddAuthentication().AddFacebook(facebookOptions =>
                {
                    facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                    facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                });
            }

            if (Configuration["Authentication:Google:IsEnabled"] == "true")
            {
                services.AddAuthentication().AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                    googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                });
            }

            services.AddRazorPages();
            services.Configure<RazorPagesOptions>(options => options.RootDirectory = "/Pagina");
            services.AddMvc().WithRazorPagesRoot("/Pagina").AddRazorPagesOptions(options =>
             {
                 options.Conventions.AuthorizeFolder("/");

                 options.Conventions.AllowAnonymousToPage("/Error");
                 options.Conventions.AllowAnonymousToPage("/Conta/AcessoNegado");
                 options.Conventions.AllowAnonymousToPage("/Conta/ConfirmacaoEmail");
                 options.Conventions.AllowAnonymousToPage("/Conta/ConectarExternamente");
                 options.Conventions.AllowAnonymousToPage("/Conta/SenhaEsquecida");
                 options.Conventions.AllowAnonymousToPage("/Conta/ConfimacaoSenhaEsquecida");
                 options.Conventions.AllowAnonymousToPage("/Conta/Bloqueado");
                 options.Conventions.AllowAnonymousToPage("/Conta/Conectar");
                 options.Conventions.AllowAnonymousToPage("/Conta/ConectarComAutenticacao");
                 options.Conventions.AllowAnonymousToPage("/Conta/ConectarComCodigoRecuperacao");
                 options.Conventions.AllowAnonymousToPage("/Conta/CriarContar");
                 options.Conventions.AllowAnonymousToPage("/Conta/RedefinirSenha");
                 options.Conventions.AllowAnonymousToPage("/Conta/ConfirmacaoRedefinirSenha");
                 options.Conventions.AllowAnonymousToPage("/Conta/Desconectar");
             }).SetCompatibilityVersion(CompatibilityVersion.Latest);

            services.Configure<MailManagerOptions>(Configuration.GetSection("Email"));

            if (Configuration["Email:EmailProvider"] == "SendGrid")
            {
                services.Configure<SendGridAuthOptions>(Configuration.GetSection("Email:SendGrid"));
                services.AddSingleton<IMailManager, SendGridMailManager>();
            }
            else
            {
                services.AddSingleton<IMailManager, EmptyMailManager>();
            }

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<Services.Profile.ProfileManager>();
            services.AddMvc(option => option.EnableEndpointRouting = false);
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages(); //Routes for pages
                endpoints.MapControllers(); //Routes for my API controllers
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller}/{action}/{id?}", new {controller = "Conta", action = "Conectar"});
            });
        }
    }
}
