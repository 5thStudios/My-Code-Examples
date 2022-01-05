using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Models;
using Umbraco.Core.Models.PublishedContent;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Web.Hosting;
using System.Net.Mime;
using System.Diagnostics;

namespace Controllers
{
    public class CommonController : SurfaceController
    {
        #region "Renders"
        public ActionResult RenderHeader(IPublishedContent ipModel)
        {
            //Instantiate scope variables
            Models.Header header = new Models.Header();

            try
            {
                //Instantiate variables
                List<Models.Link> lstNavLinks = new List<Models.Link>();
                List<Models.Link> lstImgLinks = new List<Models.Link>();

                //Create navigation links
                foreach (IPublishedContent ipChild in ipModel.Children.Where(x => x.Value<Boolean>(Common.NodeProperties.showInNavigation) == true))
                {
                    //Instantiate variables
                    string linkName = string.Empty;
                    string linkUrl = string.Empty;

                    //Obtain link title
                    if (ipChild.HasValue(Common.NodeProperties.navigationTitle))
                    {
                        linkName = ipChild.Value<string>(Common.NodeProperties.navigationTitle).Trim().ToUpper();
                    }
                    else
                    {
                        linkName = ipChild.Name.Trim().ToUpper();
                    }

                    //Create link url
                    linkUrl = "#" + linkName.ToLower().Replace(" ", "");

                    //Add link to list of navs
                    lstNavLinks.Add(new Models.Link(linkName, linkUrl));
                }
                header.LstNavLinks = lstNavLinks;

                //Obtain the header node
                IPublishedContent ipHeader = ipModel.Children.Where(x => x.ContentType.Alias == Common.DocType.header).FirstOrDefault();

                //Obtain the background image for the header.
                header.BackgroundImgUrl = ipHeader.Value<IPublishedContent>(Common.NodeProperties.backgroundImage).GetCropUrl(Common.Crop.Landscape_1900x780);

                //Add the "Supporter" images
                foreach (IPublishedContent ipSupporterOf in ipHeader.Value<List<IPublishedContent>>(Common.NodeProperties.supporterOfIcons))
                {
                    lstImgLinks.Add(new Models.Link(ipSupporterOf.Name, ipSupporterOf.Url));
                }
                header.LstSupportImgs = lstImgLinks;
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"CommonController : RenderHeader()");
                Common.SaveErrorMessage(ex, sb, typeof(CommonController));
            }

            //Return data to partialview
            return PartialView("~/Views/Partials/Header.cshtml", header);
        }
        public ActionResult RenderAbout(IPublishedContent ipModel)
        {
            //Instantiate scope variables
            Models.About about = new Models.About();

            try
            {
                //Obtain the About node
                IPublishedContent ipAbout = ipModel.Children.Where(x => x.ContentType.Alias == Common.DocType.about).FirstOrDefault();

                //Obtain the featured image
                about.FeaturedImgUrl = ipAbout.Value<IPublishedContent>(Common.NodeProperties.featuredImage).GetCropUrl(Common.Crop.Portrait_600x800);

                //Obtain section text
                about.StrWhatWeDo = ipAbout.Value<HtmlString>(Common.NodeProperties.whatWeDo);
                about.StrWhoWeAre = ipAbout.Value<HtmlString>(Common.NodeProperties.whoWeAre);
                about.StrFreeConsult = ipAbout.Value<HtmlString>(Common.NodeProperties.freeConsultation);
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"CommonController : RenderAbout()");
                Common.SaveErrorMessage(ex, sb, typeof(CommonController));
            }

