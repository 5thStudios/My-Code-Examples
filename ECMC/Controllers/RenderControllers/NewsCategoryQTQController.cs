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
    public class NewsCategoryQTQController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private readonly IExamineManager ExamineManager;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<NewsCategoryQTQController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly IPublishedContentQuery publishedContentQuery;

        public NewsCategoryQTQController(
                ILogger<NewsCategoryQTQController> _logger,
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
            var cmPage = new NewsCategoryQtq(CurrentPage, _publishedValueFallback);
            var cmParent = new NewsLattusQtq(CurrentPage.Parent, _publishedValueFallback);
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
                        .NodeTypeAlias("newsCardQTQ");


                    //Loop through list of results
                    foreach (PublishedSearchResult result in publishedContentQuery.Search(queryExecutor).OrderByDescending(x => x.Content.Value<DateTime>("postDate")))
                    {
                        //Provide results IF news is child of this page.
                        if (result.Content.Path.Contains(cmPage.Path))
                        {
                            //
                            IPublishedContent ipNews = result.Content;


                            //Obtain parent data
                            if (!lstVmodel.LstCategories.Where(x => x.Title == ipNews.Parent.Parent.Name).Any())
                            {
                                lstVmodel.LstCategories.Add(new Link()
                                {
                                    Title = ipNews.Parent.Parent.Name,
                                    Url = ipNews.Parent.Parent.Url()
                                });
                            }


                            //Obtain card data
                            ListItemViewModel item = new ListItemViewModel();
                            item = RenderNewsCard(ipNews);
                            lstVmodel.LstListItems.Add(item);
                        }
                    }


                    //Obtain filters
                    ListViewModel? tempLstVm = JsonConvert.DeserializeObject<ListViewModel>(JsonConvert.SerializeObject(lstVmodel));
                    if (tempLstVm != null)
                        lstVmodel.filterViewModel = new ListFilters().GenerateFilters(tempLstVm, CurrentPage!);


                }



                //Is search panel to be shown?
                lstVmodel.filterViewModel.ShowSearchPnl = cmParent.ShowSearchPanel;
                if (lstVmodel.filterViewModel.ShowSearchPnl) lstVmodel.filterViewModel.FilterCount++;


                //Show pagination ONLY if it is needed
                if (lstVmodel.LstListItems != null)
                {
                    if (cmParent.PaginationCount > 0 && lstVmodel.LstListItems.Count() > cmParent.PaginationCount)
                    {
                        lstVmodel.ShowPagination = true;
                        lstVmodel.PaginationCount = cmParent.PaginationCount;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }


            var viewModel = new ComposedPageViewModel<ContentModels.NewsCategoryQtq, NewsListViewModel>
            {
                Page = cmPage,
                ViewModel = lstVmodel
            };


            return View(Common.View.NewsCategoryQTQ, viewModel);
        }



        private ListItemViewModel RenderNewsCard(IPublishedContent ipChild)
        {
            //Instantiate variables
            ListItemViewModel item = new ListItemViewModel();

            //Obtain model vsn of child node
            ContentModels.NewsCardQtq cmNewsCardQtq = new ContentModels.NewsCardQtq(ipChild, new PublishedValueFallback(_serviceContext, _variationContextAccessor));

            //Obtain all data from model
            item.Id = cmNewsCardQtq.Id;
            item.DocType = cmNewsCardQtq.ContentType.Alias;
            item.Title = cmNewsCardQtq.Title ?? "";
            item.Link = new Link()
            {
                Url = cmNewsCardQtq.Url()
            };

            if (cmNewsCardQtq.Subtitle != null)
                item.Subtitle = cmNewsCardQtq.Subtitle;

            if (cmNewsCardQtq.Summary != null)
            {
                item.Summary = cmNewsCardQtq.Summary;
                item.EncodedSummary = HttpUtility.HtmlEncode(item.Summary.ToLower());
            }


            item.PostDate = cmNewsCardQtq.PostDate.ToString("MMM d, yyyy");


            //Determine if image is cropped or not
            if (cmNewsCardQtq.NoCrop)
            {
                item.PrimaryImgUrl = cmNewsCardQtq.PrimaryImage?.Url() + "?format=webp";
                item.AdditionalClasses = "no-crop-vsn";
            }
            else
            {
                item.PrimaryImgUrl = cmNewsCardQtq.PrimaryImage?.GetCropUrl(Common.Crop.LearnMore_500x550) + "&format=webp";
            }


            if (cmNewsCardQtq.AreasOfInterest != null)
            {
                foreach (var i in cmNewsCardQtq.AreasOfInterest)
                    item.LstAreasOfInterest.Add(i.Name);
                item.jsonAreasOfInterest = Newtonsoft.Json.JsonConvert.SerializeObject(item.LstAreasOfInterest);
            }


            item.InViewAnimation = cmNewsCardQtq.InViewAnimation;

            if (!string.IsNullOrWhiteSpace(cmNewsCardQtq.HoverTitle))
            {
                item.HoverTitle = cmNewsCardQtq.HoverTitle;
                item.ShowHoverContent = true;
            }
            if (cmNewsCardQtq.HoverTip != null && !string.IsNullOrWhiteSpace(cmNewsCardQtq.HoverTip.ToString()))
            {
                item.HoverTip = cmNewsCardQtq.HoverTip;
                item.ShowHoverContent = true;
            }

            //Determine if card content should appear
            if (!string.IsNullOrWhiteSpace(cmNewsCardQtq.Title) ||
                (cmNewsCardQtq.Summary != null && !string.IsNullOrWhiteSpace(cmNewsCardQtq.Summary.ToString())))
            {
                item.ShowCardContent = true;
            }
            item.ShowCardContent = true;


            return item;
        }
    }
}