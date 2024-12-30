using Microsoft.AspNetCore.Html;
using UmbracoProject.Models;

namespace ECMC_Umbraco.Models
{
    public class HeaderViewModel
    {
		public string? SiteName { get; set; }
		public string? SiteLogoUrl { get; set; }
		public List<Link>? LstLinks { get; set; }

        public bool ToggleSearch { get; set; }



        public HeaderViewModel() {
            LstLinks = new List<Link>();
        }
	}
}
