using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Modals.Models
{
    public class Employee
    {
        public int Id { get; set; }
        
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [NotMapped]
        public string EncrypteId { get; set; }


        [Display(Name = "Last Name")]
        [Required (ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }
        [Required (ErrorMessage = " Gender Required")]
        public string Gender { get; set; }
        [Required (ErrorMessage = " Address Required")]
        public string Address { get; set; }
        [EmailAddress (ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
    }
}
