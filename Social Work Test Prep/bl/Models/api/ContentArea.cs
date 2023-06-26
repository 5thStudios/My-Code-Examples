using System.Collections.Generic;


namespace bl.Models.api
{
    public class ContentArea
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string SuperContentArea { get; set; }
        public List<bl.Models.api.Question> LstQuestions { get; set; }


        public ContentArea()
        {
            LstQuestions= new List<bl.Models.api.Question>();
        }
    }
}