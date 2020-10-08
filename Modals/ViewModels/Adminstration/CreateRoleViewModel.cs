using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;
using System.Text;

namespace Modals.ViewModels.Adminstration
{
   public  class CreateRoleViewModel
    {

        [Required]
        public  string RoleName { get; set; }
    }
}
