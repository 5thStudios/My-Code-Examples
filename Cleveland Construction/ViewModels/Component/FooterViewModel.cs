using Umbraco.Cms.Core.Strings;
using www.Models;

namespace www.ViewModels
{
    public class FooterViewModel
    {
        public Link Logo { get; set; }
        public IHtmlEncodedString Description { get; set; }
        public List<Link> LstLocations { get; set; } = new List<Link>();
        public List<Link> LstMinorNav { get; set; } = new List<Link>();
        public List<Link> LstMainNav { get; set; } = new List<Link>();
        public List<Link> LstSocialLinks { get; set; } = new List<Link>();
        public bool IsListOfHomePgs { get; set; }
    }
}
