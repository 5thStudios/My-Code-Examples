using Dragonfly.UmbracoHelpers;

using Umbraco.Cms.Infrastructure.ModelsBuilder;



namespace www.Models.PublishedModels
{
    partial class Recipe
    {








        /// <summary>
        /// All Attributes for Filtering
        /// </summary>
        //public List<FilterData> FilterAttributes
        //{
        //    get
        //    {
        //        var filterAttribs = new List<FilterData>();

        //        foreach (var tag in this.Tags)
        //        {
        //            filterAttribs.Add(new FilterData().Add("Tags", tag, "AddTags"));
        //        }

        //        foreach (var tag in this.MealTypes)
        //        {
        //            filterAttribs.Add(new FilterData().Add("Meal Type", tag, "AddMealTypeTags"));
        //        }

        //        foreach (var tag in this.ProductCategories)
        //        {
        //            filterAttribs.Add(new FilterData().Add("Product Category", tag, "AddProductCategoryTags"));

        //        }

        //        if (this.PrepCookTimeMinutesDisplay != "")
        //        {
        //            filterAttribs.Add(new FilterData().Add("Time", this.PrepCookTimeMinutesDisplay, "AddPrepCookTime"));
        //        }

        //        return filterAttribs;
        //    }

        //}






        [ImplementPropertyType("Tags")]
        public IEnumerable<string> Tags => this.Tags;

        [ImplementPropertyType("PrepCookTimeMinutes")]
        public string PrepCookTimeMinutesDisplay => this.PrepCookTimeMinutesDisplay;





        //private readonly UmbracoHelper umbracoHelper = new UmbracoHelper(UmbracoContext.Current);

        //#region CompPageSettings Implementation

        /////<summary>
        ///// Open Graph Preview Image
        /////</summary>
        //[ImplementPropertyType("OpenGraphImage")]
        //public MediaImage OpenGraphImage
        //{
        //    get { return CompPageSettings.GetOpenGraphImage(this); }
        //}

        //#endregion

        //public enum TimeRange
        //{
        //    ZeroToFifteenMinutes,
        //    SixteenToThirtyMinutes,
        //    ThirtyOneToFortyFiveMinutes,
        //    FortySixToSixtyMinutes,
        //    OverAnHour,
        //    Other,
        //    Unspecified
        //}

        /////<summary>
        ///// Text Type enum
        /////</summary>
        //public TimeRange PrepCookTime
        //{
        //    get
        //    {
        //        var display = this.PrepCookTimeMinutesDisplay;

        //        switch (display)
        //        {
        //            case "0-15 minutes":
        //                return TimeRange.ZeroToFifteenMinutes;
        //                break;
        //            case "16-30 minutes":
        //                return TimeRange.SixteenToThirtyMinutes;
        //                break;
        //            case "31-45 minutes":
        //                return TimeRange.ThirtyOneToFortyFiveMinutes;
        //                break;
        //            case "46-60 minutes":
        //                return TimeRange.FortySixToSixtyMinutes;
        //                break;
        //            case "Over an hour":
        //                return TimeRange.OverAnHour;
        //                break;
        //            case "":
        //                return TimeRange.Unspecified;
        //                break;
        //            default:
        //                return TimeRange.Other;
        //                break;
        //        }
        //    }
        //}


        /////<summary>
        ///// Prep/Cook Time Minutes: Total time display
        /////</summary>
        //[ImplementPropertyType("PrepCookTimeMinutes")]
        //public string PrepCookTimeMinutesDisplay
        //{
        //    get { return this.GetSafeString("PrepCookTimeMinutes"); }
        //}


        /////<summary>
        ///// Photo
        /////</summary>
        //[ImplementPropertyType("Photo")]
        //public MediaImage Photo
        //{
        //    get
        //    {
        //        return this.GetSafeImage(umbracoHelper, "Photo") as MediaImage;
        //    }
        //}

        /////<summary>
        ///// Description
        /////</summary>
        //[ImplementPropertyType("Description")]
        //public IHtmlString Description
        //{
        //    get { return new HtmlString(this.GetSafePropertyValue<string>("Description")); }
        //}

