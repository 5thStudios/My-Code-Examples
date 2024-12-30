using Umbraco.Cms.Core.Models.Blocks;
using www.Models;

namespace www.ViewModels
{
    public class BlogPostViewModel
    {
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public string? Url { get; set; }
        public Link? LnkAuthor { get; set; }
        public DateTime DatePosted { get; set; }
        public string? Excerpt { get; set; }
        public string? PostImageUrl { get; set; }
        public BlockGridModel? MainContent { get; set; }

        public Link? LnkPrev { get; set; }
        public Link? LnkBlog { get; set; }
        public Link? LnkNext { get; set; }

        public List<Link> LstCategories { get; set; } = new List<Link>();
        public List<Link> LstSocialLinks { get; set; } = new List<Link>();

    }
}
