using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace bl.Models
{
    public sealed class Common
    {
        #region "Properties"
        public enum SiteNode : int
        {
            AcctMembership = 1142,
            AcctMnmt = 1093,
            AcctSetup = 1138,
            BugoutBags = 1094,
            Clothing = 1095,
            Communications = 1096,
            Cooking = 1097,
            CreateAnAcct = 1086,
            Dashboard = 1087,
            Fire = 1098,
            FirstAid = 1099,
            Food = 1100,
            Gardening = 1101,
            Home = 1056,
            Hygiene = 1102,
            LocationMngr = 1145,
            MembershipMnmt = 1144,
            Miscellaneous = 1103,
            Pets = 1141,
            PreppingColorMngr = 1146,
            PricePlan = 1085,
            ReactivateAcct = 1135,
            Sandbox = 1088,
            ScrumBoard = 1104,
            Security = 1105,
            Shelter = 1106,
            SpiritualNeeds = 1107,
            Tools = 1108,
            UserPreferences = 1143,
            Water = 1109
        }
        public enum LoginStatus
        {
            LoggedOut,
            InactiveMember,
            LoggedIn_Active,
            LoggedIn_SetupRequired
        }
        public enum Genders
        {
            [Display(Name = "XY [Male]")] Male,
            [Display(Name = "XX [Female]")] Female,
            [Display(Name = "Unspecified")] Unspecified
        }


        public struct NodeProperty
        {
            public const string ActiveMember = "activeMember";
            public const string ActiveTill = "activeTill";
            public const string InstructionalInfo = "instructionalInfo";
            public const string IsTool = "isTool";
            public const string IsInventoryList = "isInventoryList";
            public const string ItemSections = "itemSections";
            public const string LifetimeMember = "lifetimeMember";
            public const string MemberSince = "memberSince";
            public const string NavigationIcon = "navigationIcon";
            public const string NavigationTitleOverride = "navigationTitleOverride";
            public const string SetupComplete = "setupComplete";
            public const string ShowInSideNav = "showInSideNav";
            public const string Subtitle = "subtitle";
            public const string Title = "title";
            public const string ToolId = "toolId";
        }
        public struct DocTypes
        {
            public const string AccountManagement = "accountManagement";
            public const string AccountMembership = "accountMembership";
            public const string AccountSetup = "accountSetup";
            public const string LocationManager = "locationManager";
            public const string MembersManager = "membersManager";
            public const string PreppingColorManager = "preppingColorManager";
            public const string UserPreferences = "userPreferences";

            public const string BugoutBags = "bugoutBags";
            public const string Clothing = "clothing";
            public const string CookingStoring = "cookingStoring";
            public const string Fire = "fire";
            public const string FirstAid = "firstAid";
            public const string Food = "food";
            public const string HuntingFishing = "huntingFishing";
            public const string Hygiene = "hygiene";
            public const string Miscellaneous = "miscellaneous";
            public const string PetsAnimalCare = "petsAnimalCare";
            public const string PowerFuel = "powerFuel";
            public const string ScrumBoard = "scrumBoard";
            public const string SecurityDefense = "securityDefense";
            public const string Shelter = "shelter";
            public const string SpiritualNeeds = "spiritualNeeds";
            public const string ToolsHardware = "toolsHardware";
            //public const string Communications = "communications";
            //public const string GardeningForaging = "gardeningForaging";
            //public const string Water = "water";
        }
        public struct ItemSections
        {
            public const string BugoutBags = "Bugout Bags";
            public const string Categories = "Categories";
            public const string Clothing = "Clothing";
            public const string ExpirationDate = "Expiration Date";
            //public const string FilledWithWater = "Filled with Water";
            public const string FluidsOnly = "Fluids Only";
            public const string FuelRequirements = "Fuel Requirements";
            public const string Genders = "Genders";
            public const string Locations = "Locations";
            public const string Measurements = "Measurements";
            public const string Ownership = "Ownership";
            public const string PowerRequirements = "Power Requirements";
            public const string Seasonal = "Seasonal";
            public const string Toolsets = "Toolsets";
        }
        public struct PartialPath
        {
            public const string AcctManagement_Login = "~/Views/Partials/AcctManagement/Login.cshtml";
            public const string AcctManagement_AccountMembership = "~/Views/Partials/Dashboard/AcctManagement/AccountMembership.cshtml";
            public const string AcctManagement_LocationsManager = "~/Views/Partials/Dashboard/AcctManagement/LocationsManager.cshtml";
            public const string AcctManagement_MembersManagement = "~/Views/Partials/Dashboard/AcctManagement/MembersManagement.cshtml";
            public const string AcctManagement_SideNavigation = "~/Views/Partials/Dashboard/AcctManagement/SideNavigation.cshtml";
            public const string AcctManagement_ToolsAndColors = "~/Views/Partials/Dashboard/AcctManagement/ToolsAndColors.cshtml";
            public const string AcctManagement_UserPreferences = "~/Views/Partials/Dashboard/AcctManagement/UserPreferences.cshtml";

            public const string AcctSetup_Dedication = "~/Views/Partials/AcctManagement/AcctSetup/Dedication.cshtml";
            public const string AcctSetup_Form = "~/Views/Partials/AcctManagement/AcctSetup/AcctSetupForm.cshtml";
            public const string AcctSetup_Introduction = "~/Views/Partials/AcctManagement/AcctSetup/Introduction.cshtml";
            public const string AcctSetup_Locations = "~/Views/Partials/AcctManagement/AcctSetup/Locations.cshtml";
            public const string AcctSetup_Members = "~/Views/Partials/AcctManagement/AcctSetup/Members.cshtml";
            public const string AcctSetup_SideSteps = "~/Views/Partials/AcctManagement/AcctSetup/SideSteps.cshtml";
            public const string AcctSetup_ToolSets = "~/Views/Partials/AcctManagement/AcctSetup/ToolSets.cshtml";
            public const string AcctSetup_UserInfo = "~/Views/Partials/AcctManagement/AcctSetup/UserInfo.cshtml";

            public const string Aside = "~/Views/Partials/Dashboard/Navigations/Aside.cshtml";

            public const string Tool_CurrentInventory_BugoutBags = "~/Views/Partials/Dashboard/BugoutBags/CurrentInventory.cshtml";
            public const string Tool_CurrentInventory_Clothing = "~/Views/Partials/Dashboard/Clothing/CurrentInventory.cshtml";
            public const string Tool_CurrentInventory_Communications = "~/Views/Partials/Dashboard/Communications/CurrentInventory.cshtml";
            public const string Tool_CurrentInventory_Cooking = "~/Views/Partials/Dashboard/Cooking/CurrentInventory.cshtml";
            public const string Tool_CurrentInventory_Fire = "~/Views/Partials/Dashboard/Fire/CurrentInventory.cshtml";
            public const string Tool_CurrentInventory_FirstAid = "~/Views/Partials/Dashboard/FirstAid/CurrentInventory.cshtml";
            public const string Tool_CurrentInventory_Food = "~/Views/Partials/Dashboard/Food/CurrentInventory.cshtml";
            public const string Tool_CurrentInventory_GardeningForaging = "~/Views/Partials/Dashboard/GardeningForaging/CurrentInventory.cshtml";
            public const string Tool_CurrentInventory_HuntingFishing = "~/Views/Partials/Dashboard/HuntingFishing/CurrentInventory.cshtml";
            public const string Tool_CurrentInventory_Hygiene = "~/Views/Partials/Dashboard/Hygiene/CurrentInventory.cshtml";
            public const string Tool_CurrentInventory_Miscellaneous = "~/Views/Partials/Dashboard/Miscellaneous/CurrentInventory.cshtml";
            public const string Tool_CurrentInventory_Navigations = "~/Views/Partials/Dashboard/Navigations/CurrentInventory.cshtml";
            public const string Tool_CurrentInventory_Pets = "~/Views/Partials/Dashboard/Pets/CurrentInventory.cshtml";
            public const string Tool_CurrentInventory_PowerFuel = "~/Views/Partials/Dashboard/PowerFuel/CurrentInventory.cshtml";
            public const string Tool_CurrentInventory_SecurityDefense = "~/Views/Partials/Dashboard/SecurityDefense/CurrentInventory.cshtml";
            public const string Tool_CurrentInventory_Shelter = "~/Views/Partials/Dashboard/Shelter/CurrentInventory.cshtml";
            public const string Tool_CurrentInventory_SpiritualNeeds = "~/Views/Partials/Dashboard/SpiritualNeeds/CurrentInventory.cshtml";
            public const string Tool_CurrentInventory_ToolsHardware = "~/Views/Partials/Dashboard/ToolsHardware/CurrentInventory.cshtml";
            public const string Tool_CurrentInventory_Water = "~/Views/Partials/Dashboard/Water/CurrentInventory.cshtml";

            public const string Tool_Common_AddItem = "~/Views/Partials/Dashboard/Common/AddItem.cshtml";
            public const string Tool_Common_TabButtons = "~/Views/Partials/Dashboard/Common/TabButtons.cshtml";
        }
        public struct wwwController
        {
            public const string AcctMngmnt = "AcctMngmnt";
            public const string AcctSetup = "AcctSetup";
            public const string Login = "Login";
            public const string Navigation = "Navigation";
            public const string Tools = "Tools";
        }
        public struct wwwAction
        {
            public const string RenderForm_AccountMembership = "RenderForm_AccountMembership";
            public const string RenderForm_AcctSetup = "RenderForm_AcctSetup";
            public const string RenderForm_AddItem = "RenderForm_AddItem";
            public const string RenderForm_CurrentInventory = "RenderForm_CurrentInventory";
            public const string RenderForm_LocationsManager = "RenderForm_LocationsManager";
            public const string RenderForm_Login = "RenderForm_Login";
            public const string RenderForm_MembersManagement = "RenderForm_MembersManagement";
            public const string RenderForm_TabButtons = "RenderForm_TabButtons";
            public const string RenderForm_ToolsAndColors = "RenderForm_ToolsAndColors";
            public const string RenderForm_UserPreferences = "RenderForm_UserPreferences";

            public const string RenderSideToolNavigation = "RenderSideToolNavigation";

            public const string FormSubmit_AccountMembership = "FormSubmit_AccountMembership";
            public const string FormSubmit_AcctSetup = "FormSubmit_AcctSetup";
            public const string FormSubmit_AddItem = "FormSubmit_AddItem";
            public const string FormSubmit_CurrentInventory = "FormSubmit_CurrentInventory";
            public const string FormSubmit_LocationsManager = "FormSubmit_LocationsManager";
            public const string FormSubmit_LogMemberIn = "FormSubmit_LogMemberIn";
            public const string FormSubmit_Logout = "FormSubmit_Logout";
            public const string FormSubmit_MembersManagement = "FormSubmit_MembersManagement";
            public const string FormSubmit_ToolsAndColors = "FormSubmit_ToolsAndColors";
            public const string FormSubmit_UpdateItem = "FormSubmit_UpdateItem";
            public const string FormSubmit_UserPreferences = "FormSubmit_UserPreferences";
        }


        public struct TempData
        {
            public const string InventoryModel = "InventoryModel";
            public const string Model = "Model";
            public const string ReturnTo = "ReturnTo";
            public const string UpdateId = "UpdateId";
        }
        #endregion


        #region "Methods"
        #endregion
    }
}