        /////<summary>
        ///// Ingredients
        /////</summary>
        //[ImplementPropertyType("Ingredients")]
        //public IEnumerable<NcRecipeIngredient> Ingredients
        //{
        //    get
        //    {
        //        var value = this.GetSafePropertyValue<IEnumerable<IPublishedContent>>("Ingredients");
        //        if (value != null)
        //        {
        //            return value.ToRecipeIngredients();
        //        }
        //        else
        //        {
        //            return new List<NcRecipeIngredient>();
        //        }
        //    }
        //}

        /////<summary>
        ///// Instructions
        /////</summary>
        //[ImplementPropertyType("Instructions")]
        //public IEnumerable<NcRecipeInstruction> Instructions
        //{
        //    get
        //    {
        //        var value = this.GetSafePropertyValue<IEnumerable<IPublishedContent>>("Instructions");
        //        if (value != null)
        //        {
        //            return value.ToRecipeInstructions();
        //        }
        //        else
        //        {
        //            return new List<NcRecipeInstruction>();
        //        }
        //    }
        //}

        /////<summary>
        ///// Related Products: Any products you select here will be combined with any Product Codes you list below.
        /////</summary>
        //[ImplementPropertyType("RelatedProducts")]
        //public IEnumerable<Product> RelatedProducts
        //{
        //    get
        //    {
        //        var value = this.GetSafePropertyValue<IEnumerable<IPublishedContent>>("RelatedProducts");
        //        if (value != null)
        //        {
        //            return value.ToProducts();
        //        }
        //        else
        //        {
        //            return new List<Product>();
        //        }
        //    }
        //}




        ///// <summary>
        ///// All Searchable Text for the Product Search page
        ///// </summary>
        //public string KeywordsBlob
        //{
        //    get
        //    {
        //        var keywords = new StringBuilder();
        //        keywords.AppendLine(this.Name);
        //        keywords.AppendLine(this.Description.ToString());
        //        keywords.AppendLine(string.Join(" ", this.AllRelatedProductCodes));
        //        keywords.AppendLine(string.Join(" ", this.Ingredients));
        //        keywords.AppendLine(string.Join(" ", this.Instructions));
        //        keywords.AppendLine(string.Join(" ", this.MealTypes));
        //        keywords.AppendLine(string.Join(" ", this.Tags));
        //        keywords.AppendLine(string.Join(" ", this.ProductCategories));

        //        return keywords.ToString();
        //    }
        //}

        ///// <summary>
        ///// Combination of Product Codes CSV data with Content Picker selected product codes
        ///// </summary>
        //public IEnumerable<string> AllRelatedProductCodes
        //{
        //    get
        //    {
        //        var pcodes = new List<string>();
        //        pcodes = this.RelatedProductCodes.Split(',').ToList();

        //        foreach (var prodNode in RelatedProducts)
        //        {
        //            pcodes.Add(prodNode.ProductCode);
        //        }

        //        return pcodes.Distinct();
        //    }
        //}

        ///// <summary>
        ///// All Related Products
        ///// </summary>
        //public IEnumerable<Product> AllRelatedProducts()
        //{
        //    var allRelatedProducts = new List<Product>();
        //    allRelatedProducts.AddRange(this.RelatedProducts);

        //    var pcodes = this.RelatedProductCodes.Split(',').ToList();
        //    if (pcodes.Any())
        //    {
        //        var allSiteProducts = ProductsHelper.GetAllFoodProducts(this).ToList();
        //        foreach (var code in pcodes)
        //        {
        //            var prodMatches = allSiteProducts.Where(n => n.ProductCode == code).ToList();
        //            if (prodMatches.Any())
        //            {
        //                allRelatedProducts.Add(prodMatches.First());
        //            }
        //        }
        //    }

        //    return allRelatedProducts.DistinctBy(n => n.Id);
        //}

        ///// <summary>
        ///// Brands for all Related Products
        ///// </summary>
        //public IEnumerable<string> AllRelatedProductBrands()
        //{
        //    var fpInfos = this.AllRelatedProducts().Select(n => n.FoodProductInfo).ToList();
        //    var brands = fpInfos.Select(n => n.BrandName);

        //    return brands.Distinct();
        //}
    }
}
