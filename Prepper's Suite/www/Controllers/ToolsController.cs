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
using ContentModels = Umbraco.Web.PublishedModels;
using System.Collections.Specialized;

namespace www.Controllers
{
    public class ToolsController : SurfaceController
    {
        //ServiceContext Services { get; }
        //ISqlContext SqlContext { get; }
        //UmbracoHelper Umbraco { get; }
        //UmbracoContext UmbracoContext { get; }
        //IGlobalSettings GlobalSettings { get; }
        //IProfilingLogger Logger { get; }
        //MembershipHelper Members { get; }


        #region "Properties"
        private bl.Models.Inventory Model { get; set; }

        private readonly IAccountRepository repoAccount;
        private readonly IBatteryRepository repoBatteries;
        private readonly IBatteryTypeRepository repoBatteryTypes;
        private readonly ICategoryRepository repoCategories;
        private readonly IColorRepository repoColors;
        private readonly IFoodTypeRepository repoFoodTypes;
        private readonly IFuelRepository repoFuel;
        private readonly IFuelTypeRepository repoFuelTypes;
        private readonly IGenderRepository repoGender;
        private readonly IItemRepository repoItems;
        private readonly ILocationRepository repoLocations;
        private readonly IMeasurementStateRepository repoMeasurementStates;
        private readonly IMeasurementSystemRepository repoMeasurementSystem;
        private readonly IMeasurementTypeRepository repoMeasurementTypes;
        private readonly IMemberRepository repoMembers;
        private readonly IMemberToolRepository repoMemberTools;
        private readonly IPowerSourcesRepository repoPowerSource;
        private readonly ISeasonRepository repoSeasons;
        private readonly IToolRepository repoTools;
        private readonly IVolumeRepository repoVolume;

        public ToolsController()
        {
            bl.EF.EFPrepperSuiteDb _context = new bl.EF.EFPrepperSuiteDb();
            repoAccount = new AccountRepository(_context);
            repoBatteries = new BatteryRepository(_context);
            repoBatteryTypes = new BatteryTypeRepository(_context);
            repoCategories = new CategoryRepository(_context);
            repoColors = new ColorRepository(_context);
            repoFoodTypes = new FoodTypeRepository(_context);
            repoFuel = new FuelRepository(_context);
            repoFuelTypes = new FuelTypeRepository(_context);
            repoGender = new GenderRepository(_context);
            repoItems = new ItemRepository(_context);
            repoLocations = new LocationRepository(_context);
            repoMeasurementStates = new MeasurementStateRepository(_context);
            repoMeasurementSystem = new MeasurementSystemRepository(_context);
            repoMeasurementTypes = new MeasurementTypeRepository(_context);
            repoMembers = new MemberRepository(_context);
            repoMemberTools = new MemberToolRepository(_context);
            repoPowerSource = new PowerSourcesRepository(_context);
            repoSeasons = new SeasonRepository(_context);
            repoTools = new ToolRepository(_context);
            repoVolume = new VolumeRepository(_context);
        }
        #endregion


