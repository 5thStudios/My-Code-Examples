using Microsoft.AspNetCore.Html;
using www.Models;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace www.ViewModels
{
    public class TeaserViewModel
    {
        public List<TeaserBlockViewModel> LstTeaserBlocks { get; set; } = new List<TeaserBlockViewModel>();
    }

    public class TeaserBlockViewModel
    {
        public string? Content { get; set; }
        public string? BgColor { get; set; }
        public string? BgImageUrl { get; set; }
        public bool IsImgFirst { get; set; }

        public string? CellClasses { get; set; }
        public string? CellAttributes { get; set; }
    }
}
