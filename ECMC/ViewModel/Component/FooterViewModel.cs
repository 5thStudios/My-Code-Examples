using Microsoft.AspNetCore.Html;
using UmbracoProject.Models;

namespace ECMC_Umbraco.Models
{
    public class FooterViewModel
    {
		public string? SiteUrl { get; set; }
		public string? SiteName { get; set; }
		public string? SiteLogoUrl { get; set; }

		public List<Link>? LstSocialMediaLinks { get; set; }

		public string? ContactEmail { get; set; }
        public string? ContactHeadline { get; set; }
        public string? SocialHeadline { get; set; }
		public string? ContactName { get; set; }

        public string? Phone { get; set; }
        public string? PhoneTel { get; set; }
        public HtmlString? Address { get; set; }

		public List<Link>? LstFooterNavLinks { get; set; }



		public FooterViewModel() { }
	}
}
