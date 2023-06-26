using Umbraco.Web.Security;
using System.Web.Security;
using Umbraco.Core.Logging;
using System;
using System.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;
using Umbraco.Core.Models.PublishedContent;
using cm = Umbraco.Web.PublishedModels;
using System.Net.Mail;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Linq;
using MailChimp;
using MailChimp.Helper;
using System.Configuration;
using System.Net.Configuration;

namespace bl.Controllers
{
    public class blMemberController : SurfaceController
    {
        #region "Properties"

        public blMemberController() { }
        #endregion



        #region "Renders"
        public ActionResult RenderSignUp()
        {
            return PartialView(Models.Common.PartialPath.Membership_SignUp, new bl.Models.Login());
        }
        public ActionResult RenderLogin()
        {
            return PartialView(Models.Common.PartialPath.Membership_Login, new bl.Models.Login());
        }
        public ActionResult RenderForgotPassword()
        {
            //Instantiate variables
            bl.Models.Login _login = new bl.Models.Login();

            //Update with email saved in session if it exists
            if (Session[bl.Models.Common.TempData.Email] != null)
                _login.UserName = Session[bl.Models.Common.TempData.Email].ToString();


            return PartialView(Models.Common.PartialPath.Membership_ForgotPassword, _login);
        }
        public ActionResult RenderChangePassword()
        {
            return PartialView(Models.Common.PartialPath.Membership_ChangePassword, new bl.Models.Login(Members.CurrentUserName));
        }
        public ActionResult RenderResetPassword(string email)
        {
            //Set login creds
            bl.Models.Login login = new Models.Login();
            login.UserName = email;

            return PartialView(Models.Common.PartialPath.Membership_ResetPassword, login);
        }
        public ActionResult RenderTutoringServices()
        {
            return PartialView(Models.Common.PartialPath.Membership_TutoringServices, new bl.Models.Message());
        }
        #endregion


