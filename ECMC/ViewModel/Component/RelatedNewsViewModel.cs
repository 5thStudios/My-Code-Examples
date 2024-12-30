using Umbraco.Cms.Core.Models.PublishedContent;
using UmbracoProject.Models;

namespace ECMC_Umbraco.Models
{
    public class RelatedNewsViewModel
    {
        public List<Link> LstNewsItems {  get; set; } = new List<Link>();
        public IPublishedContent RootList { get; set; }
        public int ItemCount { get; set; } = 4;

    }
}
