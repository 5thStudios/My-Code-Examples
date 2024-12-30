using Microsoft.AspNetCore.Html;
using SEOChecker.Library.Models;
using www.Models;

namespace www.ViewModels
{
    public class MetaViewModel
    {
        //public string Title { get; set; } = "";
        //public string Description { get; set; } = "";
        //public string Keywords { get; set; } = "";
        //public string Canonical { get; set; } = "";
        //public string PgUrl { get; set; } = "";
        //public string ImgUrl { get; set; } = "";
        //public string OgTitle { get; set; } = "";
        //public string OgType { get; set; } = "";
        //public string ImgType { get; set; } = "";
        //public string OgDescription { get; set; } = "";
        //public string OgSiteName { get; set; } = "";


        public MetaData? SeoChecker { get; set; }
        public string? GoogleAnalytics { get; set; }    
    }
}
