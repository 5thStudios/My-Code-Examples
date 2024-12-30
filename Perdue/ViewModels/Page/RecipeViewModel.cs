using Microsoft.AspNetCore.Html;
using www.Models;

namespace www.ViewModels
{
    public class RecipeViewModel
    {
        public List<www.Models.Link> LstRelatedProducts { get; set; } = new List<www.Models.Link>();
        
    }
}
