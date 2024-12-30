using www.Models;

namespace www.ViewModels
{
    public class HeaderViewModel
    {
        public string? SiteLogoUrl { get; set; }
        public string? SiteName { get; set; }
        public string? PhoneNo { get; set; }
        public string? PhoneTel { get; set; }
        public List<Link>? LstLinks { get; set; }
        public string? NavigationExtraClasses { get; set; }

        public bool IsLandingPage { get; set; }
        public string? CustomNavigation {  get; set; }



        public HeaderViewModel()
        {
            LstLinks = new List<Link>();
        }
    }
}
