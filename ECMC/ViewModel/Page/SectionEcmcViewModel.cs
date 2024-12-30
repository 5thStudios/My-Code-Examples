using UmbracoProject.Models;

namespace ECMC_Umbraco.ViewModel
{
    public class SectionEcmcViewModel
    {
        public List<Link> LstContent { get; set; }


        public SectionEcmcViewModel() {
            LstContent = new List<Link>();
        }
    }
}
