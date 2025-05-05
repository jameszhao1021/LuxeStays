using LuxeStays.Domain.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LuxeStays.Web.ViewModels
{
    public class HomeVM
    {      
        public IEnumerable<Villa>? VillaList { get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set;}
        public int Nights { get; set; }
    }
}
