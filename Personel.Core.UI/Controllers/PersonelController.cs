using Microsoft.AspNetCore.Mvc;
using Personel.Core.UI.Context;
using Personel.Core.UI.ViewModels;

using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Personel.Core.UI.Controllers

{
    public class PersonelController : Controller
    {
        PersonelDbEntities db = new PersonelDbEntities();

       
       // [Authorize(Policy = "Bilgi")]
        public ActionResult Index()
        {
            List<PersonelModel> abdurrezzak = new List<PersonelModel>();

            abdurrezzak = db.Personel.Include(x => x.Departman).Select(x => new PersonelModel  
            {
                Id = x.Id,
            
                DepartmanName = x.Departman.Name,
                Name = x.Name,
                DepartmanId = x.DepartmanId,
                EvliMi = x.EvliMi,
                Gender = x.Gender,
                Salary = x.Salary,
                Surname = x.Surname,
                
            }).ToList(); 

            return View(abdurrezzak);
        }

        public ActionResult Yeni()
        {
            var model = new PersonelFormViewModel()       
            {
                Departmanlar = db.Departman.ToList(),
                Personel = new Context.Personel()      
            };
            return View("PersonelForm", model);
        }
        [ValidateAntiForgeryToken]
        public ActionResult Kaydet(Context.Personel personel)
        {
            if (!ModelState.IsValid)
            {
                var model = new PersonelFormViewModel()
                {
                    Departmanlar = db.Departman.ToList(),
                    Personel = personel
                };
                return View("PersonelForm", model);
            }
            if (personel.Id == 0)  
            {
                db.Personel.Add(personel);
            }
            else        
            {
                db.Entry(personel).State = Microsoft.EntityFrameworkCore.EntityState.Modified; 
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Update(int id)
        {
            var model = new PersonelFormViewModel()
            {
                Departmanlar = db.Departman.ToList(),
                Personel = db.Personel.FirstOrDefault(x=>x.Id==id)
            };
            return View("PersonelForm", model);    
        }

        public ActionResult Delete(int id)
        {
            var silinecekPersonel = db.Personel.FirstOrDefault(x => x.Id == id);
            if (silinecekPersonel == null)
            {
                NotFound();
            }
            db.Personel.Remove(silinecekPersonel);

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult PersonelleriListele(int id)
        {
            var model = db.Personel.Where(x => x.DepartmanId == id).ToList();
            return PartialView(model);
        }
        public int? MaasToplam(int id)  
        {
            return db.Personel.Where(x => x.DepartmanId == id).Sum(x => x.Salary);
        }

        public ActionResult ToplamMaas()
        {
            ViewBag.Salary = db.Personel.Sum(x => x.Salary);
            return PartialView(ViewBag.Salary);
        }


       

    }
}

