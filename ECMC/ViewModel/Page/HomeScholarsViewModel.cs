using UmbracoProject.Models;

namespace ECMC_Umbraco.ViewModel
{
    public class HomeScholarsViewModel
    {
        public List<Link> LstContent { get; set; }


        public HomeScholarsViewModel() {
            LstContent = new List<Link>();
        }
    }
}
