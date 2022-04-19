using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Strings;

namespace Core.Models
{
    public class Skills
    {
        public IHtmlEncodedString SkillDetails { get; set; }
        public List<string> LstApplicationPatterns { get; set; }
        public List<string> LstToolsOfTheTrade { get; set; }
        public List<SkillItem> LstLanguageSkills { get; set; }
        public List<SkillItem> LstLibraryKnowledge { get; set; }



        public Skills()
        {
            LstApplicationPatterns = new List<string>();
            LstLanguageSkills = new List<SkillItem>();
            LstLibraryKnowledge = new List<SkillItem>();
            LstToolsOfTheTrade = new List<string>();

            //StringBuilder sb = new StringBuilder();
            //sb.Append("<p>One thing I learned over the years is that any technology you learn today will likely be obsolete tomorrow. So to keep ahead of the curve I strive to evolve my programming skills by learning and adding new technologies to the foundational skills I already use regularly. The following is a basic overview of the technologies I have mastered over the years.</p>");
            //sb.Append("<br />");
            //sb.Append("<blockquote>");
            //sb.Append("    <p>*Need someone who can learn a new skill quickly?   There is little I can't learn...   unless of course it's PHP!!   Yuck... I utterly hate that language!!</p>");
            //sb.Append("</blockquote>");
            //SkillDetails = new HtmlString(sb.ToString());

            //LstApplicationPatterns.Add("Web Service APIs");
            //LstApplicationPatterns.Add("N-Tier Architecture");
            //LstApplicationPatterns.Add("MVC");
            //LstApplicationPatterns.Add("Web Forms");
            //LstApplicationPatterns.Add("Responsive Design");

            //LstLanguageSkills.Add(new SkillItem(97, "Html5/Css3", "Expert"));
            //LstLanguageSkills.Add(new SkillItem(95, "C#", "Proficient"));
            //LstLanguageSkills.Add(new SkillItem(90, "VB.Net", "Expert"));
            //LstLanguageSkills.Add(new SkillItem(75, "Javascript", "Proficient"));
            //LstLanguageSkills.Add(new SkillItem(50, "Tsql", "Enough to be Dangerous"));

            //LstLibraryKnowledge.Add(new SkillItem(98, "Zurb Foundation 6"));
            //LstLibraryKnowledge.Add(new SkillItem(95, "Zurb Foundation 5"));
            //LstLibraryKnowledge.Add(new SkillItem(85, "Jquery"));

            //LstToolsOfTheTrade.Add("Umbraco CMS");
            //LstToolsOfTheTrade.Add("Visual Studios");
            //LstToolsOfTheTrade.Add("Gimp");

        }
    }
}