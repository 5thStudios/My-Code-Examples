using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bl.Models
{
    public class ExtendSubscriptions
    {
        public string Email { get; set; }
        public bool IsValidMember { get; set; }
        public int? MemberId { get; set; }
        public int? BonusExamId { get; set; }
        public int ExtendDays { get; set; }
        public int ExtendAttempts { get; set; }
        public List<bl.Models.Exam> LstBonusExams { get; set; }
        public List<ExamSubscription> LstExamSubscription { get; set; }


        public ExtendSubscriptions() {
            LstBonusExams = new List<Exam>();
            LstExamSubscription = new List<ExamSubscription>();
        }
    }
}
