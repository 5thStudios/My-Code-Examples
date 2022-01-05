using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class Header
    {
        public List<Link> LstNavLinks { get; set; }
        public List<Link> LstSupportImgs { get; set; }
        public string BackgroundImgUrl { get; set; }



        public Header()
        {
            //Instantiate variables
            LstNavLinks = new List<Link>();
            LstSupportImgs = new List<Link>();
        }
    }
}