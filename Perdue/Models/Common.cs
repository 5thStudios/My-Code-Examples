using System.Drawing;
using System.Text.RegularExpressions;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common;


namespace www.Models
{
    public sealed class Common
    {
        #region "Properties"
        public struct Doctype
        {
            public const string AboutSection = "AboutSection";
            public const string BlogListing = "BlogListing";
            public const string BlogPost = "BlogPost";
            public const string Brand = "Brand";
            public const string BrandsListing = "BrandsListing";
            public const string ContactUsPage = "ContactUsPage";
            public const string ErrorPage = "ErrorPage";
            public const string FaqItem = "FaqItem";
            public const string FeaturedProducts = "featuredProducts";
            public const string GoogleSitemap = "GoogleSitemap";
            public const string GroupColumn = "groupColumn";
            public const string Home = "Home";
            public const string HomeCHE = "homeCHE";
            public const string HomeCOL = "homeCOL";
            public const string IngredientUnit = "IngredientUnit";
            public const string IngredientUnitsData = "IngredientUnitsData";
            public const string LiveSkuChart = "liveSkuChart";
            public const string MerchandisingItem = "MerchandisingItem";
            public const string NavigationGroup = "navigationGroup";
            public const string Offer = "Offer";
            public const string OffersListing = "OffersListing";
            public const string OrganizingFolder = "OrganizingFolder";
            public const string Product = "Product";
            public const string ProductListing = "ProductListing";
            public const string ProductsSection = "ProductsSection";
            public const string ProductType = "ProductType";
            public const string ProductTypesFolder = "ProductTypesFolder";
            public const string Recipe = "Recipe";
            public const string RecipesListing = "RecipesListing";
            public const string RedirectPage = "RedirectPage";
            public const string ResourcesSection = "ResourcesSection";
            public const string RssPage = "RssPage";
            public const string ScrollingBlogPost = "scrollingBlogPost";
            public const string SearchPage = "SearchPage";
            public const string SiteData = "SiteData";
            public const string SocialService = "SocialService";
            public const string TextPage = "TextPage";

        }
        public struct ViewComponent
        {
            public const string Footer = "Footer";
            public const string Header = "Header";
            public const string MainNav = "MainNav";
            public const string Meta = "Meta";
        }
        public struct View
        {
            public const string AboutSection = "~/Views/AboutSection.cshtml";
            public const string AboutSectionCHE = "~/Views/AboutSectionCHE.cshtml";
            public const string Brand = "~/Views/Brand.cshtml";
            public const string BlogListing = "~/Views/BlogListing.cshtml";
            public const string BlogPost = "~/Views/BlogPost.cshtml";
            public const string BrandsListing = "~/Views/BrandsListing.cshtml";
            public const string ContactUsPage = "~/Views/ContactUsPage.cshtml";
            public const string ErrorPage = "~/Views/ErrorPage.cshtml";
            public const string ErrorPageCHE = "~/Views/ErrorPageCHE.cshtml";
            public const string ErrorPageCOL = "~/Views/ErrorPageCOL.cshtml";
            public const string GoogleSitemap = "~/Views/GoogleSitemap.cshtml";
            public const string Home = "~/Views/Home.cshtml";
            public const string HomeCHE = "~/Views/HomeCHE.cshtml";
            public const string HomeCOL = "~/Views/HomeCOL.cshtml";
            public const string LiveSkuChart = "~/Views/LiveSkuChart.cshtml";
            public const string MerchandisingMaterialsListing = "~/Views/MerchandisingMaterialsListing.cshtml";
            public const string OffersListing = "~/Views/OffersListing.cshtml";
            public const string Product = "~/Views/Product.cshtml";
            public const string ProductsSection = "~/Views/ProductsSection.cshtml";
            public const string Recipe = "~/Views/Recipe.cshtml";
            public const string RecipesListing = "~/Views/RecipesListing.cshtml";
            public const string RedirectPage = "~/Views/RedirectPage.cshtml";
            public const string ResourcesSection = "~/Views/ResourcesSection.cshtml";
            public const string ScrollingBlogPost = "~/Views/ScrollingBlogPost.cshtml";
            public const string SearchPage = "~/Views/SearchPage.cshtml";
            public const string TextPage = "~/Views/TextPage.cshtml";
            public const string TextPageCHE = "~/Views/TextPageCHE.cshtml";
            public const string TextPageCOL = "~/Views/TextPageCOL.cshtml";
            public const string ViewAllProducts = "~/Views/ViewAllProducts.cshtml";
            public const string ViewProductImages = "~/Views/ViewProductImages.cshtml";
        }
        public struct Partial
        {
            public const string BlogAllCategories = "~/Views/Partials/Panels/Blog_AllCategories.cshtml";
            public const string BlogRecentPosts = "~/Views/Partials/Panels/Blog_RecentPosts.cshtml";
            public const string NutritionTable = "~/Views/Partials/Panels/NutritionTable.cshtml";
            public const string Footer = "~/Views/Partials/Common/Footer.cshtml";
            public const string FooterCHE = "~/Views/Partials/Common/Footer-CHE.cshtml";
            public const string FooterCOL = "~/Views/Partials/Common/Footer-COL.cshtml";
            public const string Header = "~/Views/Partials/Common/Header.cshtml";
            public const string HeaderCHE = "~/Views/Partials/Common/Header-CHE.cshtml";
            public const string HeaderCOL = "~/Views/Partials/Common/Header-COL.cshtml";
            public const string HeroCarousel = "~/Views/Partials/Heroes/Hero-Carousel.cshtml";
            public const string HeroCarouselCOL = "~/Views/Partials/Heroes/Hero-CarouselCOL.cshtml";
            public const string HeroCarouselCHE = "~/Views/Partials/Heroes/Hero-CarouselCHE.cshtml";
            public const string HeroCarouselSearch = "~/Views/Partials/Heroes/Hero-Carousel-Search.cshtml";
            public const string Call2Action = "~/Views/Partials/Panels/Call2Action.cshtml";
            public const string ProductListing_RenderFilters = "~/Views/Partials/Panels/ProductListing_RenderFilters.cshtml";
            public const string ProductListing_RenderProduct = "~/Views/Partials/Panels/ProductListing_RenderProduct.cshtml";
            public const string RecipesListing_RenderFilters = "~/Views/Partials/Panels/RecipesListing_RenderFilters.cshtml";
            public const string RecipesListing_RenderRecipes = "~/Views/Partials/Panels/RecipesListing_RenderRecipes.cshtml";
            public const string MainNav = "~/Views/Partials/Common/MainNav.cshtml";
            public const string MobalNav = "~/Views/Partials/Common/MobalNav.cshtml";
            public const string MetaAndAnalytics = "~/Views/Partials/Common/MetaAndAnalytics.cshtml";

