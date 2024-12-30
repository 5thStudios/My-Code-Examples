namespace ECMC_Umbraco.Models
{
    public class ChartsViewModel
    {
        public string? Title { get; set; }
        public string? ADASummary { get; set; }
        public string? ValueType { get; set; }
        public bool ShowLegend { get; set; }
        public List<string>? LstLabels { get; set; }
        public List<ChartDatasetViewModel>? LstDatasets { get; set; }



        public ChartsViewModel() { }
    }
}