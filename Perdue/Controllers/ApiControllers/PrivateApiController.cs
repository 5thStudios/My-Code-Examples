using Examine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common;
using www.Models;
using ContentModels = www.Models.PublishedModels;




namespace www.Controllers
{
	public class PrivateApiController : UmbracoAuthorizedApiController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private UmbracoHelper UmbracoHelper;
        private IContentService _ContentService;
        private ILogger<PrivateApiController> logger;
        private Umbraco.Cms.Core.Hosting.IHostingEnvironment _hostingEnvironment;
        private string _MappedRootPath;
        private IOptionsMonitor<WebRoutingSettings> _webRoutingSettings;
        private IExamineManager ExamineManager;
        private readonly IPublishedContentQuery publishedContentQuery;

        public PrivateApiController(
                ILogger<PrivateApiController> _logger,
                ICompositeViewEngine compositeViewEngine,
                IUmbracoContextAccessor umbracoContextAccessor,
                UmbracoHelper _UmbracoHelper,
                IPublishedValueFallback publishedValueFallback,
                IContentService ContentService,
                IExamineManager _ExamineManager,
                IPublishedContentQuery _publishedContentQuery,
                IOptionsMonitor<WebRoutingSettings> webRoutingSettings,
                Umbraco.Cms.Core.Hosting.IHostingEnvironment hostingEnvironment
             )
        {
            _publishedValueFallback = publishedValueFallback;
            UmbracoHelper = _UmbracoHelper;
            logger = _logger;
            _hostingEnvironment = hostingEnvironment;
            _MappedRootPath = hostingEnvironment.MapPathWebRoot("/");
            _ContentService = ContentService;
            _webRoutingSettings = webRoutingSettings;
            publishedContentQuery = _publishedContentQuery;
            ExamineManager = _ExamineManager ?? throw new ArgumentNullException(nameof(_ExamineManager));
        }







        ////      /umbraco/backoffice/Api/PrivateApi/test
        //[HttpPost]
        //public ContentResult test(string productId = "")
        //{
        //    var responseType = "text/html; charset=utf-8";
        //    List<string> lst = new List<string>();
        //    lst.Add("Private");
        //    lst.Add("API");
        //    lst.Add("Working");
        //    lst.Add(productId);

        //    return new ContentResult
        //    {
        //        ContentType = responseType,
        //        Content = Newtonsoft.Json.JsonConvert.SerializeObject(lst),
        //        StatusCode = (int)HttpStatusCode.OK
        //    };
        //}







        #region API Calls
        /// /umbraco/backoffice/Api/PrivateApi/ViewProductData?Data=ListAll&NodeId=xxx <summary>
        /// /umbraco/backoffice/Api/PrivateApi/ViewProductData?Data=ListAll
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="NodeId"></param>
        /// <returns></returns>
        [HttpGet]
        public ContentResult ViewProductData(string Data = "ListAll", string NodeId = "")
        {
            var result = new StringBuilder();
            var responseType = "text/html; charset=utf-8";

            if (Data == "ListAll")
            {
                result.AppendLine("<h2>Food Product Feed Data</h2>");
                result.AppendLine("<h3 style=\"margin-bottom:0px;\">Choose a Product</h3>");
                result.AppendLine("<p style=\"margin-top:0px;\">Select a product from the list below to view the Raw API Data ('JSON'):</p>");
                result.AppendLine(ListAllProducts());
            }
            else if (Data == "Json" || NodeId != "")
            {
                //
                IPublishedContent ipProduct = UmbracoHelper.Content(NodeId);
                ContentModels.Product cmProduct = new ContentModels.Product(ipProduct, _publishedValueFallback);

                if (!string.IsNullOrWhiteSpace(cmProduct.JsonData))
                {
                    result.Clear();
                    result.AppendLine(cmProduct.JsonData);
                    responseType = "application/json";
                }
                else
                {
                    result.AppendLine(string.Format("<p>JSON is NULL for Product with Ref Id {0}</p>", NodeId));
                }
            }


            return new ContentResult
            {
                ContentType = responseType,
                Content = result.ToString(),
                StatusCode = (int)HttpStatusCode.OK
            };

        }


