namespace bl.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ExamAnswerSet")]
    public partial class ExamAnswerSet
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ExamAnswerSet()
        {
            ExamAnswers = new HashSet<ExamAnswer>();
        }

        public int ExamAnswerSetId { get; set; }

        public int ExamRecordId { get; set; }

        public string AnswerSet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExamAnswer> ExamAnswers { get; set; }

        public virtual ExamRecord ExamRecord { get; set; }
    }
}
