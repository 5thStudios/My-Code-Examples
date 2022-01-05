using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class Testimony
    {
        public string Author { get; set; }
        public string Quote { get; set; }
        public string Title { get; set; }
        public Boolean ShowTitle { get; set; }
    }
}