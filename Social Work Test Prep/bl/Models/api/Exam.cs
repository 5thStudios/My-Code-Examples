using System.Collections.Generic;


namespace bl.Models.api
{
    public class Exam
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Duration { get; set; }
        public string Price { get; set; }
        public bool HideFromCalifornia { get; set; }
        public bool HideFromUSA { get; set; }
        public List<bl.Models.api.ContentArea> LstContentAreas { get; set; }


        public Exam()
        {
            LstContentAreas = new List<bl.Models.api.ContentArea>();
        }
    }
}



/*
    Used when calling
    http://api.swtp.localhost/Services/apiMigrateData.asmx/MigrateExams
 */