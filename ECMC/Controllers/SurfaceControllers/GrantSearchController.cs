using ECMC_Umbraco.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common.Filters;
using Umbraco.Cms.Web.Website.Controllers;




namespace www.Controllers.SurfaceControllers
{
    public class GrantSearchController : SurfaceController
    {
        public GrantSearchController(
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider) { }

        [HttpPost]
        public IActionResult Search(GrantItemsViewModel model, string btnPaginate = "")
        {
            //Ensure temp data is cleared
            TempData.Clear();


            //Set data to return to controller.
            model.LstGrantItems.Clear();
            model.SearchSubmitted = true;


            //Ensure updated page # is passed to controller
            if (!string.IsNullOrEmpty(btnPaginate))
            {
                model.Pagination.CurrentPageNo = Convert.ToInt32(btnPaginate);
            }


            //Store data for return to controller
            TempData["Model"] = Newtonsoft.Json.JsonConvert.SerializeObject(model);


            return RedirectToCurrentUmbracoPage();        
        }
    }
}
