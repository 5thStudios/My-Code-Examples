namespace ECMC_Umbraco.ViewModel
{
    public class NewsAndEventsLandingPgViewModel
    {
        public List<ListItemViewModel> LstListItems { get; set; }
        public string? PrefixTitle { get; set; }
        public string? LearnMoreTitle { get; set; }



        public NewsAndEventsLandingPgViewModel() {
            LstListItems = new List<ListItemViewModel>();
        }
    }
}
