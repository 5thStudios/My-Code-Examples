using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models.PublishedContent;

namespace bl.Models
{
    public class SeoEcommerce
    {
        public int transaction_id { get; set; }
        public decimal value { get; set; }
        public string currency { get; set; }
        public string coupon { get; set; }
        public List<SeoEcommerceItem> items { get; set; }



        public SeoEcommerce()
        {
            currency = "USD";
            coupon = "Coupon";
            items = new List<SeoEcommerceItem>();
        }
    }
}
