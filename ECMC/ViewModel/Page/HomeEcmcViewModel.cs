using UmbracoProject.Models;

namespace ECMC_Umbraco.ViewModel
{
    public class HomeEcmcViewModel
    {
        public List<Link> LstContent { get; set; }


        public HomeEcmcViewModel() {
            LstContent = new List<Link>();
        }
    }
}
