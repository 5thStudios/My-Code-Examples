using System;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using bl.Models;
using Umbraco.Web;
using System.Linq;
using System.Collections.Generic;
using bl.EF;
using bl.Repositories;

namespace www.Controllers
{
    public class AcctSetupController : SurfaceController
    {
        #region "Properties"
        private bl.Models.AcctSetupModel model { get; set; }

        private readonly IAccountRepository repoAccount;
        private readonly IMemberRepository repoMember;
        private readonly IGenderRepository repoGenders;
        private readonly ILocationRepository repoLocations;
        private readonly IToolRepository repoTools;
        private readonly IMemberToolRepository repoMemberTools;
        private readonly IColorRepository repoColors;
        #endregion

        public AcctSetupController()
        {
            bl.EF.EFPrepperSuiteDb _context = new bl.EF.EFPrepperSuiteDb();
            repoAccount = new AccountRepository(_context);
            repoMember = new MemberRepository(_context);
            repoGenders = new GenderRepository(_context);
            repoLocations = new LocationRepository(_context);
            repoTools = new ToolRepository(_context);
            repoMemberTools = new MemberToolRepository(_context);
            repoColors = new ColorRepository(_context);
        }

        #region "ActionResults"
        [ChildActionOnly]
        public ActionResult RenderForm_AcctSetup(bl.Models.AcctSetupModel _model)
        {
            //Redirect if member is not logged in?
            if (!new bl.Controllers.MembershipController().IsMemberLoggedIn())
            {
                Response.Redirect(Umbraco.Content((int)(Common.SiteNode.Sandbox)).Url());
            }

            //context = new bl.EF.EFPrepperSuiteDb();
            if (_model == null)
                model = new bl.Models.AcctSetupModel();
            else
                model = _model;

            //Increment/Decrement step
            if (model.Previous)
                model.StepIndex--;
            if (model.Next)
            {
                if (ModelState.IsValid)
                    model.StepIndex++;
            }

            //Obtain data from database depending on current step
            switch (model.StepIndex)
            {
                case 0:
                    Step_0_Setup_AccountAndModel();
                    break;
                case 1:
                    Step_1_Setup_SelectUserInfo();
                    break;
                case 2:
                    Step_2_Setup_MemberList();
                    break;
                case 3:
                    Step_3_Setup_Locations();
                    break;
                case 4:
                    Step_4_Setup_Toolsets();
                    break;
                case 5:
                    Step_5_Setup_Dedication();
                    break;
            }


            return View(Common.PartialPath.AcctSetup_Form, model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FormSubmit_AcctSetup(bl.Models.AcctSetupModel _model, string btnSubmit = "", string btnDelete = "", string btnComplete = "")
        {
            //Redirect if member is not logged in?
            if (!new bl.Controllers.MembershipController().IsMemberLoggedIn())
            {
                Response.Redirect(Umbraco.Content((int)(Common.SiteNode.Sandbox)).Url());
            }

            //
            model = _model;

            //If previous button clicked, go back.
            if (_model.Previous)
            {
                return CurrentUmbracoPage();
            }
            else if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }
            else if (ModelState.IsValid)
            {
                switch (_model.StepIndex)
                {
                    case 0:
                        Step_0_AddUpdate_AccountAndModel();
                        break;
                    case 1:
                        Step_1_AddUpdate_UserInfo();
                        break;
                    case 2:
                        Step_2_AddUpdate_MemberList(btnSubmit, btnDelete);
                        break;
                    case 3:
                        Step_3_AddUpdate_Locations(btnSubmit, btnDelete);
                        break;
                    case 4:
                        Step_4_Update_Toolsets();
                        break;
                    case 5:
                        Step_5_AddUpdate_Dedication();
                        break;
                }
            }


            //
            if (!ModelState.IsValid)
            {
                _model.Next = false;
            }
            return CurrentUmbracoPage();
        }

        #endregion


        #region "Selects"
        public void Step_0_Setup_AccountAndModel()
        {
            //Obtain the logged-in member's Id
            model.MemberId = Services.MemberService.GetById(Members.GetCurrentMemberId()).Id;


            //  Add/Update Account Record
            Account _acct;
            Boolean AcctExist = repoAccount.DoesRecordExist(model.MemberId);
            if (AcctExist)
            {
                _acct = repoAccount.GetRecord_byMemberId(model.MemberId);
            }
            else
            {
                _acct = new Account();
                _acct.AccountHolderId = model.MemberId;
                repoAccount.AddRecord(_acct);
            }

            model.AccountId = _acct.AccountId;
        }
        public void Step_1_Setup_SelectUserInfo()
        {
            //Obtain Gender List from Db
            model.LstGenders = repoGenders.GetSelectItemList();

            //Obtain the current user info
            Boolean memberExist = repoMember.DoesAccountHolderExist_byAccountId(model.AccountId);
            if (memberExist)
            {
                Member _member = repoMember.GetRecord_byAccountId(model.AccountId);
                model.CurrentUserInfo.FirstName = _member.FirstName;
                model.CurrentUserInfo.LastInitial = _member.LastInitial;
                model.CurrentUserInfo.BirthYear = _member.BirthYear;
                model.CurrentUserInfo.Gender = _member.GenderId;
            }
        }
        public void Step_2_Setup_MemberList()
        {
            //Obtain Gender List from Db
            model.LstGenders = repoGenders.GetSelectItemList();

            //Clear data for new user
            model.NewUserInfo = new AccountMember();


            //Obtain list of members.
            model.LstMembers = new List<AccountMember>();
            IEnumerable<Member> _lstMembers = repoMember.GetList_byAccountId(model.AccountId);
            foreach (var _member in _lstMembers)
            {
                AccountMember acctMember = new AccountMember();
                acctMember.MemberId = _member.MemberId;
                acctMember.FirstName = _member.FirstName;
                acctMember.LastInitial = _member.LastInitial;
                acctMember.BirthYear = _member.BirthYear;
                acctMember.Gender = _member.GenderId;
                acctMember.IsAccountOwner = _member.IsAccountOwner;
                model.LstMembers.Add(acctMember);
            }
        }
        public void Step_3_Setup_Locations()
        {
            //Obtain list of members.
            model.LstLocations = new List<bl.Models.Location>();
            IEnumerable<bl.EF.Location> _lstLocations = repoLocations.GetList_byAccountId(model.AccountId);
            foreach (var _location in _lstLocations)
            {
                bl.Models.Location newLocation = new bl.Models.Location();
                newLocation.LocationId = _location.LocationId;
                newLocation.Name = _location.Name;
                model.LstLocations.Add(newLocation);
            }
        }
        public void Step_4_Setup_Toolsets()
        {
            //
            int toolCount = repoTools.GetCount();
            int memberToolCount = repoMemberTools.GetCount_byAccountId(model.AccountId);

            //
            if (toolCount != memberToolCount)
            {
                //Get all tools
                IEnumerable<Tool> _tools = repoTools.GetList();

                foreach (var _tool in _tools)
                {
                    //
                    Boolean toolExist = repoMemberTools.DoesRecordExist(model.AccountId, _tool.ToolId);
                    if (!toolExist)
                    {
                        MemberTool _memberTool = new MemberTool();
                        _memberTool.AccountId = model.AccountId;
                        _memberTool.ToolId = _tool.ToolId;
                        _memberTool.ColorId = _tool.ColorId;
                        _memberTool.IsActive = true;
                        repoMemberTools.AddRecord(_memberTool);
                    }
                }
            }

            //Obtain All Member/Tool Data
            model.LstToolsets = new List<Toolset>();
            foreach (MemberTool _memberTool in repoMemberTools.GetList_byAccountId(model.AccountId))
            {
                bl.EF.Color _color = repoColors.GetRecord_byId(_memberTool.ColorId);
                Toolset _toolset = new Toolset();
                _toolset.ColorId = _memberTool.ColorId;
                _toolset.IsActive = _memberTool.IsActive;
                _toolset.MemberToolId = _memberTool.MemberToolId;
                _toolset.ToolId = _memberTool.ToolId;

                _toolset.ColorCode = _color.ColorCode;
                _toolset.ColorName = _color.ColorName;
                _toolset.Name = repoTools.GetName(_toolset.ToolId);

                model.LstToolsets.Add(_toolset);
            }

            //Obtain all available colors
            model.LstColors = new List<bl.Models.Color>();
            foreach (var _color in repoColors.GetList())
            {
                bl.Models.Color newColor = new bl.Models.Color();
                newColor.ColorCode = _color.ColorCode;
                newColor.ColorId = _color.ColorId;
                newColor.ColorName = _color.ColorName;
                model.LstColors.Add(newColor);
            }
        }
        public void Step_5_Setup_Dedication() { }
        #endregion


        #region "Add/Updates"
        private void Step_0_AddUpdate_AccountAndModel() { }
        private void Step_1_AddUpdate_UserInfo()
        {
            //
            //Does member exist?
            //	Yes:
            //		Retrieve data
            //	No:
            //		Create new

            //	Add data to class
            //		Save/Update



            //  Add/Update Member Record
            Member _member;
            Boolean memberExist = repoMember.DoesAccountHolderExist_byAccountId(model.AccountId); // context.Members.Any(x => x.AccountId == model.AccountId);
            if (memberExist)
            {
                _member = repoMember.GetRecord_byAccountId(model.AccountId); //context.Members.Where(x => x.AccountId == model.AccountId).FirstOrDefault();
            }
            else
            {
                _member = new Member();
            }
            _member.AccountId = model.AccountId;
            _member.IsAccountOwner = true;
            _member.FirstName = model.CurrentUserInfo.FirstName;
            _member.LastInitial = model.CurrentUserInfo.LastInitial;
            _member.BirthYear = model.CurrentUserInfo.BirthYear;
            _member.GenderId = model.CurrentUserInfo.Gender;
            if (!memberExist)
                repoMember.AddRecord(_member);
            else
                repoMember.UpdateRecord(_member);

        }
        private void Step_2_AddUpdate_MemberList(string memberId, string btnDelete)
        {
            if (!string.IsNullOrEmpty(btnDelete))
            {
                int _memberId = int.Parse(btnDelete);
                
                //Delete from database
                repoMember.DeleteRecord(_memberId);

                //Delete from local member list
                AccountMember _clsMember = model.LstMembers.Where(x => x.MemberId == _memberId).FirstOrDefault();
                model.LstMembers.Remove(_clsMember);
            }
            else if (string.IsNullOrEmpty(memberId))
            {
                //Do nothing, just go to next step.
            }
            else if (memberId == "-1")
            {
                //Add Member Record
                Member _member = new Member();
                _member.AccountId = model.AccountId;
                _member.IsAccountOwner = false;
                _member.FirstName = model.NewUserInfo.FirstName;
                _member.LastInitial = model.NewUserInfo.LastInitial;
                _member.BirthYear = model.NewUserInfo.BirthYear;
                _member.GenderId = model.NewUserInfo.Gender;

                repoMember.AddRecord(_member);
            }
            else
            {
                //Update Member Record
                int _memberId = int.Parse(memberId);
                Member _member = repoMember.GetRecord_byMemberId(_memberId); 
                AccountMember updatedMember = model.LstMembers.Where(x => x.MemberId == _memberId).FirstOrDefault();
                _member.FirstName = updatedMember.FirstName;
                _member.LastInitial = updatedMember.LastInitial;
                _member.BirthYear = updatedMember.BirthYear;
                _member.GenderId = updatedMember.Gender;
                repoMember.UpdateRecord(_member);
            }

        }
        private void Step_3_AddUpdate_Locations(string locationId, string btnDelete)
        {
            if (!string.IsNullOrEmpty(btnDelete))
            {
                int _locationId = int.Parse(btnDelete);
                repoLocations.DeleteRecord(_locationId);

                bl.Models.Location _clsLocation = model.LstLocations.Where(x => x.LocationId == _locationId).FirstOrDefault();
                model.LstLocations.Remove(_clsLocation);
            }
            else if (string.IsNullOrEmpty(locationId))
            {
                //Do nothing, just go to next step.
            }
            else if (locationId == "-1")
            {
                //Add Location Record
                bl.EF.Location _location = new bl.EF.Location();
                _location.AccountId = model.AccountId;
                _location.Name = model.NewLocation.Name;
                repoLocations.AddRecord(_location);
            }
            else
            {
                //Update Location Record
                int _locationId = int.Parse(locationId);
                bl.EF.Location _location = repoLocations.GetRecord_byId(_locationId); 
                bl.Models.Location updatedLocation = model.LstLocations.Where(x => x.LocationId == _locationId).FirstOrDefault();
                _location.Name = updatedLocation.Name;
                _location.LocationId = updatedLocation.LocationId;
                repoLocations.UpdateRecord(_location);
            }
        }
        private void Step_4_Update_Toolsets()
        {
            //Update toolsets
            foreach (var _toolset in model.LstToolsets)
            {
                MemberTool _memberTool = repoMemberTools.GetRecord_byId(_toolset.MemberToolId); 
                _memberTool.ColorId = _toolset.ColorId;
                _memberTool.IsActive = _toolset.IsActive;
                repoMemberTools.UpdateRecord(_memberTool);
            }
        }
        private void Step_5_AddUpdate_Dedication()
        {
            //Instantiate variables
            bl.Controllers.MembershipController blMembership = new bl.Controllers.MembershipController();
            blMembership.UpdateMember_SetupComplete();
            Response.Redirect(Umbraco.Content((int)(Common.SiteNode.Dashboard)).Url());
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