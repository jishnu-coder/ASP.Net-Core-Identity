using IdentityAuth.Utilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityAuth.ViewModel
{
    public class EditUserView
    {
        public EditUserView()
        {
            Roles = new List<string>();
        }
        public string Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
       
        [CustomEmailValidation(domain: "gmail.com", ErrorMessage = " Email Domain must be gmail.com ")]
        public string Email { get; set; }
        public string City { get; set; }
        public IList<string> Roles { get; set; }
        public IList<string> Claims { get; set; }
    }
}
