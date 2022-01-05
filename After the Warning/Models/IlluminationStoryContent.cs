using formulate.app.Types;
using formulate.core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using ContentModels = Umbraco.Web.PublishedModels;


namespace Models
{
    public class IlluminationStoryContent
    {
        public Boolean Redirect { get; set; }
        public string RedirectTo { get; set; }
        public bool RedirectHome { get; set; }
        public String Title { get; set; }
        public String ExperienceType { get; set; }
        public IHtmlString Story { get; set; }
        public ContentModels.Member CmMember { get; set; }
        public String MemberName { get; set; }
        public String Gender { get; set; }
        public String Religion { get; set; }
        public String Country { get; set; }
        public ConfiguredFormInfo PickedForm { get; set; }
    }
}