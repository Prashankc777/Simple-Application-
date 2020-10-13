using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Modals.ViewModels.Adminstration
{
   public  class ForgetPassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
