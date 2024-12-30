using ECMC_Umbraco.Models;
using ECMC_Umbraco.ViewModel;
using Examine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Controllers;
using static Umbraco.Cms.Core.Constants;
using ContentModels = Umbraco.Cms.Web.Common.PublishedModels;




namespace www.Controllers
{
    public class SearchEcmcController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private readonly IExamineManager ExamineManager;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<SearchEcmcController> logger;
        private readonly IPublishedContentQuery publishedContentQuery;

        public SearchEcmcController(
                ILogger<SearchEcmcController> _logger,
                UmbracoHelper _UmbracoHelper,
                IExamineManager _ExamineManager,
                ICompositeViewEngine compositeViewEngine,
                IUmbracoContextAccessor umbracoContextAccessor,
                IPublishedValueFallback publishedValueFallback,
                IPublishedContentQuery _publishedContentQuery
             )
            : base(_logger, compositeViewEngine, umbracoContextAccessor)
        {
            ExamineManager = _ExamineManager ?? throw new ArgumentNullException(nameof(_ExamineManager));
            _publishedValueFallback = publishedValueFallback;
            UmbracoHelper = _UmbracoHelper;
            logger = _logger;
            publishedContentQuery = _publishedContentQuery;
        }

        public override IActionResult Index()
        {
            //Instantiate variables
            List<SearchResultViewModel> lstSearchResults = new List<SearchResultViewModel>();
            var cmPage = new ContentModels.SearchEcmc(CurrentPage, _publishedValueFallback);
            string? query = Request.Query[Common.Query.Search];



            /* EXAMINE GUIDES
             * https://shazwazza.com/categories#tag-Examine
             * https://docs.umbraco.com/umbraco-cms/reference/searching/examine/quick-start#getting-the-content
             * https://docs.umbraco.com/umbraco-cms/reference/querying/ipublishedcontentquery
             * */


            try
            {
                if (!string.IsNullOrWhiteSpace(query))
                {
                    if (ExamineManager.TryGetIndex(UmbracoIndexes.ExternalIndexName, out IIndex? index))
                    {
                        //Doctypes to exclude from search
                        List<string> lstExcludedAliases = new List<string>();
                        lstExcludedAliases.Add(Common.Doctype.ContentFolder);
                        lstExcludedAliases.Add(Common.Doctype.SiteStyles);
                        lstExcludedAliases.Add(Common.Doctype.Tooltips);

                        lstExcludedAliases.Add(Common.Doctype.SearchEIF);
                        lstExcludedAliases.Add(Common.Doctype.DocumentCardEIF);
                        lstExcludedAliases.Add(Common.Doctype.IconCardCompEIF);
                        lstExcludedAliases.Add(Common.Doctype.PartnerCardEIF);

                        lstExcludedAliases.Add(Common.Doctype.SearchFnd);
                        lstExcludedAliases.Add(Common.Doctype.DocumentCardFnd);
                        lstExcludedAliases.Add(Common.Doctype.IconCardCompFnd);
                        lstExcludedAliases.Add(Common.Doctype.PartnerCardFnd);

                        lstExcludedAliases.Add(Common.Doctype.SearchScholars);
                        lstExcludedAliases.Add(Common.Doctype.DocumentCardScholars);
                        lstExcludedAliases.Add(Common.Doctype.IconCardCompScholars);
                        lstExcludedAliases.Add(Common.Doctype.PartnerCardScholars);

                        lstExcludedAliases.Add(Common.Doctype.SearchSol);
                        lstExcludedAliases.Add(Common.Doctype.DocumentCardSol);
                        lstExcludedAliases.Add(Common.Doctype.IconCardCompSol);
                        lstExcludedAliases.Add(Common.Doctype.PartnerCardSol);

                        lstExcludedAliases.Add(Common.Doctype.SearchEcmc);
                        lstExcludedAliases.Add(Common.Doctype.DocumentCardEcmc);
                        lstExcludedAliases.Add(Common.Doctype.IconCardCompEcmc);
                        lstExcludedAliases.Add(Common.Doctype.PartnerCardEcmc);

                        lstExcludedAliases.Add(Common.Doctype.SearchAR);
                        lstExcludedAliases.Add(Common.Doctype.DocumentCardAR);
                        lstExcludedAliases.Add(Common.Doctype.IconCardCompAR);
                        lstExcludedAliases.Add(Common.Doctype.PartnerCardAR);

                        lstExcludedAliases.Add(Common.Doctype.SearchEG);
                        lstExcludedAliases.Add(Common.Doctype.DocumentCardEG);
                        lstExcludedAliases.Add(Common.Doctype.IconCardCompEG);
                        lstExcludedAliases.Add(Common.Doctype.PartnerCardEG);

                        lstExcludedAliases.Add(Common.Doctype.SearchCN);
                        lstExcludedAliases.Add(Common.Doctype.DocumentCardCN);
                        lstExcludedAliases.Add(Common.Doctype.IconCardCompCN);
                        lstExcludedAliases.Add(Common.Doctype.PartnerCardCN);



                        //Query
                        var queryExecutor = index.Searcher
                            .CreateQuery(IndexTypes.Content)
                            .ManagedQuery(query);
                            //.And().GroupedNot(new[] { "nodeTypeAlias" }, lstExcludedAliases.ToArray());


                        //Loop through list of results
                        foreach (var result in publishedContentQuery.Search(queryExecutor))
                        {
                            //Provide results IF page is contained in root site AND is not marked hidden from search
                            //  (Searches only within site that search originated in.)
                            if (result.Content.Path.Contains("-1," + cmPage.Root().Id))
                            {
                                //Ignore if alias exists in the ignore list.
                                if (!lstExcludedAliases.Contains(result.Content.ContentType.Alias))
                                {
                                    if (Convert.ToBoolean(result.Content.GetProperty(Common.Property.HideFromSearch)?.GetValue()) == false)
                                    {
                                        var record = new SearchResultViewModel();
                                        record.Title = result.Content.Name;
                                        record.Summary = result.Content.GetProperty(Common.Property.PageSummary)?.GetValue()?.ToString();
                                        record.Url = result.Content.Url();
                                        lstSearchResults.Add(record);
                                    }
                                }


                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }


            var viewModel = new ComposedPageViewModel<ContentModels.SearchEcmc, List<SearchResultViewModel>>
            {
                Page = cmPage,
                ViewModel = lstSearchResults
            };

            return View(Common.View.SearchEcmc, viewModel);
        }
    }
}




















//===================================================================
////Convert search result into classes
//ISearcher searcher = index.Searcher;
//ISearchResults searchResults = searcher.Search(query);
//foreach (SearchResult node in searchResults)
//{
//    var record = new SearchResultViewModel();
//    record.Title = node["nodeName"];
//    record.Summary = "TBD"; //Newtonsoft.Json.JsonConvert.SerializeObject(node);

//    if (node["__IndexType"] == "content")
//        record.Url = UmbracoHelper.Content(Convert.ToInt32(node.Id))?.Url();

//    else if (node["__IndexType"] == "media")
//        record.Url = UmbracoHelper.Media(Convert.ToInt32(node.Id))?.Url();

//    lstSearchResults.Add(record);
//}


//===================================================================
//ISearchResults searchResults2 = searcher.CreateQuery("content").Field("nodeName", query).Execute();
//ISearchResults searchResults3 = searcher.CreateQuery("content").ParentId(1105).And().Field("bodyText", searchTerm).Execute();


//===================================================================
//QueryOptions queryOptions = new QueryOptions(0, int.MaxValue);
//IBooleanOperation query = searcher.CreateQuery(null, BooleanOperation.Or)
//    .NodeTypeAlias(Common.Doctype.Message)
//    .Or().NodeTypeAlias(Common.Doctype.RedirectToPage)
//    .Or().NodeTypeAlias(Common.Doctype.WebmasterMessage);
//ISearchResults isResults = query.Execute(queryOptions);


//===================================================================
//var textFields = new[] { "nodeName", "title", "bodyText" };
//var results = ((searcher.CreateQuery()
//                          .GroupedOr(textFields, query)
//                          .OrderByDescending(new SortableField("date"))
//                          .Execute())
//                          .Where(w => w["__IndexType"] == "content" &&
//                                      w["__NodeTypeAlias"] == "repository" &&
//                                      w["__Path"].Contains("16602") &&
//                                      w["__Published"] == "y" &&
//                                      w["umbracoNaviHide"] == "0"));


//===================================================================
////Search by doctype
//IBooleanOperation examineQuery = searcher.CreateQuery("Models.Common.NodeProperties.Content")
//    .NodeTypeAlias(Common.Doctype.Post);

////Sort by date (custom field added in IndexerComponent.cs)
//SortableField field = new SortableField(Models.Common.NodeProperties.PostDateSortable, SortType.Long);
//examineQuery.OrderByDescending(field);

//string[] flds = new string[3] { Models.Common.NodeProperties.Id, Models.Common.NodeProperties.PostDate, Models.Common.NodeProperties.PostDateSortable };
//examineQuery.SelectFields(flds);

////Get all results
//ISearchResults searchResults = examineQuery.Execute(maxResults: int.MaxValue);
//ISearchResult[] pagedResults = searchResults.ToArray(); //.Skip(0).Take(0);  //ALLOCATES TO MEMORY.