            public const string GridOutline = "~/Views/Partials/Styles/GridOutline.cshtml";
            public const string ZurbTestPg = "~/Views/Partials/Styles/ZurbTestPg.cshtml";

        }
        public struct Layout
        {
            public const string TopLevelPER = "TopLevel.cshtml";
            public const string TopLevelCHE = "TopLevelCHE.cshtml";
            public const string TopLevelCOL = "TopLevelCOL.cshtml";
            public const string TopLevelMinimumPER = "TopLevelMinimum.cshtml";
            public const string TopLevelMinimumCHE = "TopLevelMinimumCHE.cshtml";
            public const string TopLevelMinimumCOL = "TopLevelMinimumCOL.cshtml";
        }
        public struct Session
        {
            public const string Success = "Success";
        }
        public struct Query
        {
            public const string Page = "page";
            public const string Search = "search";
            public const string ProductKeyword = "productkeyword";
            public const string RecipeKeyword = "recipekeyword";
        }
        public struct Crop
        {
            public const string Hero_1903x550 = "Hero_1903x550";
            public const string Spotlight_760x600 = "Spotlight_760x600";
            public const string Recipe_250x180 = "Recipe_250x180";
            public const string BlogList_320x190 = "BlogList_320x190";
            public const string SocialShare_1200x630 = "SocialShare_1200x630";

