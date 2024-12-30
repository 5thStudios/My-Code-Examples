using Microsoft.AspNetCore.Html;
using Umbraco.Cms.Core.Strings;
using www.Models;

namespace www.ViewModels
{
    public class Call2ActionViewModel
    {
        public Boolean ShowCall2Action { get; set; } = false;
        public string ImgProduct { get; set; } = "";
        public string ImgCall2Action { get; set; } = "";
        public string EmbedVimeoId { get; set; } = "";
        public string EmbedVimeoTitle { get; set; } = "";
        public IHtmlEncodedString? LeadInText { get; set; }
        public IHtmlEncodedString? Msg { get; set; }
    }
}
