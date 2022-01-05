using System.Collections.Generic;

namespace Models
{
    public class SearchList
    {
        public string ErrorMsg { get; set; } = "";
        public bool ShowErrorMsg { get; set; } = false;
        public bool ShowResults { get; set; } = false;

        public string SearchFor { get; set; } = "";
        public string SearchIn { get; set; } = "";
        public string SearchInTitle { get; set; } = "";

        public bool ShowIlluminationStories { get; set; } = false;
        public List<Models.illuminationStoryLink> lstStoryLink { get; set; }

        public bool ShowMsgsFromHeaven { get; set; } = false;
        public List<MsgLink> lstMsgsFromHeavenLinks { get; set; }

        public bool ShowArticles { get; set; } = false;
        public List<ArticleLink> lstArticleLinks { get; set; }

        public bool ShowBible { get; set; } = false;
        public List<ScriptureLink> lstBibleLinks { get; set; }

        public Pagination Pagination { get; set; }



        public SearchList()
        {
            lstStoryLink = new List<Models.illuminationStoryLink>();
            lstMsgsFromHeavenLinks = new List<MsgLink>();
            lstArticleLinks = new List<ArticleLink>();
            lstBibleLinks = new List<ScriptureLink>();
            Pagination = new Pagination();
        }

    }
}
