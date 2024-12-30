using ECMC_Umbraco.Models;
using Examine;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.PublishedModels;



namespace www.ViewComponents
{
    public class RadarChartViewComponent : ViewComponent
    {
        private IUmbracoContextAccessor Context;
        private UmbracoHelper UmbracoHelper;
        private IPublishedValueFallback PublishedValueFallback;
        private readonly IExamineManager ExamineManager;
        private readonly ILogger<RadarChartViewComponent> logger;


        public RadarChartViewComponent(
            ILogger<RadarChartViewComponent> _logger,
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
                ChartsViewModel.LstLabels = new List<string>() { "January", "February", "March", "April", "May", "June", "July" };

                ChartsViewModel.LstDatasets = new List<ChartDatasetViewModel>
                {
                    new ChartDatasetViewModel()
                    {
                        Label = "Dataset 1",
                        LstBackgroundColors = new List<string>() { "rgb(196, 38, 46, 0.2)" },
                        LstBorderColors = new List<string>() { "rgb(196, 38, 46, 0.5)" },
                        LstData = new List<string>() { "28","48","40","19","96","27","100" },
                    },
                    new ChartDatasetViewModel()
                    {
                        Label = "Dataset 2",
                        LstBackgroundColors = new List<string>() { "rgb(0, 154, 166, 0.2)" },
                        LstBorderColors = new List<string>() { "rgb(0, 154, 166, 0.5)" },
                        LstData = new List<string>() { "65","59","90","81","56","55","40" },
                    },
                };

                if (ChartsViewModel.LstDatasets != null && ChartsViewModel.LstDatasets.Count() > 1)
                    ChartsViewModel.ShowLegend = true;

               
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }


            //Return data to partialview
            return View(Common.Partial.RadarChart, ChartsViewModel);
        }
    }
}
