using Umbraco.Cms.Core.Strings;
using www.Models;

namespace www.ViewModels
{
    public class BlogListingViewModel
    {
        public bool HasFeatured {  get; set; }
        public bool HasCollegeAndUniversity { get; set; }
        public bool HasHealthcare { get; set; }
        public bool HasK12 { get; set; }
        public bool HasCulinaryTrends { get; set; }
        public bool HasMenuClaims { get; set; }
        public bool HasProfitDrivingTips { get; set; }
        public bool HasArchived { get; set; }
        public List<BlogListItem> LstBlogPosts { get; set; } = new List<BlogListItem>();
    }

    public class BlogListItem
    {
        public string? Url { get; set; }
        public string? Title { get; set; }
        public IHtmlEncodedString? Summary { get; set; }
        public string? ImgUrl { get; set; }
        public List<string> LstCategories { get; set; } = new List<string>();
    }
}