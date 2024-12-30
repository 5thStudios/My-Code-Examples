using ECMC_Umbraco.Models;
using Examine;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using ContentModels = Umbraco.Cms.Web.Common.PublishedModels;
using Microsoft.AspNetCore.Html;
using Umbraco.Cms.Core.Models;
using System.Web;


namespace www.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private IUmbracoContextAccessor Context;
        private UmbracoHelper UmbracoHelper;
        private IPublishedValueFallback PublishedValueFallback;
        private readonly IExamineManager ExamineManager;
        private readonly ILogger<FooterViewComponent> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;


        public FooterViewComponent(
            ILogger<FooterViewComponent> _logger,
            IExamineManager _ExamineManager,
                IUmbracoContextAccessor _Context,
                UmbracoHelper _UmbracoHelper,
                IPublishedValueFallback _PublishedValueFallback,
                ServiceContext context,
                IVariationContextAccessor variationContextAccessor)
        {
            ExamineManager = _ExamineManager ?? throw new ArgumentNullException(nameof(_ExamineManager));
            Context = _Context;
            UmbracoHelper = _UmbracoHelper;
            PublishedValueFallback = _PublishedValueFallback;
            _serviceContext = context;
            _variationContextAccessor = variationContextAccessor;
            logger = _logger;

        }





        public async Task<IViewComponentResult> InvokeAsync(IPublishedContent ipModel, string footerSelect)
        {

            //Instantiate variables
            FooterViewModel footerVM = new FooterViewModel();

            try
            {

                //Obtain All content
                //========================================
                //Obtain content models
                // var cmHome = new ContentModels.HomeEif(ipModel.Root(), new PublishedValueFallback(_serviceContext, _variationContextAccessor));
                var cmSite = new ContentModels.Site(ipModel.Root().Value<IPublishedContent>("siteSettings"), new PublishedValueFallback(_serviceContext, _variationContextAccessor));

                //Obtain site footer data
                footerVM.SiteName = cmSite.SiteName;
                footerVM.SiteLogoUrl = cmSite.SiteLogo?.Url();
                footerVM.ContactEmail = cmSite.ContactEmail;
                footerVM.ContactHeadline = cmSite.ContactHeadline;
                footerVM.SocialHeadline = cmSite.SocialHeadline;
                footerVM.ContactName = cmSite.ContactName;
                footerVM.Address = new HtmlString(cmSite.Address?.ToHtmlString());
                if (!string.IsNullOrEmpty(cmSite.Phone))
                {
                    footerVM.Phone = cmSite.Phone;
                    string phn = cmSite.Phone.Substring(1);
                    //String.Format("{0:###-###-####}", phn);
                    var phnFormatted = String.Format("{0:###-###-####}", Int64.Parse(phn));
                    //var b = String.Format("{0:###-###-####}", 1234567884);
                    footerVM.PhoneTel = phnFormatted;
                }


                if (cmSite.SocialMediaLinks != null)
                {
                    footerVM.LstSocialMediaLinks = new List<UmbracoProject.Models.Link>();
                    foreach (var _link in cmSite.SocialMediaLinks)
                    {
                        var cmImageLink = new ContentModels.ImageLink(_link.Content, new PublishedValueFallback(_serviceContext, _variationContextAccessor));

                        footerVM.LstSocialMediaLinks.Add(new UmbracoProject.Models.Link()
                        {
                            Url = cmImageLink.Link?.Url,
                            Title = cmImageLink.Link?.Name,
                            Target = cmImageLink.Link?.Target,
                            ImgUrl = cmImageLink.Image?.Url()
                        });
                    }
                }

                if (cmSite.FooterNavigation != null)
                {
                    footerVM.LstFooterNavLinks = new List<UmbracoProject.Models.Link>();
                    foreach (var _link in cmSite.FooterNavigation)
                    {
                        footerVM.LstFooterNavLinks.Add(new UmbracoProject.Models.Link()
                        {
                            Url = _link.Url,
                            Title = _link.Name,
                            Target = _link.Target,
                            IsMedia = _link.Type == LinkType.Media
                        });
                    }
                }
            }

            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }



            //Return data to partialview
            if (footerSelect == "footerA")
            {
                return View(Common.Partial.Footer_A, footerVM);
            }
            else if(footerSelect == "footerB")
            {
                return View(Common.Partial.Footer_B, footerVM);
            }
            else if (footerSelect == "footerAR")
            {
                return View(Common.Partial.Footer_AR, footerVM);
            }
            else
            {
                return View(Common.Partial.Footer_B, footerVM);
            }

        }
    }
}
