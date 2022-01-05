using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Umbraco.Core.Services;
using Umbraco;
using Umbraco.Core;
using Umbraco.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Web.Security;
using static bl.Models.Common;
using Umbraco.Core.Models.PublishedContent;
using ContentModels = Umbraco.Web.PublishedModels;
using bl.Models;
using System.Web.Mvc;
using System.Web.Security;

namespace bl.Controllers
{
    public class MembershipController : SurfaceController
    {
        //THIS CONTROLLER MANAGES MEMBERSHIP FROM WITHIN UMBRACO


        #region "Submit ActionResults"
        public ActionResult FormSubmit_Logout()
        {
            TempData.Clear();
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToCurrentUmbracoPage();
        }
        #endregion


        #region Methods"
        public Boolean IsMemberLoggedIn()
        {
            //Return if member is logged in.
            return Members.IsLoggedIn();
        }
        public LoginStatus GetLoginStatus()
        {
            //Determin member status
            if (IsMemberLoggedIn())
            {
                //Instantiate member
                var CmMember = new ContentModels.Member(Members.GetCurrentMember());

                if (!CmMember.ActiveMember)
                {
                    //Member is not active.
                    return LoginStatus.InactiveMember;
                }
                else if (!CmMember.SetupComplete)
                {
                    //Member is active but account is not set up.
                    return LoginStatus.LoggedIn_SetupRequired;
                }
                else
                {
                    //Member is active AND account is fully set up.
                    return LoginStatus.LoggedIn_Active;
                }
            }
            else
            {
                //Member is not logged in.
                return LoginStatus.LoggedOut;
            }
        }
        public Boolean LogMemberIn(Models.LoginModel model)
        {
            //Attempt to log in and return if successful or not.
            return Members.Login(model.LoginId.Trim(), model.Password.Trim());
        }
        public int GetMemberId()
        {
            return Services.MemberService.GetById(Members.GetCurrentMemberId()).Id;
        }
        public void UpdateMember_SetupComplete()
        {
            IMember member = Services.MemberService.GetById(Members.GetCurrentMemberId());
            member.SetValue(Common.NodeProperty.SetupComplete, true);
            Services.MemberService.Save(member, true);
        }
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