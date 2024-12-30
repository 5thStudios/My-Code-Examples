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
    public class PersonLandingPageFndController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<PersonLandingPageFndController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;

        public PersonLandingPageFndController(
                ILogger<PersonLandingPageFndController> _logger,
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
            var cmPage = new PersonLandingPageFnd(CurrentPage, _publishedValueFallback);
            PersonLandingPgViewModel lstVmodel = new PersonLandingPgViewModel();


            try
            {
                //Instantiate variableas
                ContentModels.ListLattusFnd cmListLattusFnd = new ListLattusFnd(cmPage.Parent, new PublishedValueFallback(_serviceContext, _variationContextAccessor));
                lstVmodel.LearnMoreTitle = cmListLattusFnd.LearnMoreTitle;
                if (!string.IsNullOrWhiteSpace(cmListLattusFnd.PrefixTitle))
                    lstVmodel.PrefixTitle = cmListLattusFnd.PrefixTitle + ": ";


                //Get all siblings
                if (cmPage.Parent!.Children.Any())
                {
                    foreach (var ipChild in cmPage.Parent.Children)
                    {
                        //Skip current person
                        if (ipChild.Id == cmPage.Id)
                            continue;


                        //
                        ContentModels.PersonLandingPageFnd cmPersonLandingPg = new ContentModels.PersonLandingPageFnd(ipChild, new PublishedValueFallback(_serviceContext, _variationContextAccessor));

                        ListItemViewModel item = new ListItemViewModel();
                        item.Id = cmPersonLandingPg.Id;
                        item.Title = cmPersonLandingPg.Title ?? "";
                        item.Link = new Link()
                        {
                            Url = cmPersonLandingPg.Url()
                        };


                        if (cmPersonLandingPg?.Subtitle != null)
                            item.Subtitle = cmPersonLandingPg.Subtitle;


                        if (cmPersonLandingPg?.Summary != null)
                        {
                            item.Summary = cmPersonLandingPg.Summary;
                            item.EncodedSummary = HttpUtility.HtmlEncode(item.Summary.ToLower());
                        }


                        item.PrimaryImgUrl = cmPersonLandingPg?.PrimaryImage?.GetCropUrl(Common.Crop.Staff_525x500);


                        if (cmPersonLandingPg?.AreasOfInterest != null)
                        {
                            foreach (var i in cmPersonLandingPg.AreasOfInterest)
                                item.LstAreasOfInterest.Add(i.Name);
                            item.jsonAreasOfInterest = Newtonsoft.Json.JsonConvert.SerializeObject(item.LstAreasOfInterest);
                        }


                        if (cmPersonLandingPg?.Staff != null)
                        {
                            foreach (var i in cmPersonLandingPg.Staff)
                                item.LstStaff.Add(i.Name);
                            item.jsonStaff = Newtonsoft.Json.JsonConvert.SerializeObject(item.LstStaff);
                        }

                        item.InViewAnimation = cmPersonLandingPg?.InViewAnimation;

                        if (!string.IsNullOrWhiteSpace(cmPersonLandingPg?.HoverTitle))
                        {
                            item.HoverTitle = cmPersonLandingPg.HoverTitle;
                            item.ShowHoverContent = true;
                        }
                        if (cmPersonLandingPg?.HoverTip != null && !string.IsNullOrWhiteSpace(cmPersonLandingPg.HoverTip.ToString()))
                        {
                            item.HoverTip = cmPersonLandingPg.HoverTip;
                            item.ShowHoverContent = true;
                        }

                        //Determine if card content should appear
                        if (!String.IsNullOrEmpty(cmPersonLandingPg?.Title) || !String.IsNullOrEmpty(cmPersonLandingPg?.Subtitle) || !string.IsNullOrEmpty(cmPersonLandingPg?.LinkedIn) || !string.IsNullOrEmpty(cmPersonLandingPg?.Facebook))
                            item.ShowCardContent = true;


                        if (!string.IsNullOrEmpty(cmPersonLandingPg?.LinkedIn))
                            item.LinkedInUrl = cmPersonLandingPg.LinkedIn;

                        if (!string.IsNullOrEmpty(cmPersonLandingPg?.Facebook))
                            item.FacebookUrl = cmPersonLandingPg.Facebook;


                        lstVmodel.LstListItems.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }


            var viewModel = new ComposedPageViewModel<PersonLandingPageFnd, PersonLandingPgViewModel>
            {
                Page = cmPage,
                ViewModel = lstVmodel
            };

            return View(Common.View.PersonLandingPageFnd, viewModel);
        }
    }
}
