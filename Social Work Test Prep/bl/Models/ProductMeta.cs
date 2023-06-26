using bl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bl.Models
{
    public class ProductMeta
    {
        //      ...Data
        public string BundleId { get; set; }
        public string BundleTitle { get; set; }
        public string ExamId { get; set; }
        public string ExamTitle { get; set; }
        public string MemberId { get; set; }
        public string SubmitType { get; set; }
        //      ...Monetary
        public string CouponCode { get; set; }
        public string ExamCount { get; set; }
        public string OriginalPrice { get; set; }
        public string BundleDiscount { get; set; }
        public string CouponDiscount { get; set; }
        public string TotalDiscount { get; set; }
        public string TotalCost { get; set; }


        public ProductMeta()
        {
            BundleId = "";
            BundleTitle = "";
            ExamId = "";
            ExamTitle = "";
            MemberId = "";
            SubmitType = "";
            CouponCode = "";
            ExamCount = "";
            OriginalPrice = "";
            BundleDiscount = "";
            CouponDiscount = "";
            TotalDiscount = "";
            TotalCost = "";
        }
    }
}