namespace bl.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PurchaseRecordItem")]
    public partial class PurchaseRecordItem
    {
        public int PurchaseRecordItemId { get; set; }

        public int? PurchaseRecordId { get; set; }

        public int MemberId { get; set; }

        public int ExamId { get; set; }

        [Required]
        [StringLength(50)]
        public string ExamTitle { get; set; }

        public decimal OriginalPrice { get; set; }

        [Column(TypeName = "date")]
        public DateTime ExpirationDate { get; set; }

        public int Extensions { get; set; }

        public int? AttemptsTaken { get; set; }

        public int? AttemptsAllowed { get; set; }

        public virtual PurchaseRecord PurchaseRecord { get; set; }
    }
}
