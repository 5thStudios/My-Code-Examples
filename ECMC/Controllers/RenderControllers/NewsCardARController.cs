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
    public class NewsCardARController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private readonly IExamineManager ExamineManager;
        private readonly IPublishedContentQuery publishedContentQuery;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<NewsCardARController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;

        public NewsCardARController(
                ILogger<NewsCardARController> _logger,
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
            ContentModels.NewsCardAR cmPage = new ContentModels.NewsCardAR(CurrentPage, _publishedValueFallback);
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
                ContentModels.NewsLattusAR cmListLattusAR = new NewsLattusAR(ipParent, new PublishedValueFallback(_serviceContext, _variationContextAccessor));
                lstVmodel.LearnMoreTitle = cmListLattusAR.LearnMoreTitle;
                if (!string.IsNullOrWhiteSpace(cmListLattusAR.PrefixTitle))
                    lstVmodel.PrefixTitle = cmListLattusAR.PrefixTitle + ": ";


                //Get all siblings
                if (ExamineManager.TryGetIndex(UmbracoIndexes.ExternalIndexName, out IIndex? index))
                {
                    //Query
                    var queryExecutor = index.Searcher
                        .CreateQuery(IndexTypes.Content)
                        .NodeTypeAlias("newsCardAR");


                    //Loop through list of results
                    int counter = 8;
                    foreach (PublishedSearchResult result in publishedContentQuery.Search(queryExecutor).OrderByDescending(x => x.Content.Value<DateTime>("postDate")))//.Take(8))
                    {
                        //Provide results IF page is contained in root site
                        if (result.Content.Path.Contains(cmListLattusAR.Path))
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


            var viewModel = new ComposedPageViewModel<NewsCardAR, NewsCardViewModel>
            {
                Page = cmPage,
                ViewModel = lstVmodel
            };



            //return null;
            return View(Common.View.NewsCardAR, viewModel);
        }



        private ListItemViewModel RenderNewsCard(IPublishedContent ipChild)
        {
            //Instantiate variables
            ListItemViewModel item = new ListItemViewModel();

            //Obtain model vsn of child node
            ContentModels.NewsCardAR cmNewsCardAR = new ContentModels.NewsCardAR(ipChild, new PublishedValueFallback(_serviceContext, _variationContextAccessor));

            //Obtain all data from model
            item.Id = cmNewsCardAR.Id;
            item.DocType = cmNewsCardAR.ContentType.Alias;
            item.Title = cmNewsCardAR.Title ?? "";
            item.Link = new Link()
            {
                Url = cmNewsCardAR.Url()
            };

            if (cmNewsCardAR.Subtitle != null)
                item.Subtitle = cmNewsCardAR.Subtitle;

            if (cmNewsCardAR.Summary != null)
            {
                item.Summary = cmNewsCardAR.Summary;
                item.EncodedSummary = HttpUtility.HtmlEncode(item.Summary.ToLower());
            }


            item.PostDate = cmNewsCardAR.PostDate.ToString("MMM d, yyyy");


            //Determine if image is cropped or not
            if (cmNewsCardAR.NoCrop)
            {
                item.PrimaryImgUrl = cmNewsCardAR.PrimaryImage?.Url() + "?format=webp";
                item.AdditionalClasses = "no-crop-vsn";
            }
            else
            {
                item.PrimaryImgUrl = cmNewsCardAR.PrimaryImage?.GetCropUrl(Common.Crop.LearnMore_500x550) + "&format=webp";
            }


            if (cmNewsCardAR.AreasOfInterest != null)
            {
                foreach (var i in cmNewsCardAR.AreasOfInterest)
                    item.LstAreasOfInterest.Add(i.Name);
                item.jsonAreasOfInterest = Newtonsoft.Json.JsonConvert.SerializeObject(item.LstAreasOfInterest);
            }


            item.InViewAnimation = cmNewsCardAR.InViewAnimation;

            if (!string.IsNullOrWhiteSpace(cmNewsCardAR.HoverTitle))
            {
                item.HoverTitle = cmNewsCardAR.HoverTitle;
                item.ShowHoverContent = true;
            }
            if (cmNewsCardAR.HoverTip != null && !string.IsNullOrWhiteSpace(cmNewsCardAR.HoverTip.ToString()))
            {
                item.HoverTip = cmNewsCardAR.HoverTip;
                item.ShowHoverContent = true;
            }

            //Determine if card content should appear
            if (!string.IsNullOrWhiteSpace(cmNewsCardAR.Title) ||
                (cmNewsCardAR.Summary != null && !string.IsNullOrWhiteSpace(cmNewsCardAR.Summary.ToString())))
            {
                item.ShowCardContent = true;
            }
            item.ShowCardContent = true;


            return item;
        }


    }
}
