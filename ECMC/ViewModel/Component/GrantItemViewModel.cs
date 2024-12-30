using Microsoft.AspNetCore.Html;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Web.Common.PublishedModels;
using UmbracoProject.Models;

namespace ECMC_Umbraco.ViewModel
{
    public class GrantItemViewModel
    {
        public string? GrantName { get; set; }

        public string? FiscalYear { get; set; }
        public string? Organization { get; set; }
        public string? Amount { get; set; }
        public string? Status { get; set; }
        public string? Focus { get; set; }
        public string? Location { get; set; }
        public string? Overview { get; set; }

    }
}
