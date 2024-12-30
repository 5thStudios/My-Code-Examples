using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;
using www.ViewModels;
using ContentModels = www.Models.PublishedModels;


namespace www.Controllers.SurfaceControllers
{
    public class scViewAllProductsController : SurfaceController
    {
        public scViewAllProductsController(
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider) { }

        [HttpPost]
        public IActionResult RetrieveList(ComposedPageViewModel<ContentModels.ViewAllProducts, ViewAllProductsViewModel> model, int btnProductListId = -1, bool btnShowImages = false)
        {
            //Return if form is invalid
            if (ModelState.IsValid == false)
            {
                ModelState.AddModelError("", "*An error occured on our server.  Please try again.");
                return CurrentUmbracoPage();
            }


            //Set which site is active site
            foreach (var site in model.ViewModel?.LstSites)
            {
                if (site.ProductsNodeId == btnProductListId)
                {
                    site.IsActive = true;
                }
                else
                {
                    site.IsActive = false;
                }
            }


            //Set if to show images
            model.ViewModel.ShowImages = btnShowImages;


            //Return viewmodel
            ViewBag.ViewModel = model.ViewModel;
            return CurrentUmbracoPage();
        }
    }
}
