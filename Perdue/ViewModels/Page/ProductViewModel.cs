using System.Web;
using Umbraco.Cms.Core.Models;
using www.Models;
using www.Models.PublishedModels;

namespace www.ViewModels
{
    public class ProductViewModel
    {
        public string? Title { get; set; }
        public string? ProductCode { get; set; }
        public Brand? ProductBrand { get; set; }
        public List<Umbraco.Cms.Core.Models.Link> LstProductImgs { get; set; } = new List<Umbraco.Cms.Core.Models.Link>();
        public List<string> LstAttributes { get; set; } = new List<string>();
        public string? MarketingMessage { get; set; }
        public string? FeatureBenefit { get; set; }
        public string? EmailLink { get; set; }
        public string? ContactUs { get; set; }
        public string? IngredientStatement { get; set; }
        public List<string>? LstAllergens { get; set; } = new List<string>();
        public ProductSpecification Specification { get; set; } = new ProductSpecification();
        public ProductHandling Handling { get; set; } = new ProductHandling();
        public Call2ActionViewModel Call2Action { get; set; } = new Call2ActionViewModel();
        public bool IsPerdue { get; set; } = false;
        public bool IsColeman { get; set; } = false;
        public bool IsCheney { get; set; } = false;
        public ProductServing PerServing { get; set; } = new ProductServing();
        public ProductServing PerMeasure { get; set; } = new ProductServing();
        public string? LastModified { get; set; }
    }


    public class ProductSpecification
    {
        public string? ItemSize { get; set; }
        public string? TradeUnitDescriptor { get; set; }
        public string? TradeUnitGtin { get; set; }
        public string? WeightInfo { get; set; }
        public string? MaxWeightInfo { get; set; }
        public string? Dimensions { get; set; }
        public string? CubeDimensions { get; set; }
        public string? PerPallet { get; set; }
        public string? PalletTieHie { get; set; }
    }

    public class ProductHandling
    {
        public string? StorageMethod { get; set; }
        public string? productTempRange { get; set; }
        public string? LifespanFromProduction { get; set; }
        public string? Attribute { get; set; }
    }
}


