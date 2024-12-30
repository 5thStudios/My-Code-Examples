using UmbracoProject.Models;

namespace ECMC_Umbraco.ViewModel
{
    public class HomeSolViewModel
    {
        public List<Link> LstContent { get; set; }


        public HomeSolViewModel() {
            LstContent = new List<Link>();
        }
    }
}
