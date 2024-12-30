using Umbraco.Cms.Core.Models.PublishedContent;
using www.Models;

namespace www.ViewModels
{
    public class ViewAllProductsViewModel
    {
        public bool ShowImages { get; set; }
        public bool ShowList { get; set; }
        public int ProductListId { get; set; }

        public List<www.Models.ProductTools.Product> LstProducts { get; set; } = new List<Models.ProductTools.Product>();
        public List<ViewAllProducts_Sites> LstSites { get; set; } = new List<ViewAllProducts_Sites>();

    }

    public class ViewAllProducts_Sites()
    {
        public string? Name { get; set; }
        public int ProductsNodeId { get; set; }
        public IPublishedContent? IpProductList { get; set; }
        public bool IsActive { get; set; }
    }
}