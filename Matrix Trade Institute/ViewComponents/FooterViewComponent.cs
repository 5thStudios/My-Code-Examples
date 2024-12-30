using Examine;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using www.Models;
using www.ViewModels;
using cm = Umbraco.Cms.Web.Common.PublishedModels;


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





        public async Task<IViewComponentResult> InvokeAsync(IPublishedContent ipModel)
        {

            //Instantiate variables
            FooterViewModel footerVM = new FooterViewModel();

            try
            {
                //Obtain content models
                cm.Home cmHome = new cm.Home(ipModel.Root(), PublishedValueFallback);
                IPublishedContent ipSiteSettings = UmbracoHelper.ContentAtRoot().FirstOrDefault(x => x.ContentType.Alias.Equals(Common.Doctype.SiteSettings));
                cm.Common cmCommon = new cm.Common(ipSiteSettings!.FirstChildOfType(Common.Doctype.Common), new PublishedValueFallback(_serviceContext, _variationContextAccessor));
                cm.SocialMedia cmSocial = new cm.SocialMedia(ipSiteSettings!.FirstChildOfType(Common.Doctype.SocialMedia), new PublishedValueFallback(_serviceContext, _variationContextAccessor));
                cm.Footer cmFooter = new cm.Footer(ipSiteSettings!.FirstChildOfType(Common.Doctype.Footer), new PublishedValueFallback(_serviceContext, _variationContextAccessor));
                cm.LandingPage? cmLandingPg = null;


                //Obtain site footer data
                footerVM.SiteName = cmCommon.SiteName;
                footerVM.SiteLogoUrl = cmCommon.MainLogoVertical?.Url();
                footerVM.License = cmFooter.License;
                footerVM.Address = new HtmlString(cmFooter.Address);
                footerVM.ContactEmail = cmCommon.ContactEmail;

                footerVM.PhoneTel = cmCommon.PhoneNo;
                string phn = footerVM.PhoneTel!.Substring(1); //remove the leading '1'
                footerVM.PhoneNo = String.Format("{0:###-###-####}", Int64.Parse(phn));

                footerVM.Bounce = true;


                //Determine if footer to be bounced of squeezed.
                if (ipModel.IsDocumentType(www.Models.Common.Doctype.LandingPage))
                {
                    cmLandingPg = new cm.LandingPage(ipModel, PublishedValueFallback);
                    footerVM.Bounce = !cmLandingPg.UserFunnel;
                }



                if (footerVM.Bounce)  //SHOW MAIN SITE FOOTER
                {
                    //Obtain social links
                    foreach (cm.ImageLink _link in cmSocial.Links.Select(x => x.Content))
                    {
                        footerVM.LstSocialLinks!.Add(new Models.Link()
                        {
                            ImgUrl = _link.Image.Url(),
                            Title = _link.Link.Name,
                            Url = _link.Link.Url,
                            Target = _link.Link.Target
                        });
                    }

                    //Obtain footer nav
                    foreach (var _link in cmFooter.FooterNav)
                    {
                        footerVM.LstFooterLinks!.Add(new Models.Link()
                        {
                            Url = _link.Url,
                            Title = _link.Name,
                            Target = _link.Target
                        });
                    }
                }
                else //SHOW CUSTOM SITE FOOTER
                {
                    footerVM.CustomNavigation = cmLandingPg?.CustomFooterNavigation;
                }





            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }



            //Return data to partialview
            return View(Common.Partial.Footer, footerVM);

        }
    }
}
