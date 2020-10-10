using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Authentication;

namespace Modals.ViewModels
{
     public class LoginViewModel
     {
         [Required]
         [EmailAddress]
         public string Email { get; set; }
         [Required]
         [DataType(dataType:DataType.Password)]
         public string Password { get; set; }
         [Display(Name = "Remember Me")]
         public bool RememberMe { get; set; }
         public string ReturnUrl { get; set; }

         // AuthenticationScheme is in Microsoft.AspNetCore.Authentication namespace
         public IList<AuthenticationScheme> ExternalLogins { get; set; }

    }
}
