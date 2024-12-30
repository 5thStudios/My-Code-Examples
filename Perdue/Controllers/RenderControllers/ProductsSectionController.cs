using Dragonfly.NetHelpers;
using Dragonfly.UmbracoServices;
using Examine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Controllers;
using www.Models;
using www.Models.ApiResponse;
using www.Models.PublishedModels;
using www.ViewModels;
using static Umbraco.Cms.Core.Constants;
using ContentModels = www.Models.PublishedModels;


namespace www.Controllers
{
    public class ProductsSectionController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<ProductsSectionController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly Umbraco.Cms.Core.Hosting.IHostingEnvironment _hostingEnvironment;
        private readonly IExamineManager ExamineManager;
        private readonly IPublishedContentQuery publishedContentQuery;


        public ProductsSectionController(
                ILogger<ProductsSectionController> _logger,
                IPublishedContentQuery _publishedContentQuery,
                Umbraco.Cms.Core.Hosting.IHostingEnvironment hostingEnvironment,
                IExamineManager _ExamineManager,
                ICompositeViewEngine compositeViewEngine,
                IUmbracoContextAccessor umbracoContextAccessor,
                UmbracoHelper _UmbracoHelper,
                IPublishedValueFallback publishedValueFallback,
                ServiceContext context
             )
            : base(_logger, compositeViewEngine, umbracoContextAccessor)
        {
            _publishedValueFallback = publishedValueFallback;
            UmbracoHelper = _UmbracoHelper;
            logger = _logger;
            _serviceContext = context;
            _hostingEnvironment = hostingEnvironment;
            ExamineManager = _ExamineManager ?? throw new ArgumentNullException(nameof(_ExamineManager));
            publishedContentQuery = _publishedContentQuery;
        }


