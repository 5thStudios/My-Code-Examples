using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Web.Common.PublishedModels;
using UmbracoProject.Models;

namespace ECMC_Umbraco.ViewModel
{
    public class GrantItemsViewModel
    {
        public List<GrantItemViewModel> LstGrantItems { get; set; } = new List<GrantItemViewModel>();
        public PaginationViewModel Pagination { get; set; } = new PaginationViewModel();

        public List<SelectListItem> LstYears { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> LstLocations { get; set; } = new List<SelectListItem>();

        public string? YearFilter { get; set; } 
        public string? LocationFilter { get; set; }
        public string? SearchQuery { get; set; }
        public string? PageUrl { get; set; }

        public bool SearchSubmitted { get; set; }


    }
}
