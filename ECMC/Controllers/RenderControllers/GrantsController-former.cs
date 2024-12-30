using Microsoft.AspNetCore.Mvc;
using ECMC_Umbraco.Models;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;
using UmbracoProject.Models;
using Umbraco.Cms.Web.Common.Controllers;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Models.PublishedContent;


namespace www.Controllers
{
    public class GrantsControllerFormer : RenderController
    {
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly ServiceContext _serviceContext;

        public GrantsControllerFormer(
            ILogger<RenderController> logger,
            ICompositeViewEngine compositeViewEngine,
            IUmbracoContextAccessor umbracoContextAccessor,
            IVariationContextAccessor variationContextAccessor,
            ServiceContext context)
            : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _variationContextAccessor = variationContextAccessor;
            _serviceContext = context;
        }


        public override IActionResult Index()
        {
            GrantFilterViewModel viewModel = new GrantFilterViewModel(CurrentPage, new PublishedValueFallback(_serviceContext, _variationContextAccessor));


            //viewModel.Content = CurrentPage as Grants;

            //viewModel.Grants = viewModel.Content?.Children<Grant>();
            //viewModel.Activities = viewModel.Grants?.Select(o => o.Activities).Distinct().OrderBy(o => o);
            //viewModel.Types = viewModel.Grants?.Select(o => o.Type).Distinct().OrderBy(o => o);
            //viewModel.FocusAreas = viewModel.Grants?.SelectMany(o => o.Focucs.Split(new char[] { ',' })).Distinct().OrderBy(o => o);
            //viewModel.Programs = viewModel.Grants?.Select(o => o.Program).Distinct().OrderBy(o => o);
            //viewModel.Years = viewModel.Grants?.Select(o => o.FiscalYear).Distinct().OrderByDescending(o => o);
            //viewModel.Locations = viewModel.Grants?.SelectMany(o => o.Location.Split(new char[] { ',' })).Distinct().OrderBy(o => o);



            //GrantFilter filter = new GrantFilter();

            //filter.SearchTerms = HttpContext.Request.Query["search-terms"];
            //filter.Programs = HttpContext.Request.Query["programs"];
            //filter.Types = HttpContext.Request.Query["types"];
            //filter.FocusAreas = HttpContext.Request.Query["focus-areas"];
            //filter.Years = HttpContext.Request.Query["years"];
            //filter.Locations = HttpContext.Request.Query["locations"];
            //filter.Count = HttpContext.Request.Query["count"];

            //viewModel.Filter = filter;



            //IEnumerable<Grant> filteredGrants = viewModel.Grants;

            //if (!string.IsNullOrEmpty(filter.SearchTerms))
            //{
            //    filteredGrants = filteredGrants.Where(o => o.Name.ToUpper().Contains(filter.SearchTerms.ToUpper()));
            //}

            //if (filter.Programs.Length != 0)
            //{
            //    filteredGrants = filteredGrants.Where(o => filter.Programs.Contains(o.Program));
            //}

            //if (filter.FocusAreas.Length != 0)
            //{
            //    filteredGrants = filteredGrants.Where(o =>
            //    {
            //        string[] focusAreas = o.Focucs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            //        bool selected = filter.FocusAreas.Intersect(focusAreas).Any();

            //        return selected;
            //    });
            //}

            //if (filter.Types.Length != 0)
            //{
            //    filteredGrants = filteredGrants.Where(o => filter.Types.Contains(o.Type));
            //}

            //if (filter.Years.Length != 0)
            //{
            //    filteredGrants = filteredGrants.Where(o => filter.Years.Contains(o.FiscalYear));
            //}

            //if (filter.Locations.Length != 0)
            //{
            //    filteredGrants = filteredGrants.Where(o =>
            //    {
            //        string[] locations = o.Location.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            //        bool selected = filter.Locations.Intersect(locations).Any();

            //        return selected;
            //    });
            //}


            //if (filter.Count.Length <= 0)
            //{
            //    filteredGrants = Enumerable.Empty<Grant>();
            //}




            //viewModel.FilteredGrants = filteredGrants;



            return CurrentTemplate(viewModel);


        }
    }
}