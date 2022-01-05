using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class Call2Action
    {
        public Link phoneLnk { get; set; }
        public string BackgroundImgUrl { get; set; }


        public Call2Action()
        {
            phoneLnk = new Link();
        }
    }
}