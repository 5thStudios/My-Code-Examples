using Org.BouncyCastle.Pqc.Crypto.Lms;
using Umbraco.Cms.Core.Models.PublishedContent;
using Cm = Umbraco.Cms.Web.Common.PublishedModels;

namespace ECMC_Umbraco.Models
{
    public class MetaAndAnalyticsViewModel
    {
        public Cm.SiteSettings? SiteSettings { get; set; }
        public string? Title { get; set; }
        public string? Robots { get; set; }
        public string? Description { get; set; }
        public string? Keywords { get; set; }
        public string? PageUrl { get; set; }
        public string? CanonicalUrl { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageType { get; set; }
        public string? GoogleSiteVerification { get; set; }
    }
}
