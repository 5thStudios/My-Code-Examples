using ECMC_Umbraco.Models;
using ECMC_Umbraco.ViewModel;
using Examine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
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
    public class NewsCardFndController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private readonly IExamineManager ExamineManager;
        private readonly IPublishedContentQuery publishedContentQuery;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<NewsCardFndController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;

        public NewsCardFndController(
                ILogger<NewsCardFndController> _logger,
                ICompositeViewEngine compositeViewEngine,
                IUmbracoContextAccessor umbracoContextAccessor,
                UmbracoHelper _UmbracoHelper,
                IPublishedValueFallback publishedValueFallback,
                ServiceContext context,
                IVariationContextAccessor variationContextAccessor,
                IExamineManager _ExamineManager,
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
            //Instantiate variableas
            ContentModels.NewsCardFnd cmPage = new ContentModels.NewsCardFnd(CurrentPage, _publishedValueFallback);
            NewsCardViewModel lstVmodel = new NewsCardViewModel();


            try
            {
                //Get root news node
                IPublishedContent ipParent;
                if (cmPage.ContentType.Alias.Contains("Lattus"))
                {
                    ipParent = cmPage.Parent;
                }
                else
                {
                    ipParent = cmPage.Parent.Parent;
                }

                //Instantiate variableas
                ContentModels.NewsLattusFnd cmListLattusFnd = new NewsLattusFnd(ipParent, new PublishedValueFallback(_serviceContext, _variationContextAccessor));
                lstVmodel.LearnMoreTitle = cmListLattusFnd.LearnMoreTitle;
                if (!string.IsNullOrWhiteSpace(cmListLattusFnd.PrefixTitle))
                    lstVmodel.PrefixTitle = cmListLattusFnd.PrefixTitle + ": ";


                //Get all siblings
                if (ExamineManager.TryGetIndex(UmbracoIndexes.ExternalIndexName, out IIndex? index))
                {
                    //Query
                    var queryExecutor = index.Searcher
                        .CreateQuery(IndexTypes.Content)
                        .NodeTypeAlias("newsCardFnd");


                    //Loop through list of results
                    int counter = 8;
                    foreach (PublishedSearchResult result in publishedContentQuery.Search(queryExecutor).OrderByDescending(x => x.Content.Value<DateTime>("postDate")))//.Take(8))
                    {
                        //Provide results IF page is contained in root site
                        if (result.Content.Path.Contains(cmListLattusFnd.Path))
                        {
                            //
                            IPublishedContent ipNews = result.Content;


                            //Skip current person
                            if (ipNews.Id == cmPage.Id)
                                continue;


                            //Obtain card data
                            ListItemViewModel item = new ListItemViewModel();
                            item = RenderNewsCard(ipNews);
                            lstVmodel.LstListItems.Add(item);

                            counter--;
                            if (counter == 0) break;
                        }
                    }
                }




            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }


            var viewModel = new ComposedPageViewModel<NewsCardFnd, NewsCardViewModel>
            {
                Page = cmPage,
                ViewModel = lstVmodel
            };



            //return null;
            return View(Common.View.NewsCardFnd, viewModel);
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
