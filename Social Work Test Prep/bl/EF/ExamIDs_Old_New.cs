namespace bl.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ExamIDs_Old_New
    {
        public int id { get; set; }

        public int? ExamId_new { get; set; }

        public int? ContentId_new { get; set; }

        public int? QuestionId_new { get; set; }

        public int? ExamId_old { get; set; }

        public int? ContentId_old { get; set; }

        public int? QuestionId_old { get; set; }

        public string QuestionText_old { get; set; }
    }
}