        /// /umbraco/backoffice/Api/PrivateApi/ViewProductAttributes
        [HttpGet]
        public ContentResult ViewProductAttributes()
        {
            var result = new StringBuilder();
            var responseType = "text/html; charset=utf-8";
            var tableStyling = @"<style>
                body {
                    font-family: arial, sans-serif;
                    font-size: 10pt;
                }

                table {
                    border-collapse: collapse;
                    width: 50%;
                    margin-bottom: 50px;
                }

                td, th {
                    border: 1px solid #dddddd;
                    text-align: left;
                    padding: 8px;
                }

                tr:nth-child(even) {
                    background-color: #dddddd;
                }
                </style>";

            result.AppendLine(tableStyling);
            result.AppendLine("<h1>Attributes for All Products</h1>");

            try
            {
                //Obtain each product list node
                var ipHomeProductSection = UmbracoHelper.ContentAtRoot()?.FirstOrDefault(x => x.ContentType.Alias == Common.Doctype.Home)?.ChildrenOfType(Common.Doctype.ProductsSection)?.FirstOrDefault();
                var ipHomeCHEProductSection = UmbracoHelper.ContentAtRoot()?.FirstOrDefault(x => x.ContentType.Alias == Common.Doctype.HomeCHE)?.ChildrenOfType(Common.Doctype.ProductsSection)?.FirstOrDefault();
                var ipHomeCOLProductSection = UmbracoHelper.ContentAtRoot()?.FirstOrDefault(x => x.ContentType.Alias == Common.Doctype.HomeCOL)?.ChildrenOfType(Common.Doctype.ProductsSection)?.FirstOrDefault();



                //List Perdue Products
                result.AppendLine("<h2>Perdue Products</h2>");
                foreach (var ipProduct in ipHomeProductSection?.Children?.Where(x => x.ContentType.Alias == Common.Doctype.Product))
                {
                    //
                    ContentModels.Product cmProduct = new ContentModels.Product(ipProduct, _publishedValueFallback);

                    var jsonLink = string.Format("/umbraco/backoffice/Api/PrivateApi/ViewProductData?Data=Json&NodeId={0}", cmProduct.Id);
                    result.AppendLine($"<h3 style=\"margin-bottom:0;\">{cmProduct.ProductCode} - {cmProduct.Title}</h3>");
                    result.AppendLine($"<p>[{cmProduct.Gtin}] <a href=\"{jsonLink}\" target=\"_blank\">View Json Data</a></p>");
                    result.AppendLine(ProductAttributesTable(cmProduct));
                }

                //List Cheney Products
                result.AppendLine("<h2>Cheney Products</h2>");
                foreach (var ipProduct in ipHomeCHEProductSection?.Children?.Where(x => x.ContentType.Alias == Common.Doctype.Product))
                {
                    //
                    ContentModels.Product cmProduct = new ContentModels.Product(ipProduct, _publishedValueFallback);

                    var jsonLink = string.Format("/umbraco/backoffice/Api/PrivateApi/ViewProductData?Data=Json&NodeId={0}", cmProduct.Id);
                    result.AppendLine($"<h3 style=\"margin-bottom:0;\">{cmProduct.ProductCode} - {cmProduct.Title}</h3>");
                    result.AppendLine($"<p>[{cmProduct.Gtin}] <a href=\"{jsonLink}\" target=\"_blank\">View Json Data</a></p>");
                    result.AppendLine(ProductAttributesTable(cmProduct));
                }

                //List Coleman Products
                result.AppendLine("<h2>Coleman Products</h2>");
                foreach (var ipProduct in ipHomeCOLProductSection?.Children?.Where(x => x.ContentType.Alias == Common.Doctype.Product))
                {
                    //
                    ContentModels.Product cmProduct = new ContentModels.Product(ipProduct, _publishedValueFallback);

                    var jsonLink = string.Format("/umbraco/backoffice/Api/PrivateApi/ViewProductData?Data=Json&NodeId={0}", cmProduct.Id);
                    result.AppendLine($"<h3 style=\"margin-bottom:0;\">{cmProduct.ProductCode} - {cmProduct.Title}</h3>");
                    result.AppendLine($"<p>[{cmProduct.Gtin}] <a href=\"{jsonLink}\" target=\"_blank\">View Json Data</a></p>");
                    result.AppendLine(ProductAttributesTable(cmProduct));
                }



            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                result.AppendLine("<p>Unable to get a list of Products, please ");
                result.AppendLine(
                    "<a href=\"/umbraco/backoffice/Api/PrivateApi/DownloadAndUpdateProducts\" target=\"_blank\">Download & Update Products</a>");
                result.AppendLine(" now. </p>");
            }


            return new ContentResult
            {
                ContentType = responseType,
                Content = result.ToString(),
                StatusCode = (int)HttpStatusCode.OK
            };

        }


        /// /umbraco/backoffice/Api/PrivateApi/ExportAllProducts
        //[HttpGet]
        //public ContentResult ExportAllProducts(int ProductsNode = 0)
        //{
        //    //Instantiate variables
        //    var sb = new StringBuilder();
        //    //var tableData = new StringBuilder();
        //    //var timestamp = DateTime.Now.ToString("yyyy-M-d-HH-mm");
        //    //Dragonfly.UmbracoServices.FileHelperService dragonflyServices = new FileHelperService(_hostingEnvironment);
        //    //string responseFileNameFormat = "{0}{1}.json";


        //    ////FIND NODES TO DISPLAY
        //    //var startNode = UmbracoHelper.Content(ProductsNode) as ProductsSection;
        //    ////var allNodes = startNode?.Children.ToProducts().ToList();

        //    ////FileName
        //    //var siteName = startNode.ProductsWebsiteCode;
        //    //var fileName = $"{siteName}-FoodProducts-{timestamp}.csv";
        //    //var errorMsg = "";

        //    ////Header Row
        //    //var header = new StringBuilder();
        //    //header.Append($"\"#\",");
        //    //header.Append($"\"Product Name\",");
        //    //header.Append($"\"Product Code\",");
        //    //header.Append($"\"Product Type : SubType\",");
        //    //header.Append($"\"Search Attributes\",");
        //    //header.Append($"\"GTIN\",");
        //    //header.Append($"\"API Ref Id\",");
        //    //header.Append($"\"Database Last Changed\",");
        //    //header.Append($"\"Node ID\",");
        //    //header.Append($"\"Node Create Date\",");
        //    //header.Append($"\"Node Update Date\"");

        //    ////header = header.Replace(Environment.NewLine, "");
        //    //// header = header.Replace(" ", "");
        //    //tableData.AppendLine(header.ToString());

        //    ////Get Data
        //    //if (errorMsg != "")
        //    //{
        //    //    tableData.AppendLine(errorMsg);
        //    //}
        //    //else
        //    //{
        //    //    var counter = 1;
        //    //    foreach (var ipProduct in startNode.Children.Where(x => x.ContentType.Alias == Common.Doctype.Product))
        //    //    {
        //    //        ContentModels.Product cmProduct = new ContentModels.Product(ipProduct, _publishedValueFallback);

        //    //        //Obtain FoodProduct
        //    //        string? ApiReferenceId = cmProduct.ApiReferenceId;
        //    //        var prodFileName = string.Format(responseFileNameFormat, Common.Path.ApiFoodProductSavePath, ApiReferenceId);
        //    //        Models.FoodProduct? item = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.FoodProduct>(dragonflyServices.GetTextFileContents(prodFileName));



        //    //        var attribs = "";
        //    //        var lstAttribs = new List<string>();
        //    //        foreach (var att in item.SearchData.FilterAttributes)
        //    //        {
        //    //            var txt = $"[{att.Category}: {att.Attribute}]";
        //    //            lstAttribs.Add(txt.Replace("\"", "").Replace(",", ""));
        //    //        }
        //    //        attribs = Common.RemoveNonANCIICharacters(string.Join(" ", lstAttribs.Distinct()));

        //    //        var prodTypes = "";
        //    //        if (item.ProductTypeInfo != null && !item.ProductTypeInfo.CategoryName.StartsWith("~"))
        //    //        {
        //    //            prodTypes = $"{item.ProductTypeInfo.CategoryName} : {item.ProductTypeInfo.SubCategoryName}";
        //    //        }
        //    //        else
        //    //        {
        //    //            prodTypes = "[NONE]";
        //    //        }

        //    //        var prodName = Common.RemoveNonANCIICharacters(item.ProductDisplayName?.Replace("\"", "").Replace(",", "") ?? "");

        //    //        var itemLine = new StringBuilder();
        //    //        itemLine.Append($"{counter},");
        //    //        itemLine.Append($"=\"{prodName}\",");
        //    //        itemLine.Append($"{item.ProductCode},");
        //    //        itemLine.Append($"=\"{prodTypes.Replace("\"", "").Replace(",", "")}\",");
        //    //        itemLine.Append($"{attribs},"); //itemLine.Append($"=\"{attribs}\",");
        //    //        itemLine.Append($"=\"{item.TradeUnitGtin}\",");
        //    //        itemLine.Append($"=\"{item.ItemReferenceId}\",");
        //    //        itemLine.Append($"{item.DatabaseModificationDates?.lastChangeDateTime},");
        //    //        itemLine.Append($"{cmProduct.Id},");
        //    //        itemLine.Append($"{cmProduct.CreateDate},");
        //    //        itemLine.Append($"{cmProduct.UpdateDate}");
        //    //        tableData.AppendLine(itemLine.ToString());

        //    //        counter++;
        //    //    }
        //    //}

        //    //sb.Append(tableData);


        //    var responseType = "text/csv; charset=utf-8";
        //    return new ContentResult
        //    {
        //        ContentType = responseType,
        //        Content = sb.ToString(),
        //        StatusCode = (int)HttpStatusCode.OK
        //    };

        //}
        #endregion




        #region "Methods- View Products"
        private string ListAllProducts()
        {
            var sb = new StringBuilder();

            try
            {
                //Obtain each product list node
                var ipHomeProductSection = UmbracoHelper.ContentAtRoot()?.FirstOrDefault(x => x.ContentType.Alias == Common.Doctype.Home)?.ChildrenOfType(Common.Doctype.ProductsSection)?.FirstOrDefault();
                var ipHomeCHEProductSection = UmbracoHelper.ContentAtRoot()?.FirstOrDefault(x => x.ContentType.Alias == Common.Doctype.HomeCHE)?.ChildrenOfType(Common.Doctype.ProductsSection)?.FirstOrDefault();
                var ipHomeCOLProductSection = UmbracoHelper.ContentAtRoot()?.FirstOrDefault(x => x.ContentType.Alias == Common.Doctype.HomeCOL)?.ChildrenOfType(Common.Doctype.ProductsSection)?.FirstOrDefault();



                //List Perdue Products
                sb.AppendLine("<h4 style=\"margin-bottom:0px;\">Perdue Products</h4>");
                sb.AppendLine("<ol style=\"margin-top:0px;\">");
                foreach (var ipProduct in ipHomeProductSection?.Children?.Where(x => x.ContentType.Alias == Common.Doctype.Product))
                {
                    //
                    ContentModels.Product cmProduct = new ContentModels.Product(ipProduct, _publishedValueFallback);
                    var rawLink = $"/umbraco/backoffice/Api/PrivateApi/ViewProductData?Data=Json&NodeId={cmProduct.Id}";

                    sb.AppendLine("<li>");
                    sb.AppendLine($"<a href=\"{rawLink}\" target=\"_blank\">JSON</a>");
                    sb.AppendLine($"  <b>{cmProduct.Title}</b>  ({cmProduct.ProductCode})  [{cmProduct.Gtin}]");
                    sb.AppendLine("</li>");
                }
                sb.AppendLine("</ol>");
                sb.AppendLine("<br />");



                //List Coleman Products
                sb.AppendLine("<h4 style=\"margin-bottom:0px;\">Coleman Products</h4>");
                sb.AppendLine("<ol style=\"margin-top:0px;\">");
                foreach (var ipProduct in ipHomeCOLProductSection?.Children?.Where(x => x.ContentType.Alias == Common.Doctype.Product))
                {
                    //
                    ContentModels.Product cmProduct = new ContentModels.Product(ipProduct, _publishedValueFallback);
                    var rawLink = $"/umbraco/backoffice/Api/PrivateApi/ViewProductData?Data=Json&NodeId={cmProduct.Id}";

                    sb.AppendLine("<li>");
                    sb.AppendLine($"<a href=\"{rawLink}\" target=\"_blank\">JSON</a>");
                    sb.AppendLine($"  <b>{cmProduct.Title}</b>  ({cmProduct.ProductCode})  [{cmProduct.Gtin}]");
                    sb.AppendLine("</li>");
                }
                sb.AppendLine("</ol>");
                sb.AppendLine("<br />");



                //List Cheney Products
                sb.AppendLine("<h4 style=\"margin-bottom:0px;\">Cheney Products</h4>");
                sb.AppendLine("<ol style=\"margin-top:0px;\">");
                foreach (var ipProduct in ipHomeCHEProductSection?.Children?.Where(x => x.ContentType.Alias == Common.Doctype.Product))
                {
                    //
                    ContentModels.Product cmProduct = new ContentModels.Product(ipProduct, _publishedValueFallback);
                    var rawLink = $"/umbraco/backoffice/Api/PrivateApi/ViewProductData?Data=Json&NodeId={cmProduct.Id}";

                    sb.AppendLine("<li>");
                    sb.AppendLine($"<a href=\"{rawLink}\" target=\"_blank\">JSON</a>");
                    sb.AppendLine($"  <b>{cmProduct.Title}</b>  ({cmProduct.ProductCode})  [{cmProduct.Gtin}]");
                    sb.AppendLine("</li>");
                }
                sb.AppendLine("</ol>");

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                sb.AppendLine("<p style=\"margin-top:0px;\">Unable to get a list of Products, please ");
                sb.AppendLine("<a href=\"/umbraco/backoffice/Api/PrivateApi/DownloadAndUpdateProducts\" target=\"_blank\">Download & Update Products</a>");
                sb.AppendLine(" now. </p>");
            }

            return sb.ToString();
        }
        #endregion



        #region "Methods- Product Attributes"
        private string ProductAttributesTable(ContentModels.Product cmProduct)
        {
            var sb = new StringBuilder();

            try
            {
                sb.AppendLine("<table>");
                sb.AppendLine("<tr>");
                sb.AppendLine("<th>Category</th>");
                sb.AppendLine("<th>Attribute</th>");
                sb.AppendLine("</tr>");

                //
                sb.AppendLine($"<tr><td>Preparation</td><td>{Common.SplitCamelCase(cmProduct.AttributePreparation.Replace("Preparation-", ""))}</td></tr>");
                sb.AppendLine($"<tr><td>Cooking Status</td><td>{Common.SplitCamelCase(cmProduct.AttributeCookingStatus.Replace("CookingStatus-", ""))}</td></tr>");
                sb.AppendLine($"<tr><td>Fresh-Frozen</td><td>{Common.SplitCamelCase(cmProduct.AttributeFreshFrozen.Replace("Fresh-Frozen-", ""))}</td></tr>");
                sb.AppendLine($"<tr><td>Proteins</td><td>{Common.SplitCamelCase(cmProduct.AttributeProtein.Replace("Proteins-", ""))}</td></tr>");
                sb.AppendLine($"<tr><td>Brands</td><td>{Common.SplitCamelCase(cmProduct.AttributeBrand.Replace("Brand-", ""))}</td></tr>");


                sb.AppendLine($"<tr valign=\"top\"><td>Attributes</td><td>");
                foreach (var attrib in cmProduct.Attributes)
                {
                    sb.AppendLine($"<div>{Common.SplitCamelCase(attrib.Replace("Attributes-", ""))}</div>");
                }
                sb.AppendLine($"</td></tr>");


                sb.AppendLine($"<tr valign=\"top\"><td>Product Type</td><td>");
                foreach (var attrib in cmProduct.AttributeProductTypes)
                {
                    sb.AppendLine($"<div>{Common.SplitCamelCase(attrib.Replace("ProductType-", ""))}</div>");
                }
                sb.AppendLine($"</td></tr>");


                sb.AppendLine("</table>");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                sb.AppendLine("<p>ERROR: Unable to get a list of Attributes for Product. </p>");
            }

            return sb.ToString();
        }
        #endregion


        #region "Methods- Export Products"
        /// <summary>
        /// Used for Exporting text data to a file
        /// </summary>
        /// <param name="StringData"></param>
        /// <param name="OutputFileName"></param>
        /// <param name="MediaType"></param>
        /// <returns></returns>
        //private static HttpResponseMessage StringBuilderToFile(StringBuilder StringData, string OutputFileName = "Export.csv", string MediaType = "text/csv")
        //{
        //    var mediaType = MediaType;
        //    HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);

        //    var encoding = Encoding.UTF8;
        //    var content = encoding.GetString(new byte[] { 0xEF, 0xBB, 0xBF }) + StringData.ToString();

        //    result.Content = new StringContent(content, encoding, mediaType);
        //    result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment") { FileName = OutputFileName };
        //    return result;
        //}

        #endregion
    }
}




#region Clean Up Node Versions

//  JF | IF THIS IS NEEDED, THEN WE NEED TO CREATE A PUBLIC API CONTROLLER AND ADD THIS TO IT.  NEWEST VSN OF UMBRACO SHOULD DEAL WITH THIS AUTOMATICALLY THOUGH.

///// /umbraco/api/PublicApi/CleanupProductNodeVersions?MaxToKeep=3
//[System.Web.Http.AcceptVerbs("GET")]
//public HttpResponseMessage CleanupProductNodeVersions(int MaxToKeep)
//{
//    LogHelper.Info<PublicApiController>($"CleanupProductNodeVersions() Started [MaxToKeep: {MaxToKeep}]");

//    var result = new StringBuilder();
//    var responseType = "application/json";

//    var returnMsg = new StatusMessage();
//    returnMsg.Success = true; //Assume success
//    var messages = new List<string>();
//    var counter = 0;
//    int errorCounter = 0;

//    var numToKeep = MaxToKeep > 2 ? MaxToKeep : 2; //Minimum of two

//    var muvs = new ManualUnVersionService(true);

//    var productSections = SiteHelpers.GetAllProductSections(Umbraco);

//    foreach (var section in productSections)
//    {
//        var prods = section.Children.Where(n => n.ContentType.Alias == Product.ModelTypeAlias).ToList();

//        messages.Add($"=== Product Section for '{section.Parent.Name}' ({prods.Count()} product nodes) ===");

//        foreach (var prodNode in prods)
//        {
//            var nodeStatus = muvs.UnVersion(prodNode, numToKeep);
//            returnMsg.InnerStatuses.Add(nodeStatus);
//            if (nodeStatus.Success)
//            {
//                counter++;
//                messages.Add($"{prodNode.Name} cleaned");
//            }
//            else
//            {
//                errorCounter++;
//                messages.Add($"{prodNode.Name} - {nodeStatus.Message}");
//            }

//        }

//    }

//    returnMsg.Message = $"{counter} Product nodes cleaned. {errorCounter} Errors ";
//    returnMsg.RelatedObject = messages;


//    //Serialize StatusMessage
//    string json = JsonConvert.SerializeObject(returnMsg);
//    result.AppendLine(json);

//    LogHelper.Info<PublicApiController>("CleanupProductNodeVersions() Completed ");

//    return new HttpResponseMessage()
//    {
//        Content = new StringContent(
//            result.ToString(),
//            Encoding.UTF8,
//            responseType
//        )
//    };

//}

#endregion