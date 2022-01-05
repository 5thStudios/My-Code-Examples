using System;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using bl.Models;
using Umbraco.Web;
using Umbraco.Core.Models.PublishedContent;
using System.Web.Security;
using System.Collections.Specialized;

namespace www.Controllers
{
    public class LoginController : SurfaceController
    {
        #region "Renders"
        /// <summary>
        ///Logged In?
        ///  Yes
        ///	    Is active member?
        ///		    Yes
        ///			    Is setup complete?
        ///				    Yes - navigate to dashboard
        ///  				No	- navigate to setup
        ///  		No
        ///  			Msg: membership inactive.  Please subscribe
        ///  No
        ///  	show login screen
        /// </summary>
        public ActionResult RenderForm_Login()
        {
            //Instantiate variables
            bl.Controllers.MembershipController blMembership = new bl.Controllers.MembershipController();

            //Is Member Logged In?
            if (blMembership.IsMemberLoggedIn())
            {
                //Obtain Login Status
                Common.LoginStatus loginStatus = blMembership.GetLoginStatus();

                //Determine action based on status.
                switch (loginStatus)
                {
                    case Common.LoginStatus.InactiveMember:
                        ModelState.AddModelError("", "*Account membership is inactive.  Renew membership.");
                        break;

                    case Common.LoginStatus.LoggedIn_Active:


                        if (TempData[Common.TempData.ReturnTo] != null) //ViewBag.ReturnTo != null)  //???Returning null ???
                        {
                        //If returnTo querystring exists, redirect there.
                            //int ipNodeId = Int32.Parse(Request.QueryString["returnTo"]);
                            Response.Redirect(Umbraco.Content(TempData[Common.TempData.ReturnTo]).Url());
                        }
                        else
                        {
                            Response.Redirect(Umbraco.Content((int)(Common.SiteNode.Dashboard)).Url());
                        }
                        break;

                    case Common.LoginStatus.LoggedIn_SetupRequired:
                        Response.Redirect(Umbraco.Content((int)(Common.SiteNode.AcctSetup)).Url());
                        break;

                    case Common.LoginStatus.LoggedOut:
                        break;

                    default:
                        break;
                }
            }

            return PartialView(Common.PartialPath.AcctManagement_Login);
        }
        #endregion



        #region "ActionResults"
        /// <summary>
        ///	Is ModelState Valid?
        ///		Yes
        ///			Can Log In?
        ///				Yes
        ///					Redirect to current page				
        ///				No
        ///					Show invalid log msg
        ///		No
        ///			Show error msg
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FormSubmit_LogMemberIn(bl.Models.LoginModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Instantiate variables
                    bl.Controllers.MembershipController blMembership = new bl.Controllers.MembershipController();

                    //Attempt to log member in.
                    if (blMembership.LogMemberIn(model))
                    {
                        //Add querystring to viewbag
                        if (!String.IsNullOrEmpty(Request.QueryString["returnTo"]))
                        {
                            TempData[Common.TempData.ReturnTo] = Int32.Parse(Request.QueryString["returnTo"]);
                        }

                        //FormsAuthentication.SetAuthCookie(model.Username, false); // set to true for "remember me."
                        return RedirectToCurrentUmbracoPage();
                    }
                    else
                    {
                        ModelState.AddModelError("", "*Invalid User Id or Password.  Please try again.");
                        return CurrentUmbracoPage();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "*Modelstate is Invalid");
                    return CurrentUmbracoPage();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occured while logging into your account.");
                ModelState.AddModelError("", ex.Message);
                return CurrentUmbracoPage();
            }

        }
        #endregion


        #region "Static"
        public static void DetermineLoginStatus(HttpResponseBase Response, UmbracoHelper Umbraco, IPublishedContent Model)
        {
            //Instantiate variables
            bl.Controllers.MembershipController blMembership = new bl.Controllers.MembershipController();


            //Is Member Logged In?
            if (blMembership.IsMemberLoggedIn())
            {
                //Obtain Login Status
                Common.LoginStatus loginStatus = blMembership.GetLoginStatus();

                //Determine any redirect action based on status.
                switch (loginStatus)
                {
                    case Common.LoginStatus.InactiveMember:
                        if (Model.Id != (int)(Common.SiteNode.ReactivateAcct))
                            Response.Redirect(Umbraco.Content((int)(Common.SiteNode.ReactivateAcct)).Url());
                        break;

                    case Common.LoginStatus.LoggedIn_SetupRequired:
                        if (Model.Id != (int)(Common.SiteNode.AcctSetup))
                            Response.Redirect(Umbraco.Content((int)(Common.SiteNode.AcctSetup)).Url());
                        break;

                    case Common.LoginStatus.LoggedOut:
                        if (Model.Id != (int)(Common.SiteNode.Sandbox))
                            Response.Redirect(Umbraco.Content((int)(Common.SiteNode.Sandbox)).Url());
                        break;

                    case Common.LoginStatus.LoggedIn_Active:
                        //If acct setup, redirect to dashboard
                        if (Model.Id == (int)(Common.SiteNode.AcctSetup))
                        {
                            Response.Redirect(Umbraco.Content((int)(Common.SiteNode.Dashboard)).Url());
                        }

                        break;

                    default:
                        break;
                }
            }
            else
            {
                //Create querystring collection
                NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
                queryString.Add("returnTo", Model.Id.ToString());

                //build url 
                if (Model.Id != (int)(Common.SiteNode.Sandbox))
                    Response.Redirect(Umbraco.Content((int)(Common.SiteNode.Sandbox)).Url() + "?" + queryString.ToString());
            }
        }
        #endregion


        #region "Methods"
        #endregion
    }
}




//ServiceContext Services { get; }
//ISqlContext SqlContext { get; }
//UmbracoHelper Umbraco { get; }
//UmbracoContext UmbracoContext { get; }
//IGlobalSettings GlobalSettings { get; }
//IProfilingLogger Logger { get; }
//MembershipHelper Members { get; }