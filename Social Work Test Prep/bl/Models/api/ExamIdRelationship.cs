using System.Collections.Generic;


namespace bl.Models.api
{
    public class ExamIdRelationship
    {
        public int? ExamId_new { get; set; }
        public int? ContentId_new { get; set; }
        public int? QuestionId_new { get; set; }
        public int? ExamId_old { get; set; }
        public int? ContentId_old { get; set; }
        public int? QuestionId_old { get; set; }
        public string QuestionText_old { get; set; }


        public ExamIdRelationship() { }
    }
}
