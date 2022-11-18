
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Personel.Core.UI.Context;
using Personel.Core.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;

namespace Personel.Core.UI.Controllers
{

    public class DepartmanController : Controller
    {
        PersonelDbEntities db = new PersonelDbEntities();
        private object departman;
        private readonly ICompositeViewEngine _viewEngine;

        public DepartmanController(ICompositeViewEngine viewEngine)
        {
            _viewEngine = viewEngine;
        }

        // [Authorize(Policy = "Pazarlama")]
        public IActionResult Index()
        {
            //if (!HttpContext.User.Claims.Any(x => x.Type == ClaimTypes.Role && x.Value == "P"))
            //{
            //    return RedirectToAction("GirisYap", "Login");
            //}
            List<DepartmanModel> departmod = new List<DepartmanModel>();
            departmod = db.Departman.Include(x => x.Personel).Select(x => new DepartmanModel   
            {
                Idm = x.Id,
                Isim = x.Name,
                HasPersonel = x.Personel.Any()   
            }).ToList();

            return View(departmod);
        }

        //[Route("Departman/Ekle")] Url mize kullanıcının görmesini istediğimiz ismi verebiliriz.

        [HttpGet]
        public ActionResult Yeni()   
        {
            return View("DepartmanForm", new DepartmanModel());  
        }
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public ActionResult Kaydet(DepartmanModel departmandım)
        {
            if (!ModelState.IsValid)
            {
                return View("DepartmanForm");
            }
            MesajViewModel model = new MesajViewModel();

            if (departmandım.Idm == 0)
            {
                var ekleKontrolDepartman = db.Departman.FirstOrDefault(x => x.Name == departmandım.Isim); 
                if (ekleKontrolDepartman != null)
                {
                    model.Mesaj = departmandım.Isim + "  isminde Departman bulunmaktadır.Yeni Departman ekleyiniz.....";
                    model.Status = false;
                    return Json(RenderPartialViewToString(_viewEngine, "~/Views/Shared/_Mesaj.cshtml", model));
                }
                db.Departman.Add(new Departman
                {
                    Id = departmandım.Idm,
                    Name = departmandım.Isim
                });
                model.Mesaj = departmandım.Isim + " başarıyla eklendi...";
            }
            else
            {
                var guncellenecekDepartman = db.Departman.FirstOrDefault(x => x.Id == departmandım.Idm);
                if (guncellenecekDepartman == null)
                {
                    return NotFound();
                }
                else
                {
                    guncellenecekDepartman.Name = departmandım.Isim;
                    model.Mesaj = departmandım.Isim + " başarıyla güncellendi...";
                }
            }
            db.SaveChanges();
            model.Status = true;
            model.LinkText = "Departman Listesi";
            model.Url = "/Departman";

            return Json(RenderPartialViewToString(_viewEngine, "~/Views/Shared/_Mesaj.cshtml", model));  
        }
        public ActionResult Update(int id)
        { 
            var dbDepartman = db.Departman.Include(x => x.Personel).FirstOrDefault(x => x.Id == id); 
            if (dbDepartman == null)
            {
                return NotFound();
            }
            var departModel = new DepartmanPersonelEkleViewModel()       
            {
                DepartmanIdm = dbDepartman.Id,
                DepartmanIsim = dbDepartman.Name,

                PersonelEkleModels = dbDepartman.Personel.Select(x => new PersonelEkleViewModel()
                {
                    PersonelId = x.Id,
                    PersonelName = x.Name,
                    PersonelSurname = x.Surname,
                    PersonelSalary = x.Salary,
                    PersonelBirthdate = x.Birthdate,
                    PersonelGender = x.Gender,
                    PersonelEvliMi = x.EvliMi,
                }).ToList()
            };
            return PartialView("DepartKayıt", departModel);

            var dbpersonel = db.Personel.Where(x => x.DepartmanId == id).ToList();
            var personelliste = new List<PersonelEkleViewModel>();

            foreach (var item in dbpersonel)
            {
                personelliste.Add(new PersonelEkleViewModel()
                {
                    PersonelId = item.Id,
                    PersonelName = item.Name,
                    PersonelSurname = item.Surname,
                    PersonelBirthdate = item.Birthdate,
                    PersonelSalary = item.Salary,
                    PersonelGender = item.Gender,
                    PersonelEvliMi = item.EvliMi,
                });
            }
            departModel.PersonelEkleModels = personelliste;
            return PartialView("DepartKayıt", departModel);
        }
        public ActionResult Delete(int id)
        {
            var silinecekDepartman = db.Departman.Include(x => x.Personel).FirstOrDefault(x => x.Id == id);
            if (silinecekDepartman == null)
            {
                return NotFound();
            }
            else      
            {
                var silinenDepartmaninIcindekiTumPersonelleriListesi = silinecekDepartman.Personel.ToList();

                foreach (var item in silinenDepartmaninIcindekiTumPersonelleriListesi)
                {

                    /*
                    ///1. YOL 
                    ///İlgili departmanın tüm personelleri-ni sil
                    */
                    silinecekDepartman.Personel.Remove(item);     /*Personelleri tek tek modelden siliyorum*/
                    db.Personel.Remove(item);            /*Sonra Personelleri databasedende siliyorum*/

                    ///2. YOL 
                    ///İlgili departmanın personellerinin departman bağlantısını sil // Foreignkey olan DepartmanId nin içini boşaltıcaz

                    //item.DepartmanId = null;
                }             
                db.Departman.Remove(silinecekDepartman);

                db.SaveChanges();
                return RedirectToAction("Index", "Departman");
            }
        }
        public ActionResult DepartmanPersonelEkle(int DepartmanId)
        {
            var model = db.Personel.ToList();

            return View(model);
        }
        public ActionResult DepartmanMaasToplam()
        {
            return View();
        }

        public string RenderPartialViewToString(ICompositeViewEngine viewEngine, string viewName, object model)
        {
            viewName ??= ControllerContext.ActionDescriptor.ActionName;
            ViewData.Model = model;
            using StringWriter sw = new StringWriter();

            IView view = viewEngine.GetView(viewName, viewName, false).View;
            ViewContext viewContext = new ViewContext(ControllerContext, view, ViewData, TempData, sw, new HtmlHelperOptions());
            view.RenderAsync(viewContext).Wait();
            return sw.GetStringBuilder().ToString();
        }

        [HttpGet]
        public IActionResult DepartmanKayıt()       
        {
            var mod = new DepartmanPersonelEkleViewModel();
            return PartialView("DepartKayıt", mod);   //
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult DepartmanKayıt(DepartmanPersonelEkleViewModel a)  
        {
            if (!ModelState.IsValid)     
            {
                return View("DepartKayıt", a);
            }
            MesajViewModel mesajol = new MesajViewModel();
           
            ///1.YOL
            ///Var olan bir Departmana Personel ekleyebilir.Yada Yeni Departman oluşturup buraya Personel ekleyebilir.(PersonelGir butonu ile hem yeni departmana hem var olan departmana Personel girebiliriz.)
            ///  ///1.YOL
            ///Yeni Departman ise Departmanı ve Personelleri kaydedecek(PersonelGir ve Ekleme işlemlerinin ikisinide yapabilir.)
            if (a.DepartmanIdm == 0)
            {
                var depart = new Departman    
                {
                    Id = a.DepartmanIdm,
                    Name = a.DepartmanIsim
                };
                foreach (var item in a.PersonelEkleModels)
                {
                    depart.Personel.Add(new Context.Personel()
                    {
                        Id = item.PersonelId,
                        Name = item.PersonelName,
                        Surname = item.PersonelSurname,
                        Salary = item.PersonelSalary,
                        Birthdate = item.PersonelBirthdate,
                        Gender = item.PersonelGender,
                        EvliMi = item.PersonelEvliMi
                    });
                }
                db.Departman.Add(depart);  
                db.SaveChanges();
                mesajol.Mesaj = a.DepartmanIsim + "başarıyla eklendi...";
            }
            else   
            {
                var departmanEklenecekPersonel = db.Departman.Include(x => x.Personel).FirstOrDefault(x => x.Id == a.DepartmanIdm); 
                if (departmanEklenecekPersonel != null)
                {
                    departmanEklenecekPersonel.Name = a.DepartmanIsim;
                }

                var modpersonelgüncelle = a.PersonelEkleModels.Where(x => x.PersonelId != 0).ToList();

                foreach (var item in modpersonelgüncelle)
                {
                    var guncellenecekDbPersonel = db.Personel.FirstOrDefault(x => x.Id == item.PersonelId);  
                    if (guncellenecekDbPersonel != null)
                    {
                        guncellenecekDbPersonel.Name = item.PersonelName;
                        guncellenecekDbPersonel.Surname = item.PersonelSurname;
                        guncellenecekDbPersonel.Gender = item.PersonelGender;
                        guncellenecekDbPersonel.Salary = item.PersonelSalary;
                        guncellenecekDbPersonel.EvliMi = item.PersonelEvliMi;
                        guncellenecekDbPersonel.Birthdate = item.PersonelBirthdate;
                    }
                }


                var modpersonelekle = a.PersonelEkleModels.Where(x => x.PersonelId == 0).ToList();

                foreach (var item in modpersonelekle)
                {
                    departmanEklenecekPersonel.Personel.Add(new Context.Personel()  
                    {
                        Id = item.PersonelId,
                        Name = item.PersonelName,
                        Surname = item.PersonelSurname,
                        Salary = item.PersonelSalary,
                        Birthdate = item.PersonelBirthdate,
                        Gender = item.PersonelGender,
                        EvliMi = item.PersonelEvliMi
                    });
                }

                var modpersonelsil = new List<int>();
                var dbpersonelsilIds = db.Personel.Where(x => x.DepartmanId == a.DepartmanIdm).Select(x => x.Id).ToList();//1,2,3,4,5,6,7,8,9,10 //Databasemizdeki Personellerin Id sini bir listede tutalım.
                var modpersonelsilIds = a.PersonelEkleModels.Select(x => x.PersonelId).ToList();//2,4,6,8,10,12,14,16,18,20//Modelimizdeki PersonelId lerini bir listede tutalım. 
                modpersonelsil.AddRange(dbpersonelsilIds.Where(x => modpersonelsilIds.All(t => t != x)).ToList());    //Sadece Idlerin listesini tutuyor.
                                                                                                                      //1,2,3,4,5,6,7,8,9,10      
                                                                                                                      //      2,  4,  6,  8,  10,12,14,16,18,20
                                                                                                                      //1, ,3, ,5, ,7, ,9
                foreach (var item in modpersonelsil)
                {
                    var silinecekPersonel = db.Personel.FirstOrDefault(x => x.Id == item);  
                    departmanEklenecekPersonel.Personel.Remove(silinecekPersonel);
                    db.Personel.Remove(silinecekPersonel);   
                }
            }

            db.SaveChanges();
            mesajol.Mesaj = a.DepartmanIsim + "başarıyla güncellendi...";
            mesajol.Status = true;
            mesajol.LinkText = "Departman Listesi";
            mesajol.Url = "/Departman/";

            return Json(RenderPartialViewToString(_viewEngine, "~/Views/Shared/_Mesaj.cshtml", mesajol));
        }


        //////2.YOL Departman-PersonelEkleme
        //var departmanno = new Departman      //Databasedeki Departman tablosuna ekleme işlemini Departman classından yaparız.Aynı tipte olması için.
        //{
        //    Id = a.DepartmanIdm,
        //    Name = a.DepartmanIsim
        //};
        //db.Departman.Add(departmanno);
        //db.SaveChanges();

        //var departmanId = departmanno.Id;   //Artık veritabanına kaydedildiği için buardaki ıd yi departmanId ye aktarırız.

        //foreach (var item in a.PersonelEkleModels)     //Kaçtane personel girdiysek o kadar dönecek ve her seferinde database e kaydedecek.Yani modelimizi database aktarıyoruz.
        //{
        //    db.Personel.Add(new Context.Personel() //Nesne yönelimli programlama(OOP)da bir Nesne listesinin içerisine yalnızca aynı nesne tipinde veri eklenebilir
        //    {
        //        Id = item.PersonelId,
        //        Name = item.PersonelName,
        //        Surname = item.PersonelSurname,
        //        Salary = item.PersonelSalary,
        //        Birthdate = item.PersonelBirthdate,
        //        Gender = item.PersonelGender,
        //        EvliMi = item.PersonelEvliMi,
        //        DepartmanId = departmanId  /**/
        //    });
        //    db.SaveChanges();   
        //}

        //////3.YOL Departman-PersonelEkleme //Modeli database e dönüştürdük
        //var depart = new Departman
        //{
        //    Id = a.DepartmanIdm,
        //    Name = a.DepartmanIsim,
        //    Personel = a.PersonelEkleModels.Select(x => new Context.Personel()
        //     {
        //       Id = x.PersonelId,
        //       Name = x.PersonelName,
        //       Surname = x.PersonelSurname,
        //       Salary = x.PersonelSalary,
        //       Birthdate = x.PersonelBirthdate,
        //       Gender = x.PersonelGender,
        //       EvliMi = x.PersonelEvliMi
        //    }).ToList()
        //};

        public IActionResult DepPersonelSil(int DepartmanId, int PersonelId)  
        {
            var dbpersonel = db.Personel.Where(x => x.DepartmanId == DepartmanId && x.Id == PersonelId).FirstOrDefault(); 
            db.Personel.Remove(dbpersonel);
            return View();
        }
        [HttpGet]
        public IActionResult PersonelGir()
        {
            PersonelEkleViewModel permod = new PersonelEkleViewModel();
            return PartialView("_DepartPersonelKayıt", permod);
        }


    }
}




