using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Claims;

namespace Modals.Models
{
    public static class ClaimsStore
    {
        public static List<Claim> AllClaim = new List<Claim>()
        {
            new Claim("Create Role", "Create Role"),
            new Claim("Edit Role", "Edit Role"),
            new Claim("Delete Role", "Delete Role")
        };
    }
}
