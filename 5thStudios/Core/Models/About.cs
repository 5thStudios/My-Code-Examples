using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Strings;

namespace Core.Models
{
    public class About
    {
        public IHtmlEncodedString Coverletter { get; set; }


        public About()
        {
            //StringBuilder sb = new StringBuilder();
            //sb.Append(@"<p>If you're looking to hire a freelance Umbraco expert to work on a web development project, or looking to add a new member to join your growing team, let’s connect.</p>");
            //sb.Append(@"<div><a class=""navLink button"" href=""#chaptercontact"" title =""Contact Me"" > Contact Me</a></div>");
            //sb.Append(@"<p>Since 2013 I have specialized in Umbraco CMS utilizing ASP.Net MVC (and web forms when needed), C# and VB.Net, allowing me to easily create and solve just about any business solution that may arise.</p>");
            //sb.Append(@"<div><a data-udi=""umb://media/23bb230e23b34ceba1797eccff9e9776"" href =""/media/1178/resume-of-james-fifth-january-2022.pdf"" target =""_blank"" title =""Resume Of James Fifth [January 2022]"" class=""button"" rel =""noopener"" > Download resume as PDF</a></div>");
            //sb.Append(@"<p>Never used Umbraco? To see how it’s running this website, log in using the ID and password below:</p>");
            //sb.Append(@"<div class=""row"" > ");
            //sb.Append(@"<div class=""col-12"" ><a rel=""noopener"" href =""/umbraco/"" target =""_blank"" class=""button"" > View my Umbraco CMS</a></div>");
            //sb.Append(@"	<div class=""credentials"">");
            //sb.Append(@"		<div class=""col-6"" ><strong>Login Id</strong></div>");
            //sb.Append(@"		<div class=""col-6 selectable"" > guest</div>");
            //sb.Append(@"		<div class=""full-divider"" ></div>");
            //sb.Append(@"		<div class=""col-6"" ><strong>Password</strong></div>");
            //sb.Append(@"		<div class=""col-6 selectable"" > password2021</div>");
            //sb.Append(@"	</div>");
            //sb.Append(@"</div>");
            //sb.Append(@"<br />");
            //sb.Append(@"<div class=""code"" > &lt;<span class=""tag"" > i </span>  <span class=""attrName"" > dream-in</span>=<span class=""attr"" > ""code"" </span> /&gt;</div>");

            //Coverletter = new HtmlString(sb.ToString());
        }
    }
}