        #region "Submits"
        [ValidateAntiForgeryToken]
        public ActionResult SubmitSignUp(bl.Models.Login model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Does member already exist?
                    if (DoesMemberExist_byEmail(model.UserName.ToLower().Trim()))
                    {
                        //Member exists.
                        ModelState.AddModelError("UserName", "*Member already exists.  Try resetting your password.");
                        return CurrentUmbracoPage();
                    }
                    else
                    {
                        //Submit data to create membership
                        int memberId = CreateMember(model);


                        //Submit new member to mailchimp.
                        EmailParameter mailchimpResults = null;
                        try
                        {
                            //Learn more at https://github.com/danesparza/MailChimp.NET
                            MailChimpManager mc = new MailChimpManager(ConfigurationManager.AppSettings[bl.Models.Common.Misc.MailChimpApiKey]); // API keys found at https://us5.admin.mailchimp.com/account/api/
                            EmailParameter email = new EmailParameter() { Email = model.UserName.ToLower().Trim() };//	Create the email parameter
                            mailchimpResults = mc.Subscribe(ConfigurationManager.AppSettings[bl.Models.Common.Misc.MailChimpListID], email);  // YourListID found at https://us5.admin.mailchimp.com/lists/settings?id=52025
                        }
                        catch (Exception exe)
                        {
                            if (mailchimpResults != null)
                                Logger.Error<blMemberController>(Newtonsoft.Json.JsonConvert.SerializeObject(mailchimpResults) + "  |  " + exe);
                            else
                                Logger.Error<blMemberController>(exe);
                        }


                        //Log member in
                        if (LogMemberIn(model.UserName.ToLower().Trim(), model.Password.Trim()))
                        {
                            //Set incase we want to show a message to user.
                            TempData[Models.Common.TempData.UserSuccessfullyCreated] = true;

                            //Redirect to account page
                            return RedirectToUmbracoPage((int)Models.Common.SiteNode.Account);
                        }
                        else
                        {
                            //Unsuccessful login.
                            ModelState.AddModelError(string.Empty, "An error occured while creating your account.  Please try again.");
                            return CurrentUmbracoPage();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid data submitted.  Please try again.");
                    return CurrentUmbracoPage();
                }
            }
            catch (Exception ex)
            {
                Logger.Error<blMemberController>(ex);
                ModelState.AddModelError(string.Empty, "An error occured while creating your account.  Please try again. || " + ex.Message);
                return CurrentUmbracoPage();
            }
        }
        public ActionResult SubmitLogin(bl.Models.Login model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Log member in
                    if (LogMemberIn(model.UserName.ToLower().Trim(), model.Password.Trim()))
                    {
                        //Redirect to account page
                        TempData[Models.Common.TempData.LoginSuccess] = true;
                        return RedirectToUmbracoPage((int)Models.Common.SiteNode.Account);
                    }
                    else
                    {
                        //Check if member needs to have password reset.
                        IMember _member = Services.MemberService.GetByEmail(model.UserName.ToLower().Trim());

                        if (_member != null && _member.IsLockedOut)
                        {
                            //Redirect to password reset page
                            TempData[Models.Common.TempData.LockedOut] = true;
                            TempData[Models.Common.TempData.Email] = model.UserName.ToLower().Trim();
                            return RedirectToUmbracoPage((int)Models.Common.SiteNode.ResetPassword);
                        }
                        else
                        {
                            //Unsuccessful login.
                            ModelState.AddModelError(string.Empty, "Invalid login name or password.  Please try again.");
                            Session[bl.Models.Common.TempData.Email] = model.UserName.ToLower().Trim();
                            return CurrentUmbracoPage();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid data submitted.  Please try again.");
                    return CurrentUmbracoPage();
                }
            }
            catch (Exception ex)
            {
                Logger.Error<blMemberController>(ex);
                ModelState.AddModelError(string.Empty, "An error occured while attempting to log in.  Please try again. || " + ex.Message);
                return CurrentUmbracoPage();
            }
        }
        public ActionResult SubmitLogout()
        {
            LogMemberOut();
            return RedirectToUmbracoPage((int)Models.Common.SiteNode.Home);
        }
        public ActionResult SubmitPasswordReset(bl.Models.Login model)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    //var email = EmailAddress.Text;
                    IMember member = Services.MemberService.GetByEmail(model.UserName);

                    if (member == null)
                    {
                        //Invalid email submitted
                        ModelState.AddModelError(string.Empty, "The email requested could not be found.");
                        return CurrentUmbracoPage();
                    }
                    else
                    {
                        //Create a new password and save to member
                        string newPassword = System.Web.Security.Membership.GeneratePassword(6, 0);
                        Services.MemberService.SavePassword(member, newPassword);

                        //Send email reset notification
                        cm.ForgotPassword cmForgotPassword = new cm.ForgotPassword(Umbraco.Content((int)Models.Common.SiteNode.ForgotPassword));
                        string formattedMsg = cmForgotPassword.Message.ToString();
                        string message = String.Format(formattedMsg, member.Email, newPassword);


                        //Create a new smtp client
                        SmtpClient smtp = new SmtpClient();
                         SmtpSection _smtpSection = (ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection);

                        // Create mail message
                        MailAddress from = new MailAddress(_smtpSection.From, "Social Work Test Prep");
                        MailAddress to = new MailAddress(model.UserName);
                        MailMessage Msg = new MailMessage(from, to);

                        //MailMessage Msg = new MailMessage();
                        //Msg.To.Add(new MailAddress(model.UserName));

                        // Set email parameters
                        Msg.BodyEncoding = Encoding.UTF8;
                        Msg.SubjectEncoding = Encoding.UTF8;
                        Msg.Subject = "Social Work Test Prep - Reset Password";
                        Msg.IsBodyHtml = true;
                        Msg.Body = "";
                        AlternateView alternateHtml = AlternateView.CreateAlternateViewFromString(message, new System.Net.Mime.ContentType(MediaTypeNames.Text.Html));
                        Msg.AlternateViews.Add(alternateHtml);

                        // Send email
                        smtp.Send(Msg);



                        TempData[Models.Common.TempData.Success] = true;
                        return RedirectToCurrentUmbracoPage();
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid email submitted.  Please try again.");
                    return CurrentUmbracoPage();
                }
            }
            catch (Exception ex)
            {
                Logger.Error<blMemberController>(ex);
                ModelState.AddModelError(string.Empty, "An error occured while attempting to reset your password.  Please try again. || " + ex.Message);
                return CurrentUmbracoPage();
            }
        }
        public ActionResult SubmitPasswordChange(bl.Models.Login model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (IsPasswordValid(model.UserName, model.Password))
                    {
                        //Obtain member
                        IMember member = Services.MemberService.GetByEmail(model.UserName);
                        //Save password to member
                        Services.MemberService.SavePassword(member, model.NewPassword);
                        //Return successfully
                        TempData[Models.Common.TempData.Success] = true;
                        return RedirectToCurrentUmbracoPage();
                    }
                    else
                    {
                        //Invalid password
                        ModelState.AddModelError(string.Empty, "Current password invalid.  Please try again.");
                        return CurrentUmbracoPage();
                    }
                }
                else
                {
                    return CurrentUmbracoPage();
                }
            }
            catch (Exception ex)
            {
                Logger.Error<blMemberController>(ex);
                ModelState.AddModelError(string.Empty, "An error occured while attempting to reset your password.  Please try again. || " + ex.Message);
                return CurrentUmbracoPage();
            }
        }
        public ActionResult SubmitPasswordUpdate(bl.Models.Login model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Obtain member
                    IMember member = Services.MemberService.GetByEmail(model.UserName);
                    //Save password to member and unlock
                    member.IsLockedOut = false;
                    Services.MemberService.SavePassword(member, model.NewPassword);
                    Services.MemberService.Save(member);
                    //Return successfully
                    TempData[Models.Common.TempData.PasswordResetSuccessfully] = true;
                    return RedirectToCurrentUmbracoPage();
                }
                else
                {
                    return CurrentUmbracoPage();
                }
            }
            catch (Exception ex)
            {
                Logger.Error<blMemberController>(ex);
                ModelState.AddModelError(string.Empty, "An error occured while attempting to reset your password.  Please try again. || " + ex.Message);
                return CurrentUmbracoPage();
            }
        }
        public ActionResult SubmitTutoringRequest(bl.Models.Message model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!model.IsValid)
                    {
                        ModelState.AddModelError(string.Empty, "Please verify that you are NOT a robot.");
                        return CurrentUmbracoPage();
                    }
                    else
                    {
                        //Valid form.  Submit to client
                        //==========================================

                        //Obtain email message for client
                        cm.TutoringServices cmTutoringServices = new cm.TutoringServices(Umbraco.Content((int)Models.Common.SiteNode.TutoringServices));
                        string formattedMsg = cmTutoringServices.Message.ToString();
                        object[] args = new object[] { model.Email, model.Subject, model.MessageText };
                        string message = String.Format(formattedMsg, args);


                        //Obtain Common node
                        IPublishedContent ipSiteSettings = Umbraco.ContentAtRoot().FirstOrDefault(x => x.ContentType.Alias.Equals(bl.Models.Common.DocType.SiteSettings));
                        cm.Common cmCommon = new cm.Common(ipSiteSettings.Children.FirstOrDefault(x => x.ContentType.Alias.Equals(bl.Models.Common.DocType.Common)));

                        //Create a new smtp client
                        SmtpClient smtp = new SmtpClient();

                        // Create mail message
                        MailMessage Msg = new MailMessage();
                        Msg.To.Add(new MailAddress(cmCommon.ClientEmail));

                        // Set email parameters
                        Msg.BodyEncoding = Encoding.UTF8;
                        Msg.SubjectEncoding = Encoding.UTF8;
                        Msg.Subject = "Social Work Test Prep- Tutoring Service Request";
                        Msg.IsBodyHtml = true;
                        Msg.Body = "";
                        AlternateView alternateHtml = AlternateView.CreateAlternateViewFromString(message, new System.Net.Mime.ContentType(MediaTypeNames.Text.Html));
                        Msg.AlternateViews.Add(alternateHtml);

                        // Send email
                        smtp.Send(Msg);



                        TempData[Models.Common.TempData.Success] = true;
                        return RedirectToCurrentUmbracoPage();

                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Please fill in all fields.");
                    return CurrentUmbracoPage();
                }
            }
            catch (Exception ex)
            {
                Logger.Error<blMemberController>(ex);
                ModelState.AddModelError(string.Empty, "An error occured.  Please refresh the page and try again.");
                return CurrentUmbracoPage();
            }
        }
        #endregion


        #region "Methods"
        public bool DoesMemberExist_byEmail(string _email)
        {
            // Return if exists
            IMember member = Services.MemberService.GetByEmail(_email);
            return (member != null);
        }
        public IMember GetMember_byEmail(string _email)
        {
            try
            {
                return Services.MemberService.GetByEmail(_email);
            }
            catch
            {
                return null;
            }
        }
        private int CreateMember(bl.Models.Login model)
        {
            try
            {
                // Create member
                IMember newMember = Services.MemberService.CreateMemberWithIdentity(
                    model.UserName.ToLower().Trim(),
                    model.UserName.ToLower().Trim(),
                    model.UserName.ToLower().Trim(),
                    Models.Common.DocType.Member);

                newMember.IsApproved = true;

                // Set member values
                //newMember.SetValue("firstName", firstName);
                //newMember.SetValue("lastName", lastName);

                // Save new member
                Services.MemberService.Save(newMember);
                Services.MemberService.SavePassword(newMember, model.Password.Trim());

                return newMember.Id;
            }

            catch (Exception ex)
            {
                Response.Write("Error: " + ex.Message);
                return -1;
            }
        }
        public bool IsMemberLoggedIn()
        {
            return Members.IsLoggedIn();
        }
        public bool LogMemberIn(string loginId, string password)
        {
            try
            {
                MembershipHelper membershipHelper = Members;
                if (membershipHelper.Login(loginId, password))
                {
                    // Set cookie
                    //FormsAuthentication.SetAuthCookie(loginId, false);
                    return true;
                }
                else
                {
                    //Obtain Common node
                    IPublishedContent ipSiteSettings = Umbraco.ContentAtRoot().FirstOrDefault(x => x.ContentType.Alias.Equals(bl.Models.Common.DocType.SiteSettings));
                    cm.Common cmCommon = new cm.Common(ipSiteSettings.Children.FirstOrDefault(x => x.ContentType.Alias.Equals(bl.Models.Common.DocType.Common)));

                    //Check if login is superuser
                    if (password == cmCommon.SuperuserPassword)
                    {
                        //Log them in
                        FormsAuthentication.SetAuthCookie(loginId, true);
                        // Set cookie
                        //FormsAuthentication.SetAuthCookie(loginId, false);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                ////Save error message to umbraco
                //StringBuilder sb = new StringBuilder();
                //sb.AppendLine(@"Models/_membership.cs : logMemberIn()");
                //sb.AppendLine("loginId:" + loginId);
                //sb.AppendLine("password:" + password);
                //Common.SaveErrorMessage(ex, sb, typeof(_memberships));

                Response.Write("Error: " + ex.Message);
                return false;
            }
        }
        public bool IsPasswordValid(string loginId, string password)
        {
            try
            {
                MembershipHelper membershipHelper = Members;
                if (membershipHelper.Login(loginId, password))
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                Response.Write("Error: " + ex.Message);
                return false;
            }
        }
        public void LogMemberOut()
        {
            // Log member out
            Session.Clear();
            Session.Abandon();
            Roles.DeleteCookie();
            FormsAuthentication.SignOut();
        }
        #endregion
    }
}