using System;
using System.ComponentModel.DataAnnotations;

namespace Personel.Core.UI.ViewModels
{
    public class PersonelEkleViewModel
    {
        public int PersonelId { get; set; }

        [Display(Name ="Ad")]
        [Required(ErrorMessage = "Personel Adı zorunludur.")]
        public string PersonelName { get; set; }

        [Display(Name = "Soyad")]
        [Required(ErrorMessage = "Personel Soyadı zorunludur.")]
        public string PersonelSurname { get; set; }

        [Display(Name = "Maaş")]
        [Range(500, 15000, ErrorMessage = "Maaş alanı 500 ile 15000 arasında olmalıdır!")]
        public short? PersonelSalary { get; set; }   

        [Display(Name = "Doğum Günü")]
        [Required]
        public DateTime? PersonelBirthdate { get; set; }

        public bool? PersonelGender { get; set; }
        public bool PersonelEvliMi { get; set; }
    }
}
