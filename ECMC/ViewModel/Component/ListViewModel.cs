namespace ECMC_Umbraco.ViewModel
{
    public class ListViewModel
    {
        public FilterViewModel filterViewModel { get; set; }
        public List<ListItemViewModel> LstListItems { get; set; }
        public bool ShowPagination { get; set; } = false;
        public int PaginationCount { get; set; } = 0;


        public ListViewModel() {
            filterViewModel = new FilterViewModel();
            LstListItems = new List<ListItemViewModel>();
        }
    }
}
