using www.Models;

namespace www.ViewModels
{
    public class ViewProductImagesViewModel
    {
        public int ProductCount { get; set; } = 0;
        public List<www.Models.ProductTools.ImageDetail> LstImageDetails { get; set; } = new List<Models.ProductTools.ImageDetail>();
    }
}