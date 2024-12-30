using System.Web;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using www.Models;
using www.Models.PublishedModels;

namespace www.ViewModels
{
    public class ProductsSectionViewModel
    {
        public bool HasSearchQuery { get; set; }
        public string SearchQuery { get; set; } = string.Empty;
        public List<ProductListing> LstProductListings { get; set; } = new List<ProductListing>();
        public List<string> WebsiteBrands { get; set; } = new List<string>();
        public FoodProductTypes? FoodProductTypes { get; set; }

        public Dictionary<string, string> LstTags_Proteins { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> LstTags_Brands { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> LstTags_ProductTypes { get; set; } = new Dictionary<string, string>();
        public List<www.ReadApi.FilterAttribute> LstTags_Attributes { get; set; } = new List<www.ReadApi.FilterAttribute>();

        public bool IsPerdue { get; set; } = false;
        public bool IsColeman { get; set; } = false;
        public bool IsCheney { get; set; } = false;

        public IPublishedValueFallback? PublishedValueFallback { get; set; }
    }

    public class ProductListing
    {
        public List<string> LstAttribClasses { get; set; } = new List<string>();
        public Models.Link Product { get; set; }
    }
}