using Umbraco.Cms.Core.Models.PublishedContent;

namespace ECMC_Umbraco.Models
{
    public class GrantFilterViewModel : PublishedContentWrapped
    {
        public GrantFilterViewModel(
            IPublishedContent content,
            IPublishedValueFallback publishedValueFallback) : base(content, publishedValueFallback)
        {
        }

        //public Grants Content { get; set; }
        //public GrantFilter Filter { get; set; }

        //public IEnumerable<Grant> Grants { get; set; }
        //public IEnumerable<Grant> FilteredGrants { get; set; }

        public IEnumerable<string> Activities { get; set; }
        public IEnumerable<string> Types { get; set; }
        public IEnumerable<string> FocusAreas { get; set; }
        public IEnumerable<string> Programs { get; set; }
        public IEnumerable<string> Years { get; set; }
        public IEnumerable<string> Locations { get; set; }
    }
}
