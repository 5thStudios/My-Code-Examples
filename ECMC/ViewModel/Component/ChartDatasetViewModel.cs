namespace ECMC_Umbraco.Models
{
    public class ChartDatasetViewModel
    {
        public string? Label { get; set; }
        public List<string> LstData { get; set; }
        public List<bool> LstNull { get; set; }
        public List<string> LstBorderColors { get; set; }
        public List<string> LstBackgroundColors { get; set; }
        public List<string> LstBackgroundHoverColors { get; set; }
        public List<string> LstTooltip { get; set; }


        public ChartDatasetViewModel()
        {
            LstData = new List<string>();
            LstNull = new List<bool> { false };
            LstBackgroundColors = new List<string>();
            LstBackgroundHoverColors = new List<string>();
            LstBorderColors = new List<string>();
            LstTooltip = new List<string>();
        }
    }
}
