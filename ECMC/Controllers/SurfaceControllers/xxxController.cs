using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common.Filters;
using Umbraco.Cms.Web.Website.Controllers;
using www.ViewModel;



namespace www.Controllers.SurfaceControllers
{
    public class xxxController : SurfaceController
    {
        public xxxController(
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider) { }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateUmbracoFormRouteString]
        public async Task<IActionResult> xxxFunctionName(xxxViewModel model)
        {
            if (ModelState.IsValid == false)
            {
                //TempData["ValidatedSuccessfully"] = false;
                //ModelState.AddModelError("", "*An error occured on our server.  Please try again.");
                return CurrentUmbracoPage();
            }
            else
            {
                return CurrentUmbracoPage();
                //return RedirectToCurrentUmbracoPage();               
            }
        }
    }
}
