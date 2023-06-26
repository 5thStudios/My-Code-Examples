using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bl.Models.api
{
    public class pwdDiscount
    {
        public System.Guid Id { get; set; }
        public string Code { get; set; }
        public bool IsNew { get; set; }
        public bool Enabled { get; set; }
        public string DiscountTypeInfo { get; set; }
        public Nullable<decimal> DiscountAmount { get; set; }
        public Nullable<decimal> DiscountPercent { get; set; }
        public string DisabledMessage { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<System.DateTime> ExpireDate { get; set; }
        public string Notes { get; set; }
        public Nullable<long> TimesUsedLimit { get; set; }
        public long TimesUsed { get; set; }



        public pwdDiscount() { }
    }
}