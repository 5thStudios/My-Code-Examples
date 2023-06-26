using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models.PublishedContent;

namespace bl.Models
{
    public class TopLevelContent
    {
        public SEOChecker.MVC.MetaData Meta { get; set; }
        public bool ShowAnalytics { get; set; }
        public string Analytics { get; set; }
        public string DriftCode { get; set; }
        public string ErrorMsg { get; set; }
        public string SeoEcommerce { get; set; }
        public bool Redirect { get; set; }
        public string RedirectTo { get; set; }



        public TopLevelContent()
        {
            ShowAnalytics = true;
        }
    }
}
