using System.Collections.Generic;


namespace bl.Models.api
{
    public class Temp_ExamIDs
    {
        public int ExamId { get; set; }
        public int ContentId { get; set; }
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }


        public Temp_ExamIDs() {}
    }
}