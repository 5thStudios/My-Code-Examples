namespace www.Models
{
    public class BlogRecord
    {
        public string? Name { get; set; }
        public string? Category { get; set; }
        public string? Author { get; set; }
        public string? Excerpt { get; set; }
        public DateTime? PublishedDate { get; set; }
        public string? Content { get; set; }



        public BlogRecord() { }
    }
}
