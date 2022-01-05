using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Composing;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using System;
using System.Diagnostics;
using System.Text;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Core.Logging;
using Umbraco.Web.WebApi;
using Umbraco.Core.Logging.Serilog;

using Serilog;
using Serilog.Core;
using Serilog.Events;
using System.Configuration;
using Umbraco.Web.Runtime;

namespace Models
{
    public sealed class Common
    {
        #region "Properties"
        public struct NodeProperties
        {
            public const string author = "author";
            public const string backgroundImage = "backgroundImage";
            public const string backgroundTransparency = "backgroundTransparency";
            public const string description = "description";
            public const string email = "email";
            public const string featuredImage = "featuredImage";
            public const string formEmails = "formEmails";
            public const string freeConsultation = "freeConsultation";
            public const string infoServices = "infoServices";
            public const string message = "message";
            public const string metaRobots = "metaRobots";
            public const string navigationTitle = "navigationTitle";
            public const string phone = "phone";
            public const string projectTypes = "projectTypes";
            public const string quote = "quote";
            public const string recaptchaPrivateKey = "recaptchaPrivateKey";
            public const string recaptchaPublicKey = "recaptchaPublicKey";
            public const string redirects = "redirects";
            public const string sEOChecker = "sEOChecker";
            public const string serviceAreas = "serviceAreas";
            public const string serviceInfo = "serviceInfo";
            public const string showInNavigation = "showInNavigation";
            public const string submittedBy = "submittedBy";
            public const string supporterOfIcons = "supporterOfIcons";
            public const string title = "title";
            public const string whatWeDo = "whatWeDo";
            public const string whoWeAre = "whoWeAre";
            public const string workingHours = "workingHours";
        }
        public struct DocType
        {
            public const string navigation = "navigation";
            public const string project = "project";
            public const string service = "service";
            public const string testimony = "testimony";
            public const string about = "about";
            public const string callToAction = "callToAction";
            public const string contactUs = "contactUs";
            public const string header = "header";
            public const string gallery = "gallery";
            public const string inquiries = "inquiries";
            public const string inquiry = "inquiry";
            public const string services = "services";
            public const string testimonials = "testimonials";
            public const string home = "home";
        }
        public enum DataType
        {
            FilterTypes = 1134
        }
        public struct Crop
        {
            public const string Landscape_510x320 = "Landscape_510x320";
            public const string Landscape_600x400 = "Landscape_600x400";
            public const string Landscape_1900x230 = "Landscape_1900x230";
            public const string Landscape_1900x780 = "Landscape_1900x780";
            public const string Landscape_1900x1080 = "Landscape_1900x1080";
            public const string Portrait_600x800 = "Portrait_600x800";
        }
        #endregion

        
        #region "Methods"
        public static List<PreValue> GetDataTypePreValues(int id)
        {
            IDataTypeService dataTypeService = Current.Services.DataTypeService;

            List<PreValue> toReturn = new List<PreValue>();

            IDataType dataType = dataTypeService.GetDataType(id);

            if (dataType != null)
            {
                ValueListConfiguration valueList = (ValueListConfiguration)dataType.Configuration;

                if (valueList != null && valueList.Items != null && valueList.Items.Any())
                {
                    toReturn.AddRange(valueList.Items.Select(s => new PreValue
                    {
                        Id = s.Id,
                        Value = s.Value
                    }));
                }
            }

            return toReturn;
        }
        public static HtmlString ReplaceLineBreaksForHtml(string text)
        {
            return new HtmlString(text.Replace("\r\n", @"<br />").Replace("\n", @"<br />").Replace("\r", @"<br />"));
        }
        public static void SaveErrorMessage(Exception ex, StringBuilder sb, Type type, bool saveAsWarning = false)
        {
            //Instantiate variables
            StringBuilder sbGeneralInfo = new StringBuilder();
            Serilog.ILogger log = Log.ForContext(type);

            try
            {
                try
                {
                    //Attempt to obtain stack information
                    StackTrace st = new StackTrace(ex, true);
                    StackFrame frame = st.GetFrame(0);
                    sbGeneralInfo.AppendLine("fileName: " + frame.GetFileName());
                    sbGeneralInfo.AppendLine("methodName: " + frame.GetMethod().Name);
                    sbGeneralInfo.AppendLine("line: " + frame.GetFileLineNumber());
                    sbGeneralInfo.AppendLine("col: " + frame.GetFileColumnNumber());
                }
                catch (Exception exc)
                {
                    if (!saveAsWarning)
                    {
                        sbGeneralInfo.AppendLine("Error attempting to add stack information in SaveErrorMessage()");
                        sbGeneralInfo.AppendLine(exc.ToString());
                    }
                }

                //Combine data for logging
                sb.AppendLine(sbGeneralInfo.ToString());

                //Log error informaiton
                if (saveAsWarning)
                {
                    log.Warning(ex, sb.ToString());
                }
                else
                {
                    log.Error(ex, sb.ToString());
                }
            }
            catch (Exception error)
            {
                log.Fatal(error, "Error Saving Exception Message.  Original Data: " + sb.ToString() + " ||| " + ex.Message);
            }
        }
    #endregion
}
}



//log.Debug("JF- Debug");
//log.Information("JF- Info");
//log.Information(new Exception("JF Info Exception"), "JF- Info");
//log.Warning("JF- Warn");
//log.Error("JF- Error", new Exception("JF Error Exception"));
//log.Fatal("JF- Fatal", new Exception("JF Error Exception"));
