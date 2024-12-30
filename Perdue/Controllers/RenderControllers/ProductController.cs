using Dragonfly.NetHelpers;
using Dragonfly.UmbracoServices;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using NPoco.fastJSON;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Controllers;
using www.Models;
using www.Models.PublishedModels;
using www.ViewModels;
using static System.Net.Mime.MediaTypeNames;
using ContentModels = www.Models.PublishedModels;
using Umbraco.Cms.Core.Models;
using Lucene.Net.Documents;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Cmp;
using www.Models.ProductTools;


namespace www.Controllers
{
    public class ProductController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<ProductController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;
        //private readonly Umbraco.Cms.Core.Hosting.IHostingEnvironment _hostingEnvironment;


        public ProductController(
                //Umbraco.Cms.Core.Hosting.IHostingEnvironment hostingEnvironment,
                ILogger<ProductController> _logger,
                ICompositeViewEngine compositeViewEngine,
                IUmbracoContextAccessor umbracoContextAccessor,
                UmbracoHelper _UmbracoHelper,
                IPublishedValueFallback publishedValueFallback,
                ServiceContext context,
                IVariationContextAccessor variationContextAccessor
             )
            : base(_logger, compositeViewEngine, umbracoContextAccessor)
        {
            _publishedValueFallback = publishedValueFallback;
            UmbracoHelper = _UmbracoHelper;
            logger = _logger;
            _serviceContext = context;
            _variationContextAccessor = variationContextAccessor;
            //_hostingEnvironment = hostingEnvironment;
        }




