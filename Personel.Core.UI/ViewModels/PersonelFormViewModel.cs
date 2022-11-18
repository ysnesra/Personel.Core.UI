using Personel.Core.UI.Context;
using System.Collections.Generic;

namespace Personel.Core.UI.ViewModels
{
    public class PersonelFormViewModel
    {
        public List<Departman> Departmanlar { get; set; }  
                                                           
        public Context.Personel Personel { get; set; }      
    }
}
