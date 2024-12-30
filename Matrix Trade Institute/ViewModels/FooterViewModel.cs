using Microsoft.AspNetCore.Html;
using www.Models;

namespace www.ViewModels
{
    public class FooterViewModel
    {
        public string? SiteLogoUrl { get; set; }
        public string? SiteName { get; set; }
        public string? PhoneNo { get; set; }
        public string? PhoneTel { get; set; }
        public string? License { get; set; }
        public string? ContactEmail { get; set; }
        public HtmlString? Address { get; set; }
        public List<Link> LstSocialLinks { get; set; }
        public List<Link> LstFooterLinks { get; set; }

        public bool Bounce { get; set; }
        public string? CustomNavigation { get; set; }



        public FooterViewModel() {
            LstSocialLinks = new List<Link>();
            LstFooterLinks = new List<Link>();
        }
    }
}
