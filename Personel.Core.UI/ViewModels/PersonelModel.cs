using Personel.Core.UI.Context;
using System;
using System.ComponentModel.DataAnnotations;


namespace Personel.Core.UI.ViewModels
{
    public class PersonelModel
    {
        public int Id { get; set; }
        public int? DepartmanId { get; set; }
        public string Name { get; set; }
        
        public string Surname { get; set; }
        public string DepartmanName { get; set; }
       

        [Range(500, 15000, ErrorMessage = "Maaş alanı 500 ile 15000 arasında olmalıdır!")]
        public int? Salary { get; set; }

        public bool? Gender { get; set; }

        public bool EvliMi { get; set; }


    }
   
}
