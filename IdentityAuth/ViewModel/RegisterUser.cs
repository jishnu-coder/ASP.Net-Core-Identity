using IdentityAuth.Utilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityAuth.ViewModel
{
    public class RegisterUser
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [Remote(action: "IsEmailInUse", controller:"Home")]
        [CustomEmailValidation(domain:"gmail.com",ErrorMessage =" Email Domain must be gmail.com ")]
        public string Email { get; set; }

        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name ="Confirm Password")]
        [Compare("Password" ,ErrorMessage ="Password and Confirm Password do not match")]
        public string ConfirmPassword { get; set; }
        public  string City { get; set; }

    }
}
