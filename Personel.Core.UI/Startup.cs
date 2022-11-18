using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;     //Authorize (Yetkilendirme) iþlemi için gerekli cookies i ekliyoruz. 
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Personel.Core.UI.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Net;

namespace Personel.Core.UI
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
            services.AddDbContext<PersonelDbEntities>(builder => builder
                .UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));  //veritabanýnýn bilgilerini alýyor.

            //Claim'ler ile yetkilendirme yapmak için; 
            
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Pazarlama", policy => policy.RequireClaim(ClaimTypes.Role, new string[] { "P" }));
                // Claim tipindeki "P" rolünü Pazaralama ismindeki Policy de tutalým diyoruz.
                options.AddPolicy("Bilgi", policy => policy.RequireClaim(ClaimTypes.Role, new string[] { "B" }));
            });

            services.AddMvc();
            services.AddScoped<PersonelDbEntities>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)     //Kimlik doðrulama methodu tanýmladýk
                .AddCookie(x =>
                {
                    x.LoginPath = "/Login/GirisYap";   //Eðerki kiþi sisteme giriþ yapmadýysa Login Controllerýndaki GirisYap aktionresultuna gitsin
                });

            services.AddControllersWithViews();

            services.AddRazorPages().AddRazorRuntimeCompilation();    //proje çalýþýrken sayfanýn yenilenmesini saðlar.
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseStatusCodePages();

            app.UseRouting();
            /**///Bu sýralama önemli
            app.UseAuthentication();   //Kimlik kartý gibi
            app.UseAuthorization();    //Yetki kontrolü//Kimliðini kanýtladýðý zaman
            /**/
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");  //yönlendirdiðin bir yer yoksa proje baþlangýçta default olarak buraya gitsin
            });
        }
    }
}

