using System.Collections.Generic;


namespace bl.Models.api
{
    public class Question
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string QuestionText { get; set; }
        public string Rationale { get; set; }

        public List<bl.Models.api.AnswerSet> LstAnswers { get; set; }

        public string Source { get; set; }
        public string AdditionalNotes { get; set; }
        public string SuggestedStudyDescription { get; set; }
        public string SuggestedStudyLink { get; set; }


        public Question()
        {
            LstAnswers = new List<bl.Models.api.AnswerSet>();
        }
    }
}