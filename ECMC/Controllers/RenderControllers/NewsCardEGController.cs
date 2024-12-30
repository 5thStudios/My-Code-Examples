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
    public class NewsCardEGController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private readonly IExamineManager ExamineManager;
        private readonly IPublishedContentQuery publishedContentQuery;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<NewsCardEGController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;

        public NewsCardEGController(
                ILogger<NewsCardEGController> _logger,
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
            ContentModels.NewsCardEG cmPage = new ContentModels.NewsCardEG(CurrentPage, _publishedValueFallback);
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
                ContentModels.NewsLattusEG cmListLattusEG = new NewsLattusEG(ipParent, new PublishedValueFallback(_serviceContext, _variationContextAccessor));
                lstVmodel.LearnMoreTitle = cmListLattusEG.LearnMoreTitle;
                if (!string.IsNullOrWhiteSpace(cmListLattusEG.PrefixTitle))
                    lstVmodel.PrefixTitle = cmListLattusEG.PrefixTitle + ": ";


                //Get all siblings
                if (ExamineManager.TryGetIndex(UmbracoIndexes.ExternalIndexName, out IIndex? index))
                {
                    //Query
                    var queryExecutor = index.Searcher
                        .CreateQuery(IndexTypes.Content)
                        .NodeTypeAlias("newsCardEG");


                    //Loop through list of results
                    int counter = 8;
                    foreach (PublishedSearchResult result in publishedContentQuery.Search(queryExecutor).OrderByDescending(x => x.Content.Value<DateTime>("postDate")))//.Take(8))
                    {
                        //Provide results IF page is contained in root site
                        if (result.Content.Path.Contains(cmListLattusEG.Path))
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


            var viewModel = new ComposedPageViewModel<NewsCardEG, NewsCardViewModel>
            {
                Page = cmPage,
                ViewModel = lstVmodel
            };



            //return null;
            return View(Common.View.NewsCardEG, viewModel);
        }



        private ListItemViewModel RenderNewsCard(IPublishedContent ipChild)
        {
            //Instantiate variables
            ListItemViewModel item = new ListItemViewModel();

            //Obtain model vsn of child node
            ContentModels.NewsCardEG cmNewsCardEG = new ContentModels.NewsCardEG(ipChild, new PublishedValueFallback(_serviceContext, _variationContextAccessor));

            //Obtain all data from model
            item.Id = cmNewsCardEG.Id;
            item.DocType = cmNewsCardEG.ContentType.Alias;
            item.Title = cmNewsCardEG.Title ?? "";
            item.Link = new Link()
            {
                Url = cmNewsCardEG.Url()
            };

            if (cmNewsCardEG.Subtitle != null)
                item.Subtitle = cmNewsCardEG.Subtitle;

            if (cmNewsCardEG.Summary != null)
            {
                item.Summary = cmNewsCardEG.Summary;
                item.EncodedSummary = HttpUtility.HtmlEncode(item.Summary.ToLower());
            }


            item.PostDate = cmNewsCardEG.PostDate.ToString("MMM d, yyyy");


            //Determine if image is cropped or not
            if (cmNewsCardEG.NoCrop)
            {
                item.PrimaryImgUrl = cmNewsCardEG.PrimaryImage?.Url() + "?format=webp";
                item.AdditionalClasses = "no-crop-vsn";
            }
            else
            {
                item.PrimaryImgUrl = cmNewsCardEG.PrimaryImage?.GetCropUrl(Common.Crop.LearnMore_500x550) + "&format=webp";
            }


            if (cmNewsCardEG.AreasOfInterest != null)
            {
                foreach (var i in cmNewsCardEG.AreasOfInterest)
                    item.LstAreasOfInterest.Add(i.Name);
                item.jsonAreasOfInterest = Newtonsoft.Json.JsonConvert.SerializeObject(item.LstAreasOfInterest);
            }


            item.InViewAnimation = cmNewsCardEG.InViewAnimation;

            if (!string.IsNullOrWhiteSpace(cmNewsCardEG.HoverTitle))
            {
                item.HoverTitle = cmNewsCardEG.HoverTitle;
                item.ShowHoverContent = true;
            }
            if (cmNewsCardEG.HoverTip != null && !string.IsNullOrWhiteSpace(cmNewsCardEG.HoverTip.ToString()))
            {
                item.HoverTip = cmNewsCardEG.HoverTip;
                item.ShowHoverContent = true;
            }

            //Determine if card content should appear
            if (!string.IsNullOrWhiteSpace(cmNewsCardEG.Title) ||
                (cmNewsCardEG.Summary != null && !string.IsNullOrWhiteSpace(cmNewsCardEG.Summary.ToString())))
            {
                item.ShowCardContent = true;
            }
            item.ShowCardContent = true;


            return item;
        }


    }
}
