namespace www.Models
{
    public class Link
    {
        public string? Url { get; set; }
        public string? Title { get; set; }
        public string? Summary { get; set; }
        public string? Target { get; set; }
        public string? ImgUrl { get; set; }
        public int? Level { get; set; }
        public string? NavLevel { get; set; }
        public bool IsActive { get; set; }
        public bool IsMedia { get; set; } = false;
        public List<Link>? LstChildLinks { get; set; }


        public Link() { }
    }
}
