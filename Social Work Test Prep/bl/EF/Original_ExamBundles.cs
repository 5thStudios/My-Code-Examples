namespace bl.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Original_ExamBundles
    {
        public int id { get; set; }

        [Required]
        [StringLength(100)]
        public string bundleName { get; set; }

        public int bundleId { get; set; }

        public bool aswbExam1 { get; set; }

        public bool aswbExam2 { get; set; }

        public bool aswbExam3 { get; set; }

        public bool aswbExam4 { get; set; }

        public bool aswbExam5 { get; set; }

        public bool dsmBooster { get; set; }

        public bool ethicsBooster { get; set; }

        public bool californiaLawAndEthics { get; set; }
    }
}
