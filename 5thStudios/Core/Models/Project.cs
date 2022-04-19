using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Project
    {
        public Link ProjectLink { get; set; }
        //public string Title { get; set; }
        public string Summary { get; set; }
        public string ScreenshotUrl { get; set; }
        //public string SiteUrl { get; set; }
        public List<Link> LstCarouselShots { get; set; }


        public Project()
        {
            ProjectLink = new Link();
            //Summary = "";
            //ScreenshotUrl = "";
            //Title = "";
            //SiteUrl = "";

            LstCarouselShots = new List<Link>();
            //LstCarouselShots.Add("");
        }
    }
}
