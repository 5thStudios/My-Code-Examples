using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models.PublishedContent;

namespace bl.Models
{
    public partial class Region
    {
        public string Title { get; set; }
        public int Id { get; set; }
        public float BundleDiscount { get; set; }
        public float CouponDiscount { get; set; }
        public float TotalDiscount { get; set; }
        public float TotalCost { get; set; }
        public int SubscriptionTime { get; set; }
        public string CouponCode { get; set; }
        public List<Exam> LstExams { get; set; }


        public Region() {
            LstExams = new List<Exam>();
        }
    }
}
