using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Umbraco.Core.Models.PublishedContent;

namespace bl.Models
{
    public class BlogPostList
    {
        public List<BlogPost> LstBlogPosts { get; set; }
        public List<BlogPost> LstAllBlogPosts { get; set; }
        public List<Tuple<string, string>> LstCategories { get; set; } 
        public Pagination Pagination { get; set; }
        public string PgUrl { get; set; }


        public BlogPostList()
        {
            LstBlogPosts = new List<BlogPost>();
            LstAllBlogPosts = new List<BlogPost>();
            LstCategories = new List<Tuple<string, string>>();
        }
    }
}
