using ECMC_Umbraco.Models;
using ECMC_Umbraco.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Web;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;
using UmbracoProject.Models;
using ContentModels = Umbraco.Cms.Web.Common.PublishedModels;

namespace www.Controllers
{
    public class NewsAndEventsLandingPageARController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<NewsAndEventsLandingPageARController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;

        public NewsAndEventsLandingPageARController(
                ILogger<NewsAndEventsLandingPageARController> _logger,
                ICompositeViewEngine compositeViewEngine,
                IUmbracoContextAccessor umbracoContextAccessor,
                UmbracoHelper _UmbracoHelper,
                IPublishedValueFallback publishedValueFallback,
                ServiceContext context,
                IVariationContextAccessor variationContextAccessor
             )
            : base(_logger, compositeViewEngine, umbracoContextAccessor)
        {
            _publishedValueFallback = publishedValueFallback;
            UmbracoHelper = _UmbracoHelper;
            logger = _logger;
            _serviceContext = context;
            _variationContextAccessor = variationContextAccessor;
        }

        public override IActionResult Index()
        {
            //Instantiate variableas
            ContentModels.NewsAndEventsLandingPageAR cmPage = new ContentModels.NewsAndEventsLandingPageAR(CurrentPage, _publishedValueFallback);
            NewsAndEventsLandingPgViewModel lstVmodel = new NewsAndEventsLandingPgViewModel();


            try
            {
                //Instantiate variableas
                ContentModels.ListLattusAR cmListLattusAR = new ListLattusAR(cmPage.Parent, new PublishedValueFallback(_serviceContext, _variationContextAccessor));
                lstVmodel.LearnMoreTitle = cmListLattusAR.LearnMoreTitle;
                if (!string.IsNullOrWhiteSpace(cmListLattusAR.PrefixTitle))
                    lstVmodel.PrefixTitle = cmListLattusAR.PrefixTitle + ": ";


                //Get all siblings
                if (cmPage.Parent!.Children.Any())
                {
                    foreach (var ipChild in cmPage.Parent.Children)
                    {
                        //Skip current person
                        if (ipChild.Id == cmPage.Id)
                            continue;


                        //Instantiate child model
                        ContentModels.NewsAndEventsLandingPageAR cmNewsAndEventsLandingPg = new ContentModels.NewsAndEventsLandingPageAR(ipChild, new PublishedValueFallback(_serviceContext, _variationContextAccessor));


                        ListItemViewModel item = new ListItemViewModel();
                        item.Id = cmNewsAndEventsLandingPg.Id;
                        item.Title = cmNewsAndEventsLandingPg.Title ?? "";
                        item.Link = new Link()
                        {
                            Url = cmNewsAndEventsLandingPg.Url()
                        };


                        if (cmNewsAndEventsLandingPg?.Subtitle != null)
                            item.Subtitle = cmNewsAndEventsLandingPg.Subtitle;


                        if (cmNewsAndEventsLandingPg?.Summary != null)
                        {
                            item.Summary = cmNewsAndEventsLandingPg.Summary;
                            item.EncodedSummary = HttpUtility.HtmlEncode(item.Summary.ToLower());
                        }


                        if (cmNewsAndEventsLandingPg.PostDate != null)
                        {
                            item.PostDate = cmNewsAndEventsLandingPg.PostDate;
                        }


                        //Determine if image is cropped or not
                        if (cmNewsAndEventsLandingPg.NoCrop)
                        {
                            item.PrimaryImgUrl = cmNewsAndEventsLandingPg.PrimaryImage?.Url() + "?format=webp";
                            item.AdditionalClasses = "no-crop-vsn";
                        }
                        else
                        {
                            item.PrimaryImgUrl = cmNewsAndEventsLandingPg.PrimaryImage?.GetCropUrl(Common.Crop.LearnMore_500x550) + "&format=webp";
                        }


                        item.InViewAnimation = cmNewsAndEventsLandingPg?.InViewAnimation;

                        if (!string.IsNullOrWhiteSpace(cmNewsAndEventsLandingPg?.HoverTitle))
                        {
                            item.HoverTitle = cmNewsAndEventsLandingPg.HoverTitle;
                            item.ShowHoverContent = true;
                        }
                        if (cmNewsAndEventsLandingPg?.HoverTip != null && !string.IsNullOrWhiteSpace(cmNewsAndEventsLandingPg.HoverTip.ToString()))
                        {
                            item.HoverTip = cmNewsAndEventsLandingPg.HoverTip;
                            item.ShowHoverContent = true;
                        }

                        //Determine if card content should appear
                        if (!string.IsNullOrWhiteSpace(cmNewsAndEventsLandingPg.PostDate) ||
                            !string.IsNullOrWhiteSpace(cmNewsAndEventsLandingPg.Title) ||
                            (cmNewsAndEventsLandingPg.Summary != null && !string.IsNullOrWhiteSpace(cmNewsAndEventsLandingPg.Summary.ToString())))
                        {
                            item.ShowCardContent = true;
                        }


                        lstVmodel.LstListItems.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }


            var viewModel = new ComposedPageViewModel<NewsAndEventsLandingPageAR, NewsAndEventsLandingPgViewModel>
            {
                Page = cmPage,
                ViewModel = lstVmodel
            };



            //return null;
            return View(Common.View.NewsAndEventsLandingPageAR, viewModel);
        }
    }
}