        #region "Render ActionResults"
        public ActionResult RenderForm_CurrentInventory(IPublishedContent ipModel)
        {
            //Instantiate variables
            InstantiateModel(ipModel);

            //Determine what tools to display based on doctype
            switch (Model.DocType)
            {
                case Common.DocTypes.BugoutBags:
                    return PartialView(Common.PartialPath.Tool_CurrentInventory_BugoutBags, Model);
                case Common.DocTypes.Clothing:
                    return PartialView(Common.PartialPath.Tool_CurrentInventory_Clothing, Model);
                //case Common.DocTypes.Communications:
                //    return PartialView(Common.PartialPath.Tool_CurrentInventory_Communications, Model);
                case Common.DocTypes.CookingStoring:
                    return PartialView(Common.PartialPath.Tool_CurrentInventory_Cooking, Model);
                case Common.DocTypes.Fire:
                    return PartialView(Common.PartialPath.Tool_CurrentInventory_Fire, Model);
                case Common.DocTypes.FirstAid:
                    return PartialView(Common.PartialPath.Tool_CurrentInventory_FirstAid, Model);
                case Common.DocTypes.Food:
                    return PartialView(Common.PartialPath.Tool_CurrentInventory_Food, Model);
                //case Common.DocTypes.GardeningForaging:
                //    return PartialView(Common.PartialPath.Tool_CurrentInventory_GardeningForaging, Model);
                case Common.DocTypes.HuntingFishing:
                    return PartialView(Common.PartialPath.Tool_CurrentInventory_HuntingFishing, Model);
                case Common.DocTypes.Hygiene:
                    return PartialView(Common.PartialPath.Tool_CurrentInventory_Hygiene, Model);
                case Common.DocTypes.Miscellaneous:
                    return PartialView(Common.PartialPath.Tool_CurrentInventory_Miscellaneous, Model);
                case Common.DocTypes.PetsAnimalCare:
                    return PartialView(Common.PartialPath.Tool_CurrentInventory_Pets, Model);
                case Common.DocTypes.PowerFuel:
                    return PartialView(Common.PartialPath.Tool_CurrentInventory_PowerFuel, Model);
                case Common.DocTypes.SecurityDefense:
                    return PartialView(Common.PartialPath.Tool_CurrentInventory_SecurityDefense, Model);
                case Common.DocTypes.Shelter:
                    return PartialView(Common.PartialPath.Tool_CurrentInventory_Shelter, Model);
                case Common.DocTypes.SpiritualNeeds:
                    return PartialView(Common.PartialPath.Tool_CurrentInventory_SpiritualNeeds, Model);
                case Common.DocTypes.ToolsHardware:
                    return PartialView(Common.PartialPath.Tool_CurrentInventory_ToolsHardware, Model);
                //case Common.DocTypes.Water:
                //    return PartialView(Common.PartialPath.Tool_CurrentInventory_Water, Model);
                default:
                    return PartialView(Common.PartialPath.Tool_CurrentInventory_SecurityDefense, Model);
            }
        }
        public ActionResult RenderForm_AddItem(IPublishedContent ipModel)
        {
            //Instantiate variables
            InstantiateModel(ipModel);

            //Set new record's default values
            Model.NewItem.AccountId = Model.AccountId;
            Model.NewItem.ToolId = Model.ToolId;

            //Return data with partial view
            return PartialView(Common.PartialPath.Tool_Common_AddItem, Model);
        }
        public ActionResult RenderForm_TabButtons(IPublishedContent ipModel)
        {
            //Instantiate variables
            InstantiateModel(ipModel);

            //Return data with partial view
            return PartialView(Common.PartialPath.Tool_Common_TabButtons, Model);
        }
        #endregion


        #region "Submit ActionResults"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FormSubmit_CurrentInventory(bl.Models.Inventory Model, int? btnUpdate, int? btnDelete, int? btnRemoveFromBugoutBag)
        {
            //Redirect if member is not logged in?
            if (!new bl.Controllers.MembershipController().IsMemberLoggedIn()) { Response.Redirect(Umbraco.Content((int)(Common.SiteNode.Sandbox)).Url()); }

            //Determine how to process callback
            if (!ModelState.IsValid) { return CurrentUmbracoPage(); }
            else if (btnDelete != null)
            {
                //Instantiate IDs of records to delete
                int _ItemId = (int)btnDelete;
                int? _PowerSourceId = null;
                int? _BatteriesId = null;
                int? _FuelId = null;
                int? _VolumeId = null;

                //Obtain record to be deleted
                Item _item2Delete = repoItems.GetRecord_byId(_ItemId);

                //Obtain IDs of records to be deleted
                if (_item2Delete.PowerSource != null)
                {
                    _PowerSourceId = _item2Delete.PowerSource.PowerSourceId;

                    if (_item2Delete.PowerSource.BatteriesId != null)
                    {
                        _BatteriesId = _item2Delete.PowerSource.Battery.BatteriesId;
                    }
                    if (_item2Delete.PowerSource.FuelId != null)
                    {
                        _FuelId = _item2Delete.PowerSource.Fuel.FuelId;
                    }
                }
                if (_item2Delete.VolumeId != null)
                {
                    _VolumeId = _item2Delete.Volume.VolumeId;
                }

                //Delete item record and children
                repoItems.DeleteRecord(_ItemId);
                if (_PowerSourceId != null) { repoPowerSource.DeleteRecord((int)_PowerSourceId); }
                if (_BatteriesId != null) { repoBatteries.DeleteRecord((int)_BatteriesId); }
                if (_FuelId != null) { repoFuel.DeleteRecord((int)_FuelId); }
                if (_VolumeId != null) { repoVolume.DeleteRecord((int)_VolumeId); }
            }
            else if (btnUpdate != null)
            {
                //repoItems.AddRecord(Model.NewItem);
                TempData[Common.TempData.UpdateId] = (int)btnUpdate;
                Model.ShowUpdateRecord = true;
            }
            else if (btnRemoveFromBugoutBag != null)
            {
                repoItems.UpdateRecordsLocation((int)btnRemoveFromBugoutBag, null);
            }

            return RedirectToCurrentUmbracoPage();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FormSubmit_AddItem(bl.Models.Inventory Model, int? btnAdd2BugoutBag, string btnLookup)
        {
            //Redirect if member is not logged in
            if (!new bl.Controllers.MembershipController().IsMemberLoggedIn()) { Response.Redirect(Umbraco.Content((int)(Common.SiteNode.Sandbox)).Url()); }

            //Determine next step to take
            if (!ModelState.IsValid)
            {
                //Invalid modelstate.  Cannot proceed.
                ShowErrorMsgs();
                return CurrentUmbracoPage();
            }
            else if (ModelState.IsValid)
            {
                try
                {
                    if (btnAdd2BugoutBag != null)
                    {
                        repoItems.UpdateRecordsLocation((int)btnAdd2BugoutBag, Model.NewItem.LocationId);
                    }
                    else if (btnLookup != null)
                    {
                        //Update data and return to screen
                        string json = GetDataById(Model.NewItem.Barcode);

                        if (string.IsNullOrEmpty(json))
                        {
                            //No record found
                            ViewData["DataMissing"] = true;
                            return CurrentUmbracoPage();
                        }
                        else
                        {
                            ViewData["jsonResponse"] = json;

                            //Parse json
                            Product _product = Newtonsoft.Json.JsonConvert.DeserializeObject<bl.Models.Product>(json);




                            //Model.NewItem.JsonData = json;
                            //Model.NewItem.Name = _product.Hints.FirstOrDefault().Food.Label;

                            //MeasurementType measurementType = repoMeasurementTypes.GetRecord_byType(_product.Hints.FirstOrDefault().Food.ServingSizes.FirstOrDefault().Label);
                            //Model.NewItem.Volume.MeasurementTypeId = measurementType.MeasurementTypeId;

                            //int servingsPerContainer = (int)_product.Hints.FirstOrDefault().Food.ServingsPerContainer;
                            //Model.NewItem.Volume.Units = (int)_product.Hints.FirstOrDefault().Food.ServingSizes.FirstOrDefault().Quantity * servingsPerContainer;
                            //Model.NewItem.Volume.MeasurementSystemId = measurementType.MeasurementSystemId;


                            if (Model.RapidInput)
                            {
                                //Create new item and insert it automatically.
                                Model.NewItem.JsonData = json;
                                Model.NewItem.Name = _product.Hints.FirstOrDefault().Food.Label;

                                MeasurementType measurementType = repoMeasurementTypes.GetRecord_byType(_product.Hints.FirstOrDefault().Food.ServingSizes.FirstOrDefault().Label);
                                Model.NewItem.Volume.MeasurementTypeId = measurementType.MeasurementTypeId;

                                int servingsPerContainer = (int)_product.Hints.FirstOrDefault().Food.ServingsPerContainer;
                                Model.NewItem.Volume.Units = (int)_product.Hints.FirstOrDefault().Food.ServingSizes.FirstOrDefault().Quantity * servingsPerContainer;
                                Model.NewItem.Volume.MeasurementSystemId = measurementType.MeasurementSystemId;

                                Model.NewItem.CreatedTimestamp = DateTime.Now;
                                Model.NewItem.LastUpdatedTimestamp = DateTime.Now;
                                Model.NewItem.Quantity = 1;
                                repoItems.AddRecord(Model.NewItem);
                            }
                            else
                            {
                                //Pass data to razor
                                ViewData["productJson"] = json;
                                ViewData["productUpc"] = Model.NewItem.Barcode;
                                ViewData["productLabel"] = _product.Hints.FirstOrDefault().Food.Label;
                                ViewData["productServingsPerContainer"] = _product.Hints.FirstOrDefault().Food.ServingsPerContainer;
                                ViewData["productServingSizes"] = _product.Hints.FirstOrDefault().Food.ServingSizes.FirstOrDefault().Label;
                                ViewData["productUnits"] = _product.Hints.FirstOrDefault().Food.ServingSizes.FirstOrDefault().Quantity;

                                //PARSE JSON AND THEN ADD INDVIVIDUAL ITEMS: NAME, MEASUREMENTS.
                                //      ***TAKE THE MEASUREMENT AND CONVERT TO TYPE IN NAME IF APPLICABLE.
                                return CurrentUmbracoPage();
                            }
                        }


                    }
                    else
                    {
                        Model.NewItem.CreatedTimestamp = DateTime.Now;
                        Model.NewItem.LastUpdatedTimestamp = DateTime.Now;

                        //Validate dropdown sections
                        if (Model.ItemSection.FuelRequirements)
                        {
                            if (Model.NewItem.PowerSource.RequiresFuel)
                            {
                                if (Model.NewItem.PowerSource.Fuel.FuelTypeId == null)
                                {
                                    Model.NewItem.PowerSource.RequiresFuel = false;
                                    Model.NewItem.PowerSource.Fuel.HasFuel = false;
                                }
                            }
                        }
                        if (Model.ItemSection.PowerRequirements)
                        {
                            if (Model.NewItem.PowerSource.RequiresBatteries)
                            {
                                if (Model.NewItem.PowerSource.Battery.BatteryTypeId == null)
                                {
                                    Model.NewItem.PowerSource.RequiresBatteries = false;
                                    Model.NewItem.PowerSource.Battery.HasBatteries = false;
                                }
                                else
                                {
                                    if (Model.NewItem.PowerSource.Battery.BatteryQuantity == null || Model.NewItem.PowerSource.Battery.BatteryQuantity == 0)
                                    {
                                        Model.NewItem.PowerSource.Battery.BatteryQuantity = 1;
                                    }
                                }
                            }
                        }
                        if (Model.ItemSection.Measurements)
                        {
                            if (Model.NewItem.Volume.MeasurementTypeId == null)
                            {
                                Model.NewItem.Volume.Units = null;
                            }
                            else
                            {
                                if (Model.NewItem.Volume.Units == null || Model.NewItem.Volume.Units <= 0)
                                {
                                    Model.NewItem.Volume.Units = null;
                                    Model.NewItem.Volume.MeasurementTypeId = null;
                                }
                            }
                        }



                        //Add the proper quantity of items
                        if (Model.NewItem.Quantity == null || Model.NewItem.Quantity < 1)
                        {
                            Model.NewItem.Quantity = 1;
                        }
                        for (int i = 0; i < Model.NewItem.Quantity; i++)
                        {
                            repoItems.AddRecord(Model.NewItem);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowErrorMsgs(ex);
                    return CurrentUmbracoPage();
                }
            }

            return RedirectToCurrentUmbracoPage();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FormSubmit_UpdateItem(bl.Models.Inventory Model, bool btnCancel = false)
        {
            //Redirect if member is not logged in
            if (!new bl.Controllers.MembershipController().IsMemberLoggedIn()) { Response.Redirect(Umbraco.Content((int)(Common.SiteNode.Sandbox)).Url()); }

            //Determine next step to take
            if (btnCancel)
            {
                //Cancel current operation and refresh page
                return RedirectToCurrentUmbracoPage();
            }
            else if (!ModelState.IsValid)
            {
                //Invalid modelstate.  Cannot proceed.
                ShowErrorMsgs();
                return CurrentUmbracoPage();
            }
            else
            {
                try
                {
                    if (Model.ItemSection.FuelRequirements)
                    {
                        //Clear child tables if applicable
                        if (Model.UpdateItem.PowerSource.RequiresFuel == false)
                        {
                            Model.UpdateItem.PowerSource.Fuel.FuelTypeId = null;
                            Model.UpdateItem.PowerSource.Fuel.HasFuel = false;
                        }
                        else if (Model.UpdateItem.PowerSource.RequiresFuel)
                        {
                            if (Model.UpdateItem.PowerSource.Fuel.FuelTypeId == null)
                            {
                                Model.UpdateItem.PowerSource.RequiresFuel = false;
                                Model.UpdateItem.PowerSource.Fuel.HasFuel = false;
                            }
                        }
                        //Update tables
                        repoFuel.UpdateRecord(Model.UpdateItem.PowerSource.Fuel);
                    }


                    if (Model.ItemSection.PowerRequirements)
                    {
                        //Clear child tables if applicable
                        if (Model.UpdateItem.PowerSource.RequiresBatteries == false)
                        {
                            Model.UpdateItem.PowerSource.Battery.BatteryTypeId = null;
                            Model.UpdateItem.PowerSource.Battery.HasBatteries = false;
                        }
                        else if (Model.UpdateItem.PowerSource.RequiresBatteries)
                        {
                            if (Model.UpdateItem.PowerSource.Battery.BatteryTypeId == null)
                            {
                                Model.UpdateItem.PowerSource.RequiresBatteries = false;
                                Model.UpdateItem.PowerSource.Battery.HasBatteries = false;
                            }
                            else
                            {
                                if (Model.UpdateItem.PowerSource.Battery.BatteryQuantity == null || Model.UpdateItem.PowerSource.Battery.BatteryQuantity == 0)
                                {
                                    Model.UpdateItem.PowerSource.Battery.BatteryQuantity = 1;
                                }
                            }
                        }
                        //Update tables
                        repoBatteries.UpdateRecord(Model.UpdateItem.PowerSource.Battery);
                    }


                    if (Model.ItemSection.PowerRequirements || Model.ItemSection.FuelRequirements)
                    {
                        //Update tables
                        repoPowerSource.UpdateRecord(Model.UpdateItem.PowerSource);
                    }

                    if (Model.ItemSection.Measurements)
                    {
                        if (Model.UpdateItem.Volume.MeasurementTypeId == null)
                        {
                            Model.UpdateItem.Volume.Units = null;
                        }
                        else
                        {
                            if (Model.UpdateItem.Volume.Units == null || Model.UpdateItem.Volume.Units < 0)
                            {
                                Model.UpdateItem.Volume.Units = null;
                                Model.UpdateItem.Volume.MeasurementTypeId = null;
                            }
                        }

                        repoVolume.UpdateRecord(Model.UpdateItem.Volume);
                    }

                    //Update Item
                    Model.UpdateItem.LastUpdatedTimestamp = DateTime.Now;
                    repoItems.UpdateRecord(Model.UpdateItem);
                }
                catch (Exception ex)
                {
                    ShowErrorMsgs(ex);
                    return CurrentUmbracoPage();
                }
            }


            return RedirectToCurrentUmbracoPage();
        }
        #endregion


        #region "Static and Methods"
        private void InstantiateModel(IPublishedContent ipModel)
        {
            //Instantiate Model
            if (TempData[Common.TempData.InventoryModel] != null)
            {
                //Save model from temp data
                Model = (bl.Models.Inventory)TempData[Common.TempData.InventoryModel];
            }
            else if (Model == null)
            {
                //Create new model
                Model = new bl.Models.Inventory();

                //Obtain the logged-in member's Id, Account Id, Doctype and Tool Id
                Model.MemberId = Services.MemberService.GetById(Members.GetCurrentMemberId()).Id;
                Model.AccountId = repoAccount.GetRecord_byMemberId(Model.MemberId).AccountId;
                Model.ToolId = ipModel.Value<int>(Common.NodeProperty.ToolId);
                Model.DocType = ipModel.ContentType.Alias;

                //Determine what data to obtain based on doctype
                switch (Model.DocType)
                {
                    case Common.DocTypes.BugoutBags:
                        Model.ItemSection.BugoutBags = true;
                        break;
                    case Common.DocTypes.Clothing:
                        Model.ItemSection.Categories = true;
                        Model.ItemSection.Genders = true;
                        Model.ItemSection.Locations = true;
                        Model.ItemSection.Ownership = true;
                        Model.ItemSection.Quantity = true;
                        Model.ItemSection.Seasonal = true;
                        break;
                    case Common.DocTypes.CookingStoring:
                        Model.ItemSection.Categories = true;
                        Model.ItemSection.FuelRequirements = true;
                        Model.ItemSection.Locations = true;
                        Model.ItemSection.Measurements = true;
                        Model.ItemSection.PowerRequirements = true;
                        Model.ItemSection.Quantity = true;
                        break;
                    case Common.DocTypes.Fire:
                        Model.ItemSection.Categories = true;
                        Model.ItemSection.FuelRequirements = true;
                        Model.ItemSection.Locations = true;
                        Model.ItemSection.PowerRequirements = true;
                        Model.ItemSection.Quantity = true;
                        break;
                    case Common.DocTypes.FirstAid:
                        Model.ItemSection.Categories = true;
                        Model.ItemSection.ExpirationDate = true;
                        Model.ItemSection.Locations = true;
                        Model.ItemSection.PowerRequirements = true;
                        Model.ItemSection.Quantity = true;
                        break;
                    case Common.DocTypes.Food:
                        Model.ItemSection.BarcodeEntry = true;
                        Model.ItemSection.Categories = true;
                        Model.ItemSection.ExpirationDate = true;
                        Model.ItemSection.Locations = true;
                        Model.ItemSection.Measurements = true;
                        Model.ItemSection.Quantity = true;
                        break;
                    //case Common.DocTypes.GardeningForaging:
                    //    Model.ItemSection.Categories = true;
                    //    Model.ItemSection.FuelRequirements = true;
                    //    Model.ItemSection.Locations = true;
                    //    Model.ItemSection.PowerRequirements = true;
                    //    Model.ItemSection.Quantity = true;
                    //    break;
                    case Common.DocTypes.HuntingFishing:
                        Model.ItemSection.Categories = true;
                        Model.ItemSection.Locations = true;
                        Model.ItemSection.PowerRequirements = true;
                        Model.ItemSection.Quantity = true;
                        break;
                    case Common.DocTypes.Hygiene:
                        Model.ItemSection.Categories = true;
                        Model.ItemSection.Genders = true;
                        Model.ItemSection.Locations = true;
                        Model.ItemSection.Quantity = true;
                        break;
                    case Common.DocTypes.Miscellaneous:
                        Model.ItemSection.FuelRequirements = true;
                        Model.ItemSection.Locations = true;
                        Model.ItemSection.PowerRequirements = true;
                        Model.ItemSection.Quantity = true;
                        break;
                    case Common.DocTypes.PetsAnimalCare:
                        Model.ItemSection.Categories = true;
                        Model.ItemSection.ExpirationDate = true;
                        Model.ItemSection.Locations = true;
                        Model.ItemSection.Measurements = true;
                        Model.ItemSection.Quantity = true;
                        break;
                    case Common.DocTypes.PowerFuel:
                        Model.ItemSection.Categories = true;
                        Model.ItemSection.FuelRequirements = true;
                        Model.ItemSection.Locations = true;
                        Model.ItemSection.PowerRequirements = true;
                        Model.ItemSection.Quantity = true;
                        break;
                    case Common.DocTypes.SecurityDefense:
                        Model.ItemSection.Categories = true;
                        Model.ItemSection.Locations = true;
                        Model.ItemSection.Quantity = true;
                        break;
                    case Common.DocTypes.Shelter:
                        Model.ItemSection.Categories = true;
                        Model.ItemSection.Locations = true;
                        Model.ItemSection.Quantity = true;
                        break;
                    case Common.DocTypes.SpiritualNeeds:
                        Model.ItemSection.Categories = true;
                        Model.ItemSection.Locations = true;
                        Model.ItemSection.Quantity = true;
                        break;
                    case Common.DocTypes.ToolsHardware:
                        Model.ItemSection.Categories = true;
                        Model.ItemSection.FuelRequirements = true;
                        Model.ItemSection.Locations = true;
                        Model.ItemSection.PowerRequirements = true;
                        Model.ItemSection.Quantity = true;
                        break;
                    default:
                        break;
                }

                //Obtain list of categories for toolset
                if (Model.ItemSection.Categories)
                {
                    Model.LstCategories = repoCategories.GetList_byToolId(ipModel.Value<int>(Common.NodeProperty.ToolId));
                }

                //
                if (Model.ItemSection.ExpirationDate) { }

                //
                if (Model.ItemSection.FuelRequirements)
                {
                    //Obtain needed data
                    Model.LstFuelTypes = repoFuelTypes.GetList();

                    //Set default values
                    if (Model.NewItem.PowerSource == null)
                    {
                        Model.NewItem.PowerSource = new PowerSource();
                    }
                    if (Model.NewItem.PowerSource.Fuel == null)
                    {
                        Model.NewItem.PowerSource.Fuel = new Fuel();
                    }
                    Model.NewItem.PowerSource.RequiresAC = false;
                    Model.NewItem.PowerSource.RequiresBatteries = false;
                    Model.NewItem.PowerSource.RequiresFuel = false;
                    Model.NewItem.PowerSource.Fuel.HasFuel = false;
                }

                //
                if (Model.ItemSection.Genders)
                {
                    //Obtain needed data
                    Model.LstGenders = repoGender.GetList();

                }

                //Obtain member's location list
                if (Model.ItemSection.Locations)
                {
                    Model.LstLocations = repoLocations.GetList_byAccountId(Model.AccountId);
                }

                //
                if (Model.ItemSection.Measurements)
                {
                    Model.LstMeasurementStates = repoMeasurementStates.GetList();
                    Model.LstMeasurementSystems = repoMeasurementSystem.GetList();
                    Model.LstMeasurementTypes = repoMeasurementTypes.GetList();

                    Model.NewItem.Volume = new Volume();
                    if (Model.ItemSection.FluidsOnly)
                    {
                        Model.NewItem.Volume.MeasurementStateId = repoMeasurementStates.GetId_byState("Fluid");
                    }
                }

                //
                if (Model.ItemSection.Ownership)
                {
                    Model.LstMembers = repoMembers.GetList_byAccountId(Model.AccountId);
                }

                //
                if (Model.ItemSection.PowerRequirements)
                {
                    //Obtain needed data
                    Model.LstBatteryTypes = repoBatteryTypes.GetList();

                    //Set default values
                    if (Model.NewItem.PowerSource == null)
                    {
                        Model.NewItem.PowerSource = new PowerSource();
                    }
                    if (Model.NewItem.PowerSource.Battery == null)
                    {
                        Model.NewItem.PowerSource.Battery = new Battery();
                    }
                    Model.NewItem.PowerSource.RequiresAC = false;
                    Model.NewItem.PowerSource.RequiresBatteries = false;
                    Model.NewItem.PowerSource.RequiresFuel = false;
                    Model.NewItem.PowerSource.Battery.HasBatteries = false;
                }

                //
                if (Model.ItemSection.Seasonal)
                {
                    Model.LstSeasons = repoSeasons.GetList();
                }

                //
                if (Model.ItemSection.Toolsets) { }

                //
                if (Model.ItemSection.BugoutBags)
                {
                    //Obtain list of all tools
                    IPublishedContent ipDashboard = Umbraco.Content((int)(Common.SiteNode.Dashboard));
                    foreach (var ipChild in ipDashboard.Children)
                    {
                        if (ipChild.HasProperty(Common.NodeProperty.ShowInSideNav) && ipChild.Value<bool>(Common.NodeProperty.ShowInSideNav) == true)
                        {
                            if (ipChild.HasProperty(Common.NodeProperty.IsTool) && ipChild.Value<bool>(Common.NodeProperty.IsTool) == true && ipChild.Value<bool>(Common.NodeProperty.IsInventoryList) == true)
                            {
                                bl.Models.Link link = new Link();
                                link.Id = ipChild.Value<int>(Common.NodeProperty.ToolId);
                                link.Title = ipChild.Name;
                                if (ipChild.HasProperty(Common.NodeProperty.NavigationTitleOverride) && ipChild.HasValue(Common.NodeProperty.NavigationTitleOverride))
                                    link.Title = ipChild.Value<string>(Common.NodeProperty.NavigationTitleOverride);
                                link.Url = ipChild.Url();
                                link.Icon = ipChild.Value<string>(Common.NodeProperty.NavigationIcon);
                                Model.LstLinks.Add(link);
                            }
                        }
                    }

                    //Obtain list of all items for account
                    Model.LstItems = repoItems.GetList_byAccountId(Model.AccountId);

                    //Obtain list of bugout bags by account id
                    Model.LstBugoutLocations = repoLocations.GetBugoutBagList_byAccountId(Model.AccountId);

                    //Create list of all duplicate items
                    Model.LstDuplicateItems = repoItems.GetList_ofDuplicatesInBugoutBags_byAccountId(Model.AccountId);

                    //Obtain list of all categories
                    Model.LstCategories = repoCategories.GetList();
                }
                else
                {
                    //Obtain list of all items for this toolset
                    Model.LstCategoryTitles = repoItems.GetList_ofCategories_byAccountId_ToolId(Model.AccountId, ipModel.Value<int>(Common.NodeProperty.ToolId));
                    Model.LstItems = repoItems.GetList_byAccountId_ToolId(Model.AccountId, ipModel.Value<int>(Common.NodeProperty.ToolId));
                    Model.LstDuplicateItems = repoItems.GetList_ofDuplicates_byAccountId_ToolId(Model.AccountId, ipModel.Value<int>(Common.NodeProperty.ToolId));
                }


                //Save model in temp data
                TempData[Common.TempData.InventoryModel] = Model;
            }



            //Obtain data if updating record
            if (TempData[Common.TempData.UpdateId] != null)
            {
                Model.UpdateId = (int)TempData[Common.TempData.UpdateId];
                Model.UpdateItem = repoItems.GetRecord_byId(Model.UpdateId);
                Model.ShowUpdateRecord = true;
            }
        }
        private void ShowErrorMsgs(Exception ex = null)
        {
            if (ex == null)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                Response.Write("Modelstate is Invalid!!");
                foreach (var error in errors)
                {
                    Response.Write("<br />[" + error.ErrorMessage + "]");
                }
            }
            else
            {
                Response.Write("Error: <br />");
                Response.Write(ex.Message + "<br />");
                Response.Write(ex.InnerException);
            }
        }
        private string GetDataById(string Upc)
        {
            string uri = "https://api.edamam.com/api/food-database/v2/parser?";
            string appId = "bb8a881f";
            string appKey = "4a590b9293521fc4b0fd06853f5993ac";


            NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

            queryString.Add("app_id", appId);
            queryString.Add("app_key", appKey);
            queryString.Add("upc", Upc);

            string resourceUrl = uri + queryString.ToString();
            string data = "";


            using (var webClient = new System.Net.WebClient())
            {
                try
                {
                    data = webClient.DownloadString(resourceUrl);
                }
                catch //(Exception e)
                {
                    //data = e.ToString();
                }

                return data;
            }
        }
        #endregion
    }
}