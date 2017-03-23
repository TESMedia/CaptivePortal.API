using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.Models
{
    public class LoginViewModel
    {
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string Email { get; set; }

    }
    public class AdminLoginViewModel
    {
        [Required(ErrorMessage = "Email address is required")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Invalid Email address")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

    }
}