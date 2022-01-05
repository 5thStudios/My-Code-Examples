using System;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using bl.Models;
using Umbraco.Web;
using System.Linq;
using System.Collections.Generic;
using bl.EF;
using bl.Repositories;
using System.Web;
using Umbraco.Core.Models.PublishedContent;

namespace www.Controllers
{
    public class AcctMngmntController : SurfaceController
    {
        #region "Properties"
        //private bl.Models.AcctSetupModel model { get; set; }

        private readonly IAccountRepository repoAccount;
        private readonly IMemberRepository repoMember;
        private readonly IGenderRepository repoGenders;
        private readonly ILocationRepository repoLocations;
        private readonly IToolRepository repoTools;
        private readonly IMemberToolRepository repoMemberTools;
        private readonly IColorRepository repoColors;
        private readonly IItemRepository repoItems;
        #endregion

        public AcctMngmntController()
        {
            bl.EF.EFPrepperSuiteDb _context = new bl.EF.EFPrepperSuiteDb();
            repoAccount = new AccountRepository(_context);
            repoMember = new MemberRepository(_context);
            repoGenders = new GenderRepository(_context);
            repoLocations = new LocationRepository(_context);
            repoTools = new ToolRepository(_context);
            repoMemberTools = new MemberToolRepository(_context);
            repoColors = new ColorRepository(_context);
            repoItems = new ItemRepository(_context);
        }


        #region "Render ActionResults"
        public ActionResult RenderForm_AccountMembership()
        {
            //Instantiate variables
            bl.Models.AcctSetupModel Model = new AcctSetupModel();

            //Return data with partial view
            return PartialView(Common.PartialPath.AcctManagement_AccountMembership, Model);
        }
        public ActionResult RenderForm_UserPreferences()
        {
            //Instantiate variables
            bl.Models.AcctSetupModel Model = new AcctSetupModel();

            //Return data with partial view
            return PartialView(Common.PartialPath.AcctManagement_UserPreferences, Model);
        }
        public ActionResult RenderForm_MembersManagement()
        {
            //Instantiate variables
            bl.Models.AcctSetupModel Model = new AcctSetupModel();







            //Obtain the logged-in member's Id
            Model.MemberId = Services.MemberService.GetById(Members.GetCurrentMemberId()).Id;


            //  Add/Update Account Record
            Account _acct = repoAccount.GetRecord_byMemberId(Model.MemberId);

            Model.AccountId = _acct.AccountId;



            //Obtain Gender List from Db
            Model.LstGenders = repoGenders.GetSelectItemList();

            //Clear data for new user
            Model.NewUserInfo = new AccountMember();


            //Obtain list of members.
            Model.LstMembers = new List<AccountMember>();
            IEnumerable<Member> _lstMembers = repoMember.GetList_byAccountId(Model.AccountId);
            foreach (var _member in _lstMembers)
            {
                AccountMember acctMember = new AccountMember();
                acctMember.MemberId = _member.MemberId;
                acctMember.FirstName = _member.FirstName;
                acctMember.LastInitial = _member.LastInitial;
                acctMember.BirthYear = _member.BirthYear;
                acctMember.Gender = _member.GenderId;
                acctMember.IsAccountOwner = _member.IsAccountOwner;
                Model.LstMembers.Add(acctMember);
            }







            //Return data with partial view
            return PartialView(Common.PartialPath.AcctManagement_MembersManagement, Model);
        }
        public ActionResult RenderForm_LocationsManager()
        {
            //Instantiate variables
            bl.Models.AcctSetupModel Model = new AcctSetupModel();
            List<int> lstMemberIDs = new List<int>();

            //Obtain the logged-in member's Id
            Model.MemberId = Services.MemberService.GetById(Members.GetCurrentMemberId()).Id;

            //  Obtain Account Id
            Account _acct = repoAccount.GetRecord_byMemberId(Model.MemberId);
            Model.AccountId = _acct.AccountId;

            //Obtain list of locatons.
            Model.LstLocations = new List<bl.Models.Location>();
            IEnumerable<bl.EF.Location> _lstLocations = repoLocations.GetList_byAccountId(Model.AccountId);
            foreach (var _location in _lstLocations)
            {
                bl.Models.Location newLocation = new bl.Models.Location();
                newLocation.LocationId = _location.LocationId;
                newLocation.Name = _location.Name;
                newLocation.IsBugoutBag = _location.IsBugoutBag;
                newLocation.MemberId = _location.MemberId;
                Model.LstLocations.Add(newLocation);

                //Add bugout bag members to list
                if (_location.IsBugoutBag)
                {
                    if (_location.MemberId != null)
                    {
                        lstMemberIDs.Add((int)_location.MemberId);
                    }
                }
            }

            //Obtain list of members. [do not add if bugout bag already exists for member]
            Model.LstMembers = new List<AccountMember>();
            IEnumerable<Member> _lstMembers = repoMember.GetList_byAccountId(Model.AccountId);
            //Add spare bag option
            AccountMember acctMember = new AccountMember();
            acctMember.MemberId = -1;
            acctMember.FirstName = "Spare Bag";
            Model.LstMembers.Add(acctMember);

            foreach (var _member in _lstMembers)
            {
                if (!lstMemberIDs.Contains(_member.MemberId))
                {
                    acctMember = new AccountMember();
                    acctMember.MemberId = _member.MemberId;
                    acctMember.FirstName = _member.FirstName + " " + _member.LastInitial;
                    Model.LstMembers.Add(acctMember);
                }
            }

            //Return data with partial view
            return PartialView(Common.PartialPath.AcctManagement_LocationsManager, Model);
        }
        public ActionResult RenderForm_ToolsAndColors()
        {
            //Instantiate variables
            bl.Models.AcctSetupModel Model = new AcctSetupModel();

            //Obtain the logged-in member's Id
            Model.MemberId = Services.MemberService.GetById(Members.GetCurrentMemberId()).Id;

            //  Add/Update Account Record
            Account _acct = repoAccount.GetRecord_byMemberId(Model.MemberId);

            Model.AccountId = _acct.AccountId;




            //
            int toolCount = repoTools.GetCount();
            int memberToolCount = repoMemberTools.GetCount_byAccountId(Model.AccountId);


            //Obtain All Member/Tool Data
            Model.LstToolsets = new List<Toolset>();
            foreach (MemberTool _memberTool in repoMemberTools.GetList_byAccountId(Model.AccountId))
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

                Model.LstToolsets.Add(_toolset);
            }

            //Obtain all available colors
            Model.LstColors = new List<bl.Models.Color>();
            foreach (var _color in repoColors.GetList())
            {
                bl.Models.Color newColor = new bl.Models.Color();
                newColor.ColorCode = _color.ColorCode;
                newColor.ColorId = _color.ColorId;
                newColor.ColorName = _color.ColorName;
                Model.LstColors.Add(newColor);
            }




            //Return data with partial view
            return PartialView(Common.PartialPath.AcctManagement_ToolsAndColors, Model);
        }
        #endregion


        #region "Submit ActionResults"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FormSubmit_AccountMembership(bl.Models.AcctSetupModel Model, string btnSubmit = "", string btnDelete = "", string btnComplete = "")
        {
            //Redirect if member is not logged in?
            if (!new bl.Controllers.MembershipController().IsMemberLoggedIn())
            {
                Response.Redirect(Umbraco.Content((int)(Common.SiteNode.Sandbox)).Url());
            }

            //If previous button clicked, go back.
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }
            else if (ModelState.IsValid)
            {

            }

            return CurrentUmbracoPage();
        }
        public ActionResult FormSubmit_UserPreferences(bl.Models.AcctSetupModel Model, string btnSubmit = "", string btnDelete = "", string btnComplete = "")
        {
            //Redirect if member is not logged in?
            if (!new bl.Controllers.MembershipController().IsMemberLoggedIn())
            {
                Response.Redirect(Umbraco.Content((int)(Common.SiteNode.Sandbox)).Url());
            }

            //If previous button clicked, go back.
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }
            else if (ModelState.IsValid)
            {

            }

            return CurrentUmbracoPage();
        }
        public ActionResult FormSubmit_MembersManagement(bl.Models.AcctSetupModel Model, string btnSubmit = "", string btnDelete = "", string btnComplete = "")
        {
            //Redirect if member is not logged in?
            if (!new bl.Controllers.MembershipController().IsMemberLoggedIn())
            {
                Response.Redirect(Umbraco.Content((int)(Common.SiteNode.Sandbox)).Url());
            }

            //If previous button clicked, go back.
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }
            else if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(btnDelete))
                {
                    int _memberId = int.Parse(btnDelete);

                    //Delete from database
                    repoMember.DeleteRecord(_memberId);

                    //Delete from local member list
                    AccountMember _clsMember = Model.LstMembers.Where(x => x.MemberId == _memberId).FirstOrDefault();
                    Model.LstMembers.Remove(_clsMember);
                }
                else if (string.IsNullOrEmpty(btnSubmit))
                {
                    //Do nothing, just go to next step.
                }
                else if (btnSubmit == "-1")
                {
                    //Add Member Record
                    Member _member = new Member();
                    _member.AccountId = Model.AccountId;
                    _member.IsAccountOwner = false;
                    _member.FirstName = Model.NewUserInfo.FirstName;
                    _member.LastInitial = Model.NewUserInfo.LastInitial;
                    _member.BirthYear = Model.NewUserInfo.BirthYear;
                    _member.GenderId = Model.NewUserInfo.Gender;

                    repoMember.AddRecord(_member);
                }
                else
                {
                    //Update Member Record
                    int _memberId = int.Parse(btnSubmit);
                    Member _member = repoMember.GetRecord_byMemberId(_memberId);
                    AccountMember updatedMember = Model.LstMembers.Where(x => x.MemberId == _memberId).FirstOrDefault();
                    _member.FirstName = updatedMember.FirstName;
                    _member.LastInitial = updatedMember.LastInitial;
                    _member.BirthYear = updatedMember.BirthYear;
                    _member.GenderId = updatedMember.Gender;
                    repoMember.UpdateRecord(_member);
                }
            }

            return CurrentUmbracoPage();
        }
        public ActionResult FormSubmit_LocationsManager(bl.Models.AcctSetupModel Model, string btnSubmit = "", string btnDelete = "", string btnComplete = "", int? btnUpdateBag = null)
        {
            //Redirect if member is not logged in?
            if (!new bl.Controllers.MembershipController().IsMemberLoggedIn())
            {
                Response.Redirect(Umbraco.Content((int)(Common.SiteNode.Sandbox)).Url());
            }

            //If previous button clicked, go back.
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }
            else if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(btnDelete))
                {
                    //Obtain location Id to be deleted
                    int _locationId = int.Parse(btnDelete);
                    //Delete location from items and then location records.
                    repoItems.DeleteLocation(_locationId);
                    repoLocations.DeleteRecord(_locationId);
                    //
                    bl.Models.Location _clsLocation = Model.LstLocations.Where(x => x.LocationId == _locationId).FirstOrDefault();
                    Model.LstLocations.Remove(_clsLocation);
                }
                else if (btnUpdateBag != null)
                {


                    //IS THIS ALL CORRECT?????  no
                    //IDEA: FIRST MAKE SURE EVERYTHING ELSE STILL WORKS.  THEN ATTEMPT TO GET DROPDOWN VALUE ON SUBMIT.


                    //Update Location Record
                    bl.EF.Location _location = repoLocations.GetRecord_byId((int)btnUpdateBag);
                    bl.Models.Location updatedLocation = Model.LstLocations.Where(x => x.LocationId == (int)btnUpdateBag).FirstOrDefault();

                    _location.LocationId = updatedLocation.LocationId;

                    if (_location.IsBugoutBag)
                    {
                        if (Model.NewLocation.MemberId == -1)
                        {
                            //Index variables
                            int index = 1;
                            List<int> _lstIndexes = new List<int>();
                            //Get existing bugout bags and determine next spare bag #
                            IEnumerable<bl.EF.Location> _lstLocations = repoLocations.GetBugoutBagList_byAccountId(Model.AccountId);
                            foreach (string _name in _lstLocations.Select(x => x.Name))
                            {
                                if (_name.Contains('#'))
                                {
                                    _lstIndexes.Add(Convert.ToInt32(_name.Split('#')[1]));
                                    index++;
                                }
                            }
                            while (_lstIndexes.Contains(index))
                            {
                                index++;
                            }

                            //Add Spare bag
                            _location.Name = "Spare Bag #" + index.ToString();
                        }
                        else
                        {
                            _location.MemberId = Model.NewLocation.MemberId;
                            bl.EF.Member _member = repoMember.GetRecord_byMemberId((int)Model.NewLocation.MemberId);
                            _location.Name = _member.FirstName + "'s Bugout Bag";
                        }

                    }
                    else
                    {
                        _location.Name = updatedLocation.Name;
                    }

                    //repoLocations.UpdateRecord(_location);








                }
                else if (btnSubmit == "-1")
                {
                    //Add Location Record
                    bl.EF.Location _location = new bl.EF.Location();
                    _location.AccountId = Model.AccountId;
                    _location.Name = Model.NewLocation.Name;
                    //repoLocations.AddRecord(_location);
                }
                else if (btnSubmit == "-2")
                {
                    //Add Location Record
                    bl.EF.Location _location = new bl.EF.Location();
                    _location.AccountId = Model.AccountId;
                    _location.IsBugoutBag = true;
                    if (Model.NewLocation.MemberId == -1)
                    {
                        //Index variables
                        int index = 1;
                        List<int> _lstIndexes = new List<int>();
                        //Get existing bugout bags and determine next spare bag #
                        IEnumerable<bl.EF.Location> _lstLocations = repoLocations.GetBugoutBagList_byAccountId(Model.AccountId);
                        foreach (string _name in _lstLocations.Select(x => x.Name))
                        {
                            if (_name.Contains('#'))
                            {
                                _lstIndexes.Add(Convert.ToInt32(_name.Split('#')[1]));
                                index++;
                            }
                        }
                        while (_lstIndexes.Contains(index))
                        {
                            index++;
                        }

                        //Add Spare bag
                        _location.Name = "Spare Bag #" + index.ToString();
                    }
                    else
                    {
                        _location.MemberId = Model.NewLocation.MemberId;
                        bl.EF.Member _member = repoMember.GetRecord_byMemberId((int)Model.NewLocation.MemberId);
                        _location.Name = _member.FirstName + "'s Bugout Bag";
                    }

                    repoLocations.AddRecord(_location);
                }
                //else
                //{
                //    //Update Location Record
                //    int _locationId = int.Parse(btnSubmit);
                //    bl.EF.Location _location = repoLocations.GetRecord_byId(_locationId);
                //    bl.Models.Location updatedLocation = Model.LstLocations.Where(x => x.LocationId == _locationId).FirstOrDefault();

                //    _location.LocationId = updatedLocation.LocationId;

                //    if (_location.IsBugoutBag)
                //    {
                //        if (Model.NewLocation.MemberId == -1)
                //        {
                //            //Index variables
                //            int index = 1;
                //            List<int> _lstIndexes = new List<int>();
                //            //Get existing bugout bags and determine next spare bag #
                //            IEnumerable<bl.EF.Location> _lstLocations = repoLocations.GetBugoutBagList_byAccountId(Model.AccountId);
                //            foreach (string _name in _lstLocations.Select(x => x.Name))
                //            {
                //                if (_name.Contains('#'))
                //                {
                //                    _lstIndexes.Add(Convert.ToInt32(_name.Split('#')[1]));
                //                    index++;
                //                }
                //            }
                //            while (_lstIndexes.Contains(index))
                //            {
                //                index++;
                //            }

                //            //Add Spare bag
                //            _location.Name = "Spare Bag #" + index.ToString();
                //        }
                //        else
                //        {
                //            _location.MemberId = Model.NewLocation.MemberId;
                //            bl.EF.Member _member = repoMember.GetRecord_byMemberId((int)Model.NewLocation.MemberId);
                //            _location.Name = _member.FirstName + "'s Bugout Bag";
                //        }

                //    }
                //    else
                //    {
                //        _location.Name = updatedLocation.Name;
                //    }

                //    repoLocations.UpdateRecord(_location);
                //}
            }

            return CurrentUmbracoPage();
        }
        public ActionResult FormSubmit_ToolsAndColors(bl.Models.AcctSetupModel Model, string btnSubmit = "", string btnCancel = "")
        {
            //Redirect if member is not logged in?
            if (!new bl.Controllers.MembershipController().IsMemberLoggedIn())
            {
                Response.Redirect(Umbraco.Content((int)(Common.SiteNode.Sandbox)).Url());
            }

            //If previous button clicked, go back.
            if (btnCancel == "true")
            {
                return RedirectToCurrentUmbracoPage();
            }
            else if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }
            else if (ModelState.IsValid)
            {
                //Update toolsets
                foreach (var _toolset in Model.LstToolsets)
                {
                    MemberTool _memberTool = repoMemberTools.GetRecord_byId(_toolset.MemberToolId);
                    _memberTool.ColorId = _toolset.ColorId;
                    _memberTool.IsActive = _toolset.IsActive;
                    repoMemberTools.UpdateRecord(_memberTool);
                }
            }

            return CurrentUmbracoPage();
        }
        #endregion



        #region "Static"
        public static List<Link> ObtainSideNavigation(UmbracoHelper Umbraco, IPublishedContent Model)
        {
            //Instantiate variables
            List<Link> lstLinks = new List<Link>();
            IPublishedContent ipAcctMnmt = Umbraco.Content((int)(Common.SiteNode.AcctMnmt));

            //Loop through all child pages for navigation information
            foreach (IPublishedContent ip in ipAcctMnmt.Children)
            {
                if (ip.Value<Boolean>(Common.NodeProperty.ShowInSideNav))
                {
                    Link link = new Link()
                    {
                        Title = ip.Value<string>(Common.NodeProperty.Title),
                        Subtitle = ip.Value<string>(Common.NodeProperty.Subtitle),
                        Url = ip.Url(),
                        IsActive = (ip.Id == Model.Id)
                    };
                    lstLinks.Add(link);
                }
            }

            //Return list of links
            return lstLinks;
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