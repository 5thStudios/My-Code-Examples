using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models.PublishedContent;

namespace bl.Models
{
    public class SeoEcommercePush
    {
        public string _event { get; set; }
        public SeoUserProperties user_properties { get; set; }
        public SeoEcommerce ecommerce { get; set; }



        public SeoEcommercePush()
        {
            _event = "purchase";
            user_properties = new SeoUserProperties();
            ecommerce = new SeoEcommerce();
        }
    }
}
