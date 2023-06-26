
namespace bl.Models
{
    public class MarkedQuestion
    {
        public int QuestionNo { get; set; }
        public bool IsAnswered { get; set; }

        public MarkedQuestion() { }
        public MarkedQuestion(int _questionNo, bool _isAnswered)
        {
            QuestionNo = _questionNo;
            IsAnswered = _isAnswered;
        }
    }
}