            public const string Crop_1000x1000 = "1000x1000";
            public const string Crop_800x800 = "800x800";
            public const string Crop_800x484 = "800x484";
            public const string Crop_800x475 = "800x475";
        }
        public struct Path
        {
            public const string ApiSearchDataPath = "/App_Data/products/searchData/";
            public const string ApiSearchData_Categories = "/App_Data/products/searchData/categories.json";
            public const string ApiSearchData_FilterAttr = "/App_Data/products/searchData/filterAttributes.json";
            public const string ApiResponseSavePath = "/App_Data/products/incomingData/";
            //public const string ApiFoodProductSavePath = "/App_Data/products/foodproduct/";
            //public const string ApiResponseAllProductsFilePath = "/App_Data/products/response/~allproducts.json";
        }
        public struct ContentOne_Live
        {
            //NEW CREDS
            public const string BaseUrl = "https://content1-api.1worldsync.com";
            public const string SecretKey = "db501d76650740358c6167536d0731c4"; //"b5790cad04ec45f8bb328704df7453b3";
            public const string GLN = "0312463408768"; //"0072745000010";
            public const string AppId = "643x90tn"; //"o0fza0pe";

            public const string uriCount = $"/V1/product/count?timestamp=";
            public const string uriFetch = $"/V1/product/fetch?timestamp=~TIMESTAMP~&pageSize=~PAGESIZE~";

            //OLD CREDS
            //public const string ContentOneUrl = "https://marketplace.api.1worldsync.com";
            //public const string ContentOneAppId = "0e3e8582";
            //public const string ContentOneSecretKey = "f9efd62821c9adcac221625faba7ab90";
            //public const string FreeTextSearch = "informationProvider:0072745000010";
        }
        public struct ContentOne_PreProd
        {
            //NEW CREDS
            public const string BaseUrl = "https://content1-api.preprod.1worldsync.com";
            public const string SecretKey = "6084058ae4ce4deca05abf3dab436ed8";
            public const string GLN = "0072745000010";
            public const string AppId = "0oq080cc";

            public const string uriCount = $"/V1/product/count?timestamp=";
            public const string uriFetch = $"/V1/product/fetch?timestamp=~TIMESTAMP~&pageSize=~PAGESIZE~";

            //OLD CREDS
            //public const string ContentOneUrl = "https://marketplace.preprod.api.1worldsync.com";
            //public const string ContentOneAppId = "0ee06869";
            //public const string ContentOneSecretKey = "4d0beb012ce0548bc395f3c1d2d5bd4e";
            //public const string FreeTextSearch = "*";
        }
        public struct Site
        {
            public const string Cheney = "Cheney";
            public const string Coleman = "Coleman";
            public const string Perdue = "Perdue";
        }
        public struct Misc
        {
            public const string AppId = "appId";
            public const string ApplicationJson = "application/json";
            public const string CookingStatusFullyCooked = "CookingStatus-FullyCooked";
            public const string CookingStatusReadyToCook = "CookingStatus-ReadyToCook";
            public const string FreshFrozenFresh = "Fresh-Frozen-Fresh";
            public const string FreshFrozenFrozen = "Fresh-Frozen-Frozen";
            public const string Hashcode = "hashcode";
            public const string TargetMarket = "targetMarket";
            public const string TimeStampPattern = "yyyy-MM-ddTHH:mm:ssZ";
            public const string US = "US";
            public const string UserGLN = "userGLN";

        }
        public struct Property
        {
            public const string AddedSugar = "addedSugar";
            public const string AddedSugarPercent = "addedSugarPercent";
            public const string AddedSugarPercentPerMsr = "addedSugarPercentPerMsr";
            public const string AddedSugarPerMsr = "addedSugarPerMsr";
            public const string AdditionalImages = "additionalImages";
            public const string Allergens = "allergens";
            public const string AttributeBrand = "attributeBrand";
            public const string AttributeCookingStatus = "attributeCookingStatus";
            public const string AttributeFreshFrozen = "attributeFreshFrozen";
            public const string AttributePreparation = "attributePreparation";
            public const string AttributeProductTypes = "attributeProductTypes";
            public const string AttributeProtein = "attributeProtein";
            public const string Attributes = "attributes";
            public const string AveragePieceSize = "averagePieceSize";
            public const string BrandName = "BrandName";
            public const string Calcium = "calcium";
            public const string CalciumPercent = "calciumPercent";
            public const string CalciumPercentPerMsr = "calciumPercentPerMsr";
            public const string CalciumPerMsr = "calciumPerMsr";
            public const string Calories = "calories";
            public const string CaloriesFromFat = "caloriesFromFat";
            public const string CaloriesFromFatPerMsr = "caloriesFromFatPerMsr";
            public const string CaloriesPerMsr = "caloriesPerMsr";
            public const string CaseCube = "caseCube";
            public const string CasesPerPallet = "casesPerPallet";
            public const string Cholesterol = "cholesterol";
            public const string CholesterolPercent = "cholesterolPercent";
            public const string CholesterolPercentPerMsr = "cholesterolPercentPerMsr";
            public const string CholesterolPerMsr = "cholesterolPerMsr";
            public const string CookLevel = "cookLevel";
            public const string CustomScripts = "customScripts";
            public const string CustomStyles = "customStyles";
            public const string DietaryFiber = "dietaryFiber";
            public const string DietaryFiberPercent = "dietaryFiberPercent";
            public const string DietaryFiberPercentPerMsr = "dietaryFiberPercentPerMsr";
            public const string DietaryFiberPerMsr = "dietaryFiberPerMsr";
            public const string Dimensions = "dimensions";
            public const string FunctionalName = "functionalName";
            public const string GoogleAnalyticsUaCode = "GoogleAnalyticsUaCode";
            public const string GoogleTagManagerCode = "GoogleTagManagerCode";
            public const string Gtin = "gtin";
            public const string Ingredients = "ingredients";
            public const string Iron = "iron";
            public const string IronPercent = "ironPercent";
            public const string IronPercentPerMsr = "ironPercentPerMsr";
            public const string IronPerMsr = "ironPerMsr";
            public const string JsonData = "jsonData";
            public const string LastModified = "lastModified";
            public const string MaxCaseWeight = "maxCaseWeight";
            public const string MetaDescription = "MetaDescription";
            public const string MetaKeywords = "MetaKeywords";
            public const string MetaTitle = "MetaTitle";
            public const string MoreInfo = "moreInfo";
            public const string OpenGraphDescription = "OpenGraphDescription";
            public const string OpenGraphImage = "OpenGraphImage";
            public const string OpenGraphTitle = "OpenGraphTitle";
            public const string OpenGraphType = "OpenGraphType";
            public const string PageCssClass = "PageCssClass";
            public const string PalletTieHi = "palletTieHi";
            public const string Photo = "Photo";
            public const string PostCoverImage = "PostCoverImage";
            public const string Potassium = "potassium";
            public const string PotassiumPercent = "potassiumPercent";
            public const string PotassiumPercentPerMsr = "potassiumPercentPerMsr";
            public const string PotassiumPerMsr = "potassiumPerMsr";
            public const string PrimaryImageUrl = "primaryImageUrl";
            public const string ProductCode = "ProductCode";
            public const string Protein = "protein";
            public const string ProteinPercent = "proteinPercent";
            public const string ProteinPercentPerMsr = "proteinPercentPerMsr";
            public const string ProteinPerMsr = "proteinPerMsr";
            public const string RawAttributes = "rawAttributes";
            public const string RawBrand = "rawBrand";
            public const string RawCookingStatus = "rawCookingStatus";
            public const string RawFreshFrozen = "rawFreshFrozen";
            public const string RawPreparation = "rawPreparation";
            public const string RawProtein = "rawProtein";
            public const string SaturatedFat = "saturatedFat";
            public const string SaturatedFatPercent = "saturatedFatPercent";
            public const string SaturatedFatPercentPerMsr = "saturatedFatPercentPerMsr";
            public const string SaturatedFatPerMsr = "saturatedFatPerMsr";
            public const string ServingSize = "servingSize";
            public const string ServingSizeDescription = "servingSizeDescription";
            public const string ServingSizePerMsr = "servingSizePerMsr";
            public const string ServingsPerCase = "servingsPerCase";
            public const string ShelfLife = "shelfLife";
            public const string Site = "site";
            public const string SiteCanonicalDomain = "SiteCanonicalDomain";
            public const string SiteDescription = "SiteDescription";
            public const string SiteLogo = "siteLogo";
            public const string SiteMetaTitleFormat = "SiteMetaTitleFormat";
            public const string SiteTitle = "SiteTitle";
            public const string Sodium = "sodium";
            public const string SodiumPercent = "sodiumPercent";
            public const string SodiumPercentPerMsr = "sodiumPercentPerMsr";
            public const string SodiumPerMsr = "sodiumPerMsr";
            public const string StorageMethod = "storageMethod";
            public const string StorageTemperature = "storageTemperature";
            public const string Sugars = "sugars";
            public const string SugarsPerMsr = "sugarsPerMsr";
            public const string Title = "title";
            public const string TotalCarbohydrates = "totalCarbohydrates";
            public const string TotalCarbohydratesPercent = "totalCarbohydratesPercent";
            public const string TotalCarbohydratesPercentPerMsr = "totalCarbohydratesPercentPerMsr";
            public const string TotalCarbohydratesPerMsr = "totalCarbohydratesPerMsr";
            public const string TotalFat = "totalFat";
            public const string TotalFatPercent = "totalFatPercent";
            public const string TotalFatPercentPerMsr = "totalFatPercentPerMsr";
            public const string TotalFatPerMsr = "totalFatPerMsr";
            public const string TradeItemKeywords = "tradeItemKeywords";
            public const string TradeItemMarketingMessage = "tradeItemMarketingMessage";
            public const string TransFat = "transFat";
            public const string TransFatPerMsr = "transFatPerMsr";
            public const string UmbracoExtension = "umbracoExtension";
            public const string UmbracoSitemapHide = "umbracoSitemapHide";
            public const string VitaminAPercent = "vitaminAPercent";
            public const string VitaminAPercentPerMsr = "vitaminAPercentPerMsr";
            public const string VitaminCPercent = "vitaminCPercent";
            public const string VitaminCPercentPerMsr = "vitaminCPercentPerMsr";
            public const string VitaminD = "vitaminD";
            public const string VitaminDPercent = "vitaminDPercent";
            public const string VitaminDPercentPerMsr = "vitaminDPercentPerMsr";
            public const string VitaminDPerMsr = "vitaminDPerMsr";
            public const string Weight = "weight";

        }
        #endregion


        #region "Methods"
        public static string ConvertHexToRGB(string hexValue, decimal opacity = 1)
        {
            //Insure hex string contains #
            if (!hexValue.Contains("#"))
                hexValue = "#" + hexValue;

            System.Drawing.Color color = ColorTranslator.FromHtml(hexValue);
            int r = Convert.ToInt16(color.R);
            int g = Convert.ToInt16(color.G);
            int b = Convert.ToInt16(color.B);
            return string.Format("rgba({0}, {1}, {2}, {3})", r, g, b, opacity);
        }
        public static string ConvertRowPadding(string prevalue)
        {
            return "padding-" + prevalue.ToLower().Replace(" ", "-");
        }
        public static string ConvertRowMargin(string prevalue)
        {
            return "margin-" + prevalue.ToLower().Replace(" ", "-");
        }
        public static string GetImageSrcUrl(string ProductImage, int Width, int Height, string BgColorForPadding, UmbracoHelper Umbraco)
        {
            int quality = ObtainAPIImageQuality(Umbraco);
            return ObtainImageSrcUrl(ProductImage, Width, Height, BgColorForPadding, null, quality); //.Replace("/remote.axd", "https://");
        }
        private static string ObtainImageSrcUrl(string ProductImage, int Width, int Height, string BgColorForPadding, IEnumerable<KeyValuePair<string, string>> AdditionalParameters = null, int Quality = 100)
        {
            if (ProductImage == null)
            {
                return string.Empty;
            }
            else
            {
                try
                {
                    string text = ProductImage.Replace("https://", "");
                    string text2 = BgColorForPadding.Replace("#", "");
                    string text3 = "";
                    string text4 = "";
                    if (Width > 0 && Height > 0)
                    {
                        text3 = $"&width={Width}&height={Height}";
                    }
                    else if (Width == 0 && Height > 0)
                    {
                        text3 = $"&height={Height}";
                    }
                    else if (Width > 0 && Height == 0)
                    {
                        text3 = $"&width={Width}";
                    }

                    text4 = $"&quality={Quality}";
                    string text5 = "";
                    if (AdditionalParameters != null)
                    {
                        foreach (KeyValuePair<string, string> AdditionalParameter in AdditionalParameters)
                        {
                            string text6 = "&" + AdditionalParameter.Key + "=" + AdditionalParameter.Value;
                            text5 += text6;
                        }
                    }

                    return $"https://{text}?v=1{text3}{text4}&upscale=true&bgcolor={text2}{text5}";
                }
                catch
                {
                    return string.Empty;
                }
            }
        }


