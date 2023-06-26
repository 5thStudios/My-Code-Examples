using System;
using System.Collections.Generic;


namespace bl.Models.api
{
    public class ExamBundle
    {
        public int BundleId { get; set; }
        public string BundleName { get; set; }
        public string BundleCSV { get; set; }
        public Boolean AswbExam1 { get; set; }
        public Boolean AswbExam2 { get; set; }
        public Boolean AswbExam3 { get; set; }
        public Boolean AswbExam4 { get; set; }
        public Boolean AswbExam5 { get; set; }
        public Boolean DsmBooster { get; set; }
        public Boolean EthicsBooster { get; set; }
        public Boolean CaliforniaLawAndEthics { get; set; }


        public ExamBundle() { }
    }
}