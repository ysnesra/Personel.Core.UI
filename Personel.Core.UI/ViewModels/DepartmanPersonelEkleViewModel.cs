using Personel.Core.UI.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Personel.Core.UI.ViewModels
{
    public class DepartmanPersonelEkleViewModel
    {
        public DepartmanPersonelEkleViewModel()             
        {
            PersonelEkleModels = new List<PersonelEkleViewModel>();   
            DepartmanIsim = "Yeni Departman";                     
        }
        public int DepartmanIdm { get; set; }

        [Display(Name = "Departman İsmi")]
        [Required(ErrorMessage = "Departman İsmi zorunludur.")]
        public string DepartmanIsim { get; set; }

        public List<PersonelEkleViewModel> PersonelEkleModels { get; set; }    
    }
}
