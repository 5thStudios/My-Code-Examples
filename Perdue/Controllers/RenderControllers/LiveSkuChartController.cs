using Dragonfly.NetHelpers;
using Dragonfly.UmbracoServices;


using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Controllers;
using www.Models;
using www.Models.LiveSkuModel;
using www.Models.ProductTools;
using www.Models.PublishedModels;
using www.ViewModels;
using static System.Net.Mime.MediaTypeNames;
using ContentModels = www.Models.PublishedModels;


namespace www.Controllers
{
    public class LiveSkuChartController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<LiveSkuChartController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly Umbraco.Cms.Core.Hosting.IHostingEnvironment _hostingEnvironment;
        //private IOptions<OneWorldSync> OneWorldSync;

        public LiveSkuChartController(
                ILogger<LiveSkuChartController> _logger,
                ICompositeViewEngine compositeViewEngine,
                IUmbracoContextAccessor umbracoContextAccessor,
                UmbracoHelper _UmbracoHelper,
                IPublishedValueFallback publishedValueFallback,
                ServiceContext context,
                //IOptions<OneWorldSync> _oneWorldSync,
                IVariationContextAccessor variationContextAccessor,
                Umbraco.Cms.Core.Hosting.IHostingEnvironment hostingEnvironment
             )
            : base(_logger, compositeViewEngine, umbracoContextAccessor)
        {
            _publishedValueFallback = publishedValueFallback;
            UmbracoHelper = _UmbracoHelper;
            logger = _logger;
            //OneWorldSync = _oneWorldSync;
            _serviceContext = context;
            _variationContextAccessor = variationContextAccessor;
            _hostingEnvironment = hostingEnvironment;
        }

        public override IActionResult Index()
        {
            //
            var cmPage = new LiveSkuChart(CurrentPage, _publishedValueFallback);
            var vmLiveSkuChart = new LiveSkuChartViewModel();


            try
            {
                //Determine which site this is being called from.
                switch (cmPage.Root().Value<string>(Common.Property.Site))
                {
                    case Common.Site.Perdue:
                        vmLiveSkuChart.IsPerdue = true;
                        break;
                    case Common.Site.Coleman:
                        vmLiveSkuChart.IsColeman = true;
                        break;
                    case Common.Site.Cheney:
                        vmLiveSkuChart.IsCheney = true;
                        break;
                    default:
                        break;
                }

                //
                vmLiveSkuChart.BrandType = cmPage.Brand;


                //Obtain all skus by groups
                vmLiveSkuChart.SkuGroups = ObtainProductList_FilterBy(vmLiveSkuChart.BrandType);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }


            var viewModel = new ComposedPageViewModel<LiveSkuChart, LiveSkuChartViewModel>
            {
                Page = cmPage,
                ViewModel = vmLiveSkuChart
            };

            return View(Common.View.LiveSkuChart, viewModel);
        }




        public List<LiveSkuGroup> ObtainProductList_FilterBy(string FilterBy)
        {
            //Instantiate variables
            IPublishedContent? ipProductList = null;
            //FileHelperService FileHelper = new FileHelperService(_hostingEnvironment);
            //string? productJsonPath_blank = Common.Path.ApiFoodProductSavePath + "{0}.json";
            //string? productJsonPath = string.Format(productJsonPath_blank, cmPage.ApiReferenceId);


            if (FilterBy == "Coleman")
            {
                //Obtain Coleman products
                ipProductList = UmbracoHelper.ContentAtRoot()?.FirstOrDefault(x => x.ContentType.Alias == Common.Doctype.HomeCOL)?.ChildrenOfType(Common.Doctype.ProductsSection)?.FirstOrDefault();
            }
            else
            {
                //Obtain Perdue products
                ipProductList = UmbracoHelper.ContentAtRoot()?.FirstOrDefault(x => x.ContentType.Alias == Common.Doctype.Home)?.ChildrenOfType(Common.Doctype.ProductsSection)?.FirstOrDefault();
            }

            List<LiveSku> LstLiveSkus = new List<LiveSku>();
            List<LiveSkuGroup> SkuGroups = new List<LiveSkuGroup>();
            LiveSkuGroup _skuGroup = new LiveSkuGroup();
            string currentGroup = "";

            //  FILTER TYPES
            //======================================
            //	All
            //	Harvestland
            //	Perdue NAE
            //	Perdue
            //	Kings Delight
            //	Shenandoah

            //Obtain Product List Page
            foreach (IPublishedContent ipProduct in ipProductList.Children().Where(x => x.ContentType.Alias == Common.Doctype.Product))
            {
                ContentModels.Product cmProduct = new ContentModels.Product(ipProduct, _publishedValueFallback);
                try
                {
                    if (FilterBy == "All")
                    {
                        //Obtain sku record
                        LstLiveSkus.Add(GetSku(ipProduct));
                    }
                    else if (FilterBy == "Harvestland")
                    {
                        if (cmProduct.BrandName.ToLower().Contains(FilterBy.ToLower()) || cmProduct.Name.ToLower().Contains(FilterBy.ToLower()))
                        {
                            //Obtain sku record
                            LstLiveSkus.Add(GetSku(ipProduct));
                        }
                    }
                    else if (FilterBy == "Harvestland CVP")
                    {
                        if (cmProduct.BrandName.ToLower().Contains(FilterBy.ToLower()) || cmProduct.Name.ToLower().Contains("harvestland"))
                        {
                            //Obtain sku record
                            var record = GetSku(ipProduct, FilterBy);
                            if (record != null)
                            {
                                LstLiveSkus.Add(record);
                            }
                        }
                    }
                    else if (FilterBy == "Perdue")
                    {
                        if (cmProduct.BrandName.ToLower().Contains(FilterBy.ToLower()) || cmProduct.Name.ToLower().Contains(FilterBy.ToLower()))
                        {
                            if (!cmProduct.BrandName.ToLower().Contains("harvestland") && !cmProduct.Name.ToLower().Contains("harvestland"))
                            {
                                //Obtain sku record
                                LstLiveSkus.Add(GetSku(ipProduct));
                            }
                        }
                    }
                    else if (FilterBy == "Perdue NAE")
                    {
                        //Obtain sku record
                        LstLiveSkus.Add(GetSku(ipProduct));
                    }
                    else if (FilterBy == "Perdue NAE Turkey")
                    {
                        //Obtain sku record
                        LstLiveSkus.Add(GetSku(ipProduct, FilterBy));
                    }
                    else if (FilterBy == "Perdue NAE Chicken & Turkey")
                    {
                        //Obtain sku record
                        LstLiveSkus.Add(GetSku(ipProduct, FilterBy));
                    }
                    else if (FilterBy == "Halal")
                    {
                        if (cmProduct.Attributes.Any(x => x.Contains("Attributes-HalalCertified")))
                            LstLiveSkus.Add(GetSku(ipProduct));

                    }
                    else if (FilterBy == "Coleman Pork")
                    {
                        //Obtain sku record
                        //LstLiveSkus.Add(GetSku(ipProduct));
                    }
                    else if (FilterBy == "Coleman")
                    {
                        if (cmProduct.BrandName.ToLower().Contains(FilterBy.ToLower()) || cmProduct.Name.ToLower().Contains(FilterBy.ToLower()))
                        {
                            //Obtain sku record
                            LstLiveSkus.Add(GetSku(ipProduct));
                        }
                    }

                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex.ToString());
                }
            }


