using UmbracoProject.Models;

namespace ECMC_Umbraco.ViewModel
{
    public class SectionSolViewModel
    {
        public List<Link> LstContent { get; set; }


        public SectionSolViewModel() {
            LstContent = new List<Link>();
        }
    }
}
