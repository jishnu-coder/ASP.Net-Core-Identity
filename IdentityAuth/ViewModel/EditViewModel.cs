using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace IdentityAuth.ViewModel
{
    public class EditViewModel
    {
        public EditViewModel()
        {
            Users = new List<string>();
        }
        public string Id { get; set; }
        [Required(ErrorMessage ="RoleName can not be null")]
        public string RoleName { get; set; }

        public List<string> Users{ get; set; }
    }
}
