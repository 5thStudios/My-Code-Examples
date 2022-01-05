using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;

namespace Models
{
    public class AddEditIlluminationStoryContent
    {
        public bool DoesStoryExist { get; set; }
        public bool IsStoryPublished { get; set; }
        public IMember Member { get; set; }
        //public string tempData1 { get; set; }
        //public string tempData2 { get; set; }

        public AddEditIlluminationStoryContent()
        {
            DoesStoryExist = false;
            IsStoryPublished = false;
        }
    }
}