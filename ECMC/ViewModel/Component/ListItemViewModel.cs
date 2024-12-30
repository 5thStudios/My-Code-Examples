using Microsoft.AspNetCore.Html;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Web.Common.PublishedModels;
using UmbracoProject.Models;

namespace ECMC_Umbraco.ViewModel
{
    public class ListItemViewModel
    {
        public int? Id { get; set; }
        public string? DocType { get; set; }
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public bool ShowCardContent { get; set; }
        public string? Summary { get; set; }
        public string? PrimaryImgUrl { get; set; }
        public HtmlString? Content { get; set; }
        public string PostDate { get; set; }
        public string AdditionalClasses { get; set; }

        public List<string>? LstAudience { get; set; } = new List<string>();
        public List<string>? LstAreasOfInterest { get; set; } = new List<string>();
        public List<string>? LstStaff { get; set; } = new List<string>();

        public string? jsonAudience { get; set; }
        public string? jsonAreasOfInterest { get; set; }
        public string? jsonStaff { get; set; }

        public string? EncodedSummary { get; set; }

        public string? InViewAnimation { get; set; }
        public string? HoverTitle { get; set; }
        public IHtmlEncodedString? HoverTip { get; set; }
        public bool ShowHoverContent { get; set; }

        public string? LinkedInUrl { get; set; }
        public string? FacebookUrl { get; set; }

        public Link? Link { get; set; }


        //Card Models
        public FullWidthImageCard? FullWidthImageCard { get; set; }
        public IconCard? IconCard { get; set; }
        public PaddedImageCard? PaddedImageCard { get; set; }




        public ListItemViewModel() { }
    }
}
