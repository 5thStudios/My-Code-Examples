using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bl.Models.api
{
    public class UmbracoNode
    {
        public int id { get; set; }
        public int parentID { get; set; }
        public Int16 level { get; set; }
        public string text { get; set; }
        public DateTime createDate { get; set; }
        public string path { get; set; }



        public UmbracoNode() { }
    }
}