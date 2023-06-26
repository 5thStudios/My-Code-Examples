using bl.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Umbraco.Web.PublishedModels;
using ContentModels = Umbraco.Web.PublishedModels;


namespace bl.Models
{
    public class ExamQuestion
    {
        //Persistant data to pass from page to page
        public string Title { get; set; }
        public int ExamId { get; set; }
        public int ExamModeId { get; set; }
        public string ExamMode { get; set; }
        public int? ExamRecordId { get; set; }
        public int TotalNoQuestions { get; set; }
        public bool IsFreeExam { get; set; }


        //Changable data
        public int QuestionNo { get; set; }
        public int? SelectedAnswer { get; set; }
        public string QuestionText { get; set; }
        public string Rationale { get; set; }
        public string SuggestedStudyDescription { get; set; }
        public string Source { get; set; }
        public string AdditionalNotes { get; set; }
        public string TimeRemaining { get; set; }
        public string EmailSubjectText { get; set; }
        public ExamAnswer ExamAnswer { get; set; }
        public List<bl.Models.Link> LstSuggestedStudyLinks { get; set; }
        public List<bl.Models.AnswerSet> LstAnswerSets { get; set; }

        public bool PreviousBtnClicked { get; set; }
        public bool NextBtnClicked { get; set; }
        public bool CompleteBtnClicked { get; set; }

        public string GoToQuestion { get; set; }
        public string ContentAreaName { get; set; }
        public string QuestionName { get; set; }
        public string SaveStopBtnUrl { get; set; }

        public bool ShowPrevious { get; set; }
        public bool ShowNext { get; set; }
        public bool ShowComplete { get; set; }
        public bool ShowErrorMsg { get; set; }

        public DateTime StartTime { get; set; }

        public List<MarkedQuestion> LstMarkedQuestions { get; set; }



        public ExamQuestion()
        {
            LstSuggestedStudyLinks = new List<bl.Models.Link>();
            LstMarkedQuestions = new List<MarkedQuestion>();
            LstAnswerSets = new List<bl.Models.AnswerSet>();
            QuestionNo = 1;
        }
    }
}
