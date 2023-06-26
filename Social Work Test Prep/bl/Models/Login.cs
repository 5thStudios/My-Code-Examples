using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using ContentModels = Umbraco.Web.PublishedModels;


namespace bl.Models
{
    public class Login
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        //[Required(ErrorMessage = "*Password is required")]
        [StringLength(100, ErrorMessage = "*Password must be 6 or more characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        [NotMapped]
        public string NewPassword { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare(nameof(NewPassword), ErrorMessage = "*Confirmation password does not match.")]
        [NotMapped]
        public string ConfirmPassword { get; set; }


        public Login() { }
        public Login(string userName) {
            UserName = userName;
        }
        public Login(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
    }
}
