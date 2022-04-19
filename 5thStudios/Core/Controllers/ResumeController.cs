using System;
using System.Linq;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Extensions;


namespace Core.Controllers
{
    public class ResumeController : RenderController
    {
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly ServiceContext _serviceContext;
        private readonly ILogger<ResumeController> _logger;
        private IPublishedValueFallback _publishedValueFallback;

        public ResumeController(
            ILogger<ResumeController> logger,
            ICompositeViewEngine compositeViewEngine,
            IUmbracoContextAccessor umbracoContextAccessor,
            IVariationContextAccessor variationContextAccessor,
            IPublishedValueFallback publishedValueFallback,
            ServiceContext context)
            : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _variationContextAccessor = variationContextAccessor;
            _serviceContext = context;
            _logger = logger;
            _publishedValueFallback = publishedValueFallback;
        }




        public override IActionResult Index()
        {
            //Instantiate variables
            MasterClass masterClass = new MasterClass();

            IPublishedContent ipAbout = CurrentPage.Children.ToList().Where(x => x.ContentType.Alias == Common.DocType.About).FirstOrDefault();
            IPublishedContent ipContact = CurrentPage.Children.ToList().Where(x => x.ContentType.Alias == Common.DocType.ContactForm).FirstOrDefault();
            IPublishedContent ipPortfolio = CurrentPage.Children.ToList().Where(x => x.ContentType.Alias == Common.DocType.Portfolio).FirstOrDefault();
            IPublishedContent ipSkills = CurrentPage.Children.ToList().Where(x => x.ContentType.Alias == Common.DocType.Skills).FirstOrDefault();

            var cmAbout = new Umbraco.Cms.Web.Common.PublishedModels.About(ipAbout, _publishedValueFallback);
            var cmContact = new Umbraco.Cms.Web.Common.PublishedModels.ContactForm(ipContact, _publishedValueFallback);
            var cmResume = new Umbraco.Cms.Web.Common.PublishedModels.Resume(CurrentPage, _publishedValueFallback);
            var cmPortfolio = new Umbraco.Cms.Web.Common.PublishedModels.Portfolio(ipPortfolio, _publishedValueFallback);
            var cmSkills = new Umbraco.Cms.Web.Common.PublishedModels.Skills(ipSkills, _publishedValueFallback);

            //Obtain data
            masterClass.About = ObtainAbout(cmAbout);
            masterClass.Contact = ObtainContact(cmContact);
            masterClass.Introduction = ObtainIntroduction(cmResume);
            masterClass.Portfolio = ObtainPortfolio(cmPortfolio);
            masterClass.Skills = ObtainSkills(cmSkills);
            masterClass.Thankyou = ObtainThankyou(cmContact);
            masterClass.SidePnl = ObtainSidePnl(cmResume);

            return View(Common.Views.Resume, masterClass);
        }



        public Introduction ObtainIntroduction(Umbraco.Cms.Web.Common.PublishedModels.Resume cmResume)
        {
            // Introduction
            // ===================================
            //Instantiate variables
            Introduction introduction = new Introduction();

            try
            {
                introduction.CareerTitle = cmResume.CareerTitle;
                introduction.FullName = cmResume.FirstName + " " + cmResume.LastName;
                introduction.IntroImgUrl = cmResume.IntroductionImage.GetCropUrl(Common.Crop.Intro_804x990);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ResumeController : ObtainIntroduction()");
            }

            return introduction;
        }
        public About ObtainAbout(Umbraco.Cms.Web.Common.PublishedModels.About cmAbout)
        {
            // About
            // ===================================
            //Instantiate variables
            About about = new About();

            try
            {
                about.Coverletter = cmAbout.CoverLetter;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ResumeController : ObtainAbout()");
            }

            return about;
        }
        public Portfolio ObtainPortfolio(Umbraco.Cms.Web.Common.PublishedModels.Portfolio cmPortfolio)
        {
            // Portfolio
            // ===================================
            //Instantiate variables
            Portfolio portfolio = new Portfolio();

            try
            {
                foreach (IPublishedContent ipProject in cmPortfolio.Children)
                {
                    var cmProject = new Umbraco.Cms.Web.Common.PublishedModels.Project(ipProject, _publishedValueFallback);

                    Project project = new Project();
                    project.ProjectLink = new Link(cmProject.Title, cmProject.SiteUrl);
                    project.ScreenshotUrl = cmProject.Screenshot.GetCropUrl(Common.Crop.Portfolio_700x475);
                    project.Summary = cmProject.Description;
                    foreach (var screenshot in cmProject.CarouselShots)
                    {
                        project.LstCarouselShots.Add(new Link("", screenshot.GetCropUrl(Common.Crop.Portfolio_700x435), cmProject.Title));
                    }

                    portfolio.LstProjects.Add(project);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ResumeController : ObtainPortfolio()");
            }

            return portfolio;
        }
        public Skills ObtainSkills(Umbraco.Cms.Web.Common.PublishedModels.Skills cmSkills)
        {
            // Skills
            // ===================================
            //Instantiate variables
            Skills skills = new Skills();

            try
            {
                skills.SkillDetails = cmSkills.SkillDetails;

                foreach (string ApplicationPattern in cmSkills.ApplicationPatterns)
                {
                    skills.LstApplicationPatterns.Add(ApplicationPattern);
                }
                foreach (string Tool in cmSkills.ToolsOfTheTrade)
                {
                    skills.LstToolsOfTheTrade.Add(Tool);
                }

                foreach (BlockListItem block in cmSkills.LanguageSkills)
                {
                    var cmBlock = new Umbraco.Cms.Web.Common.PublishedModels.SkillLevels(block.Content, _publishedValueFallback);

                    SkillItem skillItem = new SkillItem();
                    skillItem.Percentage = cmBlock.Percentage;
                    skillItem.ExperienceLevel = cmBlock.Experience;
                    skillItem.Skill = cmBlock.Skill;

                    skills.LstLanguageSkills.Add(skillItem);
                }

                foreach (BlockListItem block in cmSkills.LibraryKnowledge)
                {
                    var cmBlock = new Umbraco.Cms.Web.Common.PublishedModels.SkillLevels(block.Content, _publishedValueFallback);

                    SkillItem skillItem = new SkillItem();
                    skillItem.Percentage = cmBlock.Percentage;
                    skillItem.ExperienceLevel = cmBlock.Experience;
                    skillItem.Skill = cmBlock.Skill;

                    skills.LstLibraryKnowledge.Add(skillItem);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ResumeController : ObtainSkills()");
            }

            return skills;
        }
        public Contact ObtainContact(Umbraco.Cms.Web.Common.PublishedModels.ContactForm cmContact)
        {
            // Contact
            // ===================================
            //Instantiate variables
            Contact contact = new Contact();

            try
            {
                contact.Summary = cmContact.Summary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ResumeController : ObtainContact()");
            }

            return contact;
        }
        public Thankyou ObtainThankyou(Umbraco.Cms.Web.Common.PublishedModels.ContactForm cmContact)
        {
            // Thankyou
            // ===================================
            //Instantiate variables
            Thankyou thankyou = new Thankyou();

            try
            {
                thankyou.ResponseText = cmContact.Response;
                thankyou.SignatureImgUrl = cmContact.Signature.GetCropUrl(Common.Crop.Signature_200x107);
                foreach (var attribute in cmContact.MyAttributes)
                {
                    thankyou.LstMyAttributes.Add(attribute);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ResumeController : ObtainThankyou()");
            }

            return thankyou;
        }
        public SidePnl ObtainSidePnl(Umbraco.Cms.Web.Common.PublishedModels.Resume cmResume)
        {
            // Side Panel
            // ===================================
            //Instantiate variables
            SidePnl sidePnl = new SidePnl();

            try
            {
                sidePnl.siteLogoImgUrl = cmResume.Logo.GetCropUrl(Common.Crop.Logo_140x140);
                foreach (var _link in cmResume.Navigation)
                {
                    sidePnl.LstNavigation.Add(new Link(_link.Name, _link.Url.Replace("/", "")));
                    //sidePnl.LstNavigation.Add(new Link(_link.Name, "#chapter" + _link.Name.Replace(" ", "")));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ResumeController : ObtainSidePnl()");
            }

            return sidePnl;
        }
    }

}