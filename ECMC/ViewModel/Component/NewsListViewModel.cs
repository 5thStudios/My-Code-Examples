using UmbracoProject.Models;

namespace ECMC_Umbraco.ViewModel
{
    public class NewsListViewModel
    {
        public List<Link> LstCategories { get; set; } = new List<Link>();
        public FilterViewModel filterViewModel { get; set; } = new FilterViewModel();
        public List<ListItemViewModel> LstListItems { get; set; } = new List<ListItemViewModel>();
        public bool ShowPagination { get; set; } = false;
        public int PaginationCount { get; set; } = 0;


        public NewsListViewModel() { }
    }
}
