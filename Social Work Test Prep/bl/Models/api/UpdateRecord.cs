using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bl.Models.api
{
    public class UpdateRecord
    {
        public int nodeId { get; set; }
        public DateTime updateDate { get; set; }



        public UpdateRecord() { }
    }
}