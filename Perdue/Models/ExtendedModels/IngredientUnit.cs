using Dragonfly.NetHelpers;
using Dragonfly.UmbracoHelpers;

using Umbraco.Cms.Infrastructure.ModelsBuilder;



namespace www.Models.PublishedModels
{
    public partial class IngredientUnit 
    {


        //public IngredientUnit(IPublishedValueFallback publishedValueFallback)
        //{
        //    _publishedValueFallback = publishedValueFallback;
        //}

        //public IngredientUnit(IPublishedContent content, IPublishedValueFallback publishedValueFallback) : base(content, publishedValueFallback)
        //{
        //}




        ///<summary>
        /// Unit Type Display Text
        ///</summary>
        [ImplementPropertyType("UnitType")]
        public string TypeDisplay
        {
            get { return this.UnitType ?? ""; }
        }



        public enum UnitTypes
        {
            Qty,
            Measurement,
            Variable,
            None,
            Other,
            Unspecified
        }

        ///<summary>
        /// Unit Type enum
        ///</summary>
        public UnitTypes Type
        {
            get
            {
                var display = this.TypeDisplay;

                switch (display)
                {
                    case "Qty":
                        return UnitTypes.Qty;
                        break;
                    case "Measurement":
                        return UnitTypes.Measurement;
                        break;
                    case "Variable":
                        return UnitTypes.Variable;
                        break;
                    case "None":
                        return UnitTypes.None;
                        break;
                    case "":
                        return UnitTypes.Unspecified;
                        break;
                    default:
                        return UnitTypes.Other;
                        break;
                }
            }
        }




        //public static string GetImageSrcUrl(ProductImage ProductImage, int Width, int Height, string BgColorForPadding, string AdditionalParametersAsString, UmbracoHelper Umbraco)
        //{
        //    //Obtain site image quality for 1WS API
        //    int Quality = ProductsHelper.ObtainAPIImageQuality(Umbraco);

        //    if (AdditionalParametersAsString == "")
        //    {
        //        return ObtainImageSrcUrl(ProductImage, Width, Height, BgColorForPadding, AdditionalParameters: null, Quality);
        //    }

        //    var kvParams = ParseStringToKvPairs(AdditionalParametersAsString);

        //    return ObtainImageSrcUrl(ProductImage, Width, Height, BgColorForPadding, AdditionalParameters: kvParams, Quality);
        //}




        ///// <summary>
        ///// Returns a url for use in an img src attribute utilizing remote.axd and Image Processor options.
        ///// </summary>
        ///// <param name="ProductImage">FoodProduct ProductImage</param>
        ///// <param name="Height">Pixel Height (use zero to exclude value)</param>
        ///// <param name="Width">Pixel Width (use zero to exclude value)</param>
        ///// <param name="BgColorForPadding">Hex code for color used to fill background, since there is no up-sizing. Example: "#FFFFFF"</param>
        ///// <param name="AdditionalParameters">List of Key/Value Pairs</param>
        ///// <returns></returns>
        //public static string ObtainImageSrcUrl(ProductImage ProductImage, int Width, int Height, string BgColorForPadding, IEnumerable<KeyValuePair<string, string>> AdditionalParameters = null, int Quality = 100)
        //{
        //    string strippedUrl = ProductImage.FullUrl.Replace("https://", "");
        //    string bgColor = BgColorForPadding.Replace("#", "");

        //    string dimensions = "";
        //    string quality = "";

        //    if (Width > 0 & Height > 0)
        //    {
        //        dimensions = $"&width={Width}&height={Height}";
        //    }
        //    else if (Width == 0 & Height > 0)
        //    {
        //        dimensions = $"&height={Height}";
        //    }
        //    else if (Width > 0 & Height == 0)
        //    {
        //        dimensions = $"&width={Width}";
        //    }

        //    quality = $"&quality={Quality}";

        //    string additionalParams = "";
        //    if (AdditionalParameters != null)
        //    {
        //        foreach (var kv in AdditionalParameters)
        //        {
        //            var param = $"&{kv.Key}={kv.Value}";
        //            additionalParams = additionalParams + param;
        //        }
        //    }

        //    string url = $"/remote.axd/{strippedUrl}?v={ProductImage.FileVersion}{dimensions}{quality}&upscale=false&bgcolor={bgColor}{additionalParams}";
        //    return url;

        //}





        //internal static IEnumerable<KeyValuePair<string, string>> ParseStringToKvPairs(string KvString)
        //{
        //    var kvPairs = new List<KeyValuePair<string, string>>();

        //    var countPairs = KvString.CountStringOccurrences("=");

        //    if (countPairs == 1)
        //    {
        //        //Single pair
        //        var kvp = ConvertStringToKvPair(KvString);
        //        kvPairs.Add(kvp);
        //    }
        //    else if (countPairs > 1)
        //    {
        //        if (KvString.Contains("&"))
        //        {
        //            var splitPairs = KvString.Split('&');

        //            foreach (var pair in splitPairs)
        //            {
        //                var kvp = ConvertStringToKvPair(pair);
        //                kvPairs.Add(kvp);
        //            }
        //        }
        //        else if (KvString.Contains(","))
        //        {
        //            var splitPairs = KvString.Split(',');

        //            foreach (var pair in splitPairs)
        //            {
        //                var kvp = ConvertStringToKvPair(pair);
        //                kvPairs.Add(kvp);
        //            }
        //        }
        //        else if (KvString.Contains("|"))
        //        {
        //            var splitPairs = KvString.Split('|');

        //            foreach (var pair in splitPairs)
        //            {
        //                var kvp = ConvertStringToKvPair(pair);
        //                kvPairs.Add(kvp);
        //            }
        //        }
        //    }

        //    return kvPairs;
        //}















        /////<summary>
        ///// Display Unit Label: On a recipe, should the unit label be displayed in the ingredient list?
        /////</summary>
        //[ImplementPropertyType("DisplayUnitLabel")]
        //public bool xDisplayUnitLabel
        //{
        //    get
        //    {
        //        if (this.GetSafePropertyValue("DisplayUnitLabel", "") == null)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return this.GetSafePropertyValue<bool>("DisplayUnitLabel");
        //        }
        //    }
        //}

        /////<summary>
        ///// Display Label After Ingredient: On a recipe, should the unit label be displayed after the ingredient rather than after the quantity?
        /////</summary>
        //[ImplementPropertyType("DisplayLabelAfterIngredient")]
        //public bool xDisplayLabelAfterIngredient
        //{
        //    get
        //    {
        //        if (this.GetSafePropertyValue("DisplayLabelAfterIngredient", "") == null)
        //        {
        //            return false;
        //        }
        //        else
        //        {
        //            return this.GetSafePropertyValue<bool>("DisplayLabelAfterIngredient");
        //        }
        //    }
        //}





        /////<summary>
        ///// Needs Qualifier: This unit should have a Qty Qualifier added (ex: '16 oz' can)
        /////</summary>
        //[ImplementPropertyType("NeedsQualifier")]
        //public bool xNeedsQualifier
        //{
        //    get
        //    {
        //        if (this.GetSafePropertyValue("NeedsQualifier", "") == null)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return this.GetSafePropertyValue<bool>("NeedsQualifier");
        //        }
        //    }
        //}

    }
}
