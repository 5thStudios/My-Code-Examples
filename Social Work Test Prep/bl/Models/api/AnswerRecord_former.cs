

namespace bl.Models.api
{
    public class AnswerRecord_former
    {
        public string oldQuestionId { get; set; }
        public string oldContentArea { get; set; }
        public string answerId { get; set; }
        public string correct { get; set; }
        public string review { get; set; }
        public string answersRendered { get; set; }

        public int newQuestionId { get; set; }
        public int newContentArea { get; set; }


        public AnswerRecord_former() { }
    }
}