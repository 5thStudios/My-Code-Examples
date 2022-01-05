using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class Services
    {
        public string Description { get; set; }
        public string BackgroundImgUrl { get; set; }
        public string BackgroundTransparency { get; set; }
        public List<Service> LstServices { get; set; }


        public Services()
        {
            LstServices = new List<Service>();
        }
    }
}