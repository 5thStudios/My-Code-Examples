namespace bl.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Coupon
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Coupon()
        {
            PurchaseRecords = new HashSet<PurchaseRecord>();
        }

        public int CouponId { get; set; }

        [Required]
        [StringLength(150)]
        public string Code { get; set; }

        public int CouponTypeId { get; set; }

        [Column(TypeName = "text")]
        public string Notes { get; set; }

        public bool DiscountByPercentage { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? DiscountPercent { get; set; }

        public decimal? DiscountAmount { get; set; }

        public long TimesUsed { get; set; }

        public long? TimesUsedLimit { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CreateDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? ExpireDate { get; set; }

        public bool Enabled { get; set; }

        public virtual CouponType CouponType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseRecord> PurchaseRecords { get; set; }
    }
}
