using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
//using System.Web.Mvc;
//using umbraco;
using Umbraco.Core;
//using Umbraco.Core.Composing;
//using Umbraco.Core.Logging;
//using Umbraco.Core.Models;
//using Umbraco.Core.Models.PublishedContent;
//using Umbraco.Core.PropertyEditors;
//using Umbraco.Core.Services;
//using Umbraco.Web;
//using Umbraco.Web.Mvc;





namespace Core.Models
{
    public sealed class Common
    {
        #region "Properties"
        public struct NodeProperties
        {
            public const string CoverLetter = "coverLetter";
            public const string Summary = "summary";
            public const string Response = "response";
            public const string Signature = "signature";
            public const string MyAttributes = "myAttributes";
            public const string Skill = "skill";
            public const string Percentage = "percentage";
            public const string Experience = "experience";
            public const string Icon = "icon";
            public const string Title = "title";
            public const string SiteUrl = "siteUrl";
            public const string Description = "description";
            public const string Screenshot = "screenshot";
            public const string CarouselShots = "carouselShots";
            public const string SkillDetails = "skillDetails";
            public const string ApplicationPatterns = "applicationPatterns";
            public const string ToolsOfTheTrade = "toolsOfTheTrade";
            public const string LanguageSkills = "languageSkills";
            public const string LibraryKnowledge = "libraryKnowledge";
            public const string FirstName = "firstName";
            public const string LastName = "lastName";
            public const string CareerTitle = "careerTitle";
            public const string IntroductionImage = "introductionImage";
            public const string Logo = "logo";
            public const string Navigation = "navigation";
        }
        public struct DocType
        {
            public const string About = "about";
            public const string ContactForm = "contactForm";
            public const string Portfolio = "portfolio";
            public const string Project = "project";
            public const string Resume = "resume";
            public const string SkillLevels = "skillLevels";
            public const string Skills = "skills";
        }
        public struct DataType
        {
            public const string DropdownExperience = "Dropdown- Experience";
            public const string SkillBlocklist = "Skill Blocklist";
        }
        public struct Crop
        {
            public const string About_804x1080 = "About_804x1080";
            public const string Intro_804x990 = "Intro_804x990";
            public const string Logo_140x140 = "Logo_140x140";
            public const string Portfolio_700x435 = "Portfolio_700x435";
            public const string Portfolio_700x475 = "Portfolio_700x475";
            public const string Signature_200x107 = "Signature_200x107";
            public const string Thankyou_1608x1080 = "Thankyou_1608x1080";
        }
        public struct Views
        {
            public const string Resume = "~/Views/Resume.cshtml";
        }
        #endregion
    }
}
