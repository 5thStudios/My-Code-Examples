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
using www.Models.PublishedModels;
using www.ViewModels;
using static System.Net.Mime.MediaTypeNames;
using ContentModels = www.Models.PublishedModels;


namespace www.Controllers
{
    public class BrandController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<BrandController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly Umbraco.Cms.Core.Hosting.IHostingEnvironment _hostingEnvironment;

        public BrandController(
                ILogger<BrandController> _logger,
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
            var cmPage = new Brand(CurrentPage, _publishedValueFallback);
            BrandViewModel vmBrand = new BrandViewModel();


            try
            {
                //Determine which site this is being called from.
                switch (cmPage.Root().Value<string>(Common.Property.Site))
                {
                    case Common.Site.Perdue:
                        vmBrand.IsPerdue = true;
                        break;
                    case Common.Site.Coleman:
                        vmBrand.IsColeman = true;
                        break;
                    case Common.Site.Cheney:
                        vmBrand.IsCheney = true;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }


            var viewModel = new ComposedPageViewModel<Brand, BrandViewModel>
            {
                Page = cmPage,
                ViewModel = vmBrand
            };

            return View(Common.View.Brand, viewModel);
        }
    }
}
