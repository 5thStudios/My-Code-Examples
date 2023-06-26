

using System;

namespace bl.Models.api
{
    public class CouponSetting
    {
        public string CouponName { get; set; }
        public decimal DiscountAmount { get; set; }
        public string DiscountType { get; set; }
        public DateTime? Expires { get; set; }
        public int? MaxAllowed { get; set; }
        public string Notes { get; set; }



        public CouponSetting() { }
    }
}