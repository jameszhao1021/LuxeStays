using Microsoft.AspNetCore.Components.Web;
using System.ComponentModel.DataAnnotations;

namespace LuxeStays.Web.ViewModels
{
    public class LoginVM
    {
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string? RedirectUrl {  get; set; }
    }
}
