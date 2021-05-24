using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace IdentityAuth.Utilities
{
    public class CustomEmailValidation:ValidationAttribute
    {
        private readonly string domain;

        public CustomEmailValidation(string domain)
        {
            this.domain = domain;
        }

        public override bool IsValid(object value)
        {
            string[] strings = value.ToString().Split("@");
            return strings[1].ToUpper() == domain.ToUpper(); 
        }
    }
}
