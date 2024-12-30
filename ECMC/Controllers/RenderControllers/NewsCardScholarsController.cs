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
    public class NewsCardScholarsController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private readonly IExamineManager ExamineManager;
        private readonly IPublishedContentQuery publishedContentQuery;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<NewsCardScholarsController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;

        public NewsCardScholarsController(
                ILogger<NewsCardScholarsController> _logger,
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
            ContentModels.NewsCardScholars cmPage = new ContentModels.NewsCardScholars(CurrentPage, _publishedValueFallback);
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
                ContentModels.NewsLattusScholars cmListLattusScholars = new NewsLattusScholars(ipParent, new PublishedValueFallback(_serviceContext, _variationContextAccessor));
                lstVmodel.LearnMoreTitle = cmListLattusScholars.LearnMoreTitle;
                if (!string.IsNullOrWhiteSpace(cmListLattusScholars.PrefixTitle))
                    lstVmodel.PrefixTitle = cmListLattusScholars.PrefixTitle + ": ";


                //Get all siblings
                if (ExamineManager.TryGetIndex(UmbracoIndexes.ExternalIndexName, out IIndex? index))
                {
                    //Query
                    var queryExecutor = index.Searcher
                        .CreateQuery(IndexTypes.Content)
                        .NodeTypeAlias("newsCardScholars");


                    //Loop through list of results
                    int counter = 8;
                    foreach (PublishedSearchResult result in publishedContentQuery.Search(queryExecutor).OrderByDescending(x => x.Content.Value<DateTime>("postDate")))//.Take(8))
                    {
                        //Provide results IF page is contained in root site
                        if (result.Content.Path.Contains(cmListLattusScholars.Path))
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







                //foreach (var ipChild in cmPage.Parent.Children)
                //    {
                //        //Skip current person
                //        if (ipChild.Id == cmPage.Id)
                //            continue;


                //        //Instantiate child model
                //        ContentModels.NewsCardScholars cmNewsAndEventsLandingPg = new ContentModels.NewsCardScholars(ipChild, new PublishedValueFallback(_serviceContext, _variationContextAccessor));


                //        ListItemViewModel item = new ListItemViewModel();
                //        item.Id = cmNewsAndEventsLandingPg.Id;
                //        item.Title = cmNewsAndEventsLandingPg.Title ?? "";
                //        item.Link = new Link()
                //        {
                //            Url = cmNewsAndEventsLandingPg.Url()
                //        };


                //        if (cmNewsAndEventsLandingPg?.Subtitle != null)
                //            item.Subtitle = cmNewsAndEventsLandingPg.Subtitle;


                //        if (cmNewsAndEventsLandingPg?.Summary != null)
                //        {
                //            item.Summary = cmNewsAndEventsLandingPg.Summary;
                //            item.EncodedSummary = HttpUtility.HtmlEncode(item.Summary.ToLower());
                //        }


                //        item.PostDate = cmNewsAndEventsLandingPg.PostDate.ToString("MMM d, yyyy");


                //        //Determine if image is cropped or not
                //        if (cmNewsAndEventsLandingPg.NoCrop)
                //        {
                //            item.PrimaryImgUrl = cmNewsAndEventsLandingPg.PrimaryImage?.Url() + "?format=webp";
                //            item.AdditionalClasses = "no-crop-vsn";
                //        }
                //        else
                //        {
                //            item.PrimaryImgUrl = cmNewsAndEventsLandingPg.PrimaryImage?.GetCropUrl(Common.Crop.LearnMore_500x550) + "&format=webp";
                //        }


                //        item.InViewAnimation = cmNewsAndEventsLandingPg?.InViewAnimation;

                //        if (!string.IsNullOrWhiteSpace(cmNewsAndEventsLandingPg?.HoverTitle))
                //        {
                //            item.HoverTitle = cmNewsAndEventsLandingPg.HoverTitle;
                //            item.ShowHoverContent = true;
                //        }
                //        if (cmNewsAndEventsLandingPg?.HoverTip != null && !string.IsNullOrWhiteSpace(cmNewsAndEventsLandingPg.HoverTip.ToString()))
                //        {
                //            item.HoverTip = cmNewsAndEventsLandingPg.HoverTip;
                //            item.ShowHoverContent = true;
                //        }

                //        //Determine if card content should appear
                //        if (!string.IsNullOrWhiteSpace(cmNewsAndEventsLandingPg.Title) ||
                //            (cmNewsAndEventsLandingPg.Summary != null && !string.IsNullOrWhiteSpace(cmNewsAndEventsLandingPg.Summary.ToString())))
                //        {
                //            item.ShowCardContent = true;
                //        }


                //        lstVmodel.LstListItems.Add(item);
                //    }
               


            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }


            var viewModel = new ComposedPageViewModel<NewsCardScholars, NewsCardViewModel>
            {
                Page = cmPage,
                ViewModel = lstVmodel
            };



            //return null;
            return View(Common.View.NewsCardScholars, viewModel);
        }



        private ListItemViewModel RenderNewsCard(IPublishedContent ipChild)
        {
            //Instantiate variables
            ListItemViewModel item = new ListItemViewModel();

            //Obtain model vsn of child node
            ContentModels.NewsCardScholars cmNewsCardScholars = new ContentModels.NewsCardScholars(ipChild, new PublishedValueFallback(_serviceContext, _variationContextAccessor));

            //Obtain all data from model
            item.Id = cmNewsCardScholars.Id;
            item.DocType = cmNewsCardScholars.ContentType.Alias;
            item.Title = cmNewsCardScholars.Title ?? "";
            item.Link = new Link()
            {
                Url = cmNewsCardScholars.Url()
            };

            if (cmNewsCardScholars.Subtitle != null)
                item.Subtitle = cmNewsCardScholars.Subtitle;

            if (cmNewsCardScholars.Summary != null)
            {
                item.Summary = cmNewsCardScholars.Summary;
                item.EncodedSummary = HttpUtility.HtmlEncode(item.Summary.ToLower());
            }


            item.PostDate = cmNewsCardScholars.PostDate.ToString("MMM d, yyyy");


            //Determine if image is cropped or not
            if (cmNewsCardScholars.NoCrop)
            {
                item.PrimaryImgUrl = cmNewsCardScholars.PrimaryImage?.Url() + "?format=webp";
                item.AdditionalClasses = "no-crop-vsn";
            }
            else
            {
                item.PrimaryImgUrl = cmNewsCardScholars.PrimaryImage?.GetCropUrl(Common.Crop.LearnMore_500x550) + "&format=webp";
            }


            if (cmNewsCardScholars.AreasOfInterest != null)
            {
                foreach (var i in cmNewsCardScholars.AreasOfInterest)
                    item.LstAreasOfInterest.Add(i.Name);
                item.jsonAreasOfInterest = Newtonsoft.Json.JsonConvert.SerializeObject(item.LstAreasOfInterest);
            }


            item.InViewAnimation = cmNewsCardScholars.InViewAnimation;

            if (!string.IsNullOrWhiteSpace(cmNewsCardScholars.HoverTitle))
            {
                item.HoverTitle = cmNewsCardScholars.HoverTitle;
                item.ShowHoverContent = true;
            }
            if (cmNewsCardScholars.HoverTip != null && !string.IsNullOrWhiteSpace(cmNewsCardScholars.HoverTip.ToString()))
            {
                item.HoverTip = cmNewsCardScholars.HoverTip;
                item.ShowHoverContent = true;
            }

            //Determine if card content should appear
            if (!string.IsNullOrWhiteSpace(cmNewsCardScholars.Title) ||
                (cmNewsCardScholars.Summary != null && !string.IsNullOrWhiteSpace(cmNewsCardScholars.Summary.ToString())))
            {
                item.ShowCardContent = true;
            }
            item.ShowCardContent = true;


            return item;
        }


    }
}