        public override IActionResult Index()
        {
            //Instantiate variables
            ContentModels.Product cmProduct = new ContentModels.Product(CurrentPage, _publishedValueFallback);
            ProductViewModel vmProduct = new ProductViewModel();


            try
            {
                //Determine which site this is being called from.
                switch (cmProduct.Root().Value<string>(Common.Property.Site))
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


                //
                vmProduct.Title = cmProduct.Title.Replace("®", "<sup>®</sup>");
                vmProduct.ProductCode = cmProduct.ProductCode;


                //Obtain brand logo
                vmProduct.ProductBrand = GetBrand(cmProduct);


                //Obtain all product images
                if (!string.IsNullOrWhiteSpace(cmProduct.PrimaryImageUrl))
                {
                    //Get primary img url
                    vmProduct.LstProductImgs.Add(new Umbraco.Cms.Core.Models.Link()
                    {
                        Url = Common.GetImageSrcUrl(cmProduct.PrimaryImageUrl, 1000, 1000, "#FFFFFF", UmbracoHelper)
                    });

                }
                if (cmProduct.AdditionalImages != null && cmProduct.AdditionalImages.Count() > 0)
                {
                    //Get secondary img urls
                    foreach (var img in cmProduct.AdditionalImages)
                    {
                        vmProduct.LstProductImgs.Add(new Umbraco.Cms.Core.Models.Link()
                        {
                            Url = Common.GetImageSrcUrl(img, 1000, 1000, "#FFFFFF", UmbracoHelper)
                        });
                    }
                }



                //Create list of attributes
                foreach (string? attribute in cmProduct.TradeItemKeywords)
                {
                    switch (attribute)
                    {
                        case "No Antibiotics Ever": //"No Antibiotics Ever / Antibiotic Free":
                            if (!vmProduct.LstAttributes.Contains("No Antibiotics Ever")) vmProduct.LstAttributes.Add("No Antibiotics Ever");
                            break;
                        case "100% Vegetarian Fed": //"100% Vegetarian Fed with No Animal By-Products":
                            if (!vmProduct.LstAttributes.Contains("100% Vegetarian Fed")) vmProduct.LstAttributes.Add("100% Vegetarian Fed");
                            break;
                        case "Organic": // "Organic / Non-GMO / Free Range":
                            if (!vmProduct.LstAttributes.Contains("Organic")) vmProduct.LstAttributes.Add("Organic");
                            break;
                        case "Gluten Free":
                            if (!vmProduct.LstAttributes.Contains("Gluten Free")) vmProduct.LstAttributes.Add("Gluten Free");
                            break;
                        case "Halal": //"Halal Certified":
                            if (!vmProduct.LstAttributes.Contains("Halal")) vmProduct.LstAttributes.Add("Halal");
                            break;
                        case "Child Nutrition": //"Child Nutrition Labeled":
                            if (!vmProduct.LstAttributes.Contains("Child Nutrition")) vmProduct.LstAttributes.Add("Child Nutrition");
                            break;
                        case "Lower Sodium":
                            if (!vmProduct.LstAttributes.Contains("Lower Sodium")) vmProduct.LstAttributes.Add("Lower Sodium");
                            //if (!vmProduct.LstAttributes.Contains("Lower Sodium<sup>&#8224;</sup>")) vmProduct.LstAttributes.Add("Lower Sodium<sup>&#8224;</sup>");
                            break;
                        case "Whole Grain":
                            if (!vmProduct.LstAttributes.Contains("Whole Grain")) vmProduct.LstAttributes.Add("Whole Grain");
                            break;
                        default:
                            break;
                    }
                }
                foreach (string? attribute in cmProduct.Attributes)
                {
                    if (attribute.Contains("Halal"))
                        if (!vmProduct.LstAttributes.Contains("Halal")) vmProduct.LstAttributes.Add("Halal");
                }



                //Marketting Message
                vmProduct.MarketingMessage = cmProduct.TradeItemMarketingMessage;


                //Featured Benefits
                //vmProduct.FeatureBenefit = vmProduct.FoodProduct.FeatureBenefit;


                vmProduct.EmailLink = string.Format("mailto:?subject={0}&body=Thought%20you%20might%20be%20interested%20in%20this%20product%3A%20{1}", vmProduct.Title, cmProduct.Url(mode: UrlMode.Absolute));


                IPublishedContent? ipContactUs = CurrentPage.Root().DescendantOfType(Common.Doctype.ContactUsPage);
                if (ipContactUs != null)
                    vmProduct.ContactUs = ipContactUs.Url();


                //Obtain Call2Action
                vmProduct.Call2Action.ShowCall2Action = cmProduct.ShowCall2Action;
                if (cmProduct.ShowCall2Action)
                {
                    vmProduct.Call2Action.ImgProduct = cmProduct.ProductImage?.Url() ?? "";
                    vmProduct.Call2Action.ImgCall2Action = cmProduct.BackgroundImage?.Url() ?? "";
                    vmProduct.Call2Action.LeadInText = cmProduct.LeadInText;
                    vmProduct.Call2Action.Msg = cmProduct.Message;
                    vmProduct.Call2Action.EmbedVimeoId = cmProduct.EmbedVimeoId ?? "";
                    vmProduct.Call2Action.EmbedVimeoTitle = cmProduct.EmbedVimeoTitle ?? "";
                }


                //Ingredients and Allergens
                vmProduct.IngredientStatement = cmProduct.Ingredients;
                foreach (var allergen in cmProduct.Allergens)
                {
                    vmProduct.LstAllergens.Add(allergen);
                }


                //Specifications
                vmProduct.Specification.ItemSize = cmProduct.AveragePieceSize;
                vmProduct.Specification.TradeUnitGtin = cmProduct.Gtin;
                vmProduct.Specification.WeightInfo = cmProduct.Weight;
                vmProduct.Specification.MaxWeightInfo = cmProduct.MaxCaseWeight;
                vmProduct.Specification.Dimensions = cmProduct.Dimensions;
                vmProduct.Specification.CubeDimensions = cmProduct.CaseCube;
                vmProduct.Specification.PerPallet = cmProduct.CasesPerPallet;
                vmProduct.Specification.PalletTieHie = cmProduct.PalletTieHi;


                //Handling
                vmProduct.Handling.Attribute = cmProduct.CookLevel ?? "";
                vmProduct.Handling.StorageMethod = cmProduct.StorageMethod ?? "";
                vmProduct.Handling.LifespanFromProduction = cmProduct.ShelfLife ?? "";
                vmProduct.Handling.productTempRange = cmProduct.StorageTemperature ?? "";



                //Per Serving
                vmProduct.PerServing.Servings.servingSize = cmProduct.ServingSize ?? "";
                vmProduct.PerServing.Servings.servingSizeDescription = cmProduct.ServingSizeDescription ?? "";
                vmProduct.PerServing.Servings.servingsPerCase = cmProduct.ServingsPerCase ?? "";

                vmProduct.PerServing.Calories.calories = cmProduct.Calories ?? "";
                vmProduct.PerServing.Calories.caloriesFromFat = cmProduct.CaloriesFromFat ?? "";

                vmProduct.PerServing.Fat.totalFat = cmProduct.TotalFat ?? "";
                vmProduct.PerServing.Fat.totalFatPercent = cmProduct.TotalFatPercent ?? "";
                vmProduct.PerServing.Fat.saturatedFat = cmProduct.SaturatedFat ?? "";
                vmProduct.PerServing.Fat.saturatedFatPercent = cmProduct.SaturatedFatPercent ?? "";
                vmProduct.PerServing.Fat.transFat = cmProduct.TransFat ?? "";

                vmProduct.PerServing.Cholesterol.cholesterol = cmProduct.Cholesterol ?? "";
                vmProduct.PerServing.Cholesterol.cholesterolPercent = cmProduct.CholesterolPercent ?? "";

                vmProduct.PerServing.Sodium.sodium = cmProduct.Sodium ?? "";
                vmProduct.PerServing.Sodium.sodiumPercent = cmProduct.SodiumPercent ?? "";

                vmProduct.PerServing.Carbohydrates.totalCarbohydrates = cmProduct.TotalCarbohydrates ?? "";
                vmProduct.PerServing.Carbohydrates.totalCarbohydratesPercent = cmProduct.TotalCarbohydratesPercent ?? "";
                vmProduct.PerServing.Carbohydrates.dietaryFiber = cmProduct.DietaryFiber ?? "";
                vmProduct.PerServing.Carbohydrates.dietaryFiberPercent = cmProduct.DietaryFiberPercent ?? "";
                vmProduct.PerServing.Carbohydrates.sugars = cmProduct.Sugars ?? "";
                vmProduct.PerServing.Carbohydrates.addedSugar = cmProduct.AddedSugar ?? "";
                vmProduct.PerServing.Carbohydrates.addedSugarPercent = cmProduct.AddedSugarPercent ?? "";

                vmProduct.PerServing.Protein.protein = cmProduct.Protein ?? "";
                vmProduct.PerServing.Protein.proteinPercent = cmProduct.ProteinPercent ?? "";

                vmProduct.PerServing.VitaminsMinerals.vitaminAPercent = cmProduct.VitaminApercent ?? "";
                vmProduct.PerServing.VitaminsMinerals.vitaminCPercent = cmProduct.VitaminCpercent ?? "";
                vmProduct.PerServing.VitaminsMinerals.vitaminD = cmProduct.VitaminD ?? "";
                vmProduct.PerServing.VitaminsMinerals.vitaminDPercent = cmProduct.VitaminDpercent ?? "";
                vmProduct.PerServing.VitaminsMinerals.calcium = cmProduct.Calcium ?? "";
                vmProduct.PerServing.VitaminsMinerals.calciumPercent = cmProduct.CalciumPercent ?? "";
                vmProduct.PerServing.VitaminsMinerals.iron = cmProduct.Iron ?? "";
                vmProduct.PerServing.VitaminsMinerals.ironPercent = cmProduct.IronPercent ?? "";
                vmProduct.PerServing.VitaminsMinerals.potassium = cmProduct.Potassium ?? "";
                vmProduct.PerServing.VitaminsMinerals.potassiumPercent = cmProduct.PotassiumPercent ?? "";




                //Per Measure
                vmProduct.PerMeasure.Servings.servingSize = cmProduct.ServingSizePerMsr ?? "";

                vmProduct.PerMeasure.Calories.calories = cmProduct.CaloriesPerMsr ?? "";
                vmProduct.PerMeasure.Calories.caloriesFromFat = cmProduct.CaloriesFromFatPerMsr ?? "";

                vmProduct.PerMeasure.Fat.totalFat = cmProduct.TotalFatPerMsr ?? "";
                vmProduct.PerMeasure.Fat.totalFatPercent = cmProduct.TotalFatPercentPerMsr ?? "";
                vmProduct.PerMeasure.Fat.saturatedFat = cmProduct.SaturatedFatPerMsr ?? "";
                vmProduct.PerMeasure.Fat.saturatedFatPercent = cmProduct.SaturatedFatPercentPerMsr ?? "";
                vmProduct.PerMeasure.Fat.transFat = cmProduct.TransFatPerMsr ?? "";

                vmProduct.PerMeasure.Cholesterol.cholesterol = cmProduct.CholesterolPerMsr ?? "";
                vmProduct.PerMeasure.Cholesterol.cholesterolPercent = cmProduct.CholesterolPercentPerMsr ?? "";

                vmProduct.PerMeasure.Sodium.sodium = cmProduct.SodiumPerMsr ?? "";
                vmProduct.PerMeasure.Sodium.sodiumPercent = cmProduct.SodiumPercentPerMsr ?? "";

                vmProduct.PerMeasure.Carbohydrates.totalCarbohydrates = cmProduct.TotalCarbohydratesPerMsr ?? "";
                vmProduct.PerMeasure.Carbohydrates.totalCarbohydratesPercent = cmProduct.TotalCarbohydratesPercentPerMsr ?? "";
                vmProduct.PerMeasure.Carbohydrates.dietaryFiber = cmProduct.DietaryFiberPerMsr ?? "";
                vmProduct.PerMeasure.Carbohydrates.dietaryFiberPercent = cmProduct.DietaryFiberPercentPerMsr ?? "";
                vmProduct.PerMeasure.Carbohydrates.sugars = cmProduct.SugarsPerMsr ?? "";
                vmProduct.PerMeasure.Carbohydrates.addedSugar = cmProduct.AddedSugarPerMsr ?? "";
                vmProduct.PerMeasure.Carbohydrates.addedSugarPercent = cmProduct.AddedSugarPercentPerMsr ?? "";

                vmProduct.PerMeasure.Protein.protein = cmProduct.ProteinPerMsr ?? "";
                vmProduct.PerMeasure.Protein.proteinPercent = cmProduct.ProteinPercentPerMsr ?? "";

                vmProduct.PerMeasure.VitaminsMinerals.vitaminAPercent = cmProduct.VitaminApercentPerMsr ?? "";
                vmProduct.PerMeasure.VitaminsMinerals.vitaminCPercent = cmProduct.VitaminCpercentPerMsr ?? "";
                vmProduct.PerMeasure.VitaminsMinerals.vitaminD = cmProduct.VitaminDperMsr ?? "";
                vmProduct.PerMeasure.VitaminsMinerals.vitaminDPercent = cmProduct.VitaminDpercentPerMsr ?? "";
                vmProduct.PerMeasure.VitaminsMinerals.calcium = cmProduct.CalciumPerMsr ?? "";
                vmProduct.PerMeasure.VitaminsMinerals.calciumPercent = cmProduct.CalciumPercentPerMsr ?? "";
                vmProduct.PerMeasure.VitaminsMinerals.iron = cmProduct.IronPerMsr ?? "";
                vmProduct.PerMeasure.VitaminsMinerals.ironPercent = cmProduct.IronPercentPerMsr ?? "";
                vmProduct.PerMeasure.VitaminsMinerals.potassium = cmProduct.PotassiumPerMsr ?? "";
                vmProduct.PerMeasure.VitaminsMinerals.potassiumPercent = cmProduct.PotassiumPercentPerMsr ?? "";



                //Obtain last modified date
                //vmProduct.LastModified = cmProduct.LastModified.ToString("MMMM d, yyyy");


                //Obtain last modified date
                vmProduct.LastModified = DateTime.Today.ToString("MMMM d, yyyy");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }


            var viewModel = new ComposedPageViewModel<ContentModels.Product, ProductViewModel>
            {
                Page = cmProduct,
                ViewModel = vmProduct
            };

            return View(Common.View.Product, viewModel);
        }


        private Brand GetBrand(ContentModels.Product cmProduct) //FoodProduct FP, IPublishedContent CurrentPage)
        {
            Brand brand = null;
            var brandsListingPage = GetBrandsListingPage(cmProduct);
            if (brandsListingPage == null)
            {
                return null;
            }

            //Create list of brands in dictionary
            IPublishedContent defaultBrand = brandsListingPage.DefaultBrand;
            IEnumerable<Brand> allBrands = GetAllBrands(cmProduct);
            Dictionary<string, Brand> dictionary = new Dictionary<string, Brand>();
            foreach (Brand item in allBrands)
            {
                string[] array = item.RelatedProductBrands.Split(',');
                string[] array2 = array;
                foreach (string text in array2)
                {
                    if (!dictionary.ContainsKey(text.Trim()))
                    {
                        dictionary.Add(text.Trim(), item);
                    }
                }
            }

            try
            {
                brand = dictionary[cmProduct.BrandName];
            }
            catch
            {
                string message = $"No brand matches Product BrandName '{cmProduct.BrandName}' (ProductCode: {cmProduct.ProductCode}). (Called for page '{cmProduct.Name}' #{cmProduct.Id})";
                //LogHelper.Warn<FoodProduct>(message, Array.Empty<Func<object>>());
                logger.LogWarning(message, Array.Empty<Func<object>>());
            }


            if (brand != null)
            {
                return brand;
            }

            return defaultBrand as Brand;
        }
        private BrandsListing GetBrandsListingPage(IPublishedContent CurrentPage)
        {
            //
            ContentModels.Home cmHome = new ContentModels.Home(CurrentPage.Root(), _publishedValueFallback);
            if (cmHome != null)
            {
                //List<IPublishedContent> lstBrands = cmHome.Descendants("BrandsListing").ToList();
                IPublishedContent ipBrandsListing = cmHome.Descendants().FirstOrDefault(x => x.ContentType.Alias == "BrandsListing");

                return ipBrandsListing as BrandsListing;
            }

            return null;
        }
        private IEnumerable<Brand> GetAllBrands(IPublishedContent CurrentPage)
        {
            //
            ContentModels.Home site = new ContentModels.Home(CurrentPage.Root(), _publishedValueFallback);
            if (site != null)
            {
                IEnumerable<IPublishedContent> source = site.Descendants("BrandsListing");
                if (source.Any())
                {
                    var lst = source.First().Descendants("Brand");
                    return ToBrands(lst);
                }
            }

            return new List<Brand>();
        }
        private IEnumerable<Brand> ToBrands(IEnumerable<IPublishedContent> Content)
        {
            List<Brand> list = new List<Brand>();
            foreach (IPublishedContent item2 in Content)
            {
                if (item2 != null && item2.ContentType.Alias == "Brand")
                {
                    Brand item = new Brand(item2, _publishedValueFallback);
                    list.Add(item);
                }
            }

            return list;
        }




    }
}