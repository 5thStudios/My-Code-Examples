using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models.PublishedContent;

namespace bl.Models
{
    public partial class CouponResult
    {
        public string SearchFor { get; set; }
        public bool IsValid { get; set; }
        public string ErrorMsg { get; set; }
        public bl.EF.Coupon Coupon { get; set; }


        public CouponResult() { }
    }
}
