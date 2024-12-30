using ECMC_Umbraco.Models;
using ECMC_Umbraco.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;
using UmbracoProject.Models;

namespace www.Controllers
{
    public class HomeCNController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<HomeCNController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;

        public HomeCNController(
                ILogger<HomeCNController> _logger,
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
            //Instantiate variables
            var cmPage = new HomeCN(CurrentPage, _publishedValueFallback);
            HomeCNViewModel lstVmodel = new HomeCNViewModel();


            try
            {
                //Obtain list of all childpages visible to navigation
                foreach (var ipChild in cmPage.Children().Where(x => x.IsVisible() && x.Value<bool>("umbracoNaviHide") == false && x.HasProperty("summary")))
                {
                    lstVmodel.LstContent.Add(new Link()
                    {
                         Title = ipChild.Name,
                         Url = ipChild.Url(),
                         Summary = ipChild.Value<string>("summary")
                    });
                }                
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }


            var viewModel = new ComposedPageViewModel<HomeCN, HomeCNViewModel>
            {
                Page = cmPage,
                ViewModel = lstVmodel
            };

            return View(Common.View.HomeCN, viewModel);
        }
    }
}
