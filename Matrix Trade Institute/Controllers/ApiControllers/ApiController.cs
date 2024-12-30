using Examine;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Infrastructure.Examine;
using static Umbraco.Cms.Core.Constants;
using www.Models;
using static Umbraco.Cms.Core.Constants.HttpContext;
using NPoco.fastJSON;
using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core.Web;
using static Umbraco.Cms.Core.Collections.TopoGraph;
using Umbraco.Cms.Web.Common.UmbracoContext;
using ContentModels = Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco.Cms.Core.Models.ContentEditing;



namespace www.Controllers
{
    public class ApiController : UmbracoApiController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private UmbracoHelper UmbracoHelper;
        private IContentService _ContentService;
        private ILogger<ApiController> logger;
        private IExamineManager ExamineManager;
        private readonly IPublishedContentQuery publishedContentQuery;
        private Umbraco.Cms.Core.Hosting.IHostingEnvironment _hostingEnvironment;
        private readonly IUmbracoContextFactory UmbracoContextFactory;


        public ApiController(
                ILogger<ApiController> _logger,
                UmbracoHelper _UmbracoHelper,
                IPublishedValueFallback publishedValueFallback,
                IContentService ContentService,
                IExamineManager _ExamineManager,
                IPublishedContentQuery _publishedContentQuery,
                Umbraco.Cms.Core.Hosting.IHostingEnvironment hostingEnvironment,
                IUmbracoContextFactory umbracoContextFactory
             )
        {
            _publishedValueFallback = publishedValueFallback;
            UmbracoHelper = _UmbracoHelper;
            logger = _logger;
            _ContentService = ContentService;
            _hostingEnvironment = hostingEnvironment;
            publishedContentQuery = _publishedContentQuery;
            UmbracoContextFactory = umbracoContextFactory;
            ExamineManager = _ExamineManager ?? throw new ArgumentNullException(nameof(_ExamineManager));
        }




        #region API Calls  
        //      https://localhost:44313/umbraco/api/Api/ApiTest
        [HttpGet]
        public ContentResult ApiTest()
        {
            List<string> lst = new List<string>()
            {
                "t1","t2"
            };


            //  Return error message
            return new ContentResult
            {
                ContentType = "application/json",
                Content = Newtonsoft.Json.JsonConvert.SerializeObject(lst),
                StatusCode = (int)HttpStatusCode.OK
            };
        }



        //      https://localhost:44313/umbraco/api/Api/ReadJson
        [HttpGet]
        public ContentResult ReadJson()
        {
            //
            string json = "";
            using (StreamReader reader = new StreamReader("blogs.json"))
            {
                json = reader.ReadToEnd();
            }
            var lstBlogPosts = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BlogRecord>>(json);


            //  Return message
            return new ContentResult
            {
                ContentType = "application/json",
                Content = Newtonsoft.Json.JsonConvert.SerializeObject(lstBlogPosts),
                StatusCode = (int)HttpStatusCode.OK
            };
        }



        //      https://localhost:44313/umbraco/api/Api/ImportBlogPosts
        [HttpGet]
        public ContentResult ImportBlogPosts()
        {
            //Instantiate variables
            string json = "";
            List<BlogRecord>? lstBlogPosts = null;
            IPublishedContent? ipBlog = null;


            try
            {
                //Get blog node
                using (var umbracoContextReference = UmbracoContextFactory.EnsureUmbracoContext())
                {
                    ipBlog = umbracoContextReference.UmbracoContext?.Content?.GetById(1069);
                }


                //Deserialize blog posts
                using (StreamReader reader = new StreamReader("blogs.json"))
                {
                    json = reader.ReadToEnd();
                }
                lstBlogPosts = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BlogRecord>>(json);


                /* Loop through each record and get Category
                 *      if Category node ! exist, create it
                 * 
                 * Loop through each record
                 *      get Category node
                 *      if post ! exist, create it
                 */


                foreach (var post in lstBlogPosts!)
                {
                    //If category does not exist, create it.
                    if (!ipBlog!.Children.Any(x => x.Name == post.Category))
                    {
                        //Save data to Umbraco
                        IContent? icNewCategory = _ContentService.CreateContent(post.Category!, Umbraco.Cms.Core.Udi.Create("document", ipBlog.Key), ContentModels.Category.ModelTypeAlias);
                        var saveResult = _ContentService.SaveAndPublish(icNewCategory);
                        if (!saveResult.Success)
                        {
                            //  Return message
                            return new ContentResult
                            {
                                ContentType = "application/json",
                                Content = saveResult.Result.ToString() + " | " + saveResult.EventMessages,
                                StatusCode = (int)HttpStatusCode.OK
                            };
                        }
                    }


                    //Obtain category node
                    var ipCategory = ipBlog.Children.FirstOrDefault(x => x.Name == post.Category);
                    //var icCategory = _ContentService.GetById(ipCategory!.Id);


                    //Create post node
                    if (!ipCategory!.Children.Any(x => x.Name == post.Name))
                    {
                        IContent? icNewPost = _ContentService.CreateContent(post.Name!, Umbraco.Cms.Core.Udi.Create("document", ipCategory.Key), ContentModels.Post.ModelTypeAlias);
                        icNewPost.SetValue("author", post.Author);
                        icNewPost.SetValue("excerpt", post.Excerpt);
                        icNewPost.SetValue("publishedDate", post.PublishedDate);
                        icNewPost.SetValue("content", post.Content);

                        var saveResult = _ContentService.SaveAndPublish(icNewPost);
                        if (!saveResult.Success)
                        {
                            //  Return message
                            return new ContentResult
                            {
                                ContentType = "application/json",
                                Content = saveResult.Result.ToString() + " | " + saveResult.EventMessages,
                                StatusCode = (int)HttpStatusCode.OK
                            };
                        }
                    }
                }



//  Return message
return new ContentResult
{
    ContentType = "application/json",
    Content = Newtonsoft.Json.JsonConvert.SerializeObject(lstBlogPosts),
    StatusCode = (int)HttpStatusCode.OK
};
            }
            catch (Exception ex)
            {
                //  Return message
                return new ContentResult
                       {
                           ContentType = "application/json",
                           Content = ex.Message,
                           StatusCode = (int)HttpStatusCode.OK
                       };
            }
        }
        #endregion


    }
}
