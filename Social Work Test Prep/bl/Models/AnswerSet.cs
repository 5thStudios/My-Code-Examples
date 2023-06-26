using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models.PublishedContent;

namespace bl.Models
{
    public partial class AnswerSet
    {
        public int RenderedOrder { get; set; }
        public string Answer { get; set; }
        public string Rationale { get; set; }
        public bool IsSelectedAnswer { get; set; }
        public bool IsCorrectAnswer { get; set; }


        public AnswerSet() { }
    }
}
