using Dragonfly.NetHelpers;
using Dragonfly.UmbracoServices;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Controllers;
using www.Models;
using www.Models.ProductTools;
using www.Models.PublishedModels;
using www.ViewModels;
using static System.Net.Mime.MediaTypeNames;
using ContentModels = www.Models.PublishedModels;


namespace www.Controllers
{
    public class BrandsListingController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<BrandsListingController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly Umbraco.Cms.Core.Hosting.IHostingEnvironment _hostingEnvironment;

        public BrandsListingController(
                ILogger<BrandsListingController> _logger,
                ICompositeViewEngine compositeViewEngine,
                IUmbracoContextAccessor umbracoContextAccessor,
                UmbracoHelper _UmbracoHelper,
                IPublishedValueFallback publishedValueFallback,
                ServiceContext context,
                IVariationContextAccessor variationContextAccessor,
                Umbraco.Cms.Core.Hosting.IHostingEnvironment hostingEnvironment
             )
            : base(_logger, compositeViewEngine, umbracoContextAccessor)
        {
            _publishedValueFallback = publishedValueFallback;
            UmbracoHelper = _UmbracoHelper;
            logger = _logger;
            _serviceContext = context;
            _variationContextAccessor = variationContextAccessor;
            _hostingEnvironment = hostingEnvironment;
        }

        public override IActionResult Index()
        {
            //
            var cmPage = new BrandsListing(CurrentPage, _publishedValueFallback);
            BrandsListingViewModel vmBrands = new BrandsListingViewModel();


            try
            {
                //Determine which site this is being called from.
                switch (cmPage.Root().Value<string>(Common.Property.Site))
                {
                    case Common.Site.Perdue:
                        vmBrands.IsPerdue = true;
                        break;
                    case Common.Site.Coleman:
                        vmBrands.IsColeman = true;
                        break;
                    case Common.Site.Cheney:
                        vmBrands.IsCheney = true;
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }


            var viewModel = new ComposedPageViewModel<BrandsListing, BrandsListingViewModel>
            {
                Page = cmPage,
                ViewModel = vmBrands
            };

            return View(Common.View.BrandsListing, viewModel);
        }
    }
}
