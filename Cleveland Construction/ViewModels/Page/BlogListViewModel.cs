using Umbraco.Cms.Core.Models.Blocks;
using www.Models;

namespace www.ViewModels
{
    public class BlogListViewModel
    {
        public List<Link> LstCategories { get; set; } = new List<Link>();
        public List<Link> LstSocialLinks { get; set; } = new List<Link>();

        public List<BlogPostViewModel> LstBlogPosts { get; set; } = new List<BlogPostViewModel>();

        public string? SearchQuery { get; set; }
        public bool ShowSearchPnl { get; set; }

        public string? CategoryQuery { get; set; }
        public bool ShowCategoryPnl { get; set; }
    }
}
