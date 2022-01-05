using bl.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace bl.Models
{
    [Serializable]
    public class AcctSetupModel
    {
        //Parameters
        public int StepIndex { get; set; }
        public int MemberId { get; set; }
        public int AccountId { get; set; }
        public int TotalSteps { get; set; }
        public bool Previous { get; set; }
        public bool Next { get; set; }

        //Instances
        public AccountMember NewUserInfo { get; set; }
        public AccountMember CurrentUserInfo { get; set; }
        public Location NewLocation { get; set; }
        //public Location CurrentLocation { get; set; }

        //Lists
        public List<AccountMember> LstMembers { get; set; }
        public List<Location> LstLocations { get; set; }
        public List<Toolset> LstToolsets { get; set; }
        public List<SelectListItem> LstBirthYears { get; set; }
        public List<SelectListItem> LstGenders { get; set; }
        public List<Color> LstColors { get; set; }


        public AcctSetupModel()
        {
            StepIndex = 0;
            TotalSteps = 5;
            //MemberId = 1063;

            CurrentUserInfo = new AccountMember();
            NewUserInfo = new AccountMember();
            LstMembers = new List<AccountMember>();
            NewLocation = new Location();
            LstLocations = new List<Location>();
            LstToolsets = new List<Toolset>();
            LstGenders = new List<SelectListItem>();
            LstColors = new List<Color>();

            //Prepopulate birth year list
            //for (var i = 1980; i > 1975; i--)
            LstBirthYears = new List<SelectListItem>();
            for (var i = DateTime.Today.Year; i > DateTime.Today.Year - 120; i--)
            {
                LstBirthYears.Add(new SelectListItem()
                {
                    Value = i.ToString(),
                    Text = i.ToString()
                });
            }
        }
    }

    [Serializable]
    public class AccountMember
    {
        public int? MemberId { get; set; }

        //[Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        //[Required]
        [Display(Name = "Last Initial")]
        public string LastInitial { get; set; }


        [Display(Name = "BirthYear")]
        public int? BirthYear { get; set; }

        [Display(Name = "Gender")]
        public int? Gender { get; set; }

        public Boolean IsAccountOwner { get; set; }


        public AccountMember() { }
        public AccountMember(string _firstName, string _lastInitial)
        {
            FirstName = _firstName;
            LastInitial = _lastInitial;
        }
    }

    [Serializable]
    public class Location
    {
        public int LocationId { get; set; }
        public string Name { get; set; }
        public Boolean IsBugoutBag { get; set; }
        public int? MemberId { get; set; }


        public Location() { }
    }


    [Serializable]
    public class Toolset
    {
        public int ToolId { get; set; }
        public int MemberToolId { get; set; }
        public int ColorId { get; set; }
        public string Name { get; set; }
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
        public Boolean IsActive { get; set; }


        public Toolset() { }
    }

    [Serializable]
    public class Color
    {
        public int ColorId { get; set; }
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
    }
}
