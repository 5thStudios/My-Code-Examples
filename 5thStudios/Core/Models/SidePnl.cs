using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class SidePnl
    {
        public string siteLogoImgUrl { get; set; }
        public List<Link> LstNavigation { get; set; }



        public SidePnl()
        {
            //siteLogoImgUrl = "/media/1139/fifthstudios_final.png?crop=0.13008736841482382,0.12136460355065465,0.041495559092765845,0.099117942819334157&cropmode=percentage&width=140&height=140&rnd=132024772690000000";

            LstNavigation = new List<Link>();
            //LstNavigation.Add(new Link("Home", "#chapterintroduction", "link_introduction"));
            //LstNavigation.Add(new Link("About", "#chapterabout", "link_about"));
            //LstNavigation.Add(new Link("Portfolio", "#chapterportfolio", "link_portfolio"));
            //LstNavigation.Add(new Link("Skills", "#chapterskills", "link_skills"));
            //LstNavigation.Add(new Link("Contact", "#chaptercontact", "link_contact"));
        }
    }
}
