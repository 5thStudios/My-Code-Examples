using UmbracoProject.Models;

namespace ECMC_Umbraco.ViewModel
{
    public class SectionScholarsViewModel
    {
        public List<Link> LstContent { get; set; }


        public SectionScholarsViewModel() {
            LstContent = new List<Link>();
        }
    }
}
