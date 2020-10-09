using System;
using System.Collections.Generic;
using System.Text;
using Modals.Models;

namespace Modals.ViewModels
{
   public class UserClaimViewModel
    {
        public UserClaimViewModel()
        {
            Claims = new List<UserClaim>();
        }


        public string UserId { get; set; }
        public List<UserClaim> Claims { get; set; }
      
    }
}
