using Dragonfly.NetHelpers;
using Examine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Text;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Controllers;
using www.Models;
using www.Models.ProductTools;
using www.Models.PublishedModels;
using www.ViewModels;
using static Umbraco.Cms.Core.Constants;
using static Umbraco.Cms.Core.Constants.HttpContext;




namespace www.Controllers
{
    public class SearchPageController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<SearchPageController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly IExamineManager ExamineManager;
        private readonly IPublishedContentQuery publishedContentQuery;


        public SearchPageController(
                ILogger<SearchPageController> _logger,
                IExamineManager _ExamineManager,
                ICompositeViewEngine compositeViewEngine,
                IUmbracoContextAccessor umbracoContextAccessor,
                UmbracoHelper _UmbracoHelper,
                IPublishedValueFallback publishedValueFallback,
                ServiceContext context,
                IVariationContextAccessor variationContextAccessor,
                IPublishedContentQuery _publishedContentQuery,
                IHtmlHelper _htmlHelper
             )
            : base(_logger, compositeViewEngine, umbracoContextAccessor)
        {
            _publishedValueFallback = publishedValueFallback;
            ExamineManager = _ExamineManager ?? throw new ArgumentNullException(nameof(_ExamineManager));
            UmbracoHelper = _UmbracoHelper;
            logger = _logger;
            _serviceContext = context;
            _variationContextAccessor = variationContextAccessor;
            publishedContentQuery = _publishedContentQuery;
        }




        /* EXAMINE GUIDES
         * https://shazwazza.com/categories#tag-Examine
         * https://docs.umbraco.com/umbraco-cms/reference/searching/examine/quick-start#getting-the-content
         * https://docs.umbraco.com/umbraco-cms/reference/querying/ipublishedcontentquery
         * */

        public override IActionResult Index()
        {
            //Instantiate variables
            SearchPageViewModel vmSearchPage = new SearchPageViewModel();
            List<SearchResultItem> LstResults = new List<SearchResultItem>();
            var cmPage = new SearchPage(CurrentPage, _publishedValueFallback);
            vmSearchPage.SearchQuery = Request.Query[Common.Query.Search];



            try
            {
                if (!string.IsNullOrWhiteSpace(vmSearchPage.SearchQuery))
                {
                    if (ExamineManager.TryGetIndex(UmbracoIndexes.ExternalIndexName, out IIndex? index))
                    {
                        //Doctypes to exclude from search
                        List<string> lstExcludedAliases = new List<string>();
                        lstExcludedAliases.Add(Common.Doctype.ContactUsPage);
                        lstExcludedAliases.Add(Common.Doctype.NavigationGroup);
                        lstExcludedAliases.Add(Common.Doctype.GroupColumn);
                        lstExcludedAliases.Add(Common.Doctype.ErrorPage);
                        lstExcludedAliases.Add(Common.Doctype.GoogleSitemap);
                        lstExcludedAliases.Add(Common.Doctype.RedirectPage);
                        lstExcludedAliases.Add(Common.Doctype.SearchPage);
                        lstExcludedAliases.Add(Common.Doctype.Home);
                        lstExcludedAliases.Add(Common.Doctype.LiveSkuChart);

                        //Additional from old code
                        lstExcludedAliases.Add(Common.Doctype.FaqItem);
                        lstExcludedAliases.Add(Common.Doctype.IngredientUnit);
                        lstExcludedAliases.Add(Common.Doctype.IngredientUnitsData);
                        lstExcludedAliases.Add(Common.Doctype.MerchandisingItem);
                        lstExcludedAliases.Add(Common.Doctype.Offer);
                        lstExcludedAliases.Add(Common.Doctype.RssPage);
                        lstExcludedAliases.Add(Common.Doctype.SiteData);
                        lstExcludedAliases.Add(Common.Doctype.ProductListing);
                        lstExcludedAliases.Add(Common.Doctype.SocialService);
                        lstExcludedAliases.Add(Common.Doctype.ProductsSection);
                        lstExcludedAliases.Add(Common.Doctype.OrganizingFolder);
                        lstExcludedAliases.Add(Common.Doctype.ProductTypesFolder);
                        lstExcludedAliases.Add(Common.Doctype.ProductType);
                        lstExcludedAliases.Add(Common.Doctype.Product);
                        //Products are searched through seperately before exclusions.  Added here to prevent duplicates.





                        //Update query with quotes to allow non-exact searches.  [Without the quotes, only exact words can be found]
                        string searchQuery = "\"" + vmSearchPage.SearchQuery + "\"";


                        //Query
                        var queryExecutor = index.Searcher
                                            .CreateQuery(IndexTypes.Content)
                                            .ManagedQuery(vmSearchPage.SearchQuery); //.And().GroupedNot(new[] { "nodeTypeAlias" }, lstExcludedAliases.ToArray());

          

                        //Loop through list of results | GET PRODUCTS ONLY to ensure products are 1st in list
                        List<string> lst = new List<string>();
                        foreach (var result in publishedContentQuery.Search(queryExecutor))
                        {
                            //Provide results IF page is contained in root site AND is not marked hidden from search (Searches only within site that search originated in.)
                            if (result.Content.Path.Contains("-1," + cmPage.Root().Id))
                            {
                                //Filter out all but Product pgs
                                if (result.Content.ContentType.Alias == Common.Doctype.Product)
                                {
                                    if (Convert.ToBoolean(result.Content.GetProperty(Common.Property.UmbracoSitemapHide)?.GetValue()) == false)  //is pg hidden from sitemap?  skip if true.
                                    {


                                        // JF | This allows us to ensure that we skip the "JsonData" property during a search in a product.  Would be better if in query, but cannot get it to work.
                                        bool IsAMatch = false;
                                        foreach (var ipProperty in result.Content.Properties) 
                                        {
                                            if (ipProperty.Alias == Common.Property.JsonData)
                                            {
                                                continue;
                                            }

                                            var source = ipProperty.GetSourceValue();
                                            if (source != null)
                                            {
                                                if (source.ToString().ToLower().Contains(vmSearchPage.SearchQuery.ToLower()))
                                                {
                                                    IsAMatch = true;
                                                    break;
                                                }
                                            }
                                        }





                                        if (IsAMatch)
                                        {
                                            StringBuilder sbSummary = new StringBuilder();

                                            try
                                            {
                                                //Variables
                                                int _length = vmSearchPage.SearchQuery.Length;
                                                Models.PublishedModels.Product cmProduct = new Models.PublishedModels.Product(result.Content, _publishedValueFallback);

                                                //Create Title
                                                string _title = string.Format("{0} - {1}", cmProduct.ProductCode, cmProduct.Title);

                                                //Obtain Summary [Marketting Message]
                                                sbSummary.Append(_title + " | ");
                                                sbSummary.Append(cmProduct.TradeItemMarketingMessage);                                                 
                                                string _summary = Dragonfly.NetHelpers.Strings.TruncateAtWord(Html.StripHtml(sbSummary.ToString().Replace("&nbsp;", " ").Trim()), 300);



                                                //Add bold tags around search query in string without replacing string. [fixes issue where case was not being preserved.]
                                                List<int> indexes = Common.FindAllSustrings(_title, vmSearchPage.SearchQuery);
                                                for (int i = indexes.Count - 1; i >= 0; i--)
                                                {
                                                    _title = _title.Insert(indexes[i] + _length, "</strong>");
                                                    _title = _title.Insert(indexes[i], "<strong>");
                                                }

                                                indexes.Clear();
                                                indexes = Common.FindAllSustrings(_summary, vmSearchPage.SearchQuery);
                                                for (int i = indexes.Count - 1; i >= 0; i--)
                                                {
                                                    _summary = _summary.Insert(indexes[i] + _length, "</strong>");
                                                    _summary = _summary.Insert(indexes[i], "<strong>");
                                                }

                                                //Add result to list.
                                                LstResults.Add(new SearchResultItem()
                                                {
                                                    Title = _title,
                                                    Summary = _summary,
                                                    Url = result.Content.Url()
                                                });

                                                ////Add result to list.  [This version replaced searched string with incoming string without matching case.  Wrong...]
                                                //LstResults.Add(new SearchResultItem()
                                                //{
                                                //    Title = result.Content.Name.Replace(vmSearchPage.SearchQuery, "<strong>" + vmSearchPage.SearchQuery + "</strong>", StringComparison.OrdinalIgnoreCase),
                                                //    Summary = (Dragonfly.NetHelpers.Strings.TruncateAtWord(Html.StripHtml(sbSummary.ToString().Replace("&nbsp;", " ").Trim()), 300)).Replace(vmSearchPage.SearchQuery, "<strong>" + vmSearchPage.SearchQuery + "</strong>", StringComparison.OrdinalIgnoreCase),
                                                //    Url = result.Content.Url()
                                                //});

                                            }
                                            catch (Exception ex)
                                            {
                                                sbSummary.Append(ex.Message);
                                            }
                                        }




                                    }
                                }
                            }
                        }



                        //Repeat Loop | OBTAIN ALL BUT PRODUCTS
                        foreach (var result in publishedContentQuery.Search(queryExecutor))
                        {
                            //Provide results IF page is contained in root site AND is not marked hidden from search
                            //  (Searches only within site that search originated in.)
                            if (result.Content.Path.Contains("-1," + cmPage.Root().Id))
                            {
                                //Ignore if alias exists in the ignore list.
                                if (!lstExcludedAliases.Contains(result.Content.ContentType.Alias))
                                {

                                    if (Convert.ToBoolean(result.Content.GetProperty(Common.Property.UmbracoSitemapHide)?.GetValue()) == false)
                                    {
                                        StringBuilder sbSummary = new StringBuilder();

                                        try
                                        {
                                            switch (result.Content.ContentType.Alias)
                                            {
                                                case Common.Doctype.AboutSection:
                                                case Common.Doctype.BlogListing:
                                                case Common.Doctype.BlogPost:
                                                case Common.Doctype.Brand:
                                                case Common.Doctype.BrandsListing:
                                                case Common.Doctype.OffersListing:
                                                case Common.Doctype.RecipesListing:
                                                case Common.Doctype.ScrollingBlogPost:
                                                case Common.Doctype.TextPage:
                                                    sbSummary.Append(result.Content.Name + " | ");

                                                    //Extract summary data
                                                    TextPage cm = new TextPage(result.Content, _publishedValueFallback);  //faking model to get RTE in blockgrid model.
                                                    foreach (var blockGridItem in cm.MainContent)
                                                    {
                                                        foreach (var blockGridArea in blockGridItem.Areas)
                                                        {
                                                            foreach (var gridItem in blockGridArea)
                                                            {
                                                                if (gridItem.Content.ContentType.Alias == "richTextEditor")
                                                                {
                                                                    sbSummary.Append(gridItem.Content.Value("Rte"));
                                                                }
                                                            }
                                                        }
                                                    }
                                                    break;

                                                case Common.Doctype.Recipe:
                                                    //use various properties
                                                    sbSummary.Append(result.Content.Name + " | ");
                                                    Recipe cmRecipe = new Recipe(result.Content, _publishedValueFallback);
                                                    sbSummary.Append(cmRecipe.Description + " ");

                                                    //foreach (NcRecipeIngredient ingredient in Model.Page?.Ingredients!.Select(x => x.Content).OfType<NcRecipeIngredient>())
                                                    foreach (var ingredient in cmRecipe.Ingredients.Select(x => x.Content).OfType<NcRecipeIngredient>())
                                                    {
                                                        if (ingredient.Type == NcRecipeIngredient.ItemType.Ingredient)
                                                        {
                                                            sbSummary.Append(ingredient.Ingredient + " ");
                                                        }
                                                    }
                                                    break;

                                                default:
                                                    //Add doctype to list with msg to add to search controller OR remove from list
                                                    break;
                                            }


                                            //Add bold tags around search query in string without replacing string. [fixes issue where case was not being preserved.]
                                            string _title = result.Content.Name;
                                            string _summary = Dragonfly.NetHelpers.Strings.TruncateAtWord(Html.StripHtml(sbSummary.ToString().Replace("&nbsp;", " ").Trim()), 300);
                                            int _length = vmSearchPage.SearchQuery.Length;

                                            List<int> indexes = Common.FindAllSustrings(_title, vmSearchPage.SearchQuery);
                                            for (int i = indexes.Count - 1; i >= 0; i--)
                                            {
                                                _title = _title.Insert(indexes[i] + _length, "</strong>");
                                                _title = _title.Insert(indexes[i], "<strong>");
                                            }

                                            indexes.Clear();
                                            indexes = Common.FindAllSustrings(_summary, vmSearchPage.SearchQuery);
                                            for (int i = indexes.Count - 1; i >= 0; i--)
                                            {
                                                _summary = _summary.Insert(indexes[i] + _length, "</strong>");
                                                _summary = _summary.Insert(indexes[i], "<strong>");
                                            }

                                            //Add result to list.
                                            LstResults.Add(new SearchResultItem()
                                            {
                                                Title = _title,
                                                Summary = _summary,
                                                Url = result.Content.Url()
                                            });



                                            ////Add result to list.  [This version replaced searched string with incoming string without matching case.  Wrong...]
                                            //LstResults.Add(new SearchResultItem()
                                            //{
                                            //    Title = result.Content.Name.Replace(vmSearchPage.SearchQuery, "<strong>" + vmSearchPage.SearchQuery + "</strong>", StringComparison.OrdinalIgnoreCase),
                                            //    Summary = (Dragonfly.NetHelpers.Strings.TruncateAtWord(Html.StripHtml(sbSummary.ToString().Replace("&nbsp;", " ").Trim()), 300)).Replace(vmSearchPage.SearchQuery, "<strong>" + vmSearchPage.SearchQuery + "</strong>", StringComparison.OrdinalIgnoreCase),
                                            //    Url = result.Content.Url()
                                            //});

                                        }
                                        catch (Exception ex)
                                        {
                                            sbSummary.Append(ex.Message);
                                        }
                                    }
                                }
                            }
                        }


                        //Calculate result values
                        string _pg = Request.Query[Common.Query.Page].ToString();
                        if (string.IsNullOrWhiteSpace(_pg))
                            vmSearchPage.PageNo = 1;
                        else
                            vmSearchPage.PageNo = Convert.ToInt32(_pg);

                        vmSearchPage.TotalItems = LstResults.Count();
                        vmSearchPage.SkipCount = (vmSearchPage.PageNo - 1) * 10;
                        vmSearchPage.ResultStartNo = ((vmSearchPage.PageNo - 1) * 10) + 1;
                        vmSearchPage.ResultThroughNo = ((vmSearchPage.PageNo) * 10);
                        vmSearchPage.TotalPages = (int)Math.Ceiling((double)vmSearchPage.TotalItems / 10);


                        //Ensure ThroughNo is valid.
                        if (vmSearchPage.ResultThroughNo > vmSearchPage.TotalItems)
                            vmSearchPage.ResultThroughNo = vmSearchPage.TotalItems;

                        if (vmSearchPage.PageNo == 1)
                            vmSearchPage.IsFirstPg = true;
                        if (vmSearchPage.PageNo == vmSearchPage.TotalPages)
                            vmSearchPage.IsLastPg = true;


                        //Obtain top and bottom of pagination values.
                        vmSearchPage.StartPageNo = Utils.Round.IntAmount(vmSearchPage.PageNo - 1, 10, Utils.Round.Type.Down) + 1;
                        vmSearchPage.EndPageNo = Utils.Round.IntAmount(vmSearchPage.PageNo, 10, Utils.Round.Type.Up);
                        if (vmSearchPage.EndPageNo > vmSearchPage.TotalPages)
                            vmSearchPage.EndPageNo = vmSearchPage.TotalPages;



                        //Take portion of list
                        vmSearchPage.LstResults = LstResults.Skip(vmSearchPage.SkipCount).Take(10).ToList();

                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }


            var viewModel = new ComposedPageViewModel<SearchPage, SearchPageViewModel>
            {
                Page = cmPage,
                ViewModel = vmSearchPage
            };

            return View(Common.View.SearchPage, viewModel);
        }
    }
}
