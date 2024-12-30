﻿using ECMC_Umbraco.Models;
using ECMC_Umbraco.ViewModel;
using Examine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Newtonsoft.Json;
using System.Web;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;
using UmbracoProject.Models;
using static Umbraco.Cms.Core.Constants;
using ContentModels = Umbraco.Cms.Web.Common.PublishedModels;

namespace www.Controllers
{
    public class NewsLattusFndController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private readonly IExamineManager ExamineManager;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<NewsLattusFndController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly IPublishedContentQuery publishedContentQuery;

        public NewsLattusFndController(
                ILogger<NewsLattusFndController> _logger,
                ICompositeViewEngine compositeViewEngine,
                IUmbracoContextAccessor umbracoContextAccessor,
                UmbracoHelper _UmbracoHelper,
                IExamineManager _ExamineManager,
                IPublishedValueFallback publishedValueFallback,
                ServiceContext context,
                IVariationContextAccessor variationContextAccessor,
                IPublishedContentQuery _publishedContentQuery
             )
            : base(_logger, compositeViewEngine, umbracoContextAccessor)
        {
            ExamineManager = _ExamineManager ?? throw new ArgumentNullException(nameof(_ExamineManager));
            _publishedValueFallback = publishedValueFallback;
            UmbracoHelper = _UmbracoHelper;
            logger = _logger;
            _serviceContext = context;
            _variationContextAccessor = variationContextAccessor;
            publishedContentQuery = _publishedContentQuery;
        }

        public override IActionResult Index()
        {
            //Instantiate variables
            var cmPage = new NewsLattusFnd(CurrentPage, _publishedValueFallback);
            NewsListViewModel lstVmodel = new NewsListViewModel();
            List<SearchResultViewModel> lstSearchResults = new List<SearchResultViewModel>();

            /* EXAMINE GUIDES
             * https://shazwazza.com/categories#tag-Examine
             * https://docs.umbraco.com/umbraco-cms/reference/searching/examine/quick-start#getting-the-content
             * https://docs.umbraco.com/umbraco-cms/reference/querying/ipublishedcontentquery
             * */



            try
            {
                if (ExamineManager.TryGetIndex(UmbracoIndexes.ExternalIndexName, out IIndex? index))
                {
                    //Query
                    var queryExecutor = index.Searcher
                        .CreateQuery(IndexTypes.Content)
                        .NodeTypeAlias("newsCardFnd");


                    //Loop through list of results
                    foreach (PublishedSearchResult result in publishedContentQuery.Search(queryExecutor).OrderByDescending(x => x.Content.Value<DateTime>("postDate")))
                    {
                        //Provide results IF news is child of this page.
                        if (result.Content.Path.Contains(cmPage.Path))
                        {
                            //
                            IPublishedContent ipNews = result.Content;


                            //Obtain card data
                            ListItemViewModel item = new ListItemViewModel();
                            item = RenderNewsCard(ipNews);
                            lstVmodel.LstListItems.Add(item);
                        }
                    }


                    //Obtain category data
                    foreach (var ipCategory in cmPage.Children)
                    {
                        lstVmodel.LstCategories.Add(new Link()
                        {
                            Title = ipCategory.Name,
                            Url = ipCategory.Url()
                        });
                    }


                    //Obtain filters
                    ListViewModel? tempLstVm = JsonConvert.DeserializeObject<ListViewModel>(JsonConvert.SerializeObject(lstVmodel));
                    if (tempLstVm != null)
                        lstVmodel.filterViewModel = new ListFilters().GenerateFilters(tempLstVm, CurrentPage!);


                }



                //Is search panel to be shown?
                lstVmodel.filterViewModel.ShowSearchPnl = cmPage.ShowSearchPanel;
                lstVmodel.filterViewModel.ShowViewSelector = cmPage.ShowViewSelector;
                if (lstVmodel.filterViewModel.ShowSearchPnl) lstVmodel.filterViewModel.FilterCount++;


                //Show pagination ONLY if it is needed
                if (lstVmodel.LstListItems != null)
                {
                    if (cmPage.PaginationCount > 0 && lstVmodel.LstListItems.Count() > cmPage.PaginationCount)
                    {
                        lstVmodel.ShowPagination = true;
                        lstVmodel.PaginationCount = cmPage.PaginationCount;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }


            var viewModel = new ComposedPageViewModel<ContentModels.NewsLattusFnd, NewsListViewModel>
            {
                Page = cmPage,
                ViewModel = lstVmodel
            };


            return View(Common.View.NewsLattusFnd, viewModel);
        }

        private ListItemViewModel RenderNewsCard(IPublishedContent ipChild)
        {
            //Instantiate variables
            ListItemViewModel item = new ListItemViewModel();

            //Obtain model vsn of child node
            ContentModels.NewsCardFnd cmNewsCardFnd = new ContentModels.NewsCardFnd(ipChild, new PublishedValueFallback(_serviceContext, _variationContextAccessor));

            //Obtain all data from model
            item.Id = cmNewsCardFnd.Id;
            item.DocType = cmNewsCardFnd.ContentType.Alias;
            item.Title = cmNewsCardFnd.Title ?? "";
            item.Link = new Link()
            {
                Url = cmNewsCardFnd.Url()
            };

            if (cmNewsCardFnd.Subtitle != null)
                item.Subtitle = cmNewsCardFnd.Subtitle;

            if (cmNewsCardFnd.Summary != null)
            {
                item.Summary = cmNewsCardFnd.Summary;
                item.EncodedSummary = HttpUtility.HtmlEncode(item.Summary.ToLower());
            }


            item.PostDate = cmNewsCardFnd.PostDate.ToString("MMM d, yyyy");


            //Determine if image is cropped or not
            if (cmNewsCardFnd.NoCrop)
            {
                item.PrimaryImgUrl = cmNewsCardFnd.PrimaryImage?.Url() + "?format=webp";
                item.AdditionalClasses = "no-crop-vsn";
            }
            else
            {
                item.PrimaryImgUrl = cmNewsCardFnd.PrimaryImage?.GetCropUrl(Common.Crop.LearnMore_500x550) + "&format=webp";
            }


            if (cmNewsCardFnd.AreasOfInterest != null)
            {
                foreach (var i in cmNewsCardFnd.AreasOfInterest)
                    item.LstAreasOfInterest.Add(i.Name);
                item.jsonAreasOfInterest = Newtonsoft.Json.JsonConvert.SerializeObject(item.LstAreasOfInterest);
            }


            item.InViewAnimation = cmNewsCardFnd.InViewAnimation;

            if (!string.IsNullOrWhiteSpace(cmNewsCardFnd.HoverTitle))
            {
                item.HoverTitle = cmNewsCardFnd.HoverTitle;
                item.ShowHoverContent = true;
            }
            if (cmNewsCardFnd.HoverTip != null && !string.IsNullOrWhiteSpace(cmNewsCardFnd.HoverTip.ToString()))
            {
                item.HoverTip = cmNewsCardFnd.HoverTip;
                item.ShowHoverContent = true;
            }

            //Determine if card content should appear
            if (!string.IsNullOrWhiteSpace(cmNewsCardFnd.Title) ||
                (cmNewsCardFnd.Summary != null && !string.IsNullOrWhiteSpace(cmNewsCardFnd.Summary.ToString())))
            {
                item.ShowCardContent = true;
            }
            item.ShowCardContent = true;


            return item;
        }
    }
}