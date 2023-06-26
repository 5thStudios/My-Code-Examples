namespace bl.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ExamRecord")]
    public partial class ExamRecord
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ExamRecord()
        {
            ExamAnswerSets = new HashSet<ExamAnswerSet>();
        }

        public int ExamRecordId { get; set; }

        public int? ExamModeId { get; set; }

        public int? MemberId { get; set; }

        public int? PurchaseRecordId { get; set; }

        public int? SubscriptionId { get; set; }

        public int ExamId { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool Submitted { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public TimeSpan? TimeRemaining { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExamAnswerSet> ExamAnswerSets { get; set; }

        public virtual ExamMode ExamMode { get; set; }

        public virtual PurchaseRecord PurchaseRecord { get; set; }
    }
}
