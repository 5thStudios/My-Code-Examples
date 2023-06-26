namespace bl.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PurchaseRecord
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PurchaseRecord()
        {
            ExamRecords = new HashSet<ExamRecord>();
            PurchaseRecordItems = new HashSet<PurchaseRecordItem>();
        }

        public int PurchaseRecordId { get; set; }

        public DateTime PurchaseDate { get; set; }

        public int PurchaseTypeId { get; set; }

        public int MemberId { get; set; }

        public int BundleId { get; set; }

        [StringLength(30)]
        public string BundleTitle { get; set; }

        public int? CouponId { get; set; }

        public decimal? BundleDiscount { get; set; }

        public decimal? CouponDiscount { get; set; }

        public decimal? TotalDiscount { get; set; }

        public decimal TotalCost { get; set; }

        public bool SubmittedToSEO { get; set; }

        [StringLength(1000)]
        public string Metadata { get; set; }

        public string StripeResponse { get; set; }

        public virtual Coupon Coupon { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExamRecord> ExamRecords { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseRecordItem> PurchaseRecordItems { get; set; }

        public virtual PurchaseType PurchaseType { get; set; }
    }
}
