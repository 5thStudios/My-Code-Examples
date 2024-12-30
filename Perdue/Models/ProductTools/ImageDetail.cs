namespace www.Models.ProductTools
{
    public class ImageDetail
    {
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public string? ImgUrl { get; set; }
        public string? ImgSize { get; set; }
        public string? FileName { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime? LastChanged { get; set; }
    }
}
