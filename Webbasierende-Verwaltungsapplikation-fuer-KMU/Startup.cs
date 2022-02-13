using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Infrastructure;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Documents;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.EMail;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU
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
            services.AddTransient<OrderService>();
            services.AddTransient<OperatorService>();
            services.AddTransient<BlobService>();
            services.AddTransient<DocumentService>();
            services.AddTransient<TicketService>();
            services.AddTransient<PdfService>();
            services.AddTransient<AuthService>();
            services.AddTransient<PartyService>();
            services.AddTransient<EmployeeService>();
            services.AddTransient<UserService>();
            services.AddTransient<BaseService>();
            services.AddTransient<EMailService>();
            services.AddTransient<IAuthProvider, DbAuthProvider>();
            services.AddTransient<ElementService>();
            services.AddTransient<CustomerService>();
            services.AddTransient<CompanyService>();
            services.AddTransient<AddressService>();
            services.AddTransient<ConversationService>();
            services.AddTransient<Database>();
            services.AddDbContext<Database>();
            services.AddHttpContextAccessor();
            services.AddRazorPages();

            #region Authentication

            services.AddRazorPages(opt =>
            {
                opt.Conventions.AuthorizeFolder("/Admin", "RequireAuthenticated");

                opt.Conventions.AuthorizeFolder("/Company", "RequireAuthenticated");

                opt.Conventions.AuthorizeFolder("/Customer", "RequireAuthenticated");

                opt.Conventions.AuthorizeFolder("/Element", "RequireAuthenticated");

                opt.Conventions.AuthorizeFolder("/Employee", "RequireAuthenticated");

                opt.Conventions.AuthorizeFolder("/Message", "RequireAuthenticated");

                opt.Conventions.AuthorizeFolder("/Order", "RequireAuthenticated");

                opt.Conventions.AuthorizeFolder("/Search");

                opt.Conventions.AuthorizeFolder("/Ticket", "RequireAuthenticated");

                opt.Conventions.AuthorizeFolder("/User", "RequireAuthenticated");

                opt.Conventions.AuthorizeFolder("/User/PhoneNumber", "RequireAuthenticated");

                opt.Conventions.AllowAnonymousToPage("/Development");

                opt.Conventions.AllowAnonymousToFolder("/Register");

                opt.Conventions.AllowAnonymousToFolder("/Login");

                opt.Conventions.AllowAnonymousToFolder("/Password");

            });
            #endregion

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(o => { o.LoginPath = "/Login"; o.AccessDeniedPath = "/404"; });

            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("RequireAuthenticated", p => p.RequireAuthenticatedUser());
            });

            services.AddAntiforgery(o => o.HeaderName = "xsrf-token");
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
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
