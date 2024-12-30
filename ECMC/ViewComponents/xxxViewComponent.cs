using ECMC_Umbraco.Models;
using ECMC_Umbraco.ViewModel.Component;
using Examine;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.PublishedModels;



namespace www.ViewComponents
{
    public class xxxViewComponent : ViewComponent
    {
        private IUmbracoContextAccessor Context;
        private UmbracoHelper UmbracoHelper;
        private IPublishedValueFallback PublishedValueFallback;
        private readonly IExamineManager ExamineManager;
        private readonly ILogger<xxxViewComponent> logger;


        public xxxViewComponent(
            ILogger<xxxViewComponent> _logger,
            IExamineManager _ExamineManager,
            IUmbracoContextAccessor _Context,
            UmbracoHelper _UmbracoHelper,
            IPublishedValueFallback _PublishedValueFallback)
        {
            ExamineManager = _ExamineManager ?? throw new ArgumentNullException(nameof(_ExamineManager));
            Context = _Context;
            UmbracoHelper = _UmbracoHelper;
            PublishedValueFallback = _PublishedValueFallback;
        }





        public async Task<IViewComponentResult> InvokeAsync()
        {

            //Instantiate variables


            try
            {

            }
            catch (Exception ex)
            {
            }



            //Add data to ViewModel
            var xxxViewModel = new xxxViewModel() { };


            //Return data to partialview
            return View("~/Views/Partials/XXX/xxx.cshtml", xxxViewModel);
        }
    }
}
