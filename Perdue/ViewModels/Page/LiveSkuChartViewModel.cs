using www.Models.LiveSkuModel;

namespace www.ViewModels
{
    public class LiveSkuChartViewModel
	{
        public string BrandType { get; set; }
        public List<LiveSkuGroup> SkuGroups { get; set; } = new List<LiveSkuGroup>();

        public bool IsPerdue { get; set; } = false;
        public bool IsColeman { get; set; } = false;
        public bool IsCheney { get; set; } = false;
    }
}
