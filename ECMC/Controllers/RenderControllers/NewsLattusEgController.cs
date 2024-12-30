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
    public class NewsLattusEgController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private readonly IExamineManager ExamineManager;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<NewsLattusEgController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly IPublishedContentQuery publishedContentQuery;

        public NewsLattusEgController(
                ILogger<NewsLattusEgController> _logger,
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
            var cmPage = new NewsLattusEG(CurrentPage, _publishedValueFallback);
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
                        .NodeTypeAlias("newsCardEG");


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


            var viewModel = new ComposedPageViewModel<ContentModels.NewsLattusEG, NewsListViewModel>
            {
                Page = cmPage,
                ViewModel = lstVmodel
            };


            return View(Common.View.NewsLattusEG, viewModel);
        }

        private ListItemViewModel RenderNewsCard(IPublishedContent ipChild)
        {
            //Instantiate variables
            ListItemViewModel item = new ListItemViewModel();

            //Obtain model vsn of child node
            ContentModels.NewsCardEG cmNewsCardEg = new ContentModels.NewsCardEG(ipChild, new PublishedValueFallback(_serviceContext, _variationContextAccessor));

            //Obtain all data from model
            item.Id = cmNewsCardEg.Id;
            item.DocType = cmNewsCardEg.ContentType.Alias;
            item.Title = cmNewsCardEg.Title ?? "";
            item.Link = new Link()
            {
                Url = cmNewsCardEg.Url()
            };

            if (cmNewsCardEg.Subtitle != null)
                item.Subtitle = cmNewsCardEg.Subtitle;

            if (cmNewsCardEg.Summary != null)
            {
                item.Summary = cmNewsCardEg.Summary;
                item.EncodedSummary = HttpUtility.HtmlEncode(item.Summary.ToLower());
            }


            item.PostDate = cmNewsCardEg.PostDate.ToString("MMM d, yyyy");


            //Determine if image is cropped or not
            if (cmNewsCardEg.NoCrop)
            {
                item.PrimaryImgUrl = cmNewsCardEg.PrimaryImage?.Url() + "?format=webp";
                item.AdditionalClasses = "no-crop-vsn";
            }
            else
            {
                item.PrimaryImgUrl = cmNewsCardEg.PrimaryImage?.GetCropUrl(Common.Crop.LearnMore_500x550) + "&format=webp";
            }


            if (cmNewsCardEg.AreasOfInterest != null)
            {
                foreach (var i in cmNewsCardEg.AreasOfInterest)
                {
                    item.LstAreasOfInterest.Add(i.Name);
                }
                item.jsonAreasOfInterest = Newtonsoft.Json.JsonConvert.SerializeObject(item.LstAreasOfInterest);
            }


            item.InViewAnimation = cmNewsCardEg.InViewAnimation;

            if (!string.IsNullOrWhiteSpace(cmNewsCardEg.HoverTitle))
            {
                item.HoverTitle = cmNewsCardEg.HoverTitle;
                item.ShowHoverContent = true;
            }
            if (cmNewsCardEg.HoverTip != null && !string.IsNullOrWhiteSpace(cmNewsCardEg.HoverTip.ToString()))
            {
                item.HoverTip = cmNewsCardEg.HoverTip;
                item.ShowHoverContent = true;
            }

            //Determine if card content should appear
            if (!string.IsNullOrWhiteSpace(cmNewsCardEg.Title) ||
                (cmNewsCardEg.Summary != null && !string.IsNullOrWhiteSpace(cmNewsCardEg.Summary.ToString())))
            {
                item.ShowCardContent = true;
            }
            item.ShowCardContent = true;


            return item;
        }
    }
}