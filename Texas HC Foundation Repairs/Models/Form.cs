using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace Models
{
    public class Form
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string Name { get; set; }


        [Required(ErrorMessage = "*Email is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "*Invalid email address. Please re-enter.")]
        [Display(Name = "Email")]
        public string Email { get; set; }


        [Required(ErrorMessage = "*Message is required")]
        [DataType(DataType.Text)]
        [Display(Name = "Message")]
        public string Message { get; set; }


        //This is our honeypot field!!!
        [DataType(DataType.Text)]
        [Display(Name = "Subject")]
        public string Subject { get; set; }
    }
}