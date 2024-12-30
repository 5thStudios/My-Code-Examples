namespace ECMC_Umbraco.ViewModel
{
    public class FilterViewModel
    {
        public int FilterCount { get; set; }

        public bool ShowSearchPnl { get; set; }
        public bool ShowAreaOfInterestFilter { get; set; }
        public bool ShowAudienceFilter { get; set; }
        public bool ShowStaffFilter { get; set; }
        public bool ShowViewSelector { get; set; }

        public List<string>? LstAreaOfInterestFilter { get; set; } = new List<string>();
        public List<string>? LstAudienceFilter { get; set; } = new List<string>();
        public List<string>? LstStaffFilter { get; set; } = new List<string>();

        public string testString { get; set; } = "";



        public FilterViewModel()
        {
            FilterCount = 0;
        }
    }
}
