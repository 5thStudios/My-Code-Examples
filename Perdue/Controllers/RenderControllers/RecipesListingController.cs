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

using static Umbraco.Cms.Core.Constants.Web;
using System.Linq;
using www.Models.ProductTools;
using System.Runtime.CompilerServices;
using System.Text;
using www.Models.Blocklist;




namespace www.Controllers
{
    public class RecipesListingController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<RecipesListingController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly Umbraco.Cms.Core.Hosting.IHostingEnvironment _hostingEnvironment;


        public RecipesListingController(
                ILogger<RecipesListingController> _logger,
                ICompositeViewEngine compositeViewEngine,
                IUmbracoContextAccessor umbracoContextAccessor,
                UmbracoHelper _UmbracoHelper,
                IPublishedValueFallback publishedValueFallback,
                ServiceContext context,
                IVariationContextAccessor variationContextAccessor,
                Umbraco.Cms.Core.Hosting.IHostingEnvironment hostingEnvironment
             )
            : base(_logger, compositeViewEngine, umbracoContextAccessor)
        {
            _publishedValueFallback = publishedValueFallback;
            UmbracoHelper = _UmbracoHelper;
            logger = _logger;
            _serviceContext = context;
            _variationContextAccessor = variationContextAccessor;
            _hostingEnvironment = hostingEnvironment;
        }

        public override IActionResult Index()
        {
            //Instantiate variables
            RecipesListing cmPage = new RecipesListing(CurrentPage, _publishedValueFallback);
            RecipesListingViewModel vmRecipe = new RecipesListingViewModel()
            {
                PageUrl = cmPage.Url()
            };


            try
            {

                //Obtain search query if exists
                vmRecipe.SearchQuery = Request.Query[Common.Query.RecipeKeyword].ToString();
                vmRecipe.HasSearchQuery = !string.IsNullOrWhiteSpace(vmRecipe.SearchQuery);

                //Instantiate list
                List<IPublishedContent> LstRecipePgs = new List<IPublishedContent>();


                //Create list of recipies
                if (vmRecipe.HasSearchQuery)
                {
                    //Obtain list of recipies if matches
                    foreach (var ipRecipe in cmPage.Children)
                    {
                        var cmRecipe = new Recipe(ipRecipe, _publishedValueFallback);
                        if (cmRecipe.RelatedProducts != null)
                        {
                            foreach (var ipProduct in cmRecipe.RelatedProducts.Where(x => string.Equals(x.Value<string>(Common.Property.ProductCode), vmRecipe.SearchQuery, StringComparison.OrdinalIgnoreCase)).ToList())  //x.Value<string>(Common.Property.ProductCode) == vmRecipe.SearchQuery
                            {
                                if (!LstRecipePgs.Contains(ipRecipe))
                                    LstRecipePgs.Add(ipRecipe);
                            }
                        }
                    }


                    if (!LstRecipePgs.Any())
                    {
                        //Search in compiled keywords blob
                        foreach (var ipRecipe in cmPage.Children.OrderByDescending(x => x.SortOrder))
                        {
                            var kw = KeywordsBlob(new Recipe(ipRecipe, _publishedValueFallback));
                            if (KeywordsBlob(new Recipe(ipRecipe, _publishedValueFallback)).Contains(vmRecipe.SearchQuery, StringComparison.OrdinalIgnoreCase))
                                LstRecipePgs.Add(ipRecipe);
                        }
                    }
                }
                else
                {
                    //Obtain all recipies
                    LstRecipePgs = cmPage.Children.OrderByDescending(x => x.SortOrder).ToList();
                }





                //Obtain listings
                foreach (var ipRecipe in LstRecipePgs)
                {
                    //Instantiate variables
                    RecipeListing listing = new RecipeListing();
                    ContentModels.Recipe cmRecipe = new Recipe(ipRecipe, _publishedValueFallback);

                    //Obtain recipe data
                    listing.Title = cmRecipe.Name;
                    if (cmRecipe.Photo != null)
                    {
                        listing.Image = new Models.Link()
                        {
                            ImgUrl = cmRecipe.Photo.GetCropUrl(Common.Crop.Recipe_250x180),
                            Title = cmRecipe.Name
                        };
                    }
                    listing.Recipe = new Models.Link()
                    {
                        Url = cmRecipe.Url(),
                        Title = cmRecipe.Name
                    };

                    //Obtain recipe tags
                    if (cmRecipe.MealTypes != null)
                    {
                        foreach (string tag in cmRecipe.MealTypes)
                        {
                            string mealType = GetSearchAttributeClassName("MealType", tag);
                            if (!string.IsNullOrEmpty(mealType))
                            {
                                listing.LstAttribClasses.Add(mealType);

                                //Add tags to filter lists
                                if (!vmRecipe.LstTags_MealTypes.Keys.Contains(mealType))
                                    vmRecipe.LstTags_MealTypes.Add(mealType, MakeCodeSafe(MakeCamelCase(tag, false, true), " "));
                            }

                        }
                    }


                    if (cmRecipe.ProductCategories != null)
                    {
                        foreach (string tag in cmRecipe.ProductCategories)
                        {
                            string productCategory = GetSearchAttributeClassName("ProductCategory", tag);
                            if (!string.IsNullOrEmpty(productCategory))
                            {
                                listing.LstAttribClasses.Add(productCategory);

                                //Add tags to filter lists
                                if (!vmRecipe.LstTags_ProductCategories.Keys.Contains(productCategory))
                                    vmRecipe.LstTags_ProductCategories.Add(productCategory, MakeCamelCase(tag, false, true));
                                //vmRecipe.LstTags_ProductCategories.Add(productCategory, MakeCodeSafe(MakeCamelCase(tag, false, true), " "));
                            }

                        }
                    }


                    //Add recipe to list
                    vmRecipe.LstRecipeListings.Add(listing);
                }


                //Obtain Call2Action
                vmRecipe.Call2Action.ShowCall2Action = cmPage.ShowCall2Action;
                if (cmPage.ShowCall2Action)
                {
                    vmRecipe.Call2Action.ImgProduct = cmPage.ProductImage?.Url() ?? "";
                    vmRecipe.Call2Action.ImgCall2Action = cmPage.BackgroundImage?.Url() ?? "";
                    vmRecipe.Call2Action.LeadInText = cmPage.LeadInText;
                    vmRecipe.Call2Action.Msg = cmPage.Message;
                    vmRecipe.Call2Action.EmbedVimeoId = cmPage.EmbedVimeoId ?? "";
                    vmRecipe.Call2Action.EmbedVimeoTitle = cmPage.EmbedVimeoTitle ?? "";
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }


            var viewModel = new ComposedPageViewModel<RecipesListing, RecipesListingViewModel>
            {
                Page = cmPage,
                ViewModel = vmRecipe
            };

            return View(Common.View.RecipesListing, viewModel);
        }







        private string GetSearchAttributeClassName(string category, string tag)
        {
            string _category = MakeCamelCase(category);
            _category = MakeCodeSafe(_category, "");

            string _tag = MakeCamelCase(tag).Replace("-", "");
            _tag = MakeCodeSafe(_tag, "").Replace("-", "");

            _tag = _tag.Replace("®", "");
            if (_tag != "")
            {
                return $"{_category}-{_tag}";
            }

            return string.Empty;
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


        private string KeywordsBlob(ContentModels.Recipe cmRecipe)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(cmRecipe.Name);
            stringBuilder.AppendLine(cmRecipe.Description?.ToString());
            stringBuilder.AppendLine(string.Join(" ", AllRelatedProductCodes(cmRecipe)));
            stringBuilder.AppendLine(string.Join(" ", Ingredients(cmRecipe)));
            stringBuilder.AppendLine(string.Join(" ", Instructions(cmRecipe)));
            if (cmRecipe.MealTypes.Any()) stringBuilder.AppendLine(string.Join(" ", cmRecipe.MealTypes));
            if (cmRecipe.ProductCategories.Any()) stringBuilder.AppendLine(string.Join(" ", cmRecipe.ProductCategories));
            return stringBuilder.ToString();
        }


        private IEnumerable<string> AllRelatedProductCodes(ContentModels.Recipe cmRecipe)
        {
            List<string> list = new List<string>();
            if (cmRecipe.RelatedProducts != null)
            {
                foreach (var ipProduct in cmRecipe.RelatedProducts)
                {
                    if (ipProduct != null && ipProduct.IsPublished())
                    {
                        ContentModels.Product cmProduct = new ContentModels.Product(ipProduct, _publishedValueFallback);
                        if (string.IsNullOrEmpty(cmProduct.ProductCode))
                        {
                            list.Add(cmProduct.ProductCode ?? "");
                        }
                    }
                }
            }

            return Enumerable.Distinct<string>((IEnumerable<string>)list);
        }
        private IEnumerable<NcRecipeIngredient> Ingredients(ContentModels.Recipe cmRecipe)
        {
            return cmRecipe.Ingredients!.Select(x => x.Content).OfType<NcRecipeIngredient>();
        }
        private IEnumerable<NcRecipeInstruction> Instructions(ContentModels.Recipe cmRecipe)
        {
            return cmRecipe.Instructions!.Select(x => x.Content).OfType<NcRecipeInstruction>();
        }

    }
}
