using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Web;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Controllers;
using www.Models;
using www.Models.PublishedModels;
using www.ViewModels;
using cm = www.Models.PublishedModels;


namespace www.Controllers
{
    public class ProjectPageController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<ProjectPageController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly Umbraco.Cms.Core.Hosting.IHostingEnvironment _hostingEnvironment;

        public ProjectPageController(
                ILogger<ProjectPageController> _logger,
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
            cm.ProjectPage cmProject = new cm.ProjectPage(CurrentPage, _publishedValueFallback);
            ProjectViewModel vmProject = new ProjectViewModel();


            try
            {
                //Create list of filters
                foreach (var _industry in cmProject.Industries)
                {
                    vmProject.LstIndustries.Add(new Link()
                    {
                        Title = _industry,
                        Url = cmProject.Parent!.Url() + "?industry=" + HttpUtility.UrlEncode(_industry),
                        Class = HttpUtility.UrlEncode(_industry)
                    });
                }
                foreach (var _scope in cmProject.ScopesOfWork)
                {
                    vmProject.LstScopes.Add(new Link()
                    {
                        Title = _scope,
                        Url = cmProject.Parent!.Url() + "?scope=" + HttpUtility.UrlEncode(_scope),
                        Class = HttpUtility.UrlEncode(_scope)
                    });
                }


                //Create list of services
                foreach (var _service in cmProject.ServicesProvided)
                {
                    vmProject.LstServicesProvided.Add(_service);
                }


                //Obtain completion date
                vmProject.CompletionDate = "Under Construction";
                if (cmProject.CompletionDate > new DateTime(1900, 01, 01))
                {
                    vmProject.CompletionDate = cmProject.CompletionDate.ToString("MMMM yyyy");
                }



            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }



            //Create view model and return to view
            var viewModel = new ComposedPageViewModel<cm.ProjectPage, ProjectViewModel>
            {
                Page = cmProject,
                ViewModel = vmProject
            };
            return View(Common.View.ProjectPage, viewModel);
        }
    }
}
