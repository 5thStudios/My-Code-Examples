using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Link
    {
        public string Title { get; set; }
        public string AltText { get; set; }
        public string Url { get; set; }



        public Link() { }
        public Link(string _Title = "", string _Url = "")
        {
            Title = _Title;
            Url = _Url;
        }
        public Link(string _Title = "", string _Url = "", string _AltText = "")
        {
            Title = _Title;
            AltText = _AltText;
            Url = _Url;
        }
    }
}
