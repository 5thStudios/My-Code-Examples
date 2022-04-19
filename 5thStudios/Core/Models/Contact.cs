using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Strings;

namespace Core.Models
{
    public class Contact
    {
        public IHtmlEncodedString Summary { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }


        public Contact()
        {
            //StringBuilder sb = new StringBuilder();
            //sb.Append(@"<h3>Let's Keep In Touch</h3>");
            //sb.Append(@"<p>Am I not local to your company?  No problem, I prefer working remotely when possible anyway.  So if you feel that we are a good match, than let's get in touch.  Fill out the form below and I'll get back to you as quickly as possible.</p>");
            //sb.Append(@"<div class=""full-divider""></div>");
            //Summary = new HtmlString(sb.ToString());
        }
    }
}