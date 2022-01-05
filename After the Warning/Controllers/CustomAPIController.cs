using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web.Hosting;
using System.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Controllers
{
    public class CustomAPIController : UmbracoAuthorizedApiController
    {
        public string SendUpdatesByEmail()
        {
            //Send updates via email and return results.
            Dictionary<string, int> dict = Controllers.MembershipController.SendUpdatesByEmail(Umbraco, Services.MemberService);
            return JsonConvert.SerializeObject(dict);
        }


        public string UpdateStats()
        {
            return Controllers.IlluminationController.UpdateAllStatistics(Umbraco, Services.MemberService, Services.ContentService); 
        }

    }
}
