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
    public class NewsCardSolController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private readonly IExamineManager ExamineManager;
        private readonly IPublishedContentQuery publishedContentQuery;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<NewsCardSolController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;

        public NewsCardSolController(
                ILogger<NewsCardSolController> _logger,
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
            ContentModels.NewsCardSol cmPage = new ContentModels.NewsCardSol(CurrentPage, _publishedValueFallback);
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
                ContentModels.NewsLattusSol cmListLattusSol = new NewsLattusSol(ipParent, new PublishedValueFallback(_serviceContext, _variationContextAccessor));
                lstVmodel.LearnMoreTitle = cmListLattusSol.LearnMoreTitle;
                if (!string.IsNullOrWhiteSpace(cmListLattusSol.PrefixTitle))
                    lstVmodel.PrefixTitle = cmListLattusSol.PrefixTitle + ": ";


                //Get all siblings
                if (ExamineManager.TryGetIndex(UmbracoIndexes.ExternalIndexName, out IIndex? index))
                {
                    //Query
                    var queryExecutor = index.Searcher
                        .CreateQuery(IndexTypes.Content)
                        .NodeTypeAlias("newsCardSol");


                    //Loop through list of results
                    int counter = 8;
                    foreach (PublishedSearchResult result in publishedContentQuery.Search(queryExecutor).OrderByDescending(x => x.Content.Value<DateTime>("postDate")))//.Take(8))
                    {
                        //Provide results IF page is contained in root site
                        if (result.Content.Path.Contains(cmListLattusSol.Path))
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


            var viewModel = new ComposedPageViewModel<NewsCardSol, NewsCardViewModel>
            {
                Page = cmPage,
                ViewModel = lstVmodel
            };



            //return null;
            return View(Common.View.NewsCardSol, viewModel);
        }



        private ListItemViewModel RenderNewsCard(IPublishedContent ipChild)
        {
            //Instantiate variables
            ListItemViewModel item = new ListItemViewModel();

            //Obtain model vsn of child node
            ContentModels.NewsCardSol cmNewsCardSol = new ContentModels.NewsCardSol(ipChild, new PublishedValueFallback(_serviceContext, _variationContextAccessor));

            //Obtain all data from model
            item.Id = cmNewsCardSol.Id;
            item.DocType = cmNewsCardSol.ContentType.Alias;
            item.Title = cmNewsCardSol.Title ?? "";
            item.Link = new Link()
            {
                Url = cmNewsCardSol.Url()
            };

            if (cmNewsCardSol.Subtitle != null)
                item.Subtitle = cmNewsCardSol.Subtitle;

            if (cmNewsCardSol.Summary != null)
            {
                item.Summary = cmNewsCardSol.Summary;
                item.EncodedSummary = HttpUtility.HtmlEncode(item.Summary.ToLower());
            }


            item.PostDate = cmNewsCardSol.PostDate.ToString("MMM d, yyyy");


            //Determine if image is cropped or not
            if (cmNewsCardSol.NoCrop)
            {
                item.PrimaryImgUrl = cmNewsCardSol.PrimaryImage?.Url() + "?format=webp";
                item.AdditionalClasses = "no-crop-vsn";
            }
            else
            {
                item.PrimaryImgUrl = cmNewsCardSol.PrimaryImage?.GetCropUrl(Common.Crop.LearnMore_500x550) + "&format=webp";
            }


            if (cmNewsCardSol.AreasOfInterest != null)
            {
                foreach (var i in cmNewsCardSol.AreasOfInterest)
                    item.LstAreasOfInterest.Add(i.Name);
                item.jsonAreasOfInterest = Newtonsoft.Json.JsonConvert.SerializeObject(item.LstAreasOfInterest);
            }


            item.InViewAnimation = cmNewsCardSol.InViewAnimation;

            if (!string.IsNullOrWhiteSpace(cmNewsCardSol.HoverTitle))
            {
                item.HoverTitle = cmNewsCardSol.HoverTitle;
                item.ShowHoverContent = true;
            }
            if (cmNewsCardSol.HoverTip != null && !string.IsNullOrWhiteSpace(cmNewsCardSol.HoverTip.ToString()))
            {
                item.HoverTip = cmNewsCardSol.HoverTip;
                item.ShowHoverContent = true;
            }

            //Determine if card content should appear
            if (!string.IsNullOrWhiteSpace(cmNewsCardSol.Title) ||
                (cmNewsCardSol.Summary != null && !string.IsNullOrWhiteSpace(cmNewsCardSol.Summary.ToString())))
            {
                item.ShowCardContent = true;
            }
            item.ShowCardContent = true;


            return item;
        }


    }
}
