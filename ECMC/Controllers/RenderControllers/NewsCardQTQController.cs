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
    public class NewsCardQTQController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private readonly IExamineManager ExamineManager;
        private readonly IPublishedContentQuery publishedContentQuery;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<NewsCardQTQController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;

        public NewsCardQTQController(
                ILogger<NewsCardQTQController> _logger,
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
            ContentModels.NewsCardQtq cmPage = new ContentModels.NewsCardQtq(CurrentPage, _publishedValueFallback);
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
                ContentModels.NewsLattusQtq cmListLattusQTQ = new NewsLattusQtq(ipParent, new PublishedValueFallback(_serviceContext, _variationContextAccessor));
                lstVmodel.LearnMoreTitle = cmListLattusQTQ.LearnMoreTitle;
                if (!string.IsNullOrWhiteSpace(cmListLattusQTQ.PrefixTitle))
                    lstVmodel.PrefixTitle = cmListLattusQTQ.PrefixTitle + ": ";


                //Get all siblings
                if (ExamineManager.TryGetIndex(UmbracoIndexes.ExternalIndexName, out IIndex? index))
                {
                    //Query
                    var queryExecutor = index.Searcher
                        .CreateQuery(IndexTypes.Content)
                        .NodeTypeAlias("newsCardQTQ");


                    //Loop through list of results
                    int counter = 8;
                    foreach (PublishedSearchResult result in publishedContentQuery.Search(queryExecutor).OrderByDescending(x => x.Content.Value<DateTime>("postDate")))//.Take(8))
                    {
                        //Provide results IF page is contained in root site
                        if (result.Content.Path.Contains(cmListLattusQTQ.Path))
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


            var viewModel = new ComposedPageViewModel<NewsCardQtq, NewsCardViewModel>
            {
                Page = cmPage,
                ViewModel = lstVmodel
            };



            //return null;
            return View(Common.View.NewsCardQTQ, viewModel);
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
