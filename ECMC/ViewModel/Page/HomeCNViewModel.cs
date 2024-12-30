using UmbracoProject.Models;

namespace ECMC_Umbraco.ViewModel
{
    public class HomeCNViewModel
    {
        public List<Link> LstContent { get; set; }


        public HomeCNViewModel() {
            LstContent = new List<Link>();
        }
    }
}
