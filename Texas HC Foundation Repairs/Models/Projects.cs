using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class Projects
    {
        public string Description { get; set; }
        public List<Project> LstProjects { get; set; }
        public List<FilterLink> LstCategories { get; set; }


        public Projects()
        {
            //Instantiate variables
            LstProjects = new List<Project>();
            LstCategories = new List<FilterLink>();
        }
    }
}