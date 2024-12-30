using ECMC_Umbraco.Models;
using Examine;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.PublishedModels;
using cm = Umbraco.Cms.Web.Common.PublishedModels;



namespace www.ViewComponents
{
    public class ChartViewComponent : ViewComponent
    {
        private IUmbracoContextAccessor Context;
        private UmbracoHelper UmbracoHelper;
        private IPublishedValueFallback PublishedValueFallback;
        private readonly IExamineManager ExamineManager;
        private readonly ILogger<ChartViewComponent> logger;


        public ChartViewComponent(
            ILogger<ChartViewComponent> _logger,
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
                //Obtain Chart Data
                ChartsViewModel.Title = Model.Content.Title;
                ChartsViewModel.ChartType = Model.Content.ChartType;
                ChartsViewModel.LstLabels = Model.Content.IndexLabels?.ToList();

                //Obtain Dataset Data
                ChartsViewModel.LstDatasets = new List<ChartDatasetViewModel>();
                if (Model.Content.Datasets is not null)
                {
                    foreach (var _dataset in Model.Content.Datasets)
                    {
                        //Convert dataset into content model
                        cm.Dataset cmDataset = new Dataset(_dataset.Content, PublishedValueFallback);
                        
                        //Determine if we are overriding colors in datapoints
                        bool colorOverride = !string.IsNullOrEmpty(cmDataset.ColorOverride);

                        //Extract dataset data
                        ChartDatasetViewModel datasetModel = new ChartDatasetViewModel();
                        datasetModel.Label = cmDataset.DatasetLabel;
                        if (colorOverride)
                        {
                            //Create override list
                            datasetModel.LstBackgroundColors.Add(Common.ConvertHexToRGB(cmDataset.ColorOverride!, (decimal)0.2));
                            datasetModel.LstBorderColors.Add(Common.ConvertHexToRGB(cmDataset.ColorOverride!, (decimal)0.5));
                        }

                        //Obtain all datapoints
                        if (cmDataset.DataPoints is not null)
                        {
                            foreach (var _datapoint in cmDataset.DataPoints)
                            {
                                //Convert datapoint into content model
                                cm.DataPoint cmDatapoint = new DataPoint(_datapoint.Content, PublishedValueFallback);

                                //Add data to data list
                                datasetModel.LstData.Add(cmDatapoint.Data.ToString());

                                //Add color to list if applicable
                                if (!colorOverride)
                                {
                                    if (!string.IsNullOrEmpty(cmDatapoint.Color))
                                    {
                                        datasetModel.LstBackgroundColors.Add(Common.ConvertHexToRGB(cmDatapoint.Color!, (decimal)0.2));
                                        datasetModel.LstBorderColors.Add(Common.ConvertHexToRGB(cmDatapoint.Color!, (decimal)0.5));
                                    }
                                }
                            }
                        }

                        //Add dataset to list
                        ChartsViewModel.LstDatasets.Add(datasetModel);
                    }
                }


                //Determine if legend should be displayed
                switch (Model.Content.ChartType)
                {
                    case ("Bar"):
                        if (ChartsViewModel.LstDatasets != null && ChartsViewModel.LstDatasets.Count() > 1)
                            ChartsViewModel.ShowLegend = true;
                        break;
                    case ("Doughnut"):
                        if (ChartsViewModel.LstLabels != null && ChartsViewModel.LstLabels.Count() > 1)
                            ChartsViewModel.ShowLegend = true;
                        break;
                    case ("Pie"):
                        if (ChartsViewModel.LstLabels != null && ChartsViewModel.LstLabels.Count() > 1)
                            ChartsViewModel.ShowLegend = true;
                        break;
                    case ("Line"):
                        if (ChartsViewModel.LstDatasets != null && ChartsViewModel.LstDatasets.Count() > 1)
                            ChartsViewModel.ShowLegend = true;
                        break;
                    case ("Polar"):
                        if (ChartsViewModel.LstLabels != null && ChartsViewModel.LstLabels.Count() > 1)
                            ChartsViewModel.ShowLegend = true;
                        break;
                    case ("Radar"):
                        if (ChartsViewModel.LstDatasets != null && ChartsViewModel.LstDatasets.Count() > 1)
                            ChartsViewModel.ShowLegend = true;
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }


            //Return data to partialview
            switch (Model.Content.ChartType)
            {
                case ("Bar"):
                    return View(Common.Partial.BarChart, ChartsViewModel);
                case ("Doughnut"):
                    return View(Common.Partial.DoughnutChart, ChartsViewModel);
                case ("Pie"):
                    return View(Common.Partial.PieChart, ChartsViewModel);
                case ("Line"):
                    return View(Common.Partial.LineChart, ChartsViewModel);
                case ("Polar"):
                    return View(Common.Partial.PolarChart, ChartsViewModel);
                case ("Radar"):
                    return View(Common.Partial.RadarChart, ChartsViewModel);

                default:
                    return null;
            }
        }
    }
}
