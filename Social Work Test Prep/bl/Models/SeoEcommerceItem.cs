using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models.PublishedContent;

namespace bl.Models
{
    public class SeoEcommerceItem
    {
        public string item_name { get; set; }
        public int item_id { get; set; }
        public decimal price { get; set; }
        public string item_brand { get; set; }
        public string item_category { get; set; }
        public int quantity { get; set; }

        public SeoEcommerceItem()
        {
            item_brand = "Social Work Test Prep";
            quantity = 1;
        }
    }
}
