using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Web.Common.PublishedModels;
using UmbracoProject.Models;

namespace ECMC_Umbraco.ViewModel
{
    public class PaginationViewModel
    {
        public int CurrentPageNo { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int TotalRecords { get; set; } = 0;
        public int RecordsPerPage { get; set; } = 10;  
        public int FirstDisplayedPageNo { get; set; } = 1;
        public int LastDisplayedPageNo { get; set; } = 1;

        public List<Link> LstLinks { get; set; } = new List<Link>();
    }
}