            //Organize list into groups
            if (FilterBy == "Perdue NAE")
            {
                //
                _skuGroup = new LiveSkuGroup();
                _skuGroup.GroupName = "PERDUE<sup>®</sup> CHICKEN";
                SkuGroups.Add(_skuGroup);

                //Create list of overrides
                List<string> lstOverrides_ConventionalChicken = new List<string>();
                lstOverrides_ConventionalChicken.Add("8003");

                foreach (LiveSku _LiveSku in LstLiveSkus.OrderByDescending(x => x.GroupName).ThenBy(x => x.ProductType).ThenBy(x => x.ProductDescription))
                {
                    //Sort and add only chicken products
                    if (lstOverrides_ConventionalChicken.Contains(_LiveSku.ProductCode))
                    {
                        //add override item
                        _skuGroup.LstLiveSkus.Add(_LiveSku);
                    }
                    else if (_LiveSku.GroupName == "CHICKEN")
                    {
                        if (_LiveSku.ProductDescription.Contains("PERDUE"))
                        {
                            if (!_LiveSku.ProductDescription.Contains("NO ANTIBIOTICS EVER"))
                            {
                                if (!_LiveSku.ProductDescription.Contains("HARVESTLAND"))
                                {
                                    if (!_LiveSku.ProductDescription.Contains("KINGS"))
                                    {
                                        if (!_LiveSku.ProductDescription.Contains("CHEF REDI"))
                                        {
                                            _skuGroup.LstLiveSkus.Add(_LiveSku);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                //
                _skuGroup = new LiveSkuGroup();
                _skuGroup.GroupName = "PERDUE<sup>®</sup> NO ANTIBIOTICS EVER CHICKEN";
                SkuGroups.Add(_skuGroup);

                //Create list of overrides
                List<string> lstOverrides_NAEChicken = new List<string>();
                lstOverrides_NAEChicken.Add("07003");
                lstOverrides_NAEChicken.Add("9712");

                foreach (LiveSku _LiveSku in LstLiveSkus.OrderByDescending(x => x.GroupName).ThenBy(x => x.ProductType).ThenBy(x => x.ProductDescription))
                {
                    //Sort and add only chicken products
                    if (lstOverrides_NAEChicken.Contains(_LiveSku.ProductCode))
                    {
                        //add override item
                        _skuGroup.LstLiveSkus.Add(_LiveSku);
                    }
                    else if (_LiveSku.GroupName == "CHICKEN")
                    {
                        if (_LiveSku.ProductDescription.Contains("PERDUE"))
                        {
                            if (_LiveSku.ProductDescription.Contains("NO ANTIBIOTICS EVER"))
                            {
                                if (!_LiveSku.ProductDescription.Contains("HARVESTLAND"))
                                {
                                    if (!_LiveSku.ProductDescription.Contains("KINGS"))
                                    {
                                        if (!_LiveSku.ProductDescription.Contains("CHEF REDI"))
                                        {
                                            _skuGroup.LstLiveSkus.Add(_LiveSku);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (FilterBy == "Perdue NAE Chicken & Turkey")
            {

                //Perdue NAE Turkey
                _skuGroup = new LiveSkuGroup();
                _skuGroup.GroupName = "PERDUE<sup>®</sup> NO ANTIBIOTICS EVER TURKEY";
                SkuGroups.Add(_skuGroup);
                foreach (LiveSku _LiveSku in LstLiveSkus.OrderByDescending(x => x.GroupName).ThenBy(x => x.ProductType).ThenBy(x => x.ProductDescription))
                {
                    //Sort and add only TURKEY products
                    if (_LiveSku.GroupName == "TURKEY")
                    {
                        if (_LiveSku.ProductDescription.Contains("PERDUE"))
                        {
                            if (_LiveSku.ProductDescription.Contains("NO ANTIBIOTICS EVER"))
                            {
                                if (!_LiveSku.ProductDescription.Contains("HARVESTLAND"))
                                {
                                    if (!_LiveSku.ProductDescription.Contains("KINGS"))
                                    {
                                        if (!_LiveSku.ProductDescription.Contains("CHEF REDI"))
                                        {
                                            _skuGroup.LstLiveSkus.Add(_LiveSku);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                //Perdue NAE Chicken
                _skuGroup = new LiveSkuGroup();
                _skuGroup.GroupName = "PERDUE<sup>®</sup> NO ANTIBIOTICS EVER CHICKEN";
                SkuGroups.Add(_skuGroup);
                //Create list of overrides
                List<string> lstOverrides_ChickenOnly = new List<string>();
                lstOverrides_ChickenOnly.Add("07003");
                lstOverrides_ChickenOnly.Add("9712");
                foreach (LiveSku _LiveSku in LstLiveSkus.OrderByDescending(x => x.GroupName).ThenBy(x => x.ProductType).ThenBy(x => x.ProductDescription))
                {
                    //Sort and add only chicken products
                    if (lstOverrides_ChickenOnly.Contains(_LiveSku.ProductCode))
                    {
                        //add override item
                        _skuGroup.LstLiveSkus.Add(_LiveSku);
                    }
                    else if (_LiveSku.GroupName == "CHICKEN")
                    {
                        if (_LiveSku.ProductDescription.Contains("PERDUE"))
                        {
                            if (_LiveSku.ProductDescription.Contains("NO ANTIBIOTICS EVER"))
                            {
                                if (!_LiveSku.ProductDescription.Contains("HARVESTLAND"))
                                {
                                    if (!_LiveSku.ProductDescription.Contains("KINGS"))
                                    {
                                        if (!_LiveSku.ProductDescription.Contains("CHEF REDI"))
                                        {
                                            _skuGroup.LstLiveSkus.Add(_LiveSku);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (FilterBy == "Perdue NAE Turkey")
            {
                //Perdue NAE Turkey
                _skuGroup = new LiveSkuGroup();
                _skuGroup.GroupName = "PERDUE<sup>®</sup> NO ANTIBIOTICS EVER TURKEY";
                SkuGroups.Add(_skuGroup);
                //Create list of overrides
                List<string> lstOverrides_Turkey = new List<string>();
                lstOverrides_Turkey.Add("75142");
                foreach (LiveSku _LiveSku in LstLiveSkus.OrderByDescending(x => x.GroupName).ThenBy(x => x.ProductType).ThenBy(x => x.ProductDescription))
                {
                    //Sort and add only TURKEY products
                    if (lstOverrides_Turkey.Contains(_LiveSku.ProductCode))
                    {
                        //add override item
                        _skuGroup.LstLiveSkus.Add(_LiveSku);
                    }
                    else if (_LiveSku.GroupName == "TURKEY")
                    {
                        if (_LiveSku.ProductDescription.Contains("PERDUE"))
                        {
                            if (_LiveSku.ProductDescription.Contains("NO ANTIBIOTICS EVER"))
                            {
                                if (!_LiveSku.ProductDescription.Contains("HARVESTLAND"))
                                {
                                    if (!_LiveSku.ProductDescription.Contains("KINGS"))
                                    {
                                        if (!_LiveSku.ProductDescription.Contains("CHEF REDI"))
                                        {
                                            _skuGroup.LstLiveSkus.Add(_LiveSku);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                //Organize list into groups
                foreach (LiveSku _LiveSku in LstLiveSkus.OrderByDescending(x => x.GroupName).ThenBy(x => x.ProductType).ThenBy(x => x.ProductDescription))
                {
                    //Sort and add all
                    if (currentGroup != _LiveSku.GroupName)
                    {
                        currentGroup = _LiveSku.GroupName;
                        _skuGroup = new LiveSkuGroup();
                        _skuGroup.GroupName = currentGroup;
                        SkuGroups.Add(_skuGroup);
                    }
                    _skuGroup.LstLiveSkus.Add(_LiveSku);
                }
            }

            //Return list of links
            //return Newtonsoft.Json.JsonConvert.SerializeObject(SkuGroups);
            return SkuGroups;
        }
        private LiveSku GetSku(IPublishedContent ipProduct, string FilterBy = "")
        {
            //
            ContentModels.Product cmProduct = new ContentModels.Product(ipProduct, _publishedValueFallback);

            //Create new sku object
            LiveSku sku = new LiveSku();

            //Obtain data from umbraco node
            sku.Id = cmProduct.Id;
            //sku.ApiReferenceId = ipChild.Value<string>("ApiReferenceId");
            sku.ProductCode = cmProduct.ProductCode;
            sku.BrandName = cmProduct.BrandName;
            sku.Url = cmProduct.Url();

            //Pull data from json file and convert into an object
            //string fileLocation = OneWorldSync.Value.DownloadIndividualProducts.Replace("[ID]", sku.ApiReferenceId);
            //StreamReader sr = System.IO.File.OpenText(fileLocation);
            //string strContents = sr.ReadToEnd();
            //sr.Close();


            //Obtain product json file
            //FileHelperService FileHelper = new FileHelperService(_hostingEnvironment);
            //string? productJsonPath_blank = Common.Path.ApiResponseSavePath + "{0}.json";
            //string? productJsonPath = string.Format(productJsonPath_blank, sku.ApiReferenceId);


            //IndividualProduct product = new IndividualProduct(); // Newtonsoft.Json.JsonConvert.DeserializeObject<IndividualProduct>(strContents);
            try
            {
                //var product = Newtonsoft.Json.JsonConvert.DeserializeObject<www.Models.ApiResponse.Response>(FileHelper.GetTextFileContents(productJsonPath));

                //Convert content to object
                //product = Newtonsoft.Json.JsonConvert.DeserializeObject<IndividualProduct>(strContents);

                //CVP SEARCH ONLY!!!  [If not cvp then skip.]
                if (FilterBy == "Harvestland CVP")
                {
                    if (cmProduct.TradeItemKeywords.FirstOrDefault() != "Fresh Chicken")
                        return null;


                    if (cmProduct.TradeItemKeywords.Any(x => x.Contains("CVP")))
                        return null;
                }


                sku.AverageCaseCount = cmProduct.ServingsPerCase;
                sku.AverageSize = cmProduct.AveragePieceSize;
                string FixedName = FixBadChars(cmProduct.Title);
                sku.ProductDescription = FixedName.Replace("®", "<sup>®</sup>");
                sku.GroupName = cmProduct.FunctionalName;
                sku.AverageWeight = cmProduct.Weight;
                sku.ProductType = cmProduct.TradeItemKeywords.FirstOrDefault();

                if (cmProduct.TradeItemKeywords.Count() > 1)
                {
                    sku.ProductSubtype = cmProduct.TradeItemKeywords.ToList()[1];
                }

                //Extract data from json object
                //sku.AverageCaseCount = product.results.FirstOrDefault().item.tradeItemInformation.FirstOrDefault().foodAndBeveragePreparationServingModule.FirstOrDefault().servingQuantityInformation.FirstOrDefault().numberOfServingsPerPackage.ToString();
                //sku.AverageSize = product.results.FirstOrDefault().item.tradeItemInformation.FirstOrDefault().nutritionalInformationModule.FirstOrDefault().nutrientHeader.FirstOrDefault().servingSizeDescription.values.FirstOrDefault().value.ToLower();
                //sku.AverageWeight = product.results.FirstOrDefault().item.tradeItemInformation.FirstOrDefault().tradeItemMeasurementsModuleGroup.FirstOrDefault().tradeItemMeasurementsModule.tradeItemMeasurements.tradeItemWeight.netWeight.value.ToLower();

                //string FixedName = FixBadChars(product.results.FirstOrDefault().item.tradeItemInformation.FirstOrDefault().tradeItemDescriptionModule.tradeItemDescriptionInformation.FirstOrDefault().additionalTradeItemDescription.values.FirstOrDefault().value);
                //sku.ProductDescription = FixedName.Replace("®", "<sup>®</sup>");
                //sku.GroupName = product.results.FirstOrDefault().item.tradeItemInformation.FirstOrDefault().tradeItemDescriptionModule.tradeItemDescriptionInformation.FirstOrDefault().functionalName.values.FirstOrDefault().value;
                //sku.ProductType = product.results.FirstOrDefault().item.tradeItemInformation.FirstOrDefault().marketingInformationModuleGroup.FirstOrDefault().marketingInformationModule.marketingInformation.tradeItemKeyWords.values.FirstOrDefault().value;

                //if (product.results.FirstOrDefault().item.tradeItemInformation.FirstOrDefault().marketingInformationModuleGroup.FirstOrDefault().marketingInformationModule.marketingInformation.tradeItemKeyWords.values.Count() > 1)
                //    sku.ProductSubtype = product.results.FirstOrDefault().item.tradeItemInformation.FirstOrDefault().marketingInformationModuleGroup.FirstOrDefault().marketingInformationModule.marketingInformation.tradeItemKeyWords.values[1].value;

                //string StorageMethod = product.results.FirstOrDefault().item.tradeItemInformation.FirstOrDefault().consumerInstructionsModule.FirstOrDefault().consumerInstructions.consumerStorageInstructions.values.FirstOrDefault().value.ToLower();

                //Obtain CVP Subtypes
                if (cmProduct.TradeItemKeywords.Any(x => x.Contains("CVP")))
                {
                    foreach (var _item in cmProduct.TradeItemKeywords.Where(x => x.Contains("CVP")))
                    {
                        if (_item != "CVP")
                            sku.LstCVPSubtypes.Add(_item.Replace("CVP", "").Trim());
                    }
                }

                //if (product.results.FirstOrDefault().item.tradeItemInformation.FirstOrDefault().marketingInformationModuleGroup.FirstOrDefault().marketingInformationModule.marketingInformation.tradeItemKeyWords.values.Where(x => x.value.Contains("CVP")).Any())
                //{
                //    foreach (var _item in product.results.FirstOrDefault().item.tradeItemInformation.FirstOrDefault().marketingInformationModuleGroup.FirstOrDefault().marketingInformationModule.marketingInformation.tradeItemKeyWords.values.Where(x => x.value.Contains("CVP")))
                //    {
                //        if (_item.value != "CVP")
                //            sku.LstCVPSubtypes.Add(_item.value.Replace("CVP", "").Trim());
                //    }
                //}


                //Obtain serving size and replace with average size if not provided
                if (!string.IsNullOrWhiteSpace(cmProduct.ServingSizeDescription))
                {
                    //sku.ServingSize = product.results.FirstOrDefault().item.tradeItemInformation.FirstOrDefault().tradeItemSizeModule.nonPackagedSizeDimension.FirstOrDefault().descriptiveSizeDimension.values.FirstOrDefault().value;
                    sku.ServingSize = cmProduct.ServingSizeDescription;
                }
                if (string.IsNullOrWhiteSpace(sku.ServingSize))
                {
                    sku.ServingSize = sku.AverageSize;
                }




                //Determine attributes
                //==========================================================
                //If field "Storage Method" includes BOTH "frozen" and "refrigerated" AND the Product Name includes "frozen", use "Frozen", otherwise use "Fresh"
                if (cmProduct.StorageMethod.ToLower().Contains("frozen") & cmProduct.StorageMethod.ToLower().Contains("refrigerated"))
                {
                    if (FixedName.ToLower().Contains("frozen"))
                    {
                        sku.Attribute = "Frozen";
                    }
                    else
                    {
                        sku.Attribute = "Fresh";
                    }
                }
                else
                {
                    //If field "Storage Method" includes "frozen" or "0 degrees", then "Frozen"
                    //If field "Storage Method" includes "refrigerated" or "34 degrees", then "Fresh"
                    if (cmProduct.StorageMethod.ToLower().Contains("frozen"))
                    {
                        sku.Attribute = "Frozen";
                    }
                    else if (cmProduct.StorageMethod.ToLower().Contains("0 degrees"))
                    {
                        sku.Attribute = "Frozen";
                    }
                    else if (cmProduct.StorageMethod.ToLower().Contains("refrigerated"))
                    {
                        sku.Attribute = "Fresh";
                    }
                    else if (cmProduct.StorageMethod.ToLower().Contains("34 degrees"))
                    {
                        sku.Attribute = "Fresh";
                    }
                }


                //JF- Update by client
                if (sku.ProductType == "Fresh Chicken")
                {
                    sku.ProductType = sku.ProductType + " (CVP products limited distribution)";
                }


                sku.Form = "";
            }
            catch (Exception ex)
            {
                //Display error
                sku.ProductDescription = ex.Message;
            }

            return sku;
        }
        private string FixBadChars(string TextToFix)
        {
            var returnText = TextToFix;

            var badChars = new Dictionary<string, string>()
            {
                {"(R)", "®"},
                {"â‚¬", "€"},
                {"â„¢", "™"},
                {"Â„¢", "™"},
                {"â€¡", "‡"},
                {"â€¦", "…"},
                {"â€˜", "‘"},
                {"â€“", "–"},
                {"â€”", "—"},
                {"â€¢", "•"},
                {"â€°", "‰"},
                {"â€¹", "‹"},
                {"â€º", "›"},
                {"â€œ", "“"},
                {"â€š", "‚"},
                {"â€™", "’"},
                {"â€ž", "„"},
                {"Â­", "­"},
                {"Ã­", "í"},
                {"Ã–", "Ö"},
                {"Ã—", "×"},
                {"Ãˆ", "È"},
                {"Â¡", "¡"},
                {"Ã¡", "á"},
                {"Å¡", "š"},
                {"Â¦", "¦"},
                {"Ã¦", "æ"},
                {"Â¨", "¨"},
                {"Ã¨", "è"},
                {"Â¯", "¯"},
                {"Ã¯", "ï"},
                {"Â´", "´"},
                {"Ã´", "ô"},
                {"Â¸", "¸"},
                {"Ã¸", "ø"},
                {"Å¸", "Ÿ"},
                {"Â¿", "¿"},
                {"Ã¿", "ÿ"},
                {"Ã˜", "Ø"},
                {"Ã‘", "Ñ"},
                {"Ã’", "Ò"},
                {"Å’", "Œ"},
                {"Ã‚", "Â"},
                {"Ã“", "Ó"},
                {"Å“", "œ"},
                {"Ã”", "Ô"},
                {"Ã„", "Ä"},
                {"Ã‹", "Ë"},
                {"Ã›", "Û"},
                {"Â¢", "¢"},
                {"Ã¢", "â"},
                {"Â£", "£"},
                {"Ã£", "ã"},
                {"Â¤", "¤"},
                {"Ã¤", "ä"},
                {"Â¥", "¥"},
                {"Ã¥", "å"},
                {"â€", "†"},
                {"Ã€", "À"},
                {"Â±", "±"},
                {"Ã±", "ñ"},
                {"Â«", "«"},
                {"Ã«", "ë"},
                {"Â»", "»"},
                {"Ã»", "û"},
                {"Â§", "§"},
                {"Ã§", "ç"},
                {"Â©", "©"},
                {"Ã©", "é"},
                {"Â¬", "¬"},
                {"Ã¬", "ì"},
                {"Â®", "®"},
                {"â®", "®"},
                {"Ã®", "î"},
                {"Â°", "°"},
                {"Ã°", "ð"},
                {"Âµ", "µ"},
                {"Ãµ", "õ"},
                {"Â¶", "¶"},
                {"Ã¶", "ö"},
                {"Â·", "·"},
                {"Ã·", "÷"},
                {"Ã…", "Å"},
                {"Ã†", "Æ"},
                {"Ã‡", "Ç"},
                {"Ã•", "Õ"},
                {"Ã‰", "É"},
                {"Â¼", "¼"},
                {"Ã¼", "ü"},
                {"Â½", "½"},
                {"Ã½", "ý"},
                {"Å½", "Ž"},
                {"Â¾", "¾"},
                {"Ã¾", "þ"},
                {"Å¾", "ž"},
                {"Â¹", "¹"},
                {"Ã¹", "ù"},
                {"Â²", "²"},
                {"Ã²", "ò"},
                {"Â³", "³"},
                {"Ã³", "ó"},
                {"Âª", "ª"},
                {"Ãª", "ê"},
                {"Æ’", "ƒ"},
                {"Ãƒ", "Ã"},
                {"Âº", "º"},
                {"Ãº", "ú"},
                {"ÃŒ", "Ì"},
                {"Ãœ", "Ü"},
                {"ÃŠ", "Ê"},
                {"Ãš", "Ú"},
                {"Ã™", "Ù"},
                {"ÃŸ", "ß"},
                {"ÃŽ", "Î"},
                {"Ãž", "Þ"},
                {"Ë†", "ˆ"},
                {"Ëœ", "˜"},
                {"Â", ""},
                {"Ã", "Á"},
                {"Å", "Š"}
            };

            foreach (var badChar in badChars)
            {
                returnText = returnText.Replace(badChar.Key, badChar.Value);
            }

            return returnText;
        }


    }
}
