using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models.PublishedContent;

namespace bl.Models
{
    public partial class Exam
    {
        public string Title { get; set; }
        public int Id { get; set; }
        public int Price { get; set; }
        public int Duration { get; set; }
        public bool IsSelected { get; set; }


        public Exam() { }
    }
}
