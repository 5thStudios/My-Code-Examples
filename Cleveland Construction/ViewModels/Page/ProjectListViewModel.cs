using Umbraco.Cms.Core.Models.PublishedContent;
using cm = www.Models.PublishedModels;
using www.Models;

namespace www.ViewModels
{
    public class ProjectListViewModel
    {
        public List<cm.ProjectPage> LstProjects { get; set; } = new List<cm.ProjectPage>();
        public List<Link> LstIndustries { get; set; } = new List<Link>();
        public List<Link> LstScopes { get; set; } = new List<Link>();
    }
}
