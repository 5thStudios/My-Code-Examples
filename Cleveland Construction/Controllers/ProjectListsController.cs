using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Linq;
using System.Web;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Forms.Core.Models;
using www.Models;
using www.ViewModels;
using cm = www.Models.PublishedModels;


namespace www.Controllers
{
    public class ProjectListsController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<ProjectListsController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly Umbraco.Cms.Core.Hosting.IHostingEnvironment _hostingEnvironment;

        public ProjectListsController(
                ILogger<ProjectListsController> _logger,
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
            cm.ProjectLists cmProjectLists = new cm.ProjectLists(CurrentPage, _publishedValueFallback);
            ProjectListViewModel vmProjectLists = new ProjectListViewModel();


            try
            {
                //Temp lists
                List<string> LstIndustries = new List<string>();
                List<string> LstScopes = new List<string>();


                foreach (var ipProjectPg in cmProjectLists.Children.Where(x => x.ContentType.Alias == Common.Doctype.ProjectPage))
                {
                    //Add ip to list
                    //vmProjectLists.LstProjects.Add(ipProjectPg);

                    //Get cm page
                    cm.ProjectPage cmProjectPg = new cm.ProjectPage(ipProjectPg, _publishedValueFallback);
                    vmProjectLists.LstProjects.Add(cmProjectPg);

                    ////Create list of filters
                    //foreach (var _industry in cmProjectPg.Industries)
                    //{
                    //    if (!LstIndustries.Contains(_industry))
                    //        LstIndustries.Add(_industry);
                    //}
                    //foreach (var _scope in cmProjectPg.ScopesOfWork)
                    //{
                    //    if (!LstScopes.Contains(_scope))
                    //        LstScopes.Add(_scope);
                    //}
                }

                //Create list of filters
                bool filterType = cmProjectLists.FilterType;
                foreach (var ipFilter in cmProjectLists.Children.Where(x => x.ContentType.Alias == Common.Doctype.RedirectTo))
                {
                    if (filterType) //Industry
                    {
                        LstIndustries.Add(ipFilter.Name);
                    }
                    else //Scope of Work
                    {
                        LstScopes.Add(ipFilter.Name);
                    }
                }



                //Create list of links
                foreach (var _industry in LstIndustries.OrderBy(x => x))
                {
                    vmProjectLists.LstIndustries.Add(new Link()
                    {
                        Title = _industry,
                        Url = cmProjectLists.Url() + "?industry=" + HttpUtility.UrlEncode(_industry),
                        Class = Common.ConvertToClass(_industry)
                        //Class = HttpUtility.UrlEncode(_industry)
                    });
                }
                foreach (var _scope in LstScopes.OrderBy(x => x))
                {
                    vmProjectLists.LstScopes.Add(new Link()
                    {
                        Title = _scope,
                        Url = cmProjectLists.Url() + "?scope=" + HttpUtility.UrlEncode(_scope),
                        Class = Common.ConvertToClass(_scope)
                        //Class = HttpUtility.UrlEncode(_scope)
                    });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }



            //Create view model and return to view
            var viewModel = new ComposedPageViewModel<cm.ProjectLists, ProjectListViewModel>
            {
                Page = cmProjectLists,
                ViewModel = vmProjectLists
            };
            return View(Common.View.ProjectLists, viewModel);
        }
    }
}
