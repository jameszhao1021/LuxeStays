using LuxeStays.Domain.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LuxeStays.Web.ViewModels
{
    public class AmenityVM
    {
        public Amenity? Amenity { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem>? VillaList { get; set; }

    }
}
