using Amazon.S3.Model;

namespace www.ViewModels
{
    public class SearchPageViewModel
    {
        public string? SearchQuery { get; set; }
        public int PageNo { get; set; } = 1;
        public int StartPageNo { get; set; } = 0;
        public int EndPageNo { get; set; } = 0;
        public int TotalPages { get; set; } = 0;
        public int TotalItems { get; set; } = 0;
        public int SkipCount { get; set; } = 0;

        public int ResultStartNo { get; set; } = 0;
        public int ResultThroughNo { get; set; } = 0;

        public bool IsFirstPg { get; set; }
        public bool IsLastPg { get; set; }

        public List<SearchResultItem> LstResults { get; set; } = new List<SearchResultItem>();
    }


    public class SearchResultItem
    {
        public string? Title { get; set; }
        public string? Url { get; set; }
        public string? Summary { get; set; }
    }
}
