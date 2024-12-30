using Examine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Controllers;
using www.Models.Import;
using static Umbraco.Cms.Core.Constants;



namespace www.Controllers
{
    public class PublicApiController : UmbracoApiController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private UmbracoHelper UmbracoHelper;
        private IContentService _ContentService;
        private ILogger<PublicApiController> logger;
        private Umbraco.Cms.Core.Hosting.IHostingEnvironment _hostingEnvironment;
        private string _MappedRootPath;
        private IOptionsMonitor<WebRoutingSettings> _webRoutingSettings;
        private IExamineManager ExamineManager;
        private readonly IPublishedContentQuery publishedContentQuery;

        public PublicApiController(
                ILogger<PublicApiController> _logger,
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




        #region API Calls
        //      https://localhost:44358/umbraco/api/PublicApi/GetAllTest
        public IEnumerable<string> GetAllTest()
        {
            return new[] { "Table", "Chair", "Desk", "Computer" };
        }


        //      https://localhost:44358/umbraco/api/PublicApi/ImportSitemapStructure
        public string ImportSitemapStructure()
        {
            try
            {
                //Obtain sitemap data
                var file = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "importData", "sitemap.txt");
                string data = System.IO.File.ReadAllText(file);


                //Convert to object
                List<LstPg> Sitemap = JsonConvert.DeserializeObject<List<LstPg>>(data);


                //Get root node ic
                //var icParent = _ContentService.GetById(4012);


                //
                foreach (var pgData in Sitemap.FirstOrDefault().LstPgs)
                {
                    CreatePg(4012, pgData); //Id of root home node.
                }


                return "Completed Successfully"; /*Newtonsoft.Json.JsonConvert.SerializeObject(Sitemap);*/
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        private void CreatePg(int parentId, LstPg pgData)
        {
            IContent icRecipe = _ContentService.Create(pgData.Name, parentId, pgData.Doctype);
            //icRecipe.SetValue("Description", record.Description);

            //Save and publish recipe
            PublishResult result = _ContentService.SaveAndPublish(icRecipe);


            foreach (var pg in pgData.LstPgs)
            {
                CreatePg(result.Content.Id, pg); 
            }
        }
        #endregion
    }
}
