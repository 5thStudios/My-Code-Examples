using formulate.app.Types;
using formulate.core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;

namespace Models
{
    public class ContactUsContent
    {
        public IHtmlString ContactSummary { get; set; }
        public ConfiguredFormInfo PickedForm { get; set; }
    }
}