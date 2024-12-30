using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Text;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;
using www.ViewModels;


namespace www.Controllers
{
    public class EfficiencyScheduleController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<EfficiencyScheduleController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly Umbraco.Cms.Core.Hosting.IHostingEnvironment _hostingEnvironment;

        public EfficiencyScheduleController(
                ILogger<EfficiencyScheduleController> _logger,
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
            List<EfficiencyScheduleViewModel> lstBootcamps = new List<EfficiencyScheduleViewModel>();
            var cmEfficiencySchedule = new EfficiencySchedule(CurrentPage, _publishedValueFallback);


            //
            if (cmEfficiencySchedule.BootcampList != null)
            {
                foreach (var cmBootcamp in cmEfficiencySchedule.BootcampList?.Select(x => (Bootcamp)x.Content).OrderBy(x => x.DateFrom))
                {
                    //Skip record if expired
                    if (cmBootcamp.DateTo < DateTime.Today)
                        continue;


                    //Create date range
                    StringBuilder sbDateRange = new StringBuilder();
                    sbDateRange.Append(cmBootcamp.DateFrom.ToString("MMM d"));
                    sbDateRange.Append("-");
                    if (cmBootcamp.DateFrom.Month != cmBootcamp.DateTo.Month)
                    {
                        sbDateRange.Append(cmBootcamp.DateTo.ToString(" MMM d"));
                    }
                    else
                    {
                        sbDateRange.Append(cmBootcamp.DateTo.Day);
                    }


                    //Create time range
                    StringBuilder sbTimeFrame = new StringBuilder();
                    sbTimeFrame.Append(cmBootcamp.TimeFrom.ToLower());
                    sbTimeFrame.Append("-");
                    sbTimeFrame.Append(cmBootcamp.TimeTo.ToLower());


                    //Create record
                    lstBootcamps.Add(new EfficiencyScheduleViewModel()
                    {
                        Bootcamp = cmBootcamp.Title,
                        Category = www.Models.Common.RemoveNonAlphaChar(cmBootcamp.Category ?? ""),
                        DateRange = sbDateRange.ToString(),
                        TimeFrame = sbTimeFrame.ToString()
                    }); ;
                }
            }



            //Redirect to view
            var viewModel = new ComposedPageViewModel<EfficiencySchedule, List<EfficiencyScheduleViewModel>>
            {
                Page = cmEfficiencySchedule,
                ViewModel = lstBootcamps
            };

            return View(www.Models.Common.View.EfficiencySchedule, viewModel);
        }
    }
}
