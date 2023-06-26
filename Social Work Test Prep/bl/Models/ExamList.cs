using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bl.Models
{
    public class ExamList
    {
        public int MemberId { get; set; }
        public string SelectedExam { get; set; }
        public string SelectedMode { get; set; }
        public Dictionary<string, int> ExamDict { get; set; }
        public Dictionary<string, string> ModeDict { get; set; }



        public ExamList() {
            ExamDict = new Dictionary<string, int>();
            ModeDict = new Dictionary<string, string>();
        }
    }
}