namespace ECMC_Umbraco.ViewModel
{
    public class PersonLandingPgViewModel
    {
        public List<ListItemViewModel> LstListItems { get; set; }
        public string? PrefixTitle { get; set; }
        public string? LearnMoreTitle { get; set; }



        public PersonLandingPgViewModel() {
            LstListItems = new List<ListItemViewModel>();
        }
    }
}