        public override IActionResult Index()
        {
            //Instantiate variables
            ProductsSection cmPage = new ProductsSection(CurrentPage, _publishedValueFallback);
            ProductsSectionViewModel vmProduct = new ProductsSectionViewModel();
            FileHelperService FileHelper = new FileHelperService(_hostingEnvironment);


            try
            {
                //Determine which site this is being called from.
                switch (cmPage.Root().Value<string>(Common.Property.Site))
                {
                    case Common.Site.Perdue:
                        vmProduct.IsPerdue = true;
                        break;
                    case Common.Site.Coleman:
                        vmProduct.IsColeman = true;
                        break;
                    case Common.Site.Cheney:
                        vmProduct.IsCheney = true;
                        break;
                    default:
                        break;
                }

                //Obtain search query if exists
                vmProduct.SearchQuery = Request.Query[Common.Query.ProductKeyword].ToString();
                vmProduct.HasSearchQuery = !string.IsNullOrWhiteSpace(vmProduct.SearchQuery);


                //Instantiate list
                List<IPublishedContent> LstProductPgs = new List<IPublishedContent>();


                if (vmProduct.HasSearchQuery)
                {
                    if (cmPage.Children.Where(x => x.ContentType.Alias == "Product" && string.Equals(x.Value<string>(Common.Property.ProductCode), vmProduct.SearchQuery, StringComparison.OrdinalIgnoreCase)).Count() == 1)
                    {
                        //Redirect to single page
                        var productUrl = cmPage.Children?.FirstOrDefault(x => x.ContentType.Alias == "Product" &&
                            string.Equals(x.Value<string>(Common.Property.ProductCode), vmProduct.SearchQuery, StringComparison.OrdinalIgnoreCase))?.Url() ?? "";

                        Response.Redirect(productUrl);
                    }
                    else
                    {
                        /*
                         -get list of all json files
                        -search each for keywordblob that contains query
                        -get product code of each into list
                        -convert list into list of ipublishedcontent into LstProductPgs
                         */


                        if (ExamineManager.TryGetIndex(UmbracoIndexes.ExternalIndexName, out IIndex? index))
                        {
                            //Update query with quotes to allow non-exact searches.  [Without the quotes, only exact words can be found]
                            string searchQuery = "\"" + vmProduct.SearchQuery + "\"";


                            //Query
                            var queryExecutor = index.Searcher
                                                .CreateQuery(IndexTypes.Content)
                                                .ManagedQuery(searchQuery); //.And().GroupedNot(new[] { "nodeTypeAlias" }, lstExcludedAliases.ToArray());



                            //Loop through list of results | GET PRODUCTS ONLY to ensure products are 1st in list
                            List<string> lst = new List<string>();
                            foreach (var result in publishedContentQuery.Search(queryExecutor))
                            {
                                try
                                {
                                    //Provide results IF page is contained in root site (Searches only within site that search originated in.)
                                    if (result.Content.Path.Contains("-1," + cmPage.Root().Id))
                                    {
                                        //Add to list of pgs
                                        if (result.Content.ContentType.Alias == Common.Doctype.Product)
                                        {
                                            //LstProductPgs.Add(result.Content);



                                            // JF | This allows us to ensure that we skip the "JsonData" property during a search in a product.  Would be better if in query, but cannot get it to work.
                                            foreach (var ipProperty in result.Content.Properties)
                                            {
                                                if (ipProperty.Alias == Common.Property.JsonData)
                                                {
                                                    continue;
                                                }

                                                var source = ipProperty.GetSourceValue();
                                                if (source != null)
                                                {
                                                    if (source.ToString().ToLower().Contains(vmProduct.SearchQuery.ToLower()))
                                                    {
                                                        LstProductPgs.Add(result.Content);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                }
                else
                {
                    //Obtain all products
                    LstProductPgs = cmPage.Children.Where(x => x.ContentType.Alias == "Product").OrderBy(x => x.Value<int>(Common.Property.ProductCode)).ToList(); //.OrderByDescending(x => x.SortOrder).ToList();
                }


                //Obtain product json file
                vmProduct.FoodProductTypes = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.FoodProductTypes>(FileHelper.GetTextFileContents(Common.Path.ApiSearchData_Categories));



                //Get fallback
                vmProduct.PublishedValueFallback = _publishedValueFallback;

                //Get Website brands
                vmProduct.WebsiteBrands = GetWebsiteBrands();


                //Obtain listings
                if (LstProductPgs != null)
                {
                    foreach (var ipProduct in LstProductPgs) //cmPage.Children.Where(x => x.ContentType.Alias == "Product").OrderByDescending(x => x.SortOrder))
                    {
                        //Instantiate variables
                        ViewModels.ProductListing listing = new ViewModels.ProductListing();
                        if (ipProduct == null) { continue; }
                        ContentModels.Product cmProduct = new Product(ipProduct, _publishedValueFallback);

                        //Skip if missing product code
                        if (cmProduct == null) { continue; }
                        if (string.IsNullOrEmpty(cmProduct.ProductCode)) continue;


                        //Obtain product title
                        //var prodDisplayTitle = cmProduct.Title?.TruncateAtWord(104);


                        //Obtain product image
                        var imgUrl = "";
                        if (string.IsNullOrWhiteSpace(cmProduct.PrimaryImageUrl))
                        {
                            //Primary is empty.  Attempt to get a secondary image.
                            if (cmProduct.AdditionalImages != null && cmProduct.AdditionalImages.Count() > 0)
                            {
                                imgUrl = Common.GetImageSrcUrl(cmProduct?.AdditionalImages?.FirstOrDefault() ?? "", 250, 180, "#f8f9fa", UmbracoHelper); //cmProduct.Photo.GetCropUrl(Common.Crop.Recipe_250x180),
                            }
                        }
                        else
                        {
                            //Get primary img url
                            imgUrl = Common.GetImageSrcUrl(cmProduct.PrimaryImageUrl, 250, 180, "#f8f9fa", UmbracoHelper); //cmProduct.Photo.GetCropUrl(Common.Crop.Recipe_250x180),
                        }


                        //Use default img if still null
                        if (string.IsNullOrWhiteSpace(imgUrl))
                            imgUrl = "/images/grey-hero.png";


                        //Create product link
                        listing.Product = new Models.Link()
                        {
                            Url = cmProduct.Url(),
                            Title = string.Format("{0}<br/>({1})", cmProduct.Title?.TruncateAtWord(104), cmProduct.ProductCode),
                            ImgUrl = imgUrl
                        };



                        //  ATTRIBUTES
                        //Get search attribute class names
                        foreach (var attrib in cmProduct.Attributes)
                        {
                            listing.LstAttribClasses.Add(attrib);
                        }
                        if (!string.IsNullOrWhiteSpace(cmProduct.AttributePreparation)) listing.LstAttribClasses.Add(cmProduct.AttributePreparation);
                        if (!string.IsNullOrWhiteSpace(cmProduct.AttributeCookingStatus)) listing.LstAttribClasses.Add(cmProduct.AttributeCookingStatus);
                        if (!string.IsNullOrWhiteSpace(cmProduct.AttributeFreshFrozen)) listing.LstAttribClasses.Add(cmProduct.AttributeFreshFrozen);
                        if (!string.IsNullOrWhiteSpace(cmProduct.AttributeProtein)) listing.LstAttribClasses.Add(cmProduct.AttributeProtein);
                        if (!string.IsNullOrWhiteSpace(cmProduct.AttributeBrand)) listing.LstAttribClasses.Add(cmProduct.AttributeBrand);

                        //  CATEGORY & SUBCATEGORY
                        foreach (var attrib in cmProduct.AttributeProductTypes)
                        {
                            listing.LstAttribClasses.Add(attrib);
                        }



                        //Add product to list
                        vmProduct.LstProductListings.Add(listing);
                    }
                }



                //Obtain Filter Attributes
                List<www.ReadApi.FilterAttribute>? LstFilterAttributes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<www.ReadApi.FilterAttribute>>(FileHelper.GetTextFileContents(Common.Path.ApiSearchData_FilterAttr));


                //  PROTEIN
                //Obtain Protein Attributes
                List<www.ReadApi.FilterAttribute>? lstFilterAttributes_Proteins = LstFilterAttributes?.Where(n => n.Category == "Proteins").OrderBy(m => m.Attribute).ToList();
                foreach (var filterAttr in lstFilterAttributes_Proteins)
                {
                    string proteinClass = GetSearchAttributeClassName(filterAttr.Category, filterAttr.Attribute);
                    vmProduct.LstTags_Proteins.Add(MakeCodeSafe(MakeCamelCase(proteinClass, false, true), " "), filterAttr.Attribute);
                }


                //  BRAND
                //Obtain Brand Attributes
                List<www.ReadApi.FilterAttribute> lstFilterAttributes_Brands = LstFilterAttributes?.Where(n => n.Category == "Brand").ToList().OrderBy(m => m.Attribute).ToList();
                foreach (var filterAttr in lstFilterAttributes_Brands)
                {
                    string brandClass = GetSearchAttributeClassName(filterAttr.Category, filterAttr.Attribute);
                    vmProduct.LstTags_Brands.Add(MakeCodeSafe(MakeCamelCase(brandClass, false, true), " "), filterAttr.Attribute);
                }



                //Obtain Product Type Attributes
                List<www.ReadApi.FilterAttribute> lstFilterAttributes_Attributes = LstFilterAttributes?.Where(n => n.Category == "Attributes").ToList();
                List<www.ReadApi.FilterAttribute> attribsCookingStatus = LstFilterAttributes?.Where(n => n.Category == "Cooking Status").ToList();
                List<www.ReadApi.FilterAttribute> attribsAttributes = LstFilterAttributes?.Where(n => n.Category == "Attributes").OrderBy(n => n.Attribute).ToList();
                List<www.ReadApi.FilterAttribute> attribsFreshFrozen = LstFilterAttributes?.Where(n => n.Category == "Fresh-Frozen").ToList();

                vmProduct.LstTags_Attributes = attribsFreshFrozen;
                vmProduct.LstTags_Attributes?.AddRange(attribsAttributes);
                vmProduct.LstTags_Attributes?.AddRange(attribsCookingStatus);


            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }


            var viewModel = new ComposedPageViewModel<ProductsSection, ProductsSectionViewModel>
            {
                Page = cmPage,
                ViewModel = vmProduct
            };

            return View(Common.View.ProductsSection, viewModel);
        }






        #region Methods
        private string GetSearchAttributeClassName(string ItemCategory, string ItemAttribute, bool AlreadyCoded = false)
        {
            //string text = "";
            string arg = ItemCategory.MakeCamelCase().MakeCodeSafe("");
            string text2 = (AlreadyCoded ? ItemAttribute : ItemAttribute.MakeCamelCase().MakeCodeSafe("").Replace("-", ""));  //WHY????  What's the purpose of the 'if' here???
            if (text2 == null)
            {
                text2 = "";
            }

            text2 = text2.Replace("®", "").Replace("/", "-");
            if (text2 != "")
            {
                return $"{arg}-{text2}";
            }

            return $"{arg}";
        }

        private string MakeCamelCase(string Original, bool LowercaseAbbreviations = false, bool preserveSpaces = false)
        {
            string text = "";
            string[] array = Original.Split(' ');
            foreach (string text2 in array)
            {
                if (preserveSpaces)
                {
                    text = ((!LowercaseAbbreviations) ? (text + " " + Capitalize(text2)) : (text + " " + Capitalize(text2.ToLower())));
                }
                else
                {
                    text = ((!LowercaseAbbreviations) ? (text + Capitalize(text2)) : (text + Capitalize(text2.ToLower())));
                }
            }

            return text.Trim();
        }
        private string MakeCodeSafe(string StringToFix, string WordSeparator = "-", bool ConvertNumbersToWords = false) //, bool preserveSpaces = false)
        {
            string stringToFix = StringToFix;
            if (ConvertNumbersToWords)
            {
                stringToFix = NumeralsToWords(stringToFix);
            }

            stringToFix = ReplaceBadChars(stringToFix, WordSeparator);
            //if (!preserveSpaces)
            //{
            //    stringToFix = stringToFix.Replace(" ", "");
            //}
            if (WordSeparator != "")
            {
                string text = WordSeparator + WordSeparator;
                bool flag = stringToFix.Contains(text);
                do
                {
                    stringToFix = stringToFix.Replace(text, WordSeparator);
                }
                while (stringToFix.Contains(text));
            }

            return stringToFix.Trim();
        }
        private string NumeralsToWords(string StringToFix, bool DoComplexReplacements = true, bool Capitalize = true)
        {
            string text = StringToFix;
            if (DoComplexReplacements)
            {
                text = text.Replace("1/2", "Half");
                text = text.Replace("1/3", "OneThird");
                text = text.Replace("2/3", "TwoThirds");
                text = text.Replace("1/4", "OneFourth");
                text = text.Replace("3/4", "ThreeFourths");
                text = text.Replace("1/5", "OneFifth");
                text = text.Replace("2/5", "TwoFifths");
                text = text.Replace("3/5", "ThreeFifths");
                text = text.Replace("4/5", "FourFifths");
                text = text.Replace("1/6", "OneSixth");
                text = text.Replace("5/6", "FiveSixths");
                text = text.Replace("1/7", "OneSeventh");
                text = text.Replace("2/7", "TwoSevenths");
                text = text.Replace("3/7", "ThreeSevenths");
                text = text.Replace("4/7", "FourSevenths");
                text = text.Replace("5/7", "FiveSevenths");
                text = text.Replace("6/7", "SixSevenths");
                text = text.Replace("1/8", "OneEighth");
                text = text.Replace("3/8", "ThreeEighths");
                text = text.Replace("5/8", "FiveEighths");
                text = text.Replace("7/8", "Eighths");
                text = text.Replace("1/9", "OneNinth");
                text = text.Replace("2/9", "TwoNinths");
                text = text.Replace("4/9", "FourNinths");
                text = text.Replace("5/9", "FiveNinths");
                text = text.Replace("7/9", "SevenNinths");
                text = text.Replace("8/9", "EightNinths");
                text = text.Replace("1/10", "OneTenth");
                text = text.Replace("2/10", "TwoTenths");
                text = text.Replace("3/10", "ThreeTenths");
                text = text.Replace("4/10", "FourTenths");
                text = text.Replace("6/10", "SixTenths");
                text = text.Replace("7/10", "SevenTenths");
                text = text.Replace("8/10", "EightTenths");
                text = text.Replace("9/10", "NineTenths");
                text = text.Replace("1/11", "OneEleventh");
                text = text.Replace("2/11", "TwoElevenths");
                text = text.Replace("3/11", "ThreeElevenths");
                text = text.Replace("4/11", "FourElevenths");
                text = text.Replace("5/11", "FiveElevenths");
                text = text.Replace("6/11", "SixElevenths");
                text = text.Replace("7/11", "SevenElevenths");
                text = text.Replace("8/11", "EightElevenths");
                text = text.Replace("9/11", "NineElevenths");
                text = text.Replace("10/11", "TenElevenths");
                text = text.Replace("1/12", "OneTwelfth");
                text = text.Replace("5/12", "FiveTwelfths");
                text = text.Replace("7/12", "SevenTwelfths");
                text = text.Replace("11/12", "ElevenTwelfths");
            }

            text = text.Replace("0", "Zero");
            text = text.Replace("1", "One");
            text = text.Replace("2", "Two");
            text = text.Replace("3", "Three");
            text = text.Replace("4", "Four");
            text = text.Replace("5", "Five");
            text = text.Replace("6", "Six");
            text = text.Replace("7", "Seven");
            text = text.Replace("8", "Eight");
            text = text.Replace("9", "Nine");
            if (!Capitalize)
            {
                text = text.ToLower();
            }

            return text;
        }
        private string Capitalize(string Word)
        {
            string text = "";
            char[] array = Word.ToCharArray();
            bool flag = true;
            char[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                char c = array2[i];
                if (flag)
                {
                    text += c.ToString().ToUpper();
                    flag = false;
                }
                else
                {
                    text += c;
                }
            }

            return text;
        }
        private string ReplaceBadChars(string StringToFix, string WordSeparator = "-")
        {
            return StringToFix
                .Replace("'", "")
                .Replace("[", "")
                .Replace("]", "")
                .Replace("\"", WordSeparator)
                .Replace("/", WordSeparator)
                .Replace("®", "")
                .Replace("%", "")
                .Replace(".", WordSeparator)
                .Replace(";", WordSeparator)
                .Replace(":", WordSeparator)
                .Replace("#", "")
                .Replace("+", "")
                .Replace("*", "")
                .Replace("&", "and")
                .Replace("?", "")
                .Replace("*", "")
                .Replace("æ", "ae")
                .Replace("ø", "oe")
                .Replace("å", "aa")
                .Replace("ä", "ae")
                .Replace("Ä", "ae")
                .Replace("ö", "oe")
                .Replace("ü", "ue")
                .Replace("ß", "ss")
                .Replace("Ö", "oe")
                .Replace(" ", WordSeparator);
        }


        private List<string> GetWebsiteBrands()
        {
            var brandNodes = GetAllBrands();
            var brandList = brandNodes.SelectMany(n => n.RelatedProductBrands.Split(',').ToList());
            return brandList.ToList();
        }
        private IEnumerable<Brand> GetAllBrands()
        {
            IPublishedContent ipBrandsListing = CurrentPage!.Children.FirstOrDefault(x => x.ContentType.Alias == "BrandsListing");

            if (ipBrandsListing != null)
            {
                List<Brand> lstBrands = new List<Brand>();
                foreach (IPublishedContent ipBrand in ipBrandsListing.Children.Where(x => x.ContentType.Alias == "Brand"))
                {
                    if (ipBrand != null && ipBrand.ContentType.Alias == "Brand")
                    {
                        Brand item = new Brand(ipBrand, _publishedValueFallback);
                        lstBrands.Add(item);
                    }
                }
                return lstBrands;
            }

            return new List<Brand>();
        }
        #endregion

    }
}