using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bl.Models.api
{
    public class CmsPropertyData
    {
        public int id { get; set; }
        public int contentNodeId { get; set; }
        public int propertytypeid { get; set; }
        public int? dataInt { get; set; }
        public DateTime? dataDate { get; set; }
        public string dataNvarchar { get; set; }
        public string dataNtext { get; set; }



        public CmsPropertyData() { }
    }
}