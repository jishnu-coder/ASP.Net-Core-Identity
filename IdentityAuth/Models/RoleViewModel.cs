using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace IdentityAuth.Models
{
    public class RoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}
