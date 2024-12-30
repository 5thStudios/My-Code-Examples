using Microsoft.AspNetCore.Html;
using UmbracoProject.Models;

namespace ECMC_Umbraco.Models
{
    public class HeaderMegaMenuViewModel
    {
        public string? SiteName { get; set; }
        public string? SiteLogoUrl { get; set; }
        public List<Link> LstLinks { get; set; } = new List<Link>();
    }
}
