using Umbraco.Cms.Core.Models.PublishedContent;

namespace ECMC_Umbraco.ViewModel
{
    public class ComposedPageViewModel<TPage, TViewModel> where TPage : PublishedContentModel
    {
        public TPage? Page { get; set; }
        public TViewModel? ViewModel { get; set; }
    }
}
