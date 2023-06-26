using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models.PublishedContent;

namespace bl.Models
{
    public partial class KeyValuePair
    {
        public int? IntKey { get; set; }
        public string StrKey { get; set; }

        public int? IntValue { get; set; }
        public string StrValue { get; set; }
        public bool? BoolValue { get; set; }


        public KeyValuePair() { }
    }
}
