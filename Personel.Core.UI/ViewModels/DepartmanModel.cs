using System.ComponentModel.DataAnnotations;

namespace Personel.Core.UI.ViewModels
{
    public class DepartmanModel
    {
        internal int Id;

        public int Idm { get; set; }

        [Required]     //zorunlu alan
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        [Display(Name = "First Name")]
        public string Isim { get; set; }
        public string Name { get; internal set; }

        public bool HasPersonel { get; set; }  
    }
}