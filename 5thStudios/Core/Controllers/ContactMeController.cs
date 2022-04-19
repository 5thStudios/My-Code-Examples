//using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.IO;
using System.Net.Mime;
using System.Text;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Mail;
using Umbraco.Cms.Core.Models.Email;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common.Filters;
using Umbraco.Cms.Web.Website.Controllers;


namespace Core.Controllers
{
    public class ContactMeController : SurfaceController
    {
        private IWebHostEnvironment _webHostingEnv;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ContactMeController> _logger;
        private readonly GlobalSettings _globalSettings;

        public ContactMeController(
            IWebHostEnvironment webHostingEnv,
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider,
            IEmailSender emailSender,
            ILogger<ContactMeController> logger,
            IOptions<GlobalSettings> globalSettings)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _webHostingEnv = webHostingEnv;
            _emailSender = emailSender;
            _logger = logger;
            _globalSettings = globalSettings.Value;
        }

        [HttpPost]
        [ValidateUmbracoFormRouteString]
        public IActionResult Postback(Core.Models.Contact contact)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    //Check my honeypot for bots.  If caught, ignore but display as if submission was successfull. Mwahhhh haaa haaa haaa haaaaaaaa.....
                    if (string.IsNullOrEmpty(contact.Subject))
                    {
                        //Send info by email
                        SendEmail(contact);

                        //Save data to Umbraco
                        //SaveMsg(contact);
                    }
                }
                else
                {
                    //TempData["SubmittedSuccessfully"] = false;
                    //TempData["SubmissionFailed"] = true;
                    //ModelState.AddModelError(null, "An error occured while submitting your request.  This form is invalid.");
                    _logger.LogError("An error occured while submitting email.  Form is invalid.");
                    return CurrentUmbracoPage();
                }
            }
            catch (Exception ex)
            {
                //TempData["SubmittedSuccessfully"] = false;
                //TempData["SubmissionFailed"] = true;
                //ModelState.AddModelError(null, "An error occured while submitting your request.");
                _logger.LogError(ex, "An error occured while submitting email.");
                return CurrentUmbracoPage();
            }


            return RedirectToCurrentUmbracoPage(QueryString.Create("page", "thankyou"));
        }





        #region "Methods"
        private void SendEmail(Core.Models.Contact contact)
        {
            //Obtain text from email file
            string filePath_Text = Path.Combine(_webHostingEnv.WebRootPath, "emails", "contact-me.txt");
            string emailBody_Text = System.IO.File.ReadAllText(filePath_Text);

            // Insert data into page
            emailBody_Text = emailBody_Text.Replace("[NAME]", contact.Name);
            emailBody_Text = emailBody_Text.Replace("[EMAIL]", contact.Email);
            emailBody_Text = emailBody_Text.Replace("[MESSAGE]", contact.Message);

            //Grab credentials from the appsettings.Development.json file.  (Not included in GitHub!!!)
            string smtpHost = _globalSettings.Smtp.Host;
            int smtpPort = _globalSettings.Smtp.Port;
            //string smtpFrom = _globalSettings.Smtp.From;
            //string smtpUserName = _globalSettings.Smtp.Username;
            //string smtpPassword = _globalSettings.Smtp.Password;

            //Create email message
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(contact.Name, contact.Email));
            msg.To.Add(new MailboxAddress(smtpHost, _globalSettings.Smtp.Username));
            msg.Subject = "Resume Submission to 5thStudios.com";
            msg.Body = new TextPart("plain") { Text = emailBody_Text };

            //Obtain smtpclient and send email
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect(smtpHost, smtpPort, false);
                //client.Authenticate(smtpUserName, smtpPassword);
                client.Send(msg);
                client.Disconnect(true);
            }


            // Use these settings when using HTML emails.
            //=============================================
            //string filePath_html = Path.Combine(_webHostingEnv.WebRootPath, "emails", "test.html");
            //string emailBody_Html = System.IO.File.ReadAllText(filePath_html);
            //msg.Body = new TextPart("html") { Text = emailBody_Html };
        }
        #endregion
    }
}