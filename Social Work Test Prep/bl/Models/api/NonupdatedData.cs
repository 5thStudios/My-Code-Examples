using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bl.Models.api
{
    public class NonupdatedData
    {
        public List<CmsDataType> LstCmsDataType { get; set; }
        public List<CmsPropertyType> LstCmsPropertyType { get; set; }



        public NonupdatedData() { }
    }
}