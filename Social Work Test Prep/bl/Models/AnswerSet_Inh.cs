using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models.PublishedContent;

namespace bl.Models
{
    public partial class AnswerSet_Inh : Umbraco.Web.PublishedModels.AnswerSet
    {
        public int RenderedOrder { get; set; }


        public AnswerSet_Inh(IPublishedElement content) : base(content) { }
        public AnswerSet_Inh() : base(null) { }
    }
}
