using www.Models;

namespace www.ViewModels
{
    public class HeaderViewModel
    {
        public Link? SiteLogo { get; set; }
        public string? SiteTagline { get; set; }
        public List<Link> LstMinorNav { get; set; } = new List<Link>();
        public List<Link> LstLoginLinks { get; set; } = new List<Link>();
        public List<Link> LstSocialLinks { get; set; } = new List<Link>();
        public List<Link> LstMainNav { get; set; } = new List<Link>();
        public bool IsListOfHomePgs { get; set; }
    }
}