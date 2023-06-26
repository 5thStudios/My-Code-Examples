using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ContentModels = Umbraco.Web.PublishedModels;


namespace bl.Models
{
    public class ExamResult
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public int CorrectAnswerCount { get; set; }
        public int TotalAnswerCount { get; set; }
        public int ExamRecordId { get; set; }
        public string RedirectUrl { get; set; }
        public HashSet<int> LstQuestionIDs { get; set; }
        public List<ExamResult> LstContentAreaResults { get; set; }


        public ExamResult()
        {
            CorrectAnswerCount = 0;
            TotalAnswerCount = 0;
        }
    }
}


