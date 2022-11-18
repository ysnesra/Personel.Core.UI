using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Personel.Core.UI.Context;
using Personel.Core.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;    
using System.Threading.Tasks;

namespace Personel.Core.UI.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        PersonelDbEntities dbb = new PersonelDbEntities();  


        [HttpGet]   
        public IActionResult GirisYap(string returnUrl)
        {

            return View(model: returnUrl);  
        }                                  

        [HttpPost]  
        public async Task<ActionResult> GirisYap(string Name, string Password, string returnUrl) 
        {                                                                        

            var bilgiler = dbb.Users.FirstOrDefault(x => x.Name == Name && x.Password == Password);  

            if (bilgiler != null)      
            {
                var claims = new List<Claim>    
                {
                    new Claim(ClaimTypes.Name,bilgiler.Name),   
                    new Claim(ClaimTypes.Role,bilgiler.Role)  
                };

                var useridentity = new ClaimsIdentity(claims, "Login");      

                ClaimsPrincipal principal = new ClaimsPrincipal(useridentity);  

                await HttpContext.SignInAsync(principal);                  




                if (!String.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);     
                else
                    return RedirectToAction("Index", "Home");  
            }

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
