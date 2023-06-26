using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bl.Models.api
{
    public class SpecialOffer
    {
        public string Title { get; set; }
        public int Price { get; set; }
        public string SubscriptionTime { get; set; }
        public List<bl.Models.api.Exam> LstExams { get; set; }
        public string ExamGroup { get; set; }


        public SpecialOffer()
        {
            LstExams = new List<bl.Models.api.Exam>();
        }
    }
}

/*
    Used when calling
    https://api.swtp.localhost/Services/apiMigrateData.asmx/MigrateSpecialOffers
 */