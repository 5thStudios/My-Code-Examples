using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bl.Models
{
    public class SideToolNav
    {
        public bl.Models.Link DashboardLink { get; set; }
        public List<bl.Models.Link> LstToolLinks { get; set; }
        public List<bl.Models.Link> LstAdminLinks { get; set; }

        public SideToolNav()
        {
            DashboardLink = new Link();
            LstToolLinks = new List<Link>();
            LstAdminLinks = new List<Link>();
        }
    }
}
