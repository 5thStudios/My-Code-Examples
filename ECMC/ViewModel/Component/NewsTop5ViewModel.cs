namespace ECMC_Umbraco.Models
{
    public class NewsTop5ViewModel
    {
        //public UmbracoProject.Models.Link? ViewAll { get; set; }
        public List<UmbracoProject.Models.Link>? LstTop5 { get; set; } = new List<UmbracoProject.Models.Link>();



        public NewsTop5ViewModel() { }
    }
}