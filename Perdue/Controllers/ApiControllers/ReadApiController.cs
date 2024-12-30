using Dragonfly.NetHelpers;
using Dragonfly.NetModels;
using Dragonfly.UmbracoServices;
using Examine;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Controllers;
using www.Models;
using www.ReadApi;
using static Umbraco.Cms.Core.Constants;
using ContentModels = www.Models.PublishedModels;



namespace www.Controllers
{
    public class ReadApiController : UmbracoApiController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private UmbracoHelper UmbracoHelper;
        private IContentService _ContentService;
        private ILogger<ReadApiController> logger;
        private IExamineManager ExamineManager;
        private readonly IPublishedContentQuery publishedContentQuery;
        private Umbraco.Cms.Core.Hosting.IHostingEnvironment _hostingEnvironment;


        public ReadApiController(
                ILogger<ReadApiController> _logger,
                UmbracoHelper _UmbracoHelper,
                IPublishedValueFallback publishedValueFallback,
                IContentService ContentService,
                IExamineManager _ExamineManager,
                IPublishedContentQuery _publishedContentQuery,
                Umbraco.Cms.Core.Hosting.IHostingEnvironment hostingEnvironment
             )
        {
            _publishedValueFallback = publishedValueFallback;
            UmbracoHelper = _UmbracoHelper;
            logger = _logger;
            _ContentService = ContentService;
            _hostingEnvironment = hostingEnvironment;
            publishedContentQuery = _publishedContentQuery;
            ExamineManager = _ExamineManager ?? throw new ArgumentNullException(nameof(_ExamineManager));
        }


        //Scope variables
        StatusMessage masterMsg = new StatusMessage();
        StatusMessage productCountStatusMsg = new StatusMessage();
        StatusMessage obtainProductsStatusMsg = new StatusMessage();
        StatusMessage importProductItemStatusMsg = new StatusMessage();
        StatusMessage saveDataToProductStatusMsg = new StatusMessage();
        StatusMessage updateFilterAttributesStatusMsg = new StatusMessage();
        StatusMessage updateCategoriesStatusMsg = new StatusMessage();
        StatusMessage deleteUnneededProductsStatusMsg = new StatusMessage();
        List<int> LstNodesToDelete = new List<int>();
        int TotalProductCountBeforeImport = 0;
        int TotalProductsFromAPI = 0;
        int PerdueProductsImported = 0;
        int CheneyProductsImported = 0;
        int ColemanProductsImported = 0;
        int ProductsDeleted = 0;
        int ImportFails = 0;



        #region API Calls  
        //      https://localhost:44369/umbraco/api/ReadApi/CallApiToUpdateProducts
        [HttpGet]
        public ContentResult CallApiToUpdateProducts(string refreshAll = "")
        {
            //logger.LogWarning("CallApiToUpdateProducts(" + refreshAll + ")");

            //Called via href link
            return ImportProductsFromApi(refreshAll);
        }
        //      https://localhost:44369/umbraco/api/ReadApi/CallApiToUpdateSingleProducts
        [HttpPost]
        public ContentResult CallApiToUpdateSingleProducts(string updateProductId = "")
        {
            //logger.LogWarning("CallApiToUpdateSingleProducts(" + updateProductId + ")");

            //Called via jquery
            return ImportProductsFromApi("true", updateProductId);
        }
        #endregion



        #region "Methods"
        private ContentResult ImportProductsFromApi(string refreshAll = "", string updateProductId = "")
        {
            HttpStatusCode statusCode = HttpStatusCode.OK;


            try
            {
                //  VARIABLES
                bool RefreshAll = (refreshAll == "true");
                bool UpdateSingleProduct = !string.IsNullOrWhiteSpace(updateProductId.Trim());
                masterMsg.TimestampStart = DateTime.Now;
                masterMsg.RunningFunctionName = "ReadApiController.cs | ImportProductsFromApi()";
                masterMsg.Success = true;
                masterMsg.Code = "200";

                ApiCredentials apiCredentials = new ApiCredentials();
                List<ReadApi.Item2> lstItemRecords = new List<ReadApi.Item2>();


                //  OBTAIN CREDENTIALS
                apiCredentials = ObtainCredentials();


                //  OBTAIN PRODUCT COUNT
                TotalProductsFromAPI = ObtainProductCountFromAPI(apiCredentials);


                //  OBTAIN ALL PRODUCTS
                lstItemRecords = ObtainProductsFromAPI(apiCredentials, TotalProductsFromAPI);


                //  OBTAIN IDs FOR ALL EXISTING PRODUCTS
                ObtainAllExistingNodeIDs();
                TotalProductCountBeforeImport = LstNodesToDelete.Count();


                //  INSTANTIATE IMPORT/SAVE STATUS'
                importProductItemStatusMsg.TimestampStart = DateTime.Now;
                importProductItemStatusMsg.RunningFunctionName = "ReadApiController.cs | ImportProductItem()";
                importProductItemStatusMsg.Success = true;
                importProductItemStatusMsg.Code = "200";

                saveDataToProductStatusMsg.TimestampStart = DateTime.Now;
                saveDataToProductStatusMsg.RunningFunctionName = "ReadApiController.cs | SaveDataToProduct()";
                saveDataToProductStatusMsg.Success = true;
                saveDataToProductStatusMsg.Code = "200";


                //  UPDATE PRODUCT(S)
                foreach (var record in lstItemRecords)
                {
                    //If single update is submitted, then skip all others.
                    if (UpdateSingleProduct)
                    {
                        List<AgencyId> lstAgencyIds = ExtractProductIDs(record);
                        foreach (var productCode in lstAgencyIds)
                        {
                            if (updateProductId.Trim() != productCode.Id)
                            {
                                continue;
                            }
                            ImportProductItem(record, RefreshAll);
                        }
                    }
                    else
                    {
                        ImportProductItem(record, RefreshAll);
                    }
                }


                //  CLOSE IMPORT/SAVE STATUS'
                importProductItemStatusMsg.TimestampEnd = DateTime.Now;
                masterMsg.InnerStatuses.Add(importProductItemStatusMsg);

                saveDataToProductStatusMsg.TimestampEnd = DateTime.Now;
                masterMsg.InnerStatuses.Add(saveDataToProductStatusMsg);


                //UPDATE FILTER ATTRIBUTES AND CATEGORIES
                updateFilterAttributesStatusMsg = UpdateFilterAttributes();
                masterMsg.InnerStatuses.Add(updateFilterAttributesStatusMsg);

                updateCategoriesStatusMsg = UpdateCategories();
                masterMsg.InnerStatuses.Add(updateCategoriesStatusMsg);



                //   DELETE UNLISTED PRODUCTS [EXCEPT IF WE ARE UPDATING A SPECIFIC PRODUCT.]
                if (!UpdateSingleProduct)
                {
                    deleteUnneededProductsStatusMsg.TimestampStart = DateTime.Now;
                    importProductItemStatusMsg.RunningFunctionName = "ReadApiController.cs | DeleteRemainingNodes()";
                    importProductItemStatusMsg.Success = true;
                    importProductItemStatusMsg.Code = "200";

                    logger.LogWarning("Deleting the following Product IDs: " + Newtonsoft.Json.JsonConvert.SerializeObject(LstNodesToDelete));
                    DeleteRemainingNodes();

                    deleteUnneededProductsStatusMsg.TimestampEnd = DateTime.Now;
                    masterMsg.InnerStatuses.Add(deleteUnneededProductsStatusMsg);
                }



                //  END MASTER MESSAGE 
                masterMsg.Message = "API Import Complete";
                masterMsg.TimestampEnd = DateTime.Now;

            }
            catch (Exception ex)
            {
                //  Log error
                logger.LogError(ex, ex.Message);


                //Record exception to show user
                masterMsg.Success = false;
                masterMsg.Code = "500";
                masterMsg.Message = ex.Message;
                masterMsg.TimestampEnd = DateTime.Now;


                //  Set return status
                statusCode = HttpStatusCode.InternalServerError;
            }



            //Add result totals to api message.
            int totalImported = PerdueProductsImported + CheneyProductsImported + ColemanProductsImported;
            masterMsg.DetailedMessages.Add($"{TotalProductCountBeforeImport} total products before import.");
            masterMsg.DetailedMessages.Add($"{TotalProductsFromAPI} total products from api.");
            masterMsg.DetailedMessages.Add($"========================================");
            masterMsg.DetailedMessages.Add($"{totalImported} total products imported.");
            masterMsg.DetailedMessages.Add($"{PerdueProductsImported} total products import into Perdue.");
            masterMsg.DetailedMessages.Add($"{CheneyProductsImported} total products import into Cheney.");
            masterMsg.DetailedMessages.Add($"{ColemanProductsImported} total products import into Coleman.");
            masterMsg.DetailedMessages.Add($"========================================");
            masterMsg.DetailedMessages.Add($"{ProductsDeleted} products deleted.");
            masterMsg.DetailedMessages.Add($"{ImportFails} failed imports.");

            //  Return error message
            return new ContentResult
            {
                ContentType = Common.Misc.ApplicationJson,
                Content = Newtonsoft.Json.JsonConvert.SerializeObject(masterMsg),
                StatusCode = (int)statusCode
            };
        }
        private ApiCredentials ObtainCredentials(bool ISPREPROD = false)
        {
            //
            ApiCredentials apiCredentials = new ApiCredentials();

            //
            if (ISPREPROD)
            {
                apiCredentials.AppID = Common.ContentOne_PreProd.AppId;
                apiCredentials.SecretKey = Common.ContentOne_PreProd.SecretKey;
                apiCredentials.UserGLN = Common.ContentOne_PreProd.GLN;
                apiCredentials.BaseAddress = Common.ContentOne_PreProd.BaseUrl;
            }
            else
            {
                apiCredentials.AppID = Common.ContentOne_Live.AppId;
                apiCredentials.SecretKey = Common.ContentOne_Live.SecretKey;
                apiCredentials.UserGLN = Common.ContentOne_Live.GLN;
                apiCredentials.BaseAddress = Common.ContentOne_Live.BaseUrl;
            }

            //
            return apiCredentials;
        }
        private int ObtainProductCountFromAPI(ApiCredentials apiCredentials)
        {
            //
            Encoding encoding = Encoding.UTF8;
            int CurrentProductCount = 0;
            productCountStatusMsg.TimestampStart = DateTime.Now;
            productCountStatusMsg.RunningFunctionName = "ReadApiController.cs | ObtainProductCount()";
            productCountStatusMsg.Success = true;
            productCountStatusMsg.Code = "200";


            try
            {
                using (var httpClient = new HttpClient())
                {
                    //
                    string timestamp = DateTime.UtcNow.ToString(Common.Misc.TimeStampPattern);
                    String uriCount = Common.ContentOne_Live.uriCount + timestamp;


                    // Calculate the HMACSHA256 hash
                    httpClient.BaseAddress = new Uri(apiCredentials.BaseAddress);
                    httpClient.DefaultRequestHeaders.Add(Common.Misc.AppId, apiCredentials.AppID);
                    using (var hmac = new HMACSHA256(encoding.GetBytes(apiCredentials.SecretKey)))
                    {
                        byte[]? hashBytes = hmac.ComputeHash(encoding.GetBytes(uriCount));
                        string? hashInBase64 = Convert.ToBase64String(hashBytes);
                        httpClient.DefaultRequestHeaders.Add(Common.Misc.Hashcode, hashInBase64);
                    }



                    // Create a JSON content with an empty body
                    Dictionary<string, string> dictApiAttributes = new Dictionary<string, string>();
                    dictApiAttributes.Add(Common.Misc.TargetMarket, Common.Misc.US);
                    dictApiAttributes.Add(Common.Misc.UserGLN, apiCredentials.UserGLN);
                    string jsonApiAttributes = Newtonsoft.Json.JsonConvert.SerializeObject(dictApiAttributes);
                    StringContent content = new StringContent(jsonApiAttributes, encoding, Common.Misc.ApplicationJson);


                    //Connect to api
                    HttpResponseMessage response = httpClient.PostAsync(uriCount, content).GetAwaiter().GetResult();
                    if (response.IsSuccessStatusCode)
                    {
                        //Obtain data from API 
                        Task<string>? taskString = response.Content.ReadAsStringAsync();

                        //Convert API result to Model
                        ProductCount? productCount = JsonConvert.DeserializeObject<ProductCount?>(taskString?.GetAwaiter().GetResult() ?? "");
                        CurrentProductCount = productCount?.count ?? 0;
                        productCountStatusMsg.DetailedMessages.Add($"Product Count: {CurrentProductCount}");
                    }
                    else
                    {
                        productCountStatusMsg.DetailedMessages.Add($"HTTP Error: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
            }
            catch (Exception ex)
            {
                productCountStatusMsg.DetailedMessages.Add($"Product Count | Error: {ex.Message}");
                productCountStatusMsg.Success = false;
                productCountStatusMsg.Code = "500";
                masterMsg.Success = false;
                masterMsg.Code = "500";

                //  Log error
                logger.LogError(ex, ex.Message);
            }


            //
            productCountStatusMsg.TimestampEnd = DateTime.Now;
            masterMsg.InnerStatuses.Add(productCountStatusMsg);
            return CurrentProductCount;
        }
        private List<ReadApi.Item2> ObtainProductsFromAPI(ApiCredentials apiCredentials, int CurrentProductCount)
        {
            obtainProductsStatusMsg.TimestampStart = DateTime.Now;
            obtainProductsStatusMsg.RunningFunctionName = "ReadApiController.cs | ObtainProducts()";
            obtainProductsStatusMsg.Success = true;
            obtainProductsStatusMsg.Code = "200";

            int processedCount = 0;
            Encoding encoding = Encoding.UTF8;
            List<ReadApi.Item2> LstItemRecords = new List<ReadApi.Item2>();
            Dictionary<string, string>? dictSearchAfter = null;


            try
            {
                int maxToGet = 100;  //Max cannot exceed 1000

                while (CurrentProductCount > 0)
                {
                    //Create fetch uri
                    int pageSize = 0;
                    if (CurrentProductCount > maxToGet)
                        pageSize = maxToGet;
                    else
                        pageSize = CurrentProductCount;

                    string timestamp = DateTime.UtcNow.ToString(Common.Misc.TimeStampPattern);
                    string uriFetch = Common.ContentOne_Live.uriFetch.Replace("~TIMESTAMP~", timestamp).Replace("~PAGESIZE~", pageSize.ToString());


                    //Create new http client to pull data from api
                    using (var httpClient = new HttpClient())
                    {
                        // Calculate the HMACSHA256 hash
                        httpClient.BaseAddress = new Uri(apiCredentials.BaseAddress);
                        httpClient.DefaultRequestHeaders.Add(Common.Misc.AppId, apiCredentials.AppID);
                        using (var hmac = new HMACSHA256(encoding.GetBytes(apiCredentials.SecretKey)))
                        {
                            byte[]? hashBytes = hmac.ComputeHash(encoding.GetBytes(uriFetch));
                            string? hashInBase64 = Convert.ToBase64String(hashBytes);
                            httpClient.DefaultRequestHeaders.Add(Common.Misc.Hashcode, hashInBase64);
                        }


                        // Create a JSON content with an empty body
                        Dictionary<string, string> dictApiAttributes = new Dictionary<string, string>();
                        dictApiAttributes.Add(Common.Misc.TargetMarket, Common.Misc.US);
                        dictApiAttributes.Add(Common.Misc.UserGLN, apiCredentials.UserGLN);


                        //Add placeholder if not null
                        if (dictSearchAfter != null)
                        {
                            dictApiAttributes.Add("searchAfter", "~searchAfter~");
                        }


                        //Stringify attributes
                        string jsonApiAttributes = Newtonsoft.Json.JsonConvert.SerializeObject(dictApiAttributes);


                        //Replace placeholder string with data
                        if (dictSearchAfter != null)
                        {
                            jsonApiAttributes = jsonApiAttributes.Replace("\"~searchAfter~\"", string.Format("[{0},\"{1}\"]", dictSearchAfter.FirstOrDefault().Key, dictSearchAfter.FirstOrDefault().Value));
                            dictSearchAfter = null;
                        }


                        //Create encoded content string
                        StringContent content = new StringContent(jsonApiAttributes, encoding, Common.Misc.ApplicationJson);


                        //Connect to api
                        HttpResponseMessage response = httpClient.PostAsync(uriFetch, content).GetAwaiter().GetResult();



                        if (response.IsSuccessStatusCode)
                        {
                            //Obtain data from API and convert to Model
                            Task<string>? taskString = response.Content.ReadAsStringAsync();
                            string? result = taskString?.GetAwaiter().GetResult();


                            //Save json data to file. [FOR TESTING ONLY]
                            string _path = Common.Path.ApiResponseSavePath + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss_") + CurrentProductCount.ToString() + ".json";
                            FileHelperService fileHelper = new FileHelperService(_hostingEnvironment);
                            fileHelper.CreateTextFile(_path, FormatJson(result));



                            //Loop through list of items and store website products in list
                            www.ReadApi.Result apiResult = JsonConvert.DeserializeObject<www.ReadApi.Result>(result);
                            foreach (var _item in apiResult.Items)
                            {
                                if (_item.Item2 != null)
                                {
                                    if (_item.Item2.AlternateItemIdentification != null)
                                    {
                                        if (_item.Item2.AlternateItemIdentification.Any(x => x.Agency == "I01" && (x.Id == "PER" || x.Id.Contains("CHE") || x.Id.Contains("COL"))))
                                        {
                                            LstItemRecords.Add(_item.Item2);
                                        }
                                    }
                                }
                            }


                            //Get next pg data to pull from api
                            if (apiResult.SearchAfter != null && apiResult.SearchAfter.Count > 0)
                            {
                                dictSearchAfter = new Dictionary<string, string>();
                                dictSearchAfter.Add(apiResult?.SearchAfter?.FirstOrDefault()?.ToString() ?? "", apiResult?.SearchAfter?.LastOrDefault()?.ToString() ?? "");
                            }
                        }
                        else
                        {
                            obtainProductsStatusMsg.DetailedMessages.Add($"ObtainProducts() | HTTP Error: {response.StatusCode} - {response.ReasonPhrase}");
                        }
                    }


                    //Decrease index of products
                    CurrentProductCount -= maxToGet;
                    processedCount++;
                }
            }
            catch (Exception ex)
            {
                obtainProductsStatusMsg.DetailedMessages.Add($"ObtainProducts() | Error: {ex.Message}");
                obtainProductsStatusMsg.Success = false;
                obtainProductsStatusMsg.Code = "500";
                masterMsg.Success = false;
                masterMsg.Code = "500";

                //  Log error
                logger.LogError(ex, ex.Message);
            }


            //
            obtainProductsStatusMsg.Message = $"ObtainProducts() | Complete";
            obtainProductsStatusMsg.TimestampEnd = DateTime.Now;
            masterMsg.InnerStatuses.Add(obtainProductsStatusMsg);


            return LstItemRecords;
        }


        private void ImportProductItem(ReadApi.Item2 Item, bool refreshAll)
        {
            try
            {
                //Extract product IDs and site the Id belongs to.
                List<AgencyId> lstAgencyIds = ExtractProductIDs(Item);


                if (ExamineManager.TryGetIndex(UmbracoIndexes.ExternalIndexName, out IIndex? index))
                {
                    foreach (var ProductCode in lstAgencyIds)
                    {
                        //Query Examine
                        var queryExecutor = index.Searcher
                            .CreateQuery(IndexTypes.Content)
                            .NodeTypeAlias(Common.Doctype.Product)
                            .And().Field(Common.Property.ProductCode, ProductCode.Id);
                        List<PublishedSearchResult> lstSearchResults = publishedContentQuery.Search(queryExecutor).ToList();


                        //Obtain product name/id   
                        var title = Item?.AdditionalDescription?.FirstOrDefault()?.Value ?? "";
                        var nodeName = string.Format("{0} - {1}", ProductCode.Id, title);


                        //Create IContent
                        IContent? icProduct = null;
                        if (lstSearchResults.Count == 0)
                        {
                            //No results exist.  Create new IContent product record with product name/id     
                            //=======================================================================================

                            //Determine root product list node
                            IPublishedContent ipProductList = null;
                            switch (ProductCode.Site)
                            {
                                case "PER":
                                    ipProductList = UmbracoHelper.ContentAtRoot()?.FirstOrDefault(x => x.ContentType.Alias == Common.Doctype.Home)?.ChildrenOfType(Common.Doctype.ProductsSection)?.FirstOrDefault();
                                    break;
                                case "COL":
                                    ipProductList = UmbracoHelper.ContentAtRoot()?.FirstOrDefault(x => x.ContentType.Alias == Common.Doctype.HomeCOL)?.ChildrenOfType(Common.Doctype.ProductsSection)?.FirstOrDefault();
                                    break;
                                case "CHE":
                                    ipProductList = UmbracoHelper.ContentAtRoot()?.FirstOrDefault(x => x.ContentType.Alias == Common.Doctype.HomeCHE)?.ChildrenOfType(Common.Doctype.ProductsSection)?.FirstOrDefault();
                                    break;
                                default:
                                    continue;
                            }


                            //Create new product node
                            icProduct = _ContentService.CreateContent(nodeName, Umbraco.Cms.Core.Udi.Create("document", ipProductList.Key), Models.PublishedModels.Product.ModelTypeAlias);


                            //Save data to product
                            if (icProduct != null)
                            {
                                SaveDataToProduct(icProduct, Item, ProductCode);
                            }
                        }
                        else
                        {
                            foreach (var searchResult in lstSearchResults)
                            {
                                //Obtain product from Id
                                icProduct = _ContentService.GetById(searchResult.Content.Id);


                                //Remove from "To Delete" list
                                if (LstNodesToDelete.Contains(searchResult.Content.Id))
                                    LstNodesToDelete.Remove(searchResult.Content.Id);


                                //if product does not exist, create it
                                if (icProduct == null)
                                {
                                    //Determine root product list node
                                    IPublishedContent ipProductList = null;
                                    switch (ProductCode.Site)
                                    {
                                        case "PER":
                                            ipProductList = UmbracoHelper.ContentAtRoot()?.FirstOrDefault(x => x.ContentType.Alias == Common.Doctype.Home)?.ChildrenOfType(Common.Doctype.ProductsSection)?.FirstOrDefault();
                                            break;
                                        case "COL":
                                            ipProductList = UmbracoHelper.ContentAtRoot()?.FirstOrDefault(x => x.ContentType.Alias == Common.Doctype.HomeCOL)?.ChildrenOfType(Common.Doctype.ProductsSection)?.FirstOrDefault();
                                            break;
                                        case "CHE":
                                            ipProductList = UmbracoHelper.ContentAtRoot()?.FirstOrDefault(x => x.ContentType.Alias == Common.Doctype.HomeCHE)?.ChildrenOfType(Common.Doctype.ProductsSection)?.FirstOrDefault();
                                            break;
                                        default:
                                            continue;
                                    }


                                    //Create new product node
                                    icProduct = _ContentService.CreateContent(nodeName, Umbraco.Cms.Core.Udi.Create("document", ipProductList.Key), Models.PublishedModels.Product.ModelTypeAlias);
                                }
                                else
                                {
                                    //Determine if product needs to be updated.  If not, skip.
                                    if (!refreshAll)
                                    {
                                        var ipProduct = UmbracoHelper.Content(searchResult.Content.Id);
                                        ContentModels.Product cmProduct = new ContentModels.Product(ipProduct, _publishedValueFallback);
                                        if (cmProduct.LastModified == Item.LastModifiedDate)
                                        {
                                            continue;
                                        }
                                    }

                                    icProduct.Name = nodeName;
                                }


                                //Save data to product
                                if (icProduct != null)
                                {
                                    SaveDataToProduct(icProduct, Item, ProductCode);
                                }
                            }
                        }


                        ////Save data to product
                        //if (icProduct != null)
                        //{
                        //    SaveDataToProduct(icProduct, Item, ProductCode);
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                importProductItemStatusMsg.Message = $"ImportProductItem() | Error";
                importProductItemStatusMsg.DetailedMessages.Add($"Error: {ex.Message}");
                importProductItemStatusMsg.Success = false;
                importProductItemStatusMsg.Code = "500";
                masterMsg.Success = false;
                masterMsg.Code = "500";

                //  Log error
                logger.LogError(ex, ex.Message);
            }
        }
        private List<AgencyId> ExtractProductIDs(ReadApi.Item2 Item)
        {
            //
            List<AgencyId> lstAgencyIds = new List<AgencyId>();

            //Extract Perdue Id (for some reason the ID is not connected to the PER record but is in seperate record.)
            if (Item?.AlternateItemIdentification?.Any(x => x.Id.Equals("PER")) == true)
            {
                string? productId = Item?.AlternateItemIdentification?.FirstOrDefault(x => x.Agency.Equals("90"))?.Id;
                if (!string.IsNullOrEmpty(productId))
                {
                    lstAgencyIds.Add(new AgencyId() { Site = "PER", Id = productId });
                }
            }


            //Extract Coleman Id
            if (Item?.AlternateItemIdentification?.Any(x => x.Id.Contains("COL")) == true) // && x.Id.Contains(">")) == true)
            {
                string? productIdAndSite = Item?.AlternateItemIdentification?.FirstOrDefault(x => x.Id.Contains("COL"))?.Id;
                if (!string.IsNullOrEmpty(productIdAndSite))
                {
                    var productId = productIdAndSite.Split('>');
                    if (productId.Length > 1 && !string.IsNullOrWhiteSpace(productId[1]))
                    {
                        lstAgencyIds.Add(new AgencyId() { Site = "COL", Id = productId[1] });
                    }
                    else
                    {
                        string? _productId = Item?.AlternateItemIdentification?.FirstOrDefault(x => x.Agency.Equals("90"))?.Id;
                        lstAgencyIds.Add(new AgencyId() { Site = "COL", Id = _productId ?? "" });
                    }
                }
            }


            //Extract Cheney Id
            if (Item?.AlternateItemIdentification?.Any(x => x.Id.Contains("CHE")) == true) //  && x.Id.Contains(">")) == true)
            {
                string? productIdAndSite = Item?.AlternateItemIdentification?.FirstOrDefault(x => x.Id.Contains("CHE"))?.Id;
                if (!string.IsNullOrEmpty(productIdAndSite))
                {
                    var productId = productIdAndSite.Split('>');
                    if (productId.Length > 1 && !string.IsNullOrWhiteSpace(productId[1]))
                    {
                        lstAgencyIds.Add(new AgencyId() { Site = "CHE", Id = productId[1] });
                    }
                    else
                    {
                        string? _productId = Item?.AlternateItemIdentification?.FirstOrDefault(x => x.Agency.Equals("90"))?.Id;
                        lstAgencyIds.Add(new AgencyId() { Site = "CHE", Id = _productId ?? "" });
                    }
                }
            }

            return lstAgencyIds;

        }
        private void SaveDataToProduct(IContent icProduct, ReadApi.Item2? Item, AgencyId agency)
        {
            try
            {
                #region "CLEAR ALL FIELDS"
                /*   Ensure all fields are cleared.  When updating nodes, old data sometimes remains if not properly cleared.   */
                icProduct.SetValue(Common.Property.ProductCode, string.Empty);
                icProduct.SetValue(Common.Property.Title, string.Empty);
                icProduct.SetValue(Common.Property.BrandName, string.Empty);
                icProduct.SetValue(Common.Property.FunctionalName, string.Empty);
                icProduct.SetValue(Common.Property.Ingredients, string.Empty);
                icProduct.SetValue(Common.Property.Allergens, string.Empty);
                icProduct.SetValue(Common.Property.LastModified, string.Empty);
                icProduct.SetValue(Common.Property.Gtin, string.Empty);
                icProduct.SetValue(Common.Property.AveragePieceSize, string.Empty);
                icProduct.SetValue(Common.Property.Weight, string.Empty);
                icProduct.SetValue(Common.Property.MaxCaseWeight, string.Empty);
                icProduct.SetValue(Common.Property.Dimensions, string.Empty);
                icProduct.SetValue(Common.Property.CaseCube, string.Empty);
                icProduct.SetValue(Common.Property.PalletTieHi, string.Empty);
                icProduct.SetValue(Common.Property.CasesPerPallet, string.Empty);
                icProduct.SetValue(Common.Property.CookLevel, string.Empty);
                icProduct.SetValue(Common.Property.StorageMethod, string.Empty);
                icProduct.SetValue(Common.Property.StorageTemperature, string.Empty);
                icProduct.SetValue(Common.Property.ShelfLife, string.Empty);
                icProduct.SetValue(Common.Property.TradeItemMarketingMessage, string.Empty);
                icProduct.SetValue(Common.Property.TradeItemKeywords, string.Empty);
                icProduct.SetValue(Common.Property.AttributePreparation, string.Empty);
                icProduct.SetValue(Common.Property.AttributeCookingStatus, string.Empty);
                icProduct.SetValue(Common.Property.AttributeFreshFrozen, string.Empty);
                icProduct.SetValue(Common.Property.AttributeProtein, string.Empty);
                icProduct.SetValue(Common.Property.AttributeBrand, string.Empty);
                icProduct.SetValue(Common.Property.Attributes, string.Empty);
                icProduct.SetValue(Common.Property.AttributeProductTypes, string.Empty);
                icProduct.SetValue(Common.Property.ServingSize, string.Empty);
                icProduct.SetValue(Common.Property.ServingSizeDescription, string.Empty);
                icProduct.SetValue(Common.Property.ServingsPerCase, string.Empty);
                icProduct.SetValue(Common.Property.Calories, string.Empty);
                icProduct.SetValue(Common.Property.CaloriesFromFat, string.Empty);
                icProduct.SetValue(Common.Property.TotalFat, string.Empty);
                icProduct.SetValue(Common.Property.TotalFatPercent, string.Empty);
                icProduct.SetValue(Common.Property.SaturatedFat, string.Empty);
                icProduct.SetValue(Common.Property.SaturatedFatPercent, string.Empty);
                icProduct.SetValue(Common.Property.TransFat, string.Empty);
                icProduct.SetValue(Common.Property.Cholesterol, string.Empty);
                icProduct.SetValue(Common.Property.CholesterolPercent, string.Empty);
                icProduct.SetValue(Common.Property.Sodium, string.Empty);
                icProduct.SetValue(Common.Property.SodiumPercent, string.Empty);
                icProduct.SetValue(Common.Property.TotalCarbohydrates, string.Empty);
                icProduct.SetValue(Common.Property.TotalCarbohydratesPercent, string.Empty);
                icProduct.SetValue(Common.Property.DietaryFiber, string.Empty);
                icProduct.SetValue(Common.Property.DietaryFiberPercent, string.Empty);
                icProduct.SetValue(Common.Property.Sugars, string.Empty);
                icProduct.SetValue(Common.Property.AddedSugar, string.Empty);
                icProduct.SetValue(Common.Property.AddedSugarPercent, string.Empty);
                icProduct.SetValue(Common.Property.Protein, string.Empty);
                icProduct.SetValue(Common.Property.ProteinPercent, string.Empty);
                icProduct.SetValue(Common.Property.VitaminAPercent, string.Empty);
                icProduct.SetValue(Common.Property.VitaminCPercent, string.Empty);
                icProduct.SetValue(Common.Property.VitaminD, string.Empty);
                icProduct.SetValue(Common.Property.VitaminDPercent, string.Empty);
                icProduct.SetValue(Common.Property.Calcium, string.Empty);
                icProduct.SetValue(Common.Property.CalciumPercent, string.Empty);
                icProduct.SetValue(Common.Property.Iron, string.Empty);
                icProduct.SetValue(Common.Property.IronPercent, string.Empty);
                icProduct.SetValue(Common.Property.Potassium, string.Empty);
                icProduct.SetValue(Common.Property.PotassiumPercent, string.Empty);
                icProduct.SetValue(Common.Property.ServingSizePerMsr, string.Empty);
                icProduct.SetValue(Common.Property.CaloriesPerMsr, string.Empty);
                icProduct.SetValue(Common.Property.CaloriesFromFatPerMsr, string.Empty);
                icProduct.SetValue(Common.Property.TotalFatPerMsr, string.Empty);
                icProduct.SetValue(Common.Property.TotalFatPercentPerMsr, string.Empty);
                icProduct.SetValue(Common.Property.SaturatedFatPerMsr, string.Empty);
                icProduct.SetValue(Common.Property.SaturatedFatPercentPerMsr, string.Empty);
                icProduct.SetValue(Common.Property.TransFatPerMsr, string.Empty);
                icProduct.SetValue(Common.Property.CholesterolPerMsr, string.Empty);
                icProduct.SetValue(Common.Property.CholesterolPercentPerMsr, string.Empty);
                icProduct.SetValue(Common.Property.SodiumPerMsr, string.Empty);
                icProduct.SetValue(Common.Property.SodiumPercentPerMsr, string.Empty);
                icProduct.SetValue(Common.Property.TotalCarbohydratesPerMsr, string.Empty);
                icProduct.SetValue(Common.Property.TotalCarbohydratesPercentPerMsr, string.Empty);
                icProduct.SetValue(Common.Property.DietaryFiberPerMsr, string.Empty);
                icProduct.SetValue(Common.Property.DietaryFiberPercentPerMsr, string.Empty);
                icProduct.SetValue(Common.Property.SugarsPerMsr, string.Empty);
                icProduct.SetValue(Common.Property.AddedSugarPerMsr, string.Empty);
                icProduct.SetValue(Common.Property.AddedSugarPercentPerMsr, string.Empty);
                icProduct.SetValue(Common.Property.ProteinPerMsr, string.Empty);
                icProduct.SetValue(Common.Property.ProteinPercentPerMsr, string.Empty);
                icProduct.SetValue(Common.Property.VitaminAPercentPerMsr, string.Empty);
                icProduct.SetValue(Common.Property.VitaminCPercentPerMsr, string.Empty);
                icProduct.SetValue(Common.Property.VitaminDPerMsr, string.Empty);
                icProduct.SetValue(Common.Property.VitaminDPercentPerMsr, string.Empty);
                icProduct.SetValue(Common.Property.CalciumPerMsr, string.Empty);
                icProduct.SetValue(Common.Property.CalciumPercentPerMsr, string.Empty);
                icProduct.SetValue(Common.Property.IronPerMsr, string.Empty);
                icProduct.SetValue(Common.Property.IronPercentPerMsr, string.Empty);
                icProduct.SetValue(Common.Property.PotassiumPerMsr, string.Empty);
                icProduct.SetValue(Common.Property.PotassiumPercentPerMsr, string.Empty);
                icProduct.SetValue(Common.Property.PrimaryImageUrl, string.Empty);
                icProduct.SetValue(Common.Property.AdditionalImages, string.Empty);
                icProduct.SetValue(Common.Property.RawAttributes, string.Empty);
                icProduct.SetValue(Common.Property.RawBrand, string.Empty);
                icProduct.SetValue(Common.Property.RawCookingStatus, string.Empty);
                icProduct.SetValue(Common.Property.RawFreshFrozen, string.Empty);
                icProduct.SetValue(Common.Property.RawPreparation, string.Empty);
                icProduct.SetValue(Common.Property.RawProtein, string.Empty);
                icProduct.SetValue(Common.Property.JsonData, string.Empty);
                #endregion


                #region "Product Data"
                //
                icProduct.SetValue(Common.Property.JsonData, Newtonsoft.Json.JsonConvert.SerializeObject(Item));
                icProduct.SetValue(Common.Property.LastModified, Item?.LastModifiedDate);

                icProduct.SetValue(Common.Property.ProductCode, agency.Id ?? "");
                icProduct.SetValue(Common.Property.Title, Item?.AdditionalDescription?.FirstOrDefault()?.Value ?? "");
                icProduct.SetValue(Common.Property.BrandName, Item?.BrandName ?? "");
                icProduct.SetValue(Common.Property.FunctionalName, Item?.FunctionalName?.FirstOrDefault()?.Value ?? "");
                #endregion


                #region "Marketing and Keywords"
                icProduct.SetValue(Common.Property.TradeItemMarketingMessage, Item?.MarketingMessage?.FirstOrDefault()?.TradeItemMarketingMessage?.FirstOrDefault()?.Value ?? "");
                //Obtain trade item keywords
                if (Item?.TradeItemKeyWords != null)
                {
                    List<string> lstTradeItemKeywords = new List<string>();
                    foreach (var keyword in Item?.TradeItemKeyWords)
                    {
                        if (!string.IsNullOrWhiteSpace(keyword.Value))
                        {
                            if (!lstTradeItemKeywords.Contains(keyword.Value))
                                lstTradeItemKeywords.Add(keyword.Value);
                        }
                    }
                    icProduct.SetValue(Common.Property.TradeItemKeywords, String.Join(Environment.NewLine, lstTradeItemKeywords.ToArray()));
                }
                #endregion


                #region "Proteins" Category
                //Add Title-cased version of "Functional Name" field data   //EX: Proteins-Chicken 
                string proteinName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Item?.FunctionalName?.FirstOrDefault()?.Value?.ToLower() ?? "");
                if (!string.IsNullOrEmpty(proteinName))
                {
                    icProduct.SetValue(Common.Property.AttributeProtein, "Proteins-" + proteinName);
                    icProduct.SetValue(Common.Property.RawProtein, proteinName);
                }
                #endregion


                #region "Brand" Category
                //  EX: Brand-PerdueHarvestland 
                if (Item?.BrandName?.ToLower() == "chef redi")
                {
                    icProduct.SetValue(Common.Property.AttributeBrand, "Brand-ChefRediBrand");
                    icProduct.SetValue(Common.Property.RawBrand, CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Item?.BrandName?.ToLower() ?? ""));
                }
                else if (Item?.BrandName?.ToLower() == "sandwich builders")
                {
                    icProduct.SetValue(Common.Property.AttributeBrand, "Brand-SandwichBuildersBrand");
                    icProduct.SetValue(Common.Property.RawBrand, CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Item?.BrandName?.ToLower() ?? ""));
                }
                else
                {
                    var finalBrandName = SetBrandName(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Item?.BrandName?.ToLower() ?? ""), Item?.AdditionalDescription?.FirstOrDefault()?.Value ?? "");
                    icProduct.SetValue(Common.Property.AttributeBrand, "Brand-" + Common.RemoveNonAlphaChar(finalBrandName ?? ""));
                    icProduct.SetValue(Common.Property.RawBrand, finalBrandName);
                }
                #endregion


                #region "Preparation" Category
                //Add "Preparation Type" field data
                //SearchData.FilterAttributes.Add(new FilterData().Add("Preparation", PreparationType, "AddSearchData-PreparationType"));
                string prep = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Item?.FoodAndBevPreparationInfo?.FirstOrDefault()?.PreparationType?.ToLower() ?? "");
                if (!string.IsNullOrEmpty(prep))
                {
                    var _prep = prep.MakeCamelCase().MakeCodeSafe("", ConvertNumbersToWords: true).Replace("-", "").Replace("_", "");
                    icProduct.SetValue(Common.Property.AttributePreparation, "Preparation-" + (_prep ?? ""));
                    icProduct.SetValue(Common.Property.RawPreparation, prep);
                }
                #endregion


                #region "Cooking Status" Category
                //see rule notes in subfunction
                if (IsFullCooked(Item))
                {
                    icProduct.SetValue(Common.Property.AttributeCookingStatus, Common.Misc.CookingStatusFullyCooked);
                    icProduct.SetValue(Common.Property.RawCookingStatus, "Fully Cooked");
                }
                else
                {
                    icProduct.SetValue(Common.Property.AttributeCookingStatus, Common.Misc.CookingStatusReadyToCook);
                    icProduct.SetValue(Common.Property.RawCookingStatus, "Ready to Cook");
                }
                #endregion


                #region "Attributes" Category

                //Instantiate variables and obtain properties with attribute data.
                //StringBuilder sbAttributes = new StringBuilder();
                List<string> lstAttributes = new List<string>();
                List<string?>? _lstDietTypeCodes = null;
                List<string>? _lstGrowingMethodCodes = null;
                List<string> lstRawAttributes = new List<string>();
                var _keywords = Item?.TradeItemKeyWords?.Select(n => n.Value.ToLower());
                List<ReadApi.ClaimDetail>? _lstClaimDetails = null;



                try
                {
                    _lstDietTypeCodes = Item?.FoodAndBevDietTypeInfo?.Select(x => x.DietTypeCode)?.ToList();

                    if (Item.ProductInformationDetail != null)
                        _lstClaimDetails = Item?.ProductInformationDetail?.FirstOrDefault()?.ClaimDetail;

                    _lstGrowingMethodCodes = Item?.GrowingMethodCode;
                }
                catch (Exception ex)
                {
                    //Continue - most likely missing data.

                    //  Log error
                    logger.LogError(ex, ex.Message);
                }




                #region 1. "Gluten Free"
                //1. "Gluten Free"

                //1A. dietTypeCode: value: "FREE_FROM_GLUTEN" 
                if (_lstDietTypeCodes != null)
                {
                    foreach (var dietType in _lstDietTypeCodes)
                    {
                        if (dietType == "FREE_FROM_GLUTEN")
                        {
                            //SearchData.FilterAttributes.Add(new FilterData().Add("Attributes", "Gluten Free", "AddSearchData-GlutenFree1A"));
                            //sbAttributes.AppendLine("Attributes-GlutenFree");
                            if (!lstAttributes.Contains("Attributes-GlutenFree"))
                            {
                                lstAttributes.Add("Attributes-GlutenFree");
                                lstRawAttributes.Add("Gluten Free");
                            }
                        }
                    }
                }

                //1B. nutritionalClaimDetail: nutritionalClaimNutrientElementCode:value: "GLUTEN" [&] nutritionalClaimTypeCode:value: "FREE_FROM",
                if (_lstClaimDetails != null)
                {
                    foreach (var detail in _lstClaimDetails)
                    {
                        var checkVal1 = detail.ClaimElementCode;
                        var checkVal2 = detail.ClaimTypeCode;

                        if (checkVal1 == "GLUTEN" && checkVal2 == "FREE_FROM")
                        {
                            //SearchData.FilterAttributes.Add(new FilterData().Add("Attributes", "Gluten Free", "AddSearchData-GlutenFree1B"));
                            //sbAttributes.AppendLine("Attributes-GlutenFree");
                            if (!lstAttributes.Contains("Attributes-GlutenFree"))
                            {
                                lstAttributes.Add("Attributes-GlutenFree");
                                lstRawAttributes.Add("Gluten Free");
                            }
                        }
                    }
                }
                #endregion

                #region 2. "No Antibiotics Ever / Antibiotic Free"
                bool isNAE = false;
                if (_lstClaimDetails != null)  //Check attributes in claims details
                {
                    foreach (var detail in _lstClaimDetails)
                    {
                        var checkVal1 = detail.ClaimElementCode;
                        var checkVal2 = detail.ClaimTypeCode;

                        if (checkVal1 == "ANTIBIOTICS" && checkVal2 == "RAISED_WITHOUT")
                        {
                            if (!lstAttributes.Contains("Attributes-NoAntibioticsEverAntibioticFree"))
                            {
                                lstAttributes.Add("Attributes-NoAntibioticsEverAntibioticFree");
                                lstRawAttributes.Add("No Antibiotics Ever / Antibiotic Free");
                                isNAE = true;
                            }
                        }
                    }
                }
                if (!isNAE)
                {
                    if (_keywords != null && _keywords.Any())  //Check attributes in keywords
                    {
                        var checkVal = "No Antibiotics Ever";
                        if (_keywords.Contains(checkVal.ToLower()))
                        {
                            if (!lstAttributes.Contains("Attributes-NoAntibioticsEverAntibioticFree"))
                            {
                                lstAttributes.Add("Attributes-NoAntibioticsEverAntibioticFree");
                                lstRawAttributes.Add("No Antibiotics Ever / Antibiotic Free");
                            }
                        }
                    }
                }
                #endregion

                #region 3. "100% Vegetarian Fed with No Animal By-Products"
                //3. "100% Vegetarian Fed with No Animal By-Products"

                //3A. TIK contains "100% vegetarian diet"
                if (_keywords != null && _keywords.Any())
                {
                    var checkVal = "100% vegetarian diet";
                    if (_keywords.Contains(checkVal.ToLower()))
                    {
                        //SearchData.FilterAttributes.Add(new FilterData().Add("Attributes", "100% Vegetarian Fed with No Animal By-Products", "AddSearchData-VegFed3A"));
                        //sbAttributes.AppendLine("Attributes-100VegetarianFedWithNoAnimalByProducts");
                        if (!lstAttributes.Contains("Attributes-100VegetarianFedWithNoAnimalByProducts"))
                        {
                            lstAttributes.Add("Attributes-100VegetarianFedWithNoAnimalByProducts");
                            lstRawAttributes.Add("100% Vegetarian Fed with No Animal By-Products");
                        }
                    }

                    //JF- Added 2022-12-16
                    checkVal = "100% Vegetarian Fed";
                    if (_keywords.Contains(checkVal.ToLower()))
                    {
                        //SearchData.FilterAttributes.Add(new FilterData().Add("Attributes", "100% Vegetarian Fed with No Animal By-Products", "AddSearchData-VegFed3A"));
                        //sbAttributes.AppendLine("Attributes-100VegetarianFedWithNoAnimalByProducts");
                        if (!lstAttributes.Contains("Attributes-100VegetarianFedWithNoAnimalByProducts"))
                        {
                            lstAttributes.Add("Attributes-100VegetarianFedWithNoAnimalByProducts");
                            lstRawAttributes.Add("100% Vegetarian Fed with No Animal By-Products");
                        }
                    }
                }

                //3B. TIK contains "all vegetarian diet"
                if (_keywords != null && _keywords.Any())
                {
                    var checkVal = "all vegetarian diet";

                    if (_keywords.Contains(checkVal.ToLower()))
                    {
                        //SearchData.FilterAttributes.Add(new FilterData().Add("Attributes", "100% Vegetarian Fed with No Animal By-Products", "AddSearchData-VegFed3B"));
                        //sbAttributes.AppendLine("Attributes-100VegetarianFedWithNoAnimalByProducts");
                        if (!lstAttributes.Contains("Attributes-100VegetarianFedWithNoAnimalByProducts"))
                        {
                            lstAttributes.Add("Attributes-100VegetarianFedWithNoAnimalByProducts");
                            lstRawAttributes.Add("100% Vegetarian Fed with No Animal By-Products");
                        }
                    }
                }

                #endregion

                #region 4. "Organic / Non-GMO / Free Range"
                //4. "Organic / Non-GMO / Free Range"

                //4A. growingMethodCode: code: "ORGANIC"
                if (_lstGrowingMethodCodes != null)
                {
                    foreach (var code in _lstGrowingMethodCodes)
                    {
                        var checkVal = code;
                        if (checkVal == "ORGANIC")
                        {
                            //SearchData.FilterAttributes.Add(new FilterData().Add("Attributes", "Organic / Non-GMO / Free Range", "AddSearchData-Organic4A"));
                            //sbAttributes.AppendLine("Attributes-OrganicNonGMOFreeRange");
                            if (!lstAttributes.Contains("Attributes-OrganicNonGMOFreeRange"))
                            {
                                lstAttributes.Add("Attributes-OrganicNonGMOFreeRange");
                                lstRawAttributes.Add("Organic / Non-GMO / Free Range");
                            }
                        }
                    }
                }

                //4B. growingMethodCode: code: "FREE_RANGE"
                if (_lstGrowingMethodCodes != null)
                {
                    foreach (var code in _lstGrowingMethodCodes)
                    {
                        var checkVal = code;
                        if (checkVal == "FREE_RANGE")
                        {
                            //SearchData.FilterAttributes.Add(new FilterData().Add("Attributes", "Organic / Non-GMO / Free Range", "AddSearchData-Organic4B"));
                            //sbAttributes.AppendLine("Attributes-OrganicNonGMOFreeRange");
                            if (!lstAttributes.Contains("Attributes-OrganicNonGMOFreeRange"))
                            {
                                lstAttributes.Add("Attributes-OrganicNonGMOFreeRange");
                                lstRawAttributes.Add("Organic / Non-GMO / Free Range");
                            }
                        }
                    }
                }


                #endregion

                #region 5. "Halal Certified"
                //5. "Halal Certified"

                //5A. dietTypeCode: value: "HALAL"
                if (_lstDietTypeCodes != null)
                {
                    foreach (var dietType in _lstDietTypeCodes)
                    {
                        if (dietType == "HALAL")
                        {
                            //SearchData.FilterAttributes.Add(new FilterData().Add("Attributes", "Halal Certified", "AddSearchData-Halal5A"));
                            //sbAttributes.AppendLine("Attributes-HalalCertified");
                            if (!lstAttributes.Contains("Attributes-HalalCertified"))
                            {
                                lstAttributes.Add("Attributes-HalalCertified");
                                lstRawAttributes.Add("Halal Certified");
                            }
                        }
                    }
                }


                #endregion

                #region 6. "Child Nutrition Labeled"
                //6. "Child Nutrition Labeled"

                //6A. doesTradeItemCarryUSDAChildNutritionLabel: value: "TRUE"
                var checkValCn = Item?.DoesTradeItemCarryUSDAChildNutritionLabel;
                if (checkValCn == "TRUE")
                {
                    //SearchData.FilterAttributes.Add(new FilterData().Add("Attributes", "Child Nutrition Labeled", "AddSearchData-CN6A"));
                    //sbAttributes.AppendLine("Attributes-ChildNutritionLabeled");
                    if (!lstAttributes.Contains("Attributes-ChildNutritionLabeled"))
                    {
                        lstAttributes.Add("Attributes-ChildNutritionLabeled");
                        lstRawAttributes.Add("Child Nutrition Labeled");
                    }
                }

                #endregion

                #region 7. "Whole Grain"
                //7. "Whole Grain"

                //7A.nutritionalClaimDetail: nutritionalClaimNutrientElementCode: value: "WHOLE_GRAIN" [&] nutritionalClaimTypeCode: value: "CONTAINS"
                if (_lstClaimDetails != null)
                {
                    foreach (var detail in _lstClaimDetails)
                    {
                        var checkVal1 = detail.ClaimElementCode;
                        var checkVal2 = detail.ClaimTypeCode;

                        if (checkVal1 == "WHOLE_GRAIN" && checkVal2 == "CONTAINS")
                        {
                            //SearchData.FilterAttributes.Add(new FilterData().Add("Attributes", "Whole Grain", "AddSearchData-WholeGrain7A"));
                            //sbAttributes.AppendLine("Attributes-WholeGrain");
                            if (!lstAttributes.Contains("Attributes-WholeGrain"))
                            {
                                lstAttributes.Add("Attributes-WholeGrain");
                                lstRawAttributes.Add("Whole Grain");
                            }
                        }
                    }
                }



                #endregion

                #region 8. "Lower Sodium"
                bool isLowSodium = false;
                if (_lstClaimDetails != null)
                {
                    List<string> LstLowSalt = new List<string>()
                    {
                        "LOW",
                        "VERY_LOW",
                        "REDUCED_LESS"
                    };
                    foreach (var detail in _lstClaimDetails)
                    {
                        var checkVal1 = detail.ClaimElementCode ?? "";
                        var checkVal2 = detail.ClaimTypeCode ?? "";

                        if (checkVal1 == "SODIUM_SALT" && LstLowSalt.Contains(checkVal2))
                        {
                            if (!lstAttributes.Contains("Attributes-LowerSodium"))
                            {
                                lstAttributes.Add("Attributes-LowerSodium");
                                lstRawAttributes.Add("Lower Sodium");
                                isLowSodium = true;
                            }
                        }
                    }
                }
                if (!isLowSodium)
                {
                    if (_keywords != null && _keywords.Any())
                    {
                        var checkVal = "Lower Sodium";
                        if (_keywords.Contains(checkVal.ToLower()))
                        {
                            if (!lstAttributes.Contains("Attributes-LowerSodium"))
                            {
                                lstAttributes.Add("Attributes-LowerSodium");
                                lstRawAttributes.Add("Lower Sodium");
                            }
                        }
                    }
                }



                #endregion

                #region 9. "Fresh CVP"
                //9. "Fresh CVP"
                //9A. TIK contains "CVP"
                if (_keywords != null && _keywords.Any())
                {
                    var checkVal = "CVP";

                    if (_keywords.Contains(checkVal.ToLower()))
                    {
                        if (!lstAttributes.Contains("Attributes-FreshCVP"))
                        {
                            lstAttributes.Add("Attributes-FreshCVP");
                            lstRawAttributes.Add("Fresh CVP");
                        }
                    }
                }

                #endregion

                //icProduct.SetValue(Common.Property.Attributes, sbAttributes.ToString());
                icProduct.SetValue(Common.Property.Attributes, String.Join(Environment.NewLine, lstAttributes.ToArray()));
                icProduct.SetValue(Common.Property.RawAttributes, String.Join(Environment.NewLine, lstRawAttributes.ToArray()));


                #endregion


                #region "Product Type" Category
                /*   JF | This came from original code.  Why does product type ONLY pull first 2 keywords?   */
                // EX: ProductType-WholeBirdCVP

                List<TradeItemKeyWord> tradeItemKeyWords = Item?.TradeItemKeyWords;
                List<string> lstProductTypes = new List<string>();
                if (tradeItemKeyWords != null)
                {
                    //Get Product Type
                    if (tradeItemKeyWords.Count >= 1)
                    {
                        string? _category = FixBadChars(tradeItemKeyWords[0].Value).Trim();
                        string _categoryName = _category ?? "";
                        string _categoryCode = _categoryName.Replace("-", "").MakeCamelCase().MakeCodeSafe("-", ConvertNumbersToWords: true);
                        if (!string.IsNullOrWhiteSpace(_categoryCode))
                            lstProductTypes.Add("ProductType-" + _categoryCode ?? "");
                    }

                    //Get Product Subtype
                    if (tradeItemKeyWords.Count >= 2)
                    {
                        string? _subcategory = FixBadChars(tradeItemKeyWords[1].Value).Trim();
                        string _subCategoryName = _subcategory ?? "";
                        string _subCategoryCode = _subCategoryName.Replace("-", "").MakeCamelCase().MakeCodeSafe("-", ConvertNumbersToWords: true);
                        if (!string.IsNullOrWhiteSpace(_subCategoryCode))
                            lstProductTypes.Add("ProductType-" + _subCategoryCode ?? "");
                    }
                }
                icProduct.SetValue(Common.Property.AttributeProductTypes, String.Join(Environment.NewLine, lstProductTypes.ToArray()));

                #endregion


                #region "Ingredients & Allergens"
                icProduct.SetValue(Common.Property.Ingredients, Item?.IngredientStatement?.FirstOrDefault()?.Statement?.FirstOrDefault()?.Value ?? "");


                //StringBuilder sbAllergens = new StringBuilder();
                List<string> lstAllergens = new List<string>();
                foreach (var allergen in Item?.AllergenRelatedInformation?.FirstOrDefault()?.Allergen)
                {
                    if (allergen.LevelOfContainmentCode.Equals("CONTAINS"))
                    {
                        string _allergen = Common.ConvertAllergenCode(allergen.AllergenTypeCode ?? "");
                        if (!string.IsNullOrWhiteSpace(_allergen))
                            lstAllergens.Add(_allergen);
                    }
                }
                icProduct.SetValue(Common.Property.Allergens, String.Join(Environment.NewLine, lstAllergens.ToArray()));
                #endregion


                #region "SPECIFICATIONS"
                //SPECIFICATIONS
                icProduct.SetValue(Common.Property.Gtin, Item?.Gtin ?? "");
                icProduct.SetValue(Common.Property.AveragePieceSize, Item?.NonPackagedSizeDimension?.FirstOrDefault()?.DescriptiveSizeDimension?.FirstOrDefault()?.Value ?? "");

                if (!string.IsNullOrWhiteSpace(Item?.NetWeight?.FirstOrDefault()?.Value))
                {
                    string weight = Item?.NetWeight?.FirstOrDefault()?.Value;
                    string qualifier = ConvertQualifier(Item?.NetWeight?.FirstOrDefault()?.Qual);
                    icProduct.SetValue(Common.Property.Weight, $"{weight} {qualifier}");
                }
                if (!string.IsNullOrWhiteSpace(Item?.NetWeight?.FirstOrDefault()?.Value))
                {
                    string weight = Item?.GrossWeight?.FirstOrDefault()?.Value;
                    string qualifier = ConvertQualifier(Item?.GrossWeight?.FirstOrDefault()?.Qual);
                    icProduct.SetValue(Common.Property.MaxCaseWeight, $"{weight} {qualifier}");
                }




                string length = Item?.Depth?.FirstOrDefault()?.Value ?? "0";
                string width = Item?.Width?.FirstOrDefault()?.Value ?? "0";
                string height = Item?.Height?.FirstOrDefault()?.Value ?? "0";
                icProduct.SetValue(Common.Property.Dimensions, length + " x " + width + " x " + height);

                string volume = Item?.Volume?.FirstOrDefault()?.Value ?? "0";
                if (!char.IsNumber(volume[0]))
                    volume = "0" + volume;
                icProduct.SetValue(Common.Property.CaseCube, volume);

                icProduct.SetValue(Common.Property.CasesPerPallet, Item?.NumberOfItemsPerPallet?.FirstOrDefault()?.Value ?? "");




                // Obtain 'HI' and 'TI' values if exist
                string? hi = string.Empty;
                if (!string.IsNullOrWhiteSpace(Item?.Hi))
                    hi = Item?.Hi;
                if (string.IsNullOrWhiteSpace(hi))
                {
                    if (Item?.NonGTINPalletHi != null && Item?.NonGTINPalletHi.Count > 0)
                    {
                        hi = Item?.NonGTINPalletHi?.FirstOrDefault()?.Value;
                    }
                }

                string? ti = string.Empty;
                if (!string.IsNullOrWhiteSpace(Item?.Ti))
                    ti = Item?.Ti;
                if (string.IsNullOrWhiteSpace(ti))
                {
                    if (Item?.NonGTINPalletTi != null && Item?.NonGTINPalletTi.Count > 0)
                    {
                        ti = Item?.NonGTINPalletTi?.FirstOrDefault()?.Value;
                    }
                }

                if (!string.IsNullOrWhiteSpace(hi) && !string.IsNullOrWhiteSpace(ti))
                    icProduct.SetValue(Common.Property.PalletTieHi, string.Format("{0} x {1}", ti, hi));




                #endregion


                #region "HANDLING"
                //HANDLING                            
                if (IsFullCooked(Item))
                    icProduct.SetValue(Common.Property.CookLevel, "Fully Cooked");
                else
                    icProduct.SetValue(Common.Property.CookLevel, "Ready-to-Cook");



                icProduct.SetValue(Common.Property.StorageMethod, Item?.ConsumerStorageInstructions?.FirstOrDefault()?.Value ?? "");
                icProduct.SetValue(Common.Property.ShelfLife, Item?.MinimumTradeItemLifespanFromProduction?.FirstOrDefault()?.Value ?? "" + " Days");

                var storageTemp = Item?.TradeItemTemperatureInformation?.FirstOrDefault(x => x.TemperatureQualifierCode.Equals("STORAGE_HANDLING"));
                if (storageTemp != null)
                {
                    string min = storageTemp?.MinimumTemperature?.Value ?? "";
                    string max = storageTemp?.MaximumTemperature?.Value ?? "";
                    icProduct.SetValue(Common.Property.StorageTemperature, string.Format("{0}-{1}° F", min, max));
                }
                #endregion


                #region "Fresh-Frozen" Category
                string freshFrozen = Common.Misc.FreshFrozenFresh;
                string freshFrozenRaw = "Fresh";


                if (!string.IsNullOrEmpty(storageTemp?.MinimumTemperature?.Value) && storageTemp?.MinimumTemperature?.Value == "0")
                {
                    freshFrozen = Common.Misc.FreshFrozenFrozen;
                    freshFrozenRaw = "Frozen";
                }
                else
                {
                    var _storageMethod = Item?.ConsumerStorageInstructions?.FirstOrDefault()?.Value?.ToLower() ?? "";
                    if (
                    _storageMethod.Contains("frozen") ||
                    _storageMethod.Contains("0 degrees") ||
                    _storageMethod.Contains("0'f") ||
                    _storageMethod.Contains("0f"))
                    {
                        //Check if product can be both fresh and frozen
                        if (_storageMethod.Contains("refri")) //shortened due to misspellings in 1ws.
                        {
                            if (icProduct.Name.ToLower().Contains("frozen")) //Only mark as frozen if name indicates it.
                            {
                                freshFrozen = Common.Misc.FreshFrozenFrozen;
                                freshFrozenRaw = "Frozen";
                            }
                        }
                        else
                        {
                            freshFrozen = Common.Misc.FreshFrozenFrozen;
                            freshFrozenRaw = "Frozen";
                        }
                    }
                }


                //ORIGINAL CODE
                //If field "Storage Method" includes BOTH "frozen" and "refrigerated" AND the Product Name includes "frozen", use "Frozen", otherwise use "Fresh"
                //if (_storageMethod.Contains("frozen") || _storageMethod.Contains("refrigerated"))
                //{
                //    if (Item.AdditionalDescription.FirstOrDefault().Value.ToLower().Contains("frozen"))
                //    {
                //        freshFrozen = Common.Misc.FreshFrozenFrozen;
                //        freshFrozenRaw = "Frozen";
                //    }
                //    else
                //    {
                //        freshFrozen = Common.Misc.FreshFrozenFresh;
                //        freshFrozenRaw = "Fresh";
                //    }
                //}
                //else
                //{
                //    if (_storageMethod.Contains("frozen") || _storageMethod.Contains("0 degrees"))
                //    {
                //        freshFrozen = Common.Misc.FreshFrozenFrozen;
                //        freshFrozenRaw = "Frozen";
                //    }
                //    else if (_storageMethod.Contains("refrigerated") || _storageMethod.Contains("34 degrees"))
                //    {
                //        freshFrozen = Common.Misc.FreshFrozenFresh;
                //        freshFrozenRaw = "Fresh";
                //    }
                //}


                icProduct.SetValue(Common.Property.AttributeFreshFrozen, freshFrozen ?? "");
                icProduct.SetValue(Common.Property.RawFreshFrozen, freshFrozenRaw);
                #endregion


                #region "PER SERVING"
                //PER SERVING
                var ByServing = Item?.NutrientInformation?.FirstOrDefault(x => x.NutrientBasisQuantityTypeCode.Equals("BY_SERVING"));


                var servingSize = ByServing?.ServingSize?.FirstOrDefault();
                icProduct.SetValue(Common.Property.ServingSize, (servingSize?.Value?.RemoveZeroDecimal() ?? "") + ConvertQualifier(servingSize?.Qual ?? ""));
                icProduct.SetValue(Common.Property.ServingSizeDescription, ByServing?.ServingSizeDescription?.FirstOrDefault()?.Value?.ToLower().RemoveZeroDecimal() ?? "");
                icProduct.SetValue(Common.Property.ServingsPerCase, Item?.NumberOfServingsPerPackage ?? "");

                var calories = ByServing?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("ENER-"));
                icProduct.SetValue(Common.Property.Calories, (calories?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(calories?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));

                var caloriesFromFat = ByServing?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("ENERPF"));
                icProduct.SetValue(Common.Property.CaloriesFromFat, (caloriesFromFat?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(caloriesFromFat?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));

                var totalFat = ByServing?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("FATNLEA"));
                icProduct.SetValue(Common.Property.TotalFat, (totalFat?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(totalFat?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));
                icProduct.SetValue(Common.Property.TotalFatPercent, (totalFat?.DailyValueIntakePercent?.RemoveZeroDecimal() ?? "0") + "%");

                var saturatedFat = ByServing?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("FASAT"));
                icProduct.SetValue(Common.Property.SaturatedFat, (saturatedFat?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(saturatedFat?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));
                icProduct.SetValue(Common.Property.SaturatedFatPercent, (saturatedFat?.DailyValueIntakePercent?.RemoveZeroDecimal() ?? "0") + "%");

                var transFat = ByServing?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("FATRN"));
                icProduct.SetValue(Common.Property.TransFat, (transFat?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(transFat?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));
                // NO "transFatPercent"

                var cholesterol = ByServing?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("CHOL-"));
                icProduct.SetValue(Common.Property.Cholesterol, (cholesterol?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(cholesterol?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));
                icProduct.SetValue(Common.Property.CholesterolPercent, (cholesterol?.DailyValueIntakePercent?.RemoveZeroDecimal() ?? "0") + "%");

                var sodium = ByServing?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("NA"));
                icProduct.SetValue(Common.Property.Sodium, (sodium?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(sodium?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));
                icProduct.SetValue(Common.Property.SodiumPercent, (sodium?.DailyValueIntakePercent?.RemoveZeroDecimal() ?? "0") + "%");

                var carbohydrates = ByServing?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("CHO-"));
                icProduct.SetValue(Common.Property.TotalCarbohydrates, (carbohydrates?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(carbohydrates?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));
                icProduct.SetValue(Common.Property.TotalCarbohydratesPercent, (carbohydrates?.DailyValueIntakePercent?.RemoveZeroDecimal() ?? "0") + "%");

                var dietaryFiber = ByServing?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("FIBTSW"));
                icProduct.SetValue(Common.Property.DietaryFiber, (dietaryFiber?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(dietaryFiber?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));
                icProduct.SetValue(Common.Property.DietaryFiberPercent, (dietaryFiber?.DailyValueIntakePercent?.RemoveZeroDecimal() ?? "0") + "%");

                var sugars = ByServing?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("SUGAR-"));
                icProduct.SetValue(Common.Property.Sugars, (sugars?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(sugars?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));
                // NO "sugarsPercent"

                var addedSugar = ByServing?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("SUGAD"));
                icProduct.SetValue(Common.Property.AddedSugar, (addedSugar?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(addedSugar?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));
                icProduct.SetValue(Common.Property.AddedSugarPercent, (addedSugar?.DailyValueIntakePercent?.RemoveZeroDecimal() ?? "0") + "%");

                var protein = ByServing?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("PRO-"));
                icProduct.SetValue(Common.Property.Protein, (protein?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(protein?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));
                icProduct.SetValue(Common.Property.ProteinPercent, (protein?.DailyValueIntakePercent?.RemoveZeroDecimal() ?? "0") + "%");

                var vitaminA = ByServing?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("VITA-"));
                icProduct.SetValue(Common.Property.VitaminAPercent, (vitaminA?.DailyValueIntakePercent?.RemoveZeroDecimal() ?? "0") + "%");
                // NO "vitaminA"

                var vitaminC = ByServing?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("VITC-"));
                icProduct.SetValue(Common.Property.VitaminCPercent, (vitaminC?.DailyValueIntakePercent?.RemoveZeroDecimal() ?? "0") + "%");
                // NO "vitaminC"

                var vitaminD = ByServing?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("VITD-"));
                icProduct.SetValue(Common.Property.VitaminD, (vitaminD?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(vitaminD?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));
                icProduct.SetValue(Common.Property.VitaminDPercent, (vitaminD?.DailyValueIntakePercent?.RemoveZeroDecimal() ?? "0") + "%");

                var calcium = ByServing?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("CA"));
                icProduct.SetValue(Common.Property.Calcium, (calcium?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(calcium?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));
                icProduct.SetValue(Common.Property.CalciumPercent, (calcium?.DailyValueIntakePercent?.RemoveZeroDecimal() ?? "0") + "%");

                var iron = ByServing?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("FE"));
                icProduct.SetValue(Common.Property.Iron, (iron?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(iron?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));
                icProduct.SetValue(Common.Property.IronPercent, (iron?.DailyValueIntakePercent?.RemoveZeroDecimal() ?? "0") + "%");

                var potassium = ByServing?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("K"));
                icProduct.SetValue(Common.Property.Potassium, (potassium?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(potassium?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));
                icProduct.SetValue(Common.Property.PotassiumPercent, (potassium?.DailyValueIntakePercent?.RemoveZeroDecimal() ?? "0") + "%");
                #endregion


                #region "PER MEASURE"
                //PER MEASURE
                var ByMeasure = Item?.NutrientInformation?.FirstOrDefault(x => x.NutrientBasisQuantityTypeCode.Equals("BY_MEASURE"));

                servingSize = ByMeasure?.ServingSize?.FirstOrDefault();
                icProduct.SetValue(Common.Property.ServingSizePerMsr, (servingSize?.Value?.RemoveZeroDecimal() ?? "") + ConvertQualifier(servingSize?.Qual));
                //icProduct.SetValue("servingSizeDescriptionPerMsr", ByMeasure?.ServingSizeDescription?.FirstOrDefault()?.Value?.ToLower());
                //icProduct.SetValue("servingsPerCasePerMsr", Item?.NumberOfServingsPerPackage);

                calories = ByMeasure?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("ENER-"));
                icProduct.SetValue(Common.Property.CaloriesPerMsr, (calories?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(calories?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));

                caloriesFromFat = ByMeasure?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("ENERPF"));
                icProduct.SetValue(Common.Property.CaloriesFromFatPerMsr, (caloriesFromFat?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(caloriesFromFat?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));

                totalFat = ByMeasure?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("FATNLEA"));
                icProduct.SetValue(Common.Property.TotalFatPerMsr, (totalFat?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(totalFat?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));
                icProduct.SetValue(Common.Property.TotalFatPercentPerMsr, (totalFat?.DailyValueIntakePercent?.RemoveZeroDecimal() ?? "0") + "%");

                saturatedFat = ByMeasure?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("FASAT"));
                icProduct.SetValue(Common.Property.SaturatedFatPerMsr, (saturatedFat?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(saturatedFat?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));
                icProduct.SetValue(Common.Property.SaturatedFatPercentPerMsr, (saturatedFat?.DailyValueIntakePercent?.RemoveZeroDecimal() ?? "0") + "%");

                transFat = ByMeasure?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("FATRN"));
                icProduct.SetValue(Common.Property.TransFatPerMsr, (transFat?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(transFat?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));
                // NO "transFatPercent"

                cholesterol = ByMeasure?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("CHOL-"));
                icProduct.SetValue(Common.Property.CholesterolPerMsr, (cholesterol?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(cholesterol?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));
                icProduct.SetValue(Common.Property.CholesterolPercentPerMsr, (cholesterol?.DailyValueIntakePercent?.RemoveZeroDecimal() ?? "0") + "%");

                sodium = ByMeasure?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("NA"));
                icProduct.SetValue(Common.Property.SodiumPerMsr, (sodium?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(sodium?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));
                icProduct.SetValue(Common.Property.SodiumPercentPerMsr, (sodium?.DailyValueIntakePercent?.RemoveZeroDecimal() ?? "0") + "%");

                carbohydrates = ByMeasure?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("CHO-"));
                icProduct.SetValue(Common.Property.TotalCarbohydratesPerMsr, (carbohydrates?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(carbohydrates?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));
                icProduct.SetValue(Common.Property.TotalCarbohydratesPercentPerMsr, (carbohydrates?.DailyValueIntakePercent?.RemoveZeroDecimal() ?? "0") + "%");

                dietaryFiber = ByMeasure?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("FIBTSW"));
                icProduct.SetValue(Common.Property.DietaryFiberPerMsr, (dietaryFiber?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(dietaryFiber?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));
                icProduct.SetValue(Common.Property.DietaryFiberPercentPerMsr, (dietaryFiber?.DailyValueIntakePercent?.RemoveZeroDecimal() ?? "0") + "%");

                sugars = ByMeasure?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("SUGAR-"));
                icProduct.SetValue(Common.Property.SugarsPerMsr, (sugars?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(sugars?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));
                // NO "sugarsPercent"

                addedSugar = ByMeasure?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("SUGAD"));
                icProduct.SetValue(Common.Property.AddedSugarPerMsr, (addedSugar?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(addedSugar?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));
                icProduct.SetValue(Common.Property.AddedSugarPercentPerMsr, (addedSugar?.DailyValueIntakePercent?.RemoveZeroDecimal() ?? "0") + "%");

                protein = ByMeasure?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("PRO-"));
                icProduct.SetValue(Common.Property.ProteinPerMsr, (protein?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(protein?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));
                icProduct.SetValue(Common.Property.ProteinPercentPerMsr, (protein?.DailyValueIntakePercent?.RemoveZeroDecimal() ?? "0") + "%");

                vitaminA = ByMeasure?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("VITA-"));
                icProduct.SetValue(Common.Property.VitaminAPercentPerMsr, (vitaminA?.DailyValueIntakePercent?.RemoveZeroDecimal() ?? "0") + "%");
                // NO "vitaminA"

                vitaminC = ByMeasure?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("VITC-"));
                icProduct.SetValue(Common.Property.VitaminCPercentPerMsr, (vitaminC?.DailyValueIntakePercent?.RemoveZeroDecimal() ?? "0") + "%");
                // NO "vitaminC"

                vitaminD = ByMeasure?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("VITD-"));
                icProduct.SetValue(Common.Property.VitaminDPerMsr, (vitaminD?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(vitaminD?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));
                icProduct.SetValue(Common.Property.VitaminDPercentPerMsr, (vitaminD?.DailyValueIntakePercent?.RemoveZeroDecimal() ?? "0") + "%");

                calcium = ByMeasure?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("CA"));
                icProduct.SetValue(Common.Property.CalciumPerMsr, (calcium?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(calcium?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));
                icProduct.SetValue(Common.Property.CalciumPercentPerMsr, (calcium?.DailyValueIntakePercent?.RemoveZeroDecimal() ?? "0") + "%");

                iron = ByMeasure?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("FE"));
                icProduct.SetValue(Common.Property.IronPerMsr, (iron?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(iron?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));
                icProduct.SetValue(Common.Property.IronPercentPerMsr, (iron?.DailyValueIntakePercent?.RemoveZeroDecimal() ?? "0") + "%");

                potassium = ByMeasure?.NutrientDetail?.FirstOrDefault(x => x.NutrientTypeCode.Equals("K"));
                icProduct.SetValue(Common.Property.PotassiumPerMsr, (potassium?.QuantityContained?.FirstOrDefault()?.Value?.RemoveZeroDecimal() ?? "0") + ConvertQualifier(potassium?.QuantityContained?.FirstOrDefault()?.Qual ?? ""));
                icProduct.SetValue(Common.Property.PotassiumPercentPerMsr, (potassium?.DailyValueIntakePercent?.RemoveZeroDecimal() ?? "0") + "%");
                #endregion


                #region "Images"
                if (Item?.Dam != null && Item?.Dam.Count() > 0)
                {
                    //Add Primary Image [skip images with file name containing "_C"]
                    IEnumerable<Dam>? lstPrimaryImgs = Item?.Dam?.Where(x => x.PrimaryImage == "true");
                    if (lstPrimaryImgs?.Count() != 0)
                    {
                        foreach (var primaryImg in lstPrimaryImgs)
                        {
                            if (!primaryImg.General.FileName.Contains("_C"))
                            {
                                icProduct.SetValue(Common.Property.PrimaryImageUrl, primaryImg?.General?.UniformResourceIdentifier ?? "");
                                break;
                            }
                        }
                    }


                    //Add secondary images
                    IEnumerable<Dam>? lstSecondaryImgs = Item?.Dam?.Where(x => x.PrimaryImage != "true");
                    if (lstSecondaryImgs?.Count() != 0)
                    {
                        List<string> lstImgURLs = new List<string>();
                        foreach (var imgModel in lstSecondaryImgs)
                        {
                            if (!imgModel.General.FileName.Contains("_C"))
                            {
                                lstImgURLs.Add(imgModel?.General?.UniformResourceIdentifier ?? "");
                            }
                        }

                        icProduct.SetValue(Common.Property.AdditionalImages, String.Join(Environment.NewLine, lstImgURLs.ToArray()));
                    }
                }
                #endregion


                #region "Save Data"
                //Save data to Umbraco
                var saveResult = _ContentService.SaveAndPublish(icProduct);


                if (saveResult.Success)
                {
                    //Increment fimport counter
                    switch (UmbracoHelper.Content(icProduct.Id).Root().ContentType.Alias)
                    {
                        case Common.Doctype.Home:
                            PerdueProductsImported++;
                            break;
                        case Common.Doctype.HomeCHE:
                            CheneyProductsImported++;
                            break;
                        case Common.Doctype.HomeCOL:
                            ColemanProductsImported++;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    saveDataToProductStatusMsg.Message = $"SaveDataToProduct() | Save Result Error";
                    saveDataToProductStatusMsg.DetailedMessages.Add($"Event Messages: {saveResult.EventMessages}");
                    saveDataToProductStatusMsg.DetailedMessages.Add($"Invalid Properties: {saveResult.InvalidProperties}");
                    saveDataToProductStatusMsg.Success = false;
                    saveDataToProductStatusMsg.Code = "500";
                    masterMsg.Success = false;
                    masterMsg.Code = "500";

                    //  Log error
                    logger.LogError(new Exception(saveResult.EventMessages?.ToString()), Newtonsoft.Json.JsonConvert.SerializeObject(saveResult.InvalidProperties));
                    ImportFails++;
                }
                #endregion
            }
            catch (Exception ex)
            {
                saveDataToProductStatusMsg.Message = $"SaveDataToProduct() | Error";
                saveDataToProductStatusMsg.DetailedMessages.Add($"Error: {ex.Message}");
                saveDataToProductStatusMsg.DetailedMessages.Add($"Error: {ex.ToString()}");
                saveDataToProductStatusMsg.Success = false;
                saveDataToProductStatusMsg.Code = "500";
                masterMsg.Success = false;
                masterMsg.Code = "500";

                //  Log error
                logger.LogError(ex, ex.Message);
                ImportFails++;
            }
        }

        private void ObtainAllExistingNodeIDs()
        {
            try
            {
                //Obtain each product list node
                var ipHomeProductSection = UmbracoHelper.ContentAtRoot()?.FirstOrDefault(x => x.ContentType.Alias == Common.Doctype.Home)?.ChildrenOfType(Common.Doctype.ProductsSection)?.FirstOrDefault();
                var ipHomeCHEProductSection = UmbracoHelper.ContentAtRoot()?.FirstOrDefault(x => x.ContentType.Alias == Common.Doctype.HomeCHE)?.ChildrenOfType(Common.Doctype.ProductsSection)?.FirstOrDefault();
                var ipHomeCOLProductSection = UmbracoHelper.ContentAtRoot()?.FirstOrDefault(x => x.ContentType.Alias == Common.Doctype.HomeCOL)?.ChildrenOfType(Common.Doctype.ProductsSection)?.FirstOrDefault();

                //Add all product node IDs to list.
                LstNodesToDelete.AddRange(ipHomeProductSection?.Children?.Where(x => x.ContentType.Alias == Common.Doctype.Product).Select(x => x.Id));
                LstNodesToDelete.AddRange(ipHomeCHEProductSection?.Children?.Where(x => x.ContentType.Alias == Common.Doctype.Product).Select(x => x.Id));
                LstNodesToDelete.AddRange(ipHomeCOLProductSection?.Children.Where(x => x.ContentType.Alias == Common.Doctype.Product).Select(x => x.Id));
            }
            catch (Exception ex)
            {
                //  Log error
                logger.LogError(ex, ex.Message);
            }
        }
        private void DeleteRemainingNodes()
        {
            //Loop thorugh all node IDs to be deleted.
            foreach (int id in LstNodesToDelete)
            {
                try
                {
                    //Obtain product by Id
                    IContent? icProduct = _ContentService.GetById(id);

                    //If exists, delete
                    if (icProduct != null)
                    {
                        var result = _ContentService.Delete(icProduct);
                        if (result.Success)
                        { ProductsDeleted++; }
                        else
                        {
                            logger.LogWarning($"Product Id {id} could not be deleted during API import.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    //  Log error
                    logger.LogError(ex, ex.Message);
                    importProductItemStatusMsg.Success = false;
                    importProductItemStatusMsg.Code = "500";
                    importProductItemStatusMsg.DetailedMessages.Add(ex.Message);
                }

            }
        }


        private StatusMessage UpdateFilterAttributes()
        {
            //  INSTANTIATE IMPORT/SAVE STATUS'
            var returnMsg = new StatusMessage();
            returnMsg.TimestampStart = DateTime.Now;
            returnMsg.RunningFunctionName = "ReadApiController.cs | UpdateFilterAttributes()";
            returnMsg.Success = true;
            returnMsg.Code = "200";


            try
            {
                //Instantiate variables
                var lstFilterAttributes = new List<www.ReadApi.FilterAttribute>();


                //
                if (ExamineManager.TryGetIndex(UmbracoIndexes.ExternalIndexName, out IIndex? index))
                {

                    //Query Examine
                    var queryExecutor = index.Searcher
                        .CreateQuery(IndexTypes.Content)
                        .NodeTypeAlias(Common.Doctype.Product);

                    List<PublishedSearchResult> lstSearchResults = publishedContentQuery.Search(queryExecutor).ToList();


                    //Extract any unique filters
                    foreach (var searchResult in lstSearchResults)
                    {
                        //Get product
                        ContentModels.Product cmProduct = new ContentModels.Product(searchResult.Content, _publishedValueFallback);


                        //Attributes
                        foreach (var attr in cmProduct.RawAttributes)
                        {
                            if (!string.IsNullOrWhiteSpace(attr) &&
                                !lstFilterAttributes.Any(x => x.Category.Equals("Attributes") && x.Attribute.Equals(attr)))
                            {
                                lstFilterAttributes.Add(new www.ReadApi.FilterAttribute()
                                {
                                    Category = "Attributes",
                                    Attribute = attr
                                });
                            }
                        }
                        /*
                            "Category": "Attributes",	    "Attribute": "No Antibiotics Ever / Antibiotic Free",
                            "Category": "Attributes",	    "Attribute": "Halal Certified",
                            "Category": "Attributes",	    "Attribute": "100% Vegetarian Fed with No Animal By-Products",
                            "Category": "Attributes",	    "Attribute": "Fresh CVP",
                            "Category": "Attributes",	    "Attribute": "Gluten Free",
                            "Category": "Attributes",	    "Attribute": "Organic / Non-GMO / Free Range",
                            "Category": "Attributes",	    "Attribute": "Lower Sodium",
                            "Category": "Attributes",	    "Attribute": "Child Nutrition Labeled",
                         */



                        //Brand
                        if (!string.IsNullOrWhiteSpace(cmProduct.RawBrand) &&
                            !lstFilterAttributes.Any(x => x.Category.Equals("Brand") && x.Attribute.Equals(cmProduct.RawBrand)))
                        {
                            lstFilterAttributes.Add(new www.ReadApi.FilterAttribute()
                            {
                                Category = "Brand",
                                Attribute = cmProduct.RawBrand
                            });
                        }
                        /*
                            "Category": "Brand",	    "Attribute": "Coleman Natural®",
                            "Category": "Brand",	    "Attribute": "Shenandoah®",
                            "Category": "Brand",	    "Attribute": "Perdue®",
                            "Category": "Brand",	    "Attribute": "Perdue® Harvestland®",
                            "Category": "Brand",	    "Attribute": "Cookin' Good®",
                            "Category": "Brand",	    "Attribute": "Duck Deli®",
                            "Category": "Brand",	    "Attribute": "Cheney Ranch®",
                            "Category": "Brand",	    "Attribute": "Coleman Organic®",
                            "Category": "Brand",	    "Attribute": "Accento Latino®",
                            "Category": "Brand",	    "Attribute": "No Brand®",
                            "Category": "Brand",	    "Attribute": "Carolina Fare®",
                            "Category": "Brand",	    "Attribute": "Gol-Pak®",
                            "Category": "Brand",	    "Attribute": "Perdue® Harvestland® Organic",
                         */



                        //Cooking Status
                        if (!string.IsNullOrWhiteSpace(cmProduct.RawCookingStatus) &&
                            !lstFilterAttributes.Any(x => x.Category.Equals("Cooking Status") && x.Attribute.Equals(cmProduct.RawCookingStatus)))
                        {
                            lstFilterAttributes.Add(new www.ReadApi.FilterAttribute()
                            {
                                Category = "Cooking Status",
                                Attribute = cmProduct.RawCookingStatus
                            });
                        }
                        /*
                            "Category": "Cooking Status",	    "Attribute": "Ready-to-Cook",
                            "Category": "Cooking Status",	    "Attribute": "Fully Cooked",
                         */



                        //Fresh-Frozen
                        if (!string.IsNullOrWhiteSpace(cmProduct.RawFreshFrozen) &&
                            !lstFilterAttributes.Any(x => x.Category.Equals("Fresh-Frozen") && x.Attribute.Equals(cmProduct.RawFreshFrozen)))
                        {
                            lstFilterAttributes.Add(new www.ReadApi.FilterAttribute()
                            {
                                Category = "Fresh-Frozen",
                                Attribute = cmProduct.RawFreshFrozen
                            });
                        }
                        /*
                            "Category": "Fresh-Frozen",	    "Attribute": "Fresh",
                            "Category": "Fresh-Frozen",	    "Attribute": "Frozen",
                         */



                        //Preparation
                        if (!string.IsNullOrWhiteSpace(cmProduct.RawPreparation) &&
                            !lstFilterAttributes.Any(x => x.Category.Equals("Preparation") && x.Attribute.Equals(cmProduct.RawPreparation)))
                        {
                            lstFilterAttributes.Add(new www.ReadApi.FilterAttribute()
                            {
                                Category = "Preparation",
                                Attribute = cmProduct.RawPreparation
                            });
                        }
                        /*
                            "Category": "Preparation",	    "Attribute": "Bake",
                            "Category": "Preparation",	    "Attribute": "Deep Fry",
                            "Category": "Preparation",	    "Attribute": "Roast",
                            "Category": "Preparation",	    "Attribute": "",
                            "Category": "Preparation",	    "Attribute": "Heat and Serve",
                            "Category": "Preparation",	    "Attribute": "Unspecified",
                            "Category": "Preparation",	    "Attribute": null,
                            "Category": "Preparation",	    "Attribute": "Fry",
                            "Category": "Preparation",	    "Attribute": "Unprepared",
                            "Category": "Preparation",	    "Attribute": "Saute",
                            "Category": "Preparation",	    "Attribute": "Grill",
                            "Category": "Preparation",	    "Attribute": "Rotisserie",
                            "Category": "Preparation",	    "Attribute": "Pan Fry",
                            "Category": "Preparation",	    "Attribute": "Ready to Eat",
                         */



                        //Proteins
                        if (!string.IsNullOrWhiteSpace(cmProduct.RawProtein) &&
                            !lstFilterAttributes.Any(x => x.Category.Equals("Proteins") && x.Attribute.Equals(cmProduct.RawProtein)))
                        {
                            lstFilterAttributes.Add(new www.ReadApi.FilterAttribute()
                            {
                                Category = "Proteins",
                                Attribute = cmProduct.RawProtein
                            });
                        }
                        /*
                            "Category": "Proteins",	    "Attribute": "Pork",
                            "Category": "Proteins",	    "Attribute": "Turkey",
                            "Category": "Proteins",	    "Attribute": "Chicken",
                         */

                    }
                }



                //Narrow list to only distinct items???
                //List<FilterData> filteredFD = MoreLinq.MoreEnumerable.DistinctBy(allFD, n => new { n.Category, n.Attribute }).ToList();

                string? jsonFilterAttributes = JsonConvert.SerializeObject(lstFilterAttributes);

                FileHelperService dragonflyServices = new FileHelperService(_hostingEnvironment);
                dragonflyServices.CreateTextFile(Common.Path.ApiSearchData_FilterAttr, jsonFilterAttributes, true);

                //Final Message to Return
                returnMsg.TimestampEnd = DateTime.Now;
                returnMsg.Success = true;
                returnMsg.Message = string.Format("Filter Attributes Updated Successfully.");
                returnMsg.MessageDetails = string.Format("Operation took {0} seconds.", returnMsg.TimeDuration().Value.TotalSeconds);

            }
            catch (Exception ex)
            {
                returnMsg.TimestampEnd = DateTime.Now;
                returnMsg.Success = true;
                returnMsg.Message = string.Format("Filter Attributes Update Failed.");
                returnMsg.MessageDetails = string.Format("Operation took {0} seconds.", returnMsg.TimeDuration().Value.TotalSeconds);
            }


            return returnMsg;
        }
        private static string FormatJson(string json = "")
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }



        private StatusMessage UpdateCategories()
        {
            //  INSTANTIATE IMPORT/SAVE STATUS'
            var returnMsg = new StatusMessage();
            returnMsg.TimestampStart = DateTime.Now;
            returnMsg.RunningFunctionName = "ReadApiController.cs | UpdateCategories()";
            returnMsg.Success = true;
            returnMsg.Code = "200";


            try
            {
                //Instantiate variables
                var lstFoodProductTypes = new www.Models.FoodProductTypes();


                //
                if (ExamineManager.TryGetIndex(UmbracoIndexes.ExternalIndexName, out IIndex? index))
                {

                    //Query Examine
                    var queryExecutor = index.Searcher
                        .CreateQuery(IndexTypes.Content)
                        .NodeTypeAlias(Common.Doctype.Product);

                    List<PublishedSearchResult> lstSearchResults = publishedContentQuery.Search(queryExecutor).ToList();


                    //Extract any unique filters
                    foreach (var searchResult in lstSearchResults)
                    {
                        //Get product
                        ContentModels.Product cmProduct = new ContentModels.Product(searchResult.Content, _publishedValueFallback);


                        //Attributes
                        bool isSubcategory = false;
                        string category = "";
                        foreach (string attr in cmProduct.AttributeProductTypes)
                        {
                            /*
                             if 1st record
                                category = attr
                                if category !exist
                                    add category
                            else
                                get category
                                if subcategory !exist
                                    add subcategory                             
                             */


                            //Clean attribute
                            string attribute = attr.Replace("ProductType-", "");

                            if (!isSubcategory)
                            {
                                if (!lstFoodProductTypes.Types.Any(x => x.Code == attribute))
                                {
                                    //Create category and add to list.
                                    www.Models.FoodProductType foodProductType = new www.Models.FoodProductType()
                                    {
                                        Name = cmProduct.TradeItemKeywords?.ToList()[0],
                                        Code = attribute
                                    };
                                    lstFoodProductTypes.Types.Add(foodProductType);
                                }
                                category = attribute;
                                isSubcategory = true;
                            }
                            else
                            {
                                //Get category
                                var productType = lstFoodProductTypes.Types?.FirstOrDefault(x => x.Code == category);
                                if (productType != null)
                                {
                                    if (!productType.SubTypes.Any(x => x.Code == attribute))
                                    {
                                        //Add subcategory
                                        lstFoodProductTypes.Types?.FirstOrDefault(x => x.Code == category)?.SubTypes.Add(new FoodProductType()
                                        {
                                            Name = cmProduct.TradeItemKeywords?.ToList()[1],
                                            Code = attribute
                                        });
                                    }
                                }
                            }

                        }
                    }
                }


                //Alphabetize lists
                lstFoodProductTypes.Types = lstFoodProductTypes.Types.OrderBy(x => x.Name).ToList();
                for (var i = 0; i < lstFoodProductTypes.Types.Count(); i++)
                {
                    lstFoodProductTypes.Types[i].SubTypes = lstFoodProductTypes.Types[i].SubTypes.OrderBy(x => x.Name).ToList();
                }


                //Save list in categories.json
                FileHelperService dragonflyServices = new FileHelperService(_hostingEnvironment);
                string? jsonFilterAttributes = JsonConvert.SerializeObject(lstFoodProductTypes);
                dragonflyServices.CreateTextFile(Common.Path.ApiSearchData_Categories, jsonFilterAttributes, true);

                //Final Message to Return
                returnMsg.TimestampEnd = DateTime.Now;
                returnMsg.Success = true;
                returnMsg.Message = string.Format("Categories Updated Successfully.");
                returnMsg.MessageDetails = string.Format("Operation took {0} seconds.", returnMsg.TimeDuration().Value.TotalSeconds);

            }
            catch (Exception ex)
            {
                returnMsg.TimestampEnd = DateTime.Now;
                returnMsg.Success = true;
                returnMsg.Message = string.Format("Categories Update Failed.");
                returnMsg.MessageDetails = string.Format("Operation took {0} seconds.", returnMsg.TimeDuration().Value.TotalSeconds);
            }


            return returnMsg;
        }
        #endregion



        #region "Helpers"
        private string ConvertQualifier(string qualifier)
        {
            switch (qualifier.ToLower())
            {
                case "grm":
                    return "g";

                case "mc":
                    return "mcg";

                case "mgm":
                    return "mg";

                case "lbr":
                    return "LB.";

                default:
                    return "";
            }
        }
        private bool IsFullCooked(Item2 Item)
        {
            //Get values to determine if product is fully cooked or not.
            var productName = Item?.AdditionalDescription?.FirstOrDefault()?.Value?.ToLower() ?? "";
            var description = Item?.MarketingMessage?.FirstOrDefault()?.TradeItemMarketingMessage?.FirstOrDefault()?.Value?.ToLower() ?? "";
            var prepType = Item?.FoodAndBevPreparationInfo?.FirstOrDefault()?.PreparationType?.ToLower() ?? "";
            var prepStatus = Item?.NutrientInformation?.FirstOrDefault(x => x.NutrientBasisQuantityTypeCode.Equals("BY_SERVING"))?.PreparationStateCode?.ToLower() ?? "";



            if (productName.Contains("fully cooked") || productName.Contains("fully-cooked"))
            {
                return true;
            }
            else if (productName.Contains("ready to cook") || productName.Contains("ready-to-cook"))
            {
                return false;
            }
            else if (description.Contains("fully cooked") || description.Contains("fully-cooked"))
            {
                return true;
            }
            else if (description.Contains("ready to cook") || description.Contains("ready-to-cook"))
            {
                return false;
            }
            else if (prepType == "ready to eat" || prepType == "ready-to-eat" || prepType == "ready_to_eat" ||
                prepType == "heat and serve" || prepType == "heat-and-serve" || prepType == "heat_and_serve")
            {
                return true;
            }
            else if (prepStatus.Equals("unprepared"))
            {
                return false;
            }
            else
            {
                return false;
            }
        }
        private string SetBrandName(string brandName, string productTitle)
        {
            var finalBrand = "";

            if (productTitle.ToLower().Contains("harvestland") && productTitle.ToLower().Contains("organic"))
            {
                finalBrand = "Perdue® Harvestland® Organic";
            }
            else if (brandName.ToLower() == "harvestland")
            {
                finalBrand = "Perdue® Harvestland®";
            }
            else
            {
                finalBrand = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(brandName.ToLower()) + "®";
            }

            return finalBrand;
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
        #endregion



        #region "Models"
        private class ProductCount
        {
            public int count { get; set; }
        }
        private class AgencyId()
        {
            public string? Site { get; set; }
            public string? Id { get; set; }
        }
        #endregion


    }
}






////      https://localhost:44369/umbraco/api/ReadApi/test
//[HttpPost]
//public ContentResult test(string productId = "")
//{
//    var responseType = "text/html; charset=utf-8";
//    List<string> lst = new List<string>();
//    lst.Add(productId);

//    return new ContentResult
//    {
//        ContentType = responseType,
//        Content = Newtonsoft.Json.JsonConvert.SerializeObject(lst),
//        StatusCode = (int)HttpStatusCode.OK
//    };
//}
