using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Controllers;
using www.Models;
using www.ViewModels;
using cm = www.Models.PublishedModels;


namespace www.Controllers
{
    public class ListOfHomePgsController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<ListOfHomePgsController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly Umbraco.Cms.Core.Hosting.IHostingEnvironment _hostingEnvironment;

        public ListOfHomePgsController(
                ILogger<ListOfHomePgsController> _logger,
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
            //Instantiate variables
            cm.ListOfHomePgs cmPage = new cm.ListOfHomePgs(CurrentPage, _publishedValueFallback);
            ListOfHomePgsViewModel vmListOfHomePgs = new ListOfHomePgsViewModel();


            try
            {
                foreach (var site in cmPage.Sites)
                {
                    //Extract site link 
                    var cmImgLink = new cm.ImageLink(site.Content, _publishedValueFallback);
                    vmListOfHomePgs.LstSites.Add(new Link()
                    {
                         ImgUrl = cmImgLink.Image.Url(),
                         Class = cmImgLink.Class,
                         Url = cmImgLink.Link.Url,
                         Title = cmImgLink.Link.Name
                    });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }



            //Create view model and return to view
            var viewModel = new ComposedPageViewModel<cm.ListOfHomePgs, ListOfHomePgsViewModel>
            {
                Page = cmPage,
                ViewModel = vmListOfHomePgs
            };
            return View(Common.View.ListOfHomePgs, viewModel);
        }
    }
}
