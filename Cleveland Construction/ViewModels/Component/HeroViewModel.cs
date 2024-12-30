using Umbraco.Cms.Core.Models.PublishedContent;
using www.Models;

namespace www.ViewModels
{
    public class HeroViewModel
    {
        public string? BgImageUrl { get; set; }
        public string? BgVideoSrc { get; set; }
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public www.Models.Link? Button { get; set; }

        public bool ShowVideoBg { get; set; } = false;
        public bool IsProjectPg { get; set; } = false;

    }
}
