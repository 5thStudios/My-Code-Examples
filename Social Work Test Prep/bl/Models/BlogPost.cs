using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Umbraco.Core.Models.PublishedContent;

namespace bl.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string PostImageUrl { get; set; }
        public Boolean ShowPrevBtn { get; set; } = false;
        public Boolean ShowNextBtn { get; set; } = false;
        public Link Previous { get; set; }
        public Link Next { get; set; }
        public Link Blog { get; set; }
        public IHtmlString Content { get; set; }
        public DateTime PostDate { get; set; }
        public List<string> Categories { get; set; }


        public BlogPost()
        {
            Categories = new List<string>();
        }
    }
}
