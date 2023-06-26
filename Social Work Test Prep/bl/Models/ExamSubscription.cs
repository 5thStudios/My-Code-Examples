using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models.PublishedContent;

namespace bl.Models
{
    public partial class ExamSubscription
    {
        public int Id { get; set; }
        public bool IsSelected { get; set; }
        public string Title { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Status { get; set; }
        public int? AttemptsTaken { get; set; }
        public int? AttemptsAllowed { get; set; }


        public ExamSubscription() { }
    }
}
