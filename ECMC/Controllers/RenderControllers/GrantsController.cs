using ECMC_Umbraco.Models;
using ECMC_Umbraco.ViewModel;
using Examine;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Linq;
using System.Web;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;
using UmbracoProject.Models;
using static Umbraco.Cms.Core.Constants;
using ContentModels = Umbraco.Cms.Web.Common.PublishedModels;

namespace www.Controllers
{
    public class GrantsController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<GrantsController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly IExamineManager ExamineManager;
        private readonly IPublishedContentQuery publishedContentQuery;

        public GrantsController(
                ILogger<GrantsController> _logger,
                ICompositeViewEngine compositeViewEngine,
                IUmbracoContextAccessor umbracoContextAccessor,
                UmbracoHelper _UmbracoHelper,
                IPublishedValueFallback publishedValueFallback,
                ServiceContext context,
                IExamineManager _ExamineManager,
                IVariationContextAccessor variationContextAccessor,
                IPublishedContentQuery _publishedContentQuery
             )
            : base(_logger, compositeViewEngine, umbracoContextAccessor)
        {
            _publishedValueFallback = publishedValueFallback;
            UmbracoHelper = _UmbracoHelper;
            logger = _logger;
            _serviceContext = context;
            _variationContextAccessor = variationContextAccessor;
            publishedContentQuery = _publishedContentQuery;
            ExamineManager = _ExamineManager ?? throw new ArgumentNullException(nameof(_ExamineManager));
        }



        public override IActionResult Index()
        {
            //Instantiate variables
            var cmPage = new Grants(CurrentPage, _publishedValueFallback);
            GrantItemsViewModel vmGrantItems = null;


            try
            {
                /*      CREATE OR OBTAIN VIEWMODEL      */
                //Extract data that has been returned from filter in postback, or create a new viewmodel.
                if (TempData["Model"] != null)
                {
                    vmGrantItems = Newtonsoft.Json.JsonConvert.DeserializeObject<GrantItemsViewModel>((string)TempData["Model"]);
                }
                else
                {
                    //Create new vm
                    vmGrantItems = new GrantItemsViewModel();

                    //Set initial values
                    vmGrantItems.PageUrl = cmPage.Url();
                    vmGrantItems.Pagination.CurrentPageNo = 1;
                    vmGrantItems.Pagination.TotalRecords = 0;
                    vmGrantItems.Pagination.FirstDisplayedPageNo = 1;
                    vmGrantItems.Pagination.LastDisplayedPageNo = 1;
                }



                if (vmGrantItems != null)
                {
                    /*      SET LIST VALUES      */
                    //Initialize years
                    for (var i = DateTime.Today.Year; i >= 2015; i--)
                    {
                        vmGrantItems.LstYears.Add(new SelectListItem()
                        {
                            Value = i.ToString(),
                            Text = i.ToString()
                        });
                    }

                    //Initialize locations using examine
                    if (ExamineManager.TryGetIndex(UmbracoIndexes.ExternalIndexName, out IIndex? searcherIndex))
                    {
                        //Create query
                        var queryExecutor = searcherIndex.Searcher
                        .CreateQuery(IndexTypes.Content)
                        .NodeTypeAlias("grant");

                        //Obtain list of unique locations from search.
                        List<string> LstLocations = new List<string>();
                        foreach (PublishedSearchResult result in publishedContentQuery.Search(queryExecutor))
                        {
                            Grant cmGrant = new Grant(result.Content, _publishedValueFallback);
                            if (!string.IsNullOrEmpty(cmGrant.Location) && !LstLocations.Any(x => x == cmGrant.Location)  && cmGrant.Location != "National")
                            {
                                LstLocations.Add(cmGrant.Location);
                            }
                        }

                        //Create locations list with National as 1st.
                        vmGrantItems.LstLocations.Add(new SelectListItem() { Text = "National", Value = "National" });
                        foreach (var location in LstLocations.OrderBy(x => x))
                        {
                            vmGrantItems.LstLocations.Add(new SelectListItem() { Text = location, Value = location });
                        }
                       


                    }



                    /*      OBTAIN RECORDS USING EXAMINE IF SEARCH HAS BEEN SUBMITTED      */
                    if (vmGrantItems.SearchSubmitted)
                    {
                        if (ExamineManager.TryGetIndex(UmbracoIndexes.ExternalIndexName, out IIndex? index))
                        {
                            //Query base
                            var queryExecutor = index.Searcher
                                .CreateQuery(IndexTypes.Content)
                                .NodeTypeAlias("grant");


                            //Dynamically expand query
                            if (!string.IsNullOrWhiteSpace(vmGrantItems.YearFilter))
                            {
                                queryExecutor.And().Field("fiscalYear", vmGrantItems.YearFilter);
                            }
                            if (!string.IsNullOrWhiteSpace(vmGrantItems.LocationFilter))
                            {
                                queryExecutor.And().Field("location", vmGrantItems.LocationFilter);
                            }
                            if (!string.IsNullOrWhiteSpace(vmGrantItems.SearchQuery))
                            {
                                queryExecutor.And().ManagedQuery(vmGrantItems.SearchQuery);
                            }



                            //Determine pagination
                            vmGrantItems.Pagination.TotalRecords = publishedContentQuery.Search(queryExecutor).Count();
                            vmGrantItems.Pagination.TotalPages = (int)Math.Ceiling((double)vmGrantItems.Pagination.TotalRecords / (double)vmGrantItems.Pagination.RecordsPerPage);

                            vmGrantItems.Pagination.FirstDisplayedPageNo = RoundDown(vmGrantItems.Pagination.CurrentPageNo - 1, 10) + 1;
                            vmGrantItems.Pagination.LastDisplayedPageNo = RoundUp(vmGrantItems.Pagination.CurrentPageNo - 1, 10);

                            //Ensure ThroughNo is valid.
                            if (vmGrantItems.Pagination.FirstDisplayedPageNo < 1)
                                vmGrantItems.Pagination.FirstDisplayedPageNo = 1;
                            if (vmGrantItems.Pagination.LastDisplayedPageNo > vmGrantItems.Pagination.TotalPages)
                                vmGrantItems.Pagination.LastDisplayedPageNo = vmGrantItems.Pagination.TotalPages;





                            //Create First/Next buttons
                            string _status = "";
                            if (vmGrantItems.Pagination.CurrentPageNo == 1)
                                _status = " disabled";
                            vmGrantItems.Pagination.LstLinks.Add(new Link()
                            {
                                Title = "First",
                                Class = "first" + _status,
                                Misc = _status,
                                Level = 1
                            });
                            vmGrantItems.Pagination.LstLinks.Add(new Link()
                            {
                                Title = "«",
                                Class = "prev" + _status,
                                Misc = _status,
                                Level = vmGrantItems.Pagination.CurrentPageNo - 1
                            });

                            //Create all buttons in middle of nav
                            for (int i = vmGrantItems.Pagination.FirstDisplayedPageNo; i <= vmGrantItems.Pagination.LastDisplayedPageNo; i++)
                            {
                                string isActive = (i == vmGrantItems.Pagination.CurrentPageNo) ? " active" : "";
                                vmGrantItems.Pagination.LstLinks.Add(new Link()
                                {
                                    Title = i.ToString(),
                                    Class = "page" + isActive,
                                    Level = i
                                });
                            }

                            //Create Last/Prev buttons
                            _status = "";
                            if (vmGrantItems.Pagination.CurrentPageNo == vmGrantItems.Pagination.TotalPages)
                                _status = " disabled";
                            vmGrantItems.Pagination.LstLinks.Add(new Link()
                            {
                                Title = "»",
                                Class = "next" + _status,
                                Misc = _status,
                                Level = vmGrantItems.Pagination.CurrentPageNo + 1
                            });
                            vmGrantItems.Pagination.LstLinks.Add(new Link()
                            {
                                Title = "Last",
                                Class = "last" + _status,
                                Misc = _status,
                                Level = vmGrantItems.Pagination.TotalPages
                            });





                            //Loop through list of results
                            int _skip = (vmGrantItems.Pagination.CurrentPageNo - 1) * 10;
                            foreach (PublishedSearchResult result in publishedContentQuery.Search(queryExecutor).Skip(_skip).Take(vmGrantItems.Pagination.RecordsPerPage))
                            {
                                Grant cmGrant = new Grant(result.Content, _publishedValueFallback);
                                GrantItemViewModel grant = new GrantItemViewModel();


                                //
                                grant.GrantName = cmGrant.Name;
                                grant.Organization = cmGrant.Organization; //Grantee
                                grant.Amount = cmGrant.Amount;
                                grant.Location = cmGrant.Location;
                                grant.Status = cmGrant.Status;
                                grant.FiscalYear = cmGrant.FiscalYear;
                                grant.Focus = cmGrant.Focucs;
                                grant.Overview = cmGrant.Overview;

                                //
                                vmGrantItems.LstGrantItems.Add(grant);
                            }
                        }
                    }

                }





                //viewModel.Grants = viewModel.Content?.Children<Grant>();
                //viewModel.Activities = viewModel.Grants?.Select(o => o.Activities).Distinct().OrderBy(o => o);
                //viewModel.Types = viewModel.Grants?.Select(o => o.Type).Distinct().OrderBy(o => o);
                //viewModel.FocusAreas = viewModel.Grants?.SelectMany(o => o.Focucs.Split(new char[] { ',' })).Distinct().OrderBy(o => o);
                //viewModel.Programs = viewModel.Grants?.Select(o => o.Program).Distinct().OrderBy(o => o);
                //viewModel.Years = viewModel.Grants?.Select(o => o.FiscalYear).Distinct().OrderByDescending(o => o);
                //viewModel.Locations = viewModel.Grants?.SelectMany(o => o.Location.Split(new char[] { ',' })).Distinct().OrderBy(o => o);




            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }


            //Create viewmodel
            var viewModel = new ComposedPageViewModel<Grants, GrantItemsViewModel>
            {
                Page = cmPage,
                ViewModel = vmGrantItems
            };


            //Ensure temp data is always cleared after use.
            TempData.Clear();


            return View(Common.View.Grants, viewModel);
        }

        private int RoundUp(int number, int interval)
        {
            int remainder = number % interval;
            number += (interval - remainder);
            return number;
        }
        private int RoundDown(int number, int interval)
        {
            int remainder = number % interval;
            number += -remainder;
            return number;
        }
    }
}
