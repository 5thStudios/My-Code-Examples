using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace bl.Models
{
    public class Footer
    {
        public Link Logo { get; set; }
        public List<Link> LstSocialLinks { get; set; }
        public List<Link> LstFooterNav { get; set; }
        public IHtmlString Description { get; set; }


        public Footer()
        {
            LstSocialLinks = new List<Link>();
            LstFooterNav = new List<Link>();
        }
    }
}
