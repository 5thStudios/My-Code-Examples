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
    public class ExamData
    {
        //Instruction info
        public string Title { get; set; }
        public IHtmlString Instructions { get; set; }

        //Parameter info
        public string ExamType { get; set; }
        public int? ExamId { get; set; }
        public int? ExamModeId { get; set; }
        public int? ExamRecordId { get; set; }
        public int? QuestionId { get; set; }

        //Navigation URLs
        public string PrevUrl { get; set; }
        public string NextUrl { get; set; }



        public ExamData() {}
    }
}
