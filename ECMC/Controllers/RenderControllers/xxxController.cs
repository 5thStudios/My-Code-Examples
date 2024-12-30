using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;
using ECMC_Umbraco.ViewModel;
using Umbraco.Cms.Web.Common;
using ContentModels = Umbraco.Cms.Web.Common.PublishedModels;


namespace www.Controllers
{
    public class xxxController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<xxxController> logger;

        public xxxController(
                ILogger<xxxController> _logger,
                ICompositeViewEngine compositeViewEngine,
                IUmbracoContextAccessor umbracoContextAccessor,
                UmbracoHelper _UmbracoHelper,
                IPublishedValueFallback publishedValueFallback
             )
            : base(_logger, compositeViewEngine, umbracoContextAccessor)
        {
            _publishedValueFallback = publishedValueFallback;
            UmbracoHelper = _UmbracoHelper;
            logger = _logger;
        }

        public override IActionResult Index()
        {
            //
            var cmPage = new xxx(CurrentPage, _publishedValueFallback);


            try
            {

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }


            var viewModel = new ComposedPageViewModel<xxx, xxxViewModel>
            {
                Page = cmPage,
                ViewModel = new xxxViewModel()
            };

            return View("~/Views/xxx.cshtml", viewModel);
        }
    }
}
