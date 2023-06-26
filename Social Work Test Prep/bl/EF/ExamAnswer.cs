namespace bl.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ExamAnswer
    {
        [Key]
        public int ExamAnswersId { get; set; }

        public int ExamAnswerSetId { get; set; }

        public int ContentAreaId { get; set; }

        public int QuestionId { get; set; }

        public int? QuestionRenderOrder { get; set; }

        [Required]
        [StringLength(7)]
        public string AnswerRenderedOrder { get; set; }

        public int? SelectedAnswer { get; set; }

        public int CorrectAnswer { get; set; }

        public bool IsCorrect { get; set; }

        public bool ReviewQuestion { get; set; }

        public virtual ExamAnswerSet ExamAnswerSet { get; set; }
    }
}
