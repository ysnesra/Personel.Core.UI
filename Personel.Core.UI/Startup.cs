using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;     //Authorize (Yetkilendirme) i�lemi i�in gerekli cookies i ekliyoruz. 
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
                .UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));  //veritaban�n�n bilgilerini al�yor.

            //Claim'ler ile yetkilendirme yapmak i�in; 
            
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Pazarlama", policy => policy.RequireClaim(ClaimTypes.Role, new string[] { "P" }));
                // Claim tipindeki "P" rol�n� Pazaralama ismindeki Policy de tutal�m diyoruz.
                options.AddPolicy("Bilgi", policy => policy.RequireClaim(ClaimTypes.Role, new string[] { "B" }));
            });

            services.AddMvc();
            services.AddScoped<PersonelDbEntities>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)     //Kimlik do�rulama methodu tan�mlad�k
                .AddCookie(x =>
                {
                    x.LoginPath = "/Login/GirisYap";   //E�erki ki�i sisteme giri� yapmad�ysa Login Controller�ndaki GirisYap aktionresultuna gitsin
                });

            services.AddControllersWithViews();

            services.AddRazorPages().AddRazorRuntimeCompilation();    //proje �al���rken sayfan�n yenilenmesini sa�lar.
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
            /**///Bu s�ralama �nemli
            app.UseAuthentication();   //Kimlik kart� gibi
            app.UseAuthorization();    //Yetki kontrol�//Kimli�ini kan�tlad��� zaman
            /**/
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");  //y�nlendirdi�in bir yer yoksa proje ba�lang��ta default olarak buraya gitsin
            });
        }
    }
}

