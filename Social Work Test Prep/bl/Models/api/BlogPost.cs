using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bl.Models.api
{
    public class BlogPost
    {
        public int Id { get; set; }
        public DateTime PostDate { get; set; }
        public string Title { get; set; }
        public string ArticleImgUrl { get; set; }
        public List<string> LstCategories { get; set; }
        public bool DisableComments { get; set; }
        public string Content { get; set; }
        public string PreviousUrl { get; set; }

        public BlogPost()
        {
            LstCategories = new List<string>();
        }
    }
}

/*
    Used when calling
    https://api.swtp.localhost/Services/apiMigrateData.asmx/MigrateBlogPosts
 */