using Microsoft.AspNetCore.Html;
using System.Web;
using Umbraco.Cms.Core.Strings;
using www.Models;

namespace www.ViewModels
{
    public class FooterViewModel
    {
        public string? CopyrightText { get; set; }
        public Link? FooterLogo { get; set; }
        public List<Link> LstFooterNav {  get; set; } = new List<Link>();
        public IHtmlEncodedString? MoreInformation { get; set; }

    }
}
