namespace www.Models.ProductTools
{
    public class Product
    {
        public string? EditUrl { get; set; }
        public string? ViewUrl { get; set; }
        public string? ViewJsonUrl { get; set; }

        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductType { get; set; }
        public string? ProductSubtype { get; set; }

        public Link? PrimaryImgLink { get; set; }
        public List<Link> LstImgLinks { get; set;} = new List<Link>();

        public List<string> LstAttributes { get; set; } = new List<string>();

        public string? Gtin { get; set; }
        //public string? ApiRefId { get; set; }
        public int NodeId { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? LastChanged { get; set; }
    }
}
