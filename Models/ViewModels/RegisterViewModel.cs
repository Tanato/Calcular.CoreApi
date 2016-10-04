using System;
using System.ComponentModel.DataAnnotations;

namespace Calcular.CoreApi.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "BirthDate")]
        public DateTime BirthDate { get; set; }
    }
}