        private static int ObtainAPIImageQuality(UmbracoHelper Umbraco)
        {
            int result = 100;
            try
            {
                IPublishedContent publishedContent = (from x in Umbraco.ContentAtRoot()
                                                      where x.ContentType.Alias.Equals("SiteData")
                                                      select x).FirstOrDefault();
                IPublishedContent content = publishedContent.Children.Where((IPublishedContent x) => x.ContentType.Alias.Equals("owsImageQuality")).FirstOrDefault();
                result = content.Value<int>("imageAPIQuality");
            }
            catch
            {
            }

            return result;
        }

        public static Uri GetUri(HttpRequest request)
        {
            var builder = new UriBuilder();
            builder.Scheme = request.Scheme;
            builder.Host = request.Host.Value;
            builder.Path = request.Path;
            builder.Query = request.QueryString.ToUriComponent();
            return builder.Uri;
        }
        public static string GetHost(HttpRequest request)
        {
            return request.Host.Value;
        }

        public static string RemoveNonANCIICharacters(string str)
        {
            //Ensure only ANCII characters are used.  All others for removed.
            return Regex.Replace(str, @"[^\u0020-\u007E]", string.Empty);
        }
        public static string RemoveNonAlphaChar(string str)
        {
            //Using Array.FindAll() method
            //return String.Concat(Array.FindAll(str.ToCharArray(), Char.IsLetterOrDigit));

            //Using Regular Expression
            return Regex.Replace(str, "[^a-zA-Z0-9]", String.Empty);
        }

        public static string GetRootSite(IPublishedContent ipPage)
        {
            var ipRoot = ipPage.Root();
            return ipRoot.Value<string>(Property.Site) ?? "Perdue";
        }
        public static string SplitCamelCase(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
        }

        public static string ConvertAllergenCode(string code)
        {
            switch (code)
            {
                case "AC":
                    return "Crustaceans and their derivates";
                case "AE":
                    return "Eggs and their derivates";
                case "AF":
                    return "Fish and their derivates";
                case "AM":
                    return "Milk and its derivates";
                case "AN":
                    return "Nuts and their derivates";
                case "AP":
                    return "Peanuts and their derivates";
                case "AS":
                    return "Sesame Seeds and their derivates";
                case "AY":
                    return "Soybean and its Derivatives";
                case "UW":
                    return "Wheat and Their Derivatives";
                default:
                    return string.Empty;
            }
        }

        public static List<int> FindAllSustrings(string text, string searchText)
        {
            List<int> positions = [];
            var index = text.IndexOf(searchText, StringComparison.OrdinalIgnoreCase);

            while (index != -1)
            {
                positions.Add(index);
                index = text.IndexOf(searchText, index + searchText.Length, StringComparison.OrdinalIgnoreCase);
            }
            return positions;
        }
        #endregion
    }
}
