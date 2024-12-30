using www.Models;

namespace www.ViewModels
{
    public class ProjectViewModel
    {
        public List<Link> LstIndustries { get; set; } = new List<Link>();
        public List<Link> LstScopes { get; set; } = new List<Link>();
        public List<string> LstServicesProvided { get; set; } = new List<string>();
        public string? CompletionDate { get; set; }
    }
}
