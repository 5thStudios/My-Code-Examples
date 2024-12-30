using Examine;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using www.Models;
using www.ViewModels;
using cm = www.Models.PublishedModels;


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





        public async Task<IViewComponentResult> InvokeAsync(IPublishedContent ipModel, string site = "")
        {

            //Instantiate variables
            FooterViewModel footerVM = new FooterViewModel();

            try
            {
                //Obtain content models
                cm.Home cmHome = new cm.Home(ipModel.Root(), PublishedValueFallback);


                //Obtain site footer data
                footerVM.CopyrightText = cmHome.SiteCopyright?.Replace("%%YEAR%%", DateTime.Today.Year.ToString());
                if (ipModel.Root().HasProperty(Common.Property.MoreInfo) && ipModel.Root().HasValue(Common.Property.MoreInfo))
                {
                    footerVM.MoreInformation = ipModel.Root().Value<IHtmlEncodedString>(Common.Property.MoreInfo);
                }


                if (cmHome.FooterLogoLink != null && cmHome.FooterLogo != null)
                {
                    footerVM.FooterLogo = new Link()
                    {
                        Title = cmHome.FooterLogoLink?.Name,
                        Url = cmHome.FooterLogoLink?.Url,
                        Target = cmHome.FooterLogoLink?.Target,
                        ImgUrl = cmHome.FooterLogo?.Url()
                    };
                }


                foreach (var _link in cmHome.SiteFooterLinks)
                {
                    footerVM.LstFooterNav!.Add(new Models.Link()
                    {
                        Url = _link.Url,
                        Title = _link.Name,
                        Target = _link.Target
                    });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }



            //Return data to partialview
            switch (site)
            {
                case Common.Site.Cheney:
                    return View(Common.Partial.FooterCHE, footerVM);

                case Common.Site.Coleman:
                    return View(Common.Partial.FooterCOL, footerVM);

                default:
                    return View(Common.Partial.Footer, footerVM);
            }

        }
    }
}
