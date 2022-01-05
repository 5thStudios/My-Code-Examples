using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bl.Models
{
    public class Link
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Url { get; set; }
        public Boolean IsActive { get; set; }
        public Boolean NewWindow { get; set; }
        public string Icon { get; set; }



        public Link() { }
        public Link(string _url, string _title)
        {
            Title = _title;
            Url = _url;
        }
        public Link(string _url, string _title, string _subtitle = "")
        {
            Title = _title;
            Subtitle = _subtitle;
            Url = _url;
        }
        public Link(string _url, string _title, string _subtitle = "", Boolean _isActive = false)
        {
            Title = _title;
            Subtitle = _subtitle;
            Url = _url;
            IsActive = _isActive;
        }
        public Link(string _url, string _title, string _subtitle = "", Boolean _isActive = false, Boolean _newWindow = false)
        {
            Title = _title;
            Subtitle = _subtitle;
            Url = _url;
            IsActive = _isActive;
            NewWindow = _newWindow;
        }
    }
}
