using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bl.Models.api
{
    public class CmsPropertyType
    {
        public int id { get; set; }
        public int dataTypeId { get; set; }
        public int contentTypeId { get; set; }
        public string Alias { get; set; }
        public string Name { get; set; }



        public CmsPropertyType() { }
    }
}