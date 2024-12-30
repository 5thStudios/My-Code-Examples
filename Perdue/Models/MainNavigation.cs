namespace www.Models
{
    public class MainNavigation
    {
        public List<NavGroup>? LstNavGroups { get; set; } = new List<NavGroup>();
    }


    public class NavGroup
    {
        public Link? Link { get; set; }
        public string? NavLevel { get; set; }
        public List<GroupColumn> LstColumns { get; set; } = new List<GroupColumn>();
    }


    public class GroupColumn
    {
        public List<NavElement> LstElements { get; set; } = new List<NavElement>();
    }


    public class NavElement
    {
        public bool IsHeader { get; set; }
        public Link? Link { get; set; }
        public string? NavLevel { get; set; }
        public List<Link> LstChildLinks { get; set; } = new List<Link>();
    }
}
