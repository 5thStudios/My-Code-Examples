using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bl.Models
{
    public class Link
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Subname { get; set; }
        public DateTime Date { get; set; }
        public string Url { get; set; }
        public string ImgUrl { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string Class { get; set; }
        public string Target { get; set; }
        public string MiscNote { get; set; }
        public Boolean NewWindow { get; set; } = false;
        public List<Link> LstChildLinks { get; set; }


        public Link() { }
        public Link(string _name = "", string _url = "", string _class = "")
        {
            Name = _name;
            Url = _url;
            Class = _class;
        }
    }
}
