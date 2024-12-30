using UmbracoProject.Models;

namespace ECMC_Umbraco.ViewModel
{
    public class SectionCNViewModel
    {
        public List<Link> LstContent { get; set; }


        public SectionCNViewModel() {
            LstContent = new List<Link>();
        }
    }
}