            //Return data to partialview
            return PartialView("~/Views/Partials/About.cshtml", about);
        }
        public ActionResult RenderServices(IPublishedContent ipModel)
        {
            //Instantiate scope variables
            Models.Services services = new Models.Services();

            try
            {
                //Obtain the services node
                IPublishedContent ipServices = ipModel.Children.Where(x => x.ContentType.Alias == Common.DocType.services).FirstOrDefault();

                //Obtain the background image and transparency
                services.BackgroundImgUrl = ipServices.Value<IPublishedContent>(Common.NodeProperties.backgroundImage).GetCropUrl(Common.Crop.Landscape_1900x1080);
                services.BackgroundTransparency = ipServices.Value<string>(Common.NodeProperties.backgroundTransparency);

                //Obtain the section's description text
                services.Description = Common.ReplaceLineBreaksForHtml(ipServices.Value<string>(Common.NodeProperties.description)).ToString();

                //Pull each of the first ~n services
                foreach (IPublishedContent ipService in ipServices.Children)  //foreach (IPublishedContent ipService in ipServices.Children.Take(3).ToList())
                {
                    Models.Service service = new Models.Service();
                    service.Title = ipService.Name;
                    //service.Description = ipService.Value<HtmlString>(Common.NodeProperties.description);
                    service.FeaturedImgUrl = ipService.Value<IPublishedContent>(Common.NodeProperties.featuredImage).GetCropUrl(Common.Crop.Landscape_510x320);
                    services.LstServices.Add(service);
                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"CommonController : RenderServices()");
                Common.SaveErrorMessage(ex, sb, typeof(CommonController));
            }

            //Return data to partialview
            return PartialView("~/Views/Partials/Services.cshtml", services);
        }
        public ActionResult RenderGallery(IPublishedContent ipModel)
        {
            //Instantiate scope variables
            Models.Projects projects = new Models.Projects();

            try
            {
                //Obtain the project gallery node
                IPublishedContent ipProjects = ipModel.Children.Where(x => x.ContentType.Alias == Common.DocType.gallery).FirstOrDefault();

                //Obtain the section's description text
                projects.Description = Common.ReplaceLineBreaksForHtml(ipProjects.Value<string>(Common.NodeProperties.description)).ToString();

                //Pull each project
                foreach (IPublishedContent ipProject in ipProjects.Children)
                {
                    Models.Project project = new Models.Project();
                    project.Description = ipProject.Value<string>(Common.NodeProperties.description);
                    project.FeaturedImgUrl = ipProject.Value<IPublishedContent>(Common.NodeProperties.featuredImage).GetCropUrl(Common.Crop.Landscape_600x400);
                    project.Title = ipProject.Value<string>(Common.NodeProperties.title);
                    //Create the project's category class
                    StringBuilder sbClasses = new StringBuilder();
                    foreach (string type in ipProject.Value<string[]>(Common.NodeProperties.projectTypes))
                    {
                        sbClasses.Append(" " + type.ToLower().Replace("&", "").Replace("  ", " ").Replace(" ", "-"));
                    }
                    project.ProjectClasses = sbClasses.ToString();
                    //Add the project to the list.
                    projects.LstProjects.Add(project);
                }

                //Create list of filters from prevalue
                projects.LstCategories.Add(new Models.FilterLink("All Projects", "rbAllProjects", "all-projects", "*"));
                List<PreValue> lstPv = Common.GetDataTypePreValues((int)Common.DataType.FilterTypes);
                foreach (PreValue pv in lstPv)
                {
                    string rbId = "rb" + pv.Value.Replace("&", "").Replace(" ", "");
                    string rbClass = pv.Value.ToLower().Replace("&", "").Replace("  ", " ").Replace(" ", "-");
                    projects.LstCategories.Add(new Models.FilterLink(pv.Value, rbId, rbClass, "." + rbClass));
                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"CommonController : RenderGallery()");
                Common.SaveErrorMessage(ex, sb, typeof(CommonController));
            }

            //Return data to partialview
            return PartialView("~/Views/Partials/Gallery.cshtml", projects);
        }
        public ActionResult RenderTestimonials(IPublishedContent ipModel)
        {
            //Instantiate scope variables
            List<Models.Testimony> LstTestimonies = new List<Models.Testimony>();

            try
            {
                //Obtain the testimonials node and its children
                IPublishedContent ipTestimonials = ipModel.Children.Where(x => x.ContentType.Alias == Common.DocType.testimonials).FirstOrDefault();
                foreach (IPublishedContent ipTestimonial in ipTestimonials.Children)
                {
                    Models.Testimony testimony = new Models.Testimony();
                    testimony.Author = ipTestimonial.Value<string>(Common.NodeProperties.author);
                    testimony.Quote = ipTestimonial.Value<string>(Common.NodeProperties.quote);
                    if (ipTestimonial.HasValue(Common.NodeProperties.title))
                    {
                        testimony.Title = ipTestimonial.Value<string>(Common.NodeProperties.title);
                        testimony.ShowTitle = true;
                    }
                    LstTestimonies.Add(testimony);
                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"CommonController : RenderTestimonials()");
                Common.SaveErrorMessage(ex, sb, typeof(CommonController));
            }

            //Return data to partialview
            return PartialView("~/Views/Partials/Testimonials.cshtml", LstTestimonies);
        }
        public ActionResult RenderCallToAction(IPublishedContent ipModel)
        {
            //Instantiate scope variables
            //Models.Link link = new Models.Link();
            Models.Call2Action call2Action = new Call2Action();

            try
            {
                //Obtain the CtA node
                IPublishedContent ipCallToAction = ipModel.Children.Where(x => x.ContentType.Alias == Common.DocType.callToAction).FirstOrDefault();

                //Create the proper links for the phone number
                string strPhone = ipCallToAction.Value<string>(Common.NodeProperties.phone);
                string intPhone = new String(strPhone.Where(Char.IsDigit).ToArray());
                if (intPhone.Length == 10) intPhone = "1" + intPhone;
                call2Action.phoneLnk = new Models.Link(strPhone, "tel:" + intPhone);


                //
                call2Action.BackgroundImgUrl = ipCallToAction.Value<IPublishedContent>(Common.NodeProperties.backgroundImage).GetCropUrl(Common.Crop.Landscape_1900x230);
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"CommonController : RenderCallToAction()");
                Common.SaveErrorMessage(ex, sb, typeof(CommonController));
            }

            //Return data to partialview
            return PartialView("~/Views/Partials/CallToAction.cshtml", call2Action);
        }
        public ActionResult RenderContact(IPublishedContent ipModel)
        {
            //Instantiate scope variables
            Models.Contact contact = new Models.Contact();

            try
            {
                //Obtain the Contact node
                IPublishedContent ipContactUs = ipModel.Children.Where(x => x.ContentType.Alias == Common.DocType.contactUs).FirstOrDefault();

                //Obtain the section's data
                contact.BackgroundImgUrl = ipContactUs.Value<IPublishedContent>(Common.NodeProperties.backgroundImage).GetCropUrl(Common.Crop.Landscape_1900x1080);
                contact.BackgroundTransparency = ipContactUs.Value<string>(Common.NodeProperties.backgroundTransparency);
                contact.Description = Common.ReplaceLineBreaksForHtml(ipContactUs.Value<string>(Common.NodeProperties.description)).ToString();
                contact.ServiceAreas = Common.ReplaceLineBreaksForHtml(ipContactUs.Value<string>(Common.NodeProperties.serviceAreas)).ToString();
                contact.Email = ipContactUs.Value<string>(Common.NodeProperties.email);
                contact.RecaptchaPublicKey = ipContactUs.Value<string>(Common.NodeProperties.recaptchaPublicKey);
                contact.RecaptchaPrivateKey = ipContactUs.Value<string>(Common.NodeProperties.recaptchaPrivateKey);

                //Create the proper links for the phone number
                string strPhone = ipContactUs.Value<string>(Common.NodeProperties.phone);
                string intPhone = new String(strPhone.Where(Char.IsDigit).ToArray());
                if (intPhone.Length == 10) intPhone = "1" + intPhone;
                contact.LnkPhone = new Models.Link(strPhone, "tel:" + intPhone);
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"CommonController : RenderContact()");
                Common.SaveErrorMessage(ex, sb, typeof(CommonController));
            }

            //Return data to partialview
            return PartialView("~/Views/Partials/Contact.cshtml", contact);
        }
        public ActionResult RenderForm()
        {
            return PartialView("~/Views/Partials/Forms/ContactForm.cshtml");
        }
        #endregion



        #region "HttpPosts"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(Form _form)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Instantiate variables
                    TempData["SubmittedSuccessfully"] = true;

                    if (string.IsNullOrEmpty(_form.Subject))
                    {
                        //Send info by email
                        SendEmail(_form);

                        //Save data to Umbraco
                        SaveMsg(_form);
                    }

                    return CurrentUmbracoPage();
                }
                else
                {
                    TempData["SubmittedSuccessfully"] = false;
                    TempData["SubmissionFailed"] = true;
                    ModelState.AddModelError(null, "An error occured while submitting your request.  This form is invalid.");
                    return CurrentUmbracoPage();
                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"CommonController : RenderContact()");
                Common.SaveErrorMessage(ex, sb, typeof(CommonController));

                TempData["SubmittedSuccessfully"] = false;
                TempData["SubmissionFailed"] = true;
                ModelState.AddModelError(null, "An error occured while submitting your request.");
                return CurrentUmbracoPage();
            }
        }
        #endregion



        #region "Methods"
        private void SendEmail(Form _form)
        {

            try
            {
                //Obtain the Contact node
                IPublishedContent ipHome = Umbraco.ContentAtRoot().FirstOrDefault();
                IPublishedContent ipContact = ipHome.Children.Where(x => x.ContentType.Alias == Common.DocType.contactUs).FirstOrDefault();

                // set the content by openning the files.
                string filePath_html = HostingEnvironment.MapPath("~/Emails/ContactUs.html");
                string filePath_Text = HostingEnvironment.MapPath("~/Emails/ContactUs.txt");
                string emailBody_Html = System.IO.File.ReadAllText(filePath_html);
                string emailBody_Text = System.IO.File.ReadAllText(filePath_Text);

                // Insert data into page
                emailBody_Html = emailBody_Html.Replace("[Name]", _form.Name);
                emailBody_Text = emailBody_Text.Replace("[Name]", _form.Name);
                emailBody_Html = emailBody_Html.Replace("[EMAIL]", _form.Email);
                emailBody_Text = emailBody_Text.Replace("[EMAIL]", _form.Email);
                emailBody_Html = emailBody_Html.Replace("[MESSAGE]", _form.Message);
                emailBody_Text = emailBody_Text.Replace("[MESSAGE]", _form.Message);

                // Obtain smtp
                SmtpClient smtp = new SmtpClient();

                // Create mail message
                MailMessage Msg = new MailMessage();
                Msg.BodyEncoding = Encoding.UTF8;
                Msg.SubjectEncoding = Encoding.UTF8;
                Msg.Subject = "Submitted Inquiry | Texas H.C. Foundation Repairs";
                Msg.IsBodyHtml = true;
                Msg.Body = "";
                Msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(emailBody_Html, new System.Net.Mime.ContentType(MediaTypeNames.Text.Html)));
                Msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(emailBody_Text, new System.Net.Mime.ContentType(MediaTypeNames.Text.Plain)));

                //Create list of all emails to send form to
                foreach (string emailTo in ipContact.Value<string[]>(Common.NodeProperties.formEmails))
                {
                    Msg.To.Add(new MailAddress(emailTo));
                }

                // Send email
                smtp.Send(Msg);
                smtp.ServicePoint.CloseConnectionGroup(smtp.ServicePoint.ConnectionName);
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"CommonController : SendEmail()");
                Common.SaveErrorMessage(ex, sb, typeof(CommonController));

                TempData["SubmittedSuccessfully"] = false;
                TempData["SubmissionFailed"] = true;
                ModelState.AddModelError(null, "An error occured while submitting your inquiry by email.");
            }
        }
        private void SaveMsg(Form _form)
        {
            try
            {
                //Instantiate variables
                IContentService cs = Services.ContentService;
                DateTime timeStamp = DateTime.Now;

                //Obtain the Inquiries node
                IPublishedContent ipHome = Umbraco.ContentAtRoot().FirstOrDefault();
                IPublishedContent ipInquiries = ipHome.Children.Where(x => x.ContentType.Alias == Common.DocType.inquiries).FirstOrDefault();

                //Obtain the udi code for parent folder
                GuidUdi udi = new GuidUdi("document", cs.GetById(ipInquiries.Id).Key);

                // Create a new node
                IContent icInquiry = cs.CreateContent(timeStamp.ToString(), udi, Common.DocType.inquiry);
                icInquiry.SetValue(Common.NodeProperties.submittedBy, _form.Name);
                icInquiry.SetValue(Common.NodeProperties.email, _form.Email);
                icInquiry.SetValue(Common.NodeProperties.message, _form.Message);
                cs.SaveAndPublish(icInquiry);
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"CommonController : SaveMsg()");
                Common.SaveErrorMessage(ex, sb, typeof(CommonController));

                TempData["SubmittedSuccessfully"] = false;
                TempData["SubmissionFailed"] = true;
                ModelState.AddModelError(null, "An error occured while saving the inquiry.");
            }
        }
        #endregion
    }
}