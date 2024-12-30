using Umbraco.Cms.Core.Models.PublishedContent;
using www.Models;

namespace www.ViewModels
{
    public class HeaderViewModel
    {
        public string? SiteLogoUrl { get; set; }
        public string? SiteName { get; set; }
        public IPublishedContent? IpPage { get; set; }




        public HeaderViewModel() { }
    }
}
