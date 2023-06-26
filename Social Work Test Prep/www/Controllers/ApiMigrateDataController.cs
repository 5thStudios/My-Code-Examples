using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using Umbraco.Web.WebApi;
using bl.Models.api;
using Umbraco.Core.Services;
using System.Text;
using Umbraco.ModelsBuilder;
using bl.EF;

namespace Controllers
{
    public class ApiMigrateDataController : UmbracoAuthorizedApiController 
    {
        #region "Properties"
        public IContentService _contentService { get; set; }
        private IMediaService _mediaService;
        public ApiMigrateDataController(IMediaService mediaService) 
        {
            _mediaService = mediaService;
            _contentService = Services.ContentService;
        }
        #endregion


        #region "API Calls"
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> UpdateBlogPosts_Thumbnails()
        {
            //Update blog thumbnails
            bl.api.Controllers.ApiMigrateDataController apiMigrateData = new bl.api.Controllers.ApiMigrateDataController(_contentService, _mediaService);
            string response = apiMigrateData.UpdateBlogPosts_AddThumbnails();

            if (!string.IsNullOrEmpty(response))
                return Json("Errors: " + response);

            else
                return Json("Update Complete!!");
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> ImportBlogPosts()
        {
            //Convert json into list of exams
            StringBuilder sb = new StringBuilder();
            bool hasErrors = false;
            List<BlogPost> lstBlogPosts = JsonConvert.DeserializeObject<List<BlogPost>>(HttpContext.Current.Request.Form["hfldJson"]); //Convert json to list
            bl.api.Controllers.ApiMigrateDataController apiMigrateData = new bl.api.Controllers.ApiMigrateDataController(_contentService, _mediaService);


            foreach (BlogPost blogPost in lstBlogPosts)
            {
                string results = apiMigrateData.ImportBlogPost(blogPost);
                if (!string.IsNullOrEmpty(results))
                {
                    hasErrors = true;
                    sb.AppendLine(results + "  |  ");
                }
            }


            if (hasErrors)
                return Json("Errors: " + sb.ToString());

            else
                return Json("Import Complete!!");
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> ImportSpecialOffers()
        {
            //Convert json into list of exams
            List<SpecialOffer> lstExams = JsonConvert.DeserializeObject<List<SpecialOffer>>(HttpContext.Current.Request.Form["hfldJson"]); //Convert json to list
            bl.api.Controllers.ApiMigrateDataController apiMigrateData = new bl.api.Controllers.ApiMigrateDataController(_contentService, _mediaService);
            string results = apiMigrateData.ImportSpecialOffers(lstExams);

            return Json(results);
            //return Json(Newtonsoft.Json.JsonConvert.SerializeObject(lstExams));
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> ImportExams()
        {
            //Convert json into list of exams
            List<Exam> lstExams = JsonConvert.DeserializeObject<List<Exam>>(HttpContext.Current.Request.Form["hfldJson"]); //Convert json to list
            bl.api.Controllers.ApiMigrateDataController apiMigrateData = new bl.api.Controllers.ApiMigrateDataController(_contentService, _mediaService);
            //Imports exams (not questions within)
            //string results = apiMigrateData.ImportExams(lstExams);


            //Imports questions within exams.
            string results = apiMigrateData.ImportExamQuestions(lstExams);

            return Json(results);


            //return Json(Newtonsoft.Json.JsonConvert.SerializeObject(lstExams));
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> UpdateExams_StudyLinks()
        {
            //Update blog thumbnails
            bl.api.Controllers.ApiMigrateDataController apiMigrateData = new bl.api.Controllers.ApiMigrateDataController(_contentService, _mediaService);
            string response = apiMigrateData.UpdateExamQuestions_StudyLinks();

            if (!string.IsNullOrEmpty(response))
                return Json("Errors: " + response);

            else
                return Json("Update Complete!!");

        }











        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> ImportExistingQuestionIDs()
        {
            //Instantiate variables
            bl.api.Controllers.ApiMigrateDataController apiMigrateData = new bl.api.Controllers.ApiMigrateDataController(_contentService, _mediaService);

            //Convert json into list
            List<Exam> lstExams = JsonConvert.DeserializeObject<List<Exam>>(HttpContext.Current.Request.Form["hfldJson"]); //Convert json to list

            //Imports members
            string results = apiMigrateData.ImportExistingQuestionIDs(lstExams);

            return Json(results);
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> UpdateExistingQuestionIDs()
        {
            //Instantiate variables
            bl.api.Controllers.ApiMigrateDataController apiMigrateData = new bl.api.Controllers.ApiMigrateDataController(_contentService, _mediaService);

            //Imports members
            string results = apiMigrateData.UpdateExistingQuestionIDs();

            return Json(results);
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> ImportMember()
        {
            //Instantiate variables
            bl.api.Controllers.ApiMigrateDataController apiMigrateData = new bl.api.Controllers.ApiMigrateDataController(_contentService, _mediaService);
            List<Original_MemberData> lstMembers;

            //Convert json into list
            lstMembers = JsonConvert.DeserializeObject<List<Original_MemberData>>(HttpContext.Current.Request.Form["hfldJson"]);

            //Imports members
            string results = apiMigrateData.ImportMembers(lstMembers);

            return Json(results);
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> ConvertDbMemberToUmbraco()
        {
            //Instantiate variables
            bl.api.Controllers.ApiMigrateDataController apiMigrateData = new bl.api.Controllers.ApiMigrateDataController(_contentService, _mediaService);

            //Imports members
            string results = apiMigrateData.ConvertDbMemberToUmbraco();

            return Json(results);
        }


        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> AddImportedPurchaseRecordsToSite()
        {
            //Instantiate variables
            bl.api.Controllers.ApiMigrateDataController apiMigrateData = new bl.api.Controllers.ApiMigrateDataController(_contentService, _mediaService);

            //Run function
            string results = apiMigrateData.AddImportedPurchaseRecordsToSite();

            return Json(results);
        }





        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> ImportAllNonupdatedExamData()
        {
            //Instantiate variables
            bl.api.Controllers.ApiMigrateDataController apiMigrateData = new bl.api.Controllers.ApiMigrateDataController(_contentService, _mediaService);
            NonupdatedData _data;

            //Convert json into list
            _data = JsonConvert.DeserializeObject<NonupdatedData>(HttpContext.Current.Request.Form["hfldJson"]);

            //Imports data
            string results = apiMigrateData.ImportNonupdatedData(_data);

            return Json(results);
            //return Json(Newtonsoft.Json.JsonConvert.SerializeObject(_data));
        }


        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> ObtainLatestUpdates()
        {
            //Instantiate variables
            bl.api.Controllers.ApiMigrateDataController apiMigrateData = new bl.api.Controllers.ApiMigrateDataController(_contentService, _mediaService);
            List<UpdateRecord> _data;

            //Convert json into list
            _data = JsonConvert.DeserializeObject<List<UpdateRecord>>(HttpContext.Current.Request.Form["hfldJson"]);

            //Imports data
            string results = apiMigrateData.ObtainLatestUpdates(_data);

            return Json(results);
            //return Json(Newtonsoft.Json.JsonConvert.SerializeObject(_data));
        }


        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> ObtainAllUmbracoNodes()
        {
            //Instantiate variables
            bl.api.Controllers.ApiMigrateDataController apiMigrateData = new bl.api.Controllers.ApiMigrateDataController(_contentService, _mediaService);
            List<UmbracoNode> _data;

            //Convert json into list
            _data = JsonConvert.DeserializeObject<List<UmbracoNode>>(HttpContext.Current.Request.Form["hfldJson"]);

            //Imports data
            string results = apiMigrateData.ObtainAllUmbracoNodes(_data); //.Take(10).ToList()); //JF TEST

            return Json(results);
            //return Json(Newtonsoft.Json.JsonConvert.SerializeObject(_data));
        }


        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> ObtainAllCmsPropertyData()
        {
            //Instantiate variables
            bl.api.Controllers.ApiMigrateDataController apiMigrateData = new bl.api.Controllers.ApiMigrateDataController(_contentService, _mediaService);
            List<Original_CmsPropertyData> _data;

            //Convert json into list
            _data = JsonConvert.DeserializeObject<List<Original_CmsPropertyData>>(HttpContext.Current.Request.Form["hfldJson"]);

            //Imports data
            string results = apiMigrateData.ObtainAllCmsPropertyData(_data); //.Take(150000).ToList()); //JF TEST

            return Json(results);
            //return Json(Newtonsoft.Json.JsonConvert.SerializeObject(_data));
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> CleanImportedExams()
        {
            //Instantiate variables
            bl.api.Controllers.ApiMigrateDataController apiMigrateData = new bl.api.Controllers.ApiMigrateDataController(_contentService, _mediaService);

            //Imports members
            string results = apiMigrateData.CleanImportedExams();

            return Json(results);
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> ConvertImportedExamResults()
        {
            //Instantiate variables
            bl.api.Controllers.ApiMigrateDataController apiMigrateData = new bl.api.Controllers.ApiMigrateDataController(_contentService, _mediaService);

            //Imports members
            string results = apiMigrateData.ConvertImportedExamResults();

            return Json(results);
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> ExtendAllExams()
        {
            //Instantiate variables
            bl.api.Controllers.ApiMigrateDataController apiMigrateData = new bl.api.Controllers.ApiMigrateDataController(_contentService, _mediaService);

            //Imports members
            string results = apiMigrateData.ExtendAllExams();

            return Json(results);
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> UpdateCouponCounters()
        {
            //Instantiate variables
            bl.api.Controllers.ApiMigrateDataController apiMigrateData = new bl.api.Controllers.ApiMigrateDataController(_contentService, _mediaService);
            List<pwdDiscount> _data;

            //Convert json into list
            _data = JsonConvert.DeserializeObject<List<pwdDiscount>>(HttpContext.Current.Request.Form["hfldJson"]);

            //Imports data
            string results = apiMigrateData.UpdateCouponCounters(_data);

            return Json(results);
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> AddMissingExamAnswers()
        {
            //Instantiate variables
            bl.api.Controllers.ApiMigrateDataController apiMigrateData = new bl.api.Controllers.ApiMigrateDataController(_contentService, _mediaService);

            //Imports members
            string results = apiMigrateData.AddMissingExamAnswers();

            return Json(results);
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> RemoveDuplicateMembers()
        {
            //Instantiate variables
            bl.api.Controllers.ApiMigrateDataController apiMigrateData = new bl.api.Controllers.ApiMigrateDataController(_contentService, _mediaService);

            //Imports members
            string results = apiMigrateData.RemoveDuplicateMembers();

            return Json(results);
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> MoveDeletedAcctPurchasesToLive()
        {
            //Instantiate variables
            bl.api.Controllers.ApiMigrateDataController apiMigrateData = new bl.api.Controllers.ApiMigrateDataController(_contentService, _mediaService);

            //Imports members
            string results = apiMigrateData.MoveDeletedAcctPurchasesToLive();

            return Json(results);
        }











        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> SaveExamBundles()
        {
            //Instantiate variables
            bl.api.Controllers.ApiMigrateDataController apiMigrateData = new bl.api.Controllers.ApiMigrateDataController(_contentService, _mediaService);
            List<ExamBundle> _data;

            //Convert json into list
            _data = JsonConvert.DeserializeObject<List<ExamBundle>>(HttpContext.Current.Request.Form["hfldJson"]);

            //Imports data
            string results = apiMigrateData.SaveExamBundles(_data);

            return Json(results);
            //return Json(Newtonsoft.Json.JsonConvert.SerializeObject(_data));
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> SavePurchaseRecords()
        {
            //Instantiate variables
            bl.api.Controllers.ApiMigrateDataController apiMigrateData = new bl.api.Controllers.ApiMigrateDataController(_contentService, _mediaService);
            List<bl.Models.api.PurchaseRecord> _data;

            //Convert json into list
            _data = JsonConvert.DeserializeObject<List<bl.Models.api.PurchaseRecord>>(HttpContext.Current.Request.Form["hfldJson"]);

            //Imports data
            string results = apiMigrateData.SavePurchaseRecords(_data);

            return Json(results);
            //return Json(Newtonsoft.Json.JsonConvert.SerializeObject(_data));
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> SaveInternalPurchaseRecords()
        {
            //Instantiate variables
            bl.api.Controllers.ApiMigrateDataController apiMigrateData = new bl.api.Controllers.ApiMigrateDataController(_contentService, _mediaService);
            List<bl.Models.api.PurchaseRecord> _data;

            //Convert json into list
            _data = JsonConvert.DeserializeObject<List<bl.Models.api.PurchaseRecord>>(HttpContext.Current.Request.Form["hfldJson"]);

            //Imports data
            string results = apiMigrateData.SaveInternalPurchaseRecords(_data);

            return Json(results);
            //return Json(Newtonsoft.Json.JsonConvert.SerializeObject(_data));
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> UpdatePurchaseRecordsMemberIDs()
        {
            //Instantiate variables
            bl.api.Controllers.ApiMigrateDataController apiMigrateData = new bl.api.Controllers.ApiMigrateDataController(_contentService, _mediaService);

            //Imports members
            string results = apiMigrateData.UpdatePurchaseRecordsMemberIDs();

            return Json(results);
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> PageNotFoundRedirects()
        {
            //Instantiate variables
            bl.api.Controllers.ApiMigrateDataController apiMigrateData = new bl.api.Controllers.ApiMigrateDataController(_contentService, _mediaService);

            //Imports members
            string results = apiMigrateData.PageNotFoundRedirects();

            return Json(results);
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> FindMatchingRedirects()
        {
            //Instantiate variables
            bl.api.Controllers.ApiMigrateDataController apiMigrateData = new bl.api.Controllers.ApiMigrateDataController(_contentService, _mediaService);

            //Imports members
            string results = apiMigrateData.FindMatchingRedirects();

            return Json(results);
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> PullDescriptions()
        {
            //Instantiate variables
            bl.api.Controllers.ApiMigrateDataController apiMigrateData = new bl.api.Controllers.ApiMigrateDataController(_contentService, _mediaService);
            List<SeoDescription> _data;

            //Convert json into list
            _data = JsonConvert.DeserializeObject<List<SeoDescription>>(HttpContext.Current.Request.Form["hfldJson"]);

            //Imports data
            string results = apiMigrateData.PullDescriptions(_data); 

            return Json(results);
            //return Json(Newtonsoft.Json.JsonConvert.SerializeObject(_data));
        }





        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> ImportTest()
        {
            string hfldJson = HttpContext.Current.Request.Form["hfldJson"];
            return Json(hfldJson);
            //return Json("API reached!!!");
        }
        #endregion

    }
}

