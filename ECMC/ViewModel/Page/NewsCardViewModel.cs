namespace ECMC_Umbraco.ViewModel
{
    public class NewsCardViewModel
    {
        public List<ListItemViewModel> LstListItems { get; set; }
        public string? PrefixTitle { get; set; }
        public string? LearnMoreTitle { get; set; }



        public NewsCardViewModel() {
            LstListItems = new List<ListItemViewModel>();
        }
    }
}
