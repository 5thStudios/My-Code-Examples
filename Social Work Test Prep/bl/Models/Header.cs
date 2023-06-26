using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace bl.Models
{
    public class Header
    {
        public Link SiteLogo { get; set; }
        public IHtmlString Intro { get; set; }
        public Boolean IsHome { get; set; }
        public Boolean IsLoggedIn { get; set; }
        public List<Link> LstMainNav { get; set; }
        public List<Link> LstMinorNav { get; set; }
        //public List<Link> LstAcctNav { get; set; }
        public Link LnkGetStarted { get; set; }
        public Link LnkFreePracticeTest { get; set; }


        public Header() {
            LstMainNav = new List<Link>();
            LstMinorNav = new List<Link>();
            //LstAcctNav = new List<Link>();
        }
    }
}
