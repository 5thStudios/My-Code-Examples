using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class Link
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public Link() { }
        public Link(string _name, string _url)
        {
            //Set values
            Name = _name;
            Url = _url;
        }
    }
}