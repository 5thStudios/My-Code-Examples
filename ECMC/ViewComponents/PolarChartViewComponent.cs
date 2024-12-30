using ECMC_Umbraco.Models;
using Examine;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.PublishedModels;



namespace www.ViewComponents
{
    public class PolarChartViewComponent : ViewComponent
    {
        private IUmbracoContextAccessor Context;
        private UmbracoHelper UmbracoHelper;
        private IPublishedValueFallback PublishedValueFallback;
        private readonly IExamineManager ExamineManager;
        private readonly ILogger<PolarChartViewComponent> logger;


        public PolarChartViewComponent(
            ILogger<PolarChartViewComponent> _logger,
            IExamineManager _ExamineManager,
            IUmbracoContextAccessor _Context,
            UmbracoHelper _UmbracoHelper,
            IPublishedValueFallback _PublishedValueFallback)
        {
            ExamineManager = _ExamineManager ?? throw new ArgumentNullException(nameof(_ExamineManager));
            Context = _Context;
            logger = _logger;
            UmbracoHelper = _UmbracoHelper;
            PublishedValueFallback = _PublishedValueFallback;
        }





        public async Task<IViewComponentResult> InvokeAsync(Umbraco.Cms.Core.Models.Blocks.BlockGridItem<Chart> Model)
        {

            //Instantiate variables
            var ChartsViewModel = new ChartsViewModel();


            try
            {

                ChartsViewModel.Title = Model.Content.ChartType + " Chart";
                ChartsViewModel.ChartType = Model.Content.ChartType;
                ChartsViewModel.LstLabels = new List<string>() { "Red", "Orange", "Yellow", "Green", "Teal" };

                ChartsViewModel.LstDatasets = new List<ChartDatasetViewModel>
                {
                    new ChartDatasetViewModel()
                    {
                        Label = "Dataset 01",
                        LstBackgroundColors = new List<string>() { 
                            "rgb(196, 38, 46, 0.2)", 
                            "rgb(227, 114, 34, 0.2)", 
                            "rgb(246, 190, 0, 0.2)", 
                            "rgb(105, 190, 40, 0.2)", 
                            "rgb(0, 154, 166, 0.2)", },
                        LstBorderColors = new List<string>() { 
                            "rgb(196, 38, 46, 0.5)", 
                            "rgb(227, 114, 34, 0.5)", 
                            "rgb(246, 190, 0, 0.5)", 
                            "rgb(105, 190, 40, 0.5)", 
                            "rgb(0, 154, 166, 0.5)", },
                        LstData = new List<string>() { "11", "16", "7", "3",  "14"},
                    },
                };

                if (ChartsViewModel.LstLabels != null && ChartsViewModel.LstLabels.Count() > 1)
                    ChartsViewModel.ShowLegend = true;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }


            //Return data to partialview
            return View(Common.Partial.PolarChart, ChartsViewModel);
        }
    }
}
