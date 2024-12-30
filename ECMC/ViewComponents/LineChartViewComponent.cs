using CMSImport.Core.Extensions;
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
    public class LineChartViewComponent : ViewComponent
    {
        private IUmbracoContextAccessor Context;
        private UmbracoHelper UmbracoHelper;
        private IPublishedValueFallback PublishedValueFallback;
        private readonly IExamineManager ExamineManager;
        private readonly ILogger<LineChartViewComponent> logger;


        public LineChartViewComponent(
            ILogger<LineChartViewComponent> _logger,
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





        public async Task<IViewComponentResult> InvokeAsync(Umbraco.Cms.Core.Models.Blocks.BlockGridItem<LineChart> Model)
        {
            //Instantiate variables
            var ChartsViewModel = new ChartsViewModel();


            try
            {
                //Obtain Chart Data
                ChartsViewModel.Title = Model.Content.Title;
                ChartsViewModel.ADASummary = Model.Content.ADasummary;
                ChartsViewModel.LstLabels = Model.Content.IndexLabels?.ToList();
                ChartsViewModel.ValueType = Model.Content.ValueType;



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
                            datasetModel.LstBorderColors.Add(Common.ConvertHexToRGB(cmDataset.ColorOverride!, (decimal)1.0));
                            datasetModel.LstBackgroundColors.Add(Common.ConvertHexToRGB(cmDataset.ColorOverride!, (decimal)1.0));
                            datasetModel.LstBackgroundHoverColors.Add(Common.ConvertHexToRGB(cmDataset.ColorOverride!, (decimal)0.75));
                        }

                        //Obtain all datapoints
                        if (cmDataset.DataPoints is not null)
                        {
                            foreach (var _datapoint in cmDataset.DataPoints)
                            {
                                //Convert datapoint into content model
                                cm.DataPoint cmDatapoint = new DataPoint(_datapoint.Content, PublishedValueFallback);



                                //Add data to data list if not null
                                if (cmDatapoint.NullDataPoint) {
                                    datasetModel.LstData.Add("null");
                                }
                                else
                                {
                                    datasetModel.LstData.Add(cmDatapoint.Data.ToString());
                                }

                                //Add toolTip to toolTip list
                                datasetModel.LstTooltip.Add(cmDatapoint.Tooltip ?? "");

                                //Add color to list if applicable
                                if (!colorOverride)
                                {
                                    if (!string.IsNullOrEmpty(cmDatapoint.Color))
                                    {
                                        datasetModel.LstBorderColors.Add(Common.ConvertHexToRGB(cmDatapoint.Color!, (decimal)1.0));
                                        datasetModel.LstBackgroundColors.Add(Common.ConvertHexToRGB(cmDatapoint.Color!, (decimal)1.0));
                                        datasetModel.LstBackgroundHoverColors.Add(Common.ConvertHexToRGB(cmDatapoint.Color!, (decimal)0.75));
                                    }
                                }
                            }
                        }

                        //Add dataset to list
                        ChartsViewModel.LstDatasets.Add(datasetModel);
                    }
                }


                //Determine if legend should be displayed
                if (ChartsViewModel.LstDatasets != null && ChartsViewModel.LstDatasets.Count() > 1)
                    ChartsViewModel.ShowLegend = true;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }


            //Return data to partialview
            return View(Common.Partial.LineChart, ChartsViewModel);
        }
    }
}
