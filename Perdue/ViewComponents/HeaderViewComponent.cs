using Examine;
using Lucene.Net.Index;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using www.Models;
using www.Models.PublishedModels;
using www.ViewModels;
//using cm = www.Models.PublishedModels;


namespace www.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private IUmbracoContextAccessor Context;
        private UmbracoHelper UmbracoHelper;
        private IPublishedValueFallback PublishedValueFallback;
        private readonly IExamineManager ExamineManager;
        private readonly ILogger<HeaderViewComponent> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private List<int> lstActiveNodeIDs;


        public HeaderViewComponent(
            ILogger<HeaderViewComponent> _logger,
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
            HeaderViewModel headerVM = new HeaderViewModel();

            try
            {
                //Determine root pg
                headerVM.IpPage = ipModel.Root();
                Home cmHome = new Home(headerVM.IpPage, PublishedValueFallback);


                //Obtain site logo
                headerVM.SiteLogoUrl = cmHome.SiteLogo?.Url();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }



            //Return data to partialview
            switch (site)
            {
                case Common.Site.Cheney:
                    return View(Common.Partial.HeaderCHE, headerVM);

                case Common.Site.Coleman:
                    return View(Common.Partial.HeaderCOL, headerVM);

                default:
                    return View(Common.Partial.Header, headerVM);
            }
        }
    }
}
