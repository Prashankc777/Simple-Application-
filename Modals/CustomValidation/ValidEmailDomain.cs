using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Modals.CustomValidation
{
    public class ValidEmailDomain : ValidationAttribute
    {
        public string AllowedDomain;

        public ValidEmailDomain(string allowedDomain)
        {
            this.AllowedDomain = allowedDomain;
        }

      


        public override bool IsValid(object value)
        {
            var strings = value.ToString().Split('@');
            return string.Equals(strings[1], AllowedDomain, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
