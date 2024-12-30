using Microsoft.AspNetCore.Html;
using www.Models;

namespace www.ViewModels
{
    public class RecipesListingViewModel
    {
        public string PageUrl { get; set; } = string.Empty;
        public bool HasSearchQuery { get; set; }
        public string SearchQuery { get; set; } = string.Empty;
        public List<RecipeListing> LstRecipeListings { get; set; } = new List<RecipeListing>();
        public Dictionary<string, string> LstTags_MealTypes { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> LstTags_ProductCategories { get; set; } = new Dictionary<string, string>();
        public Call2ActionViewModel Call2Action { get; set; } = new Call2ActionViewModel();
    }

    public class RecipeListing
    {
        public List<string> LstAttribClasses { get; set; } = new List<string>();
        public Link Recipe { get; set; }
        public Link Image { get; set; }
        public string Title { get; set; }
    }

}
