﻿using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
//using umbraco;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Mvc;





namespace Models
{
    public sealed class Common
    {
        #region "Properties"
        public enum siteNode : int
        {
            AddEditIlluminationStory = 1123,
            ByAge = 15327,
            ByCountry = 15328,
            ByExperienceType = 15329,
            ByGender = 15330,
            ByRace = 15331,
            ByReligion = 15332,
            ContactUs = 1118,
            CreateAccount = 1120,
            Donate = 1116,
            EditAccount = 3634,
            ForgotPassword = 1121,
            Home = 1089,
            IlluminationStatistics = 1112,
            IlluminationStories = 1111,
            Login = 1119,
            Logout = 3631,
            MannageAccount = 1115,
            MessagesFromHeaven = 1095,
            Preface = 1110,
            Search = 1117,
            TheDouayRheimsBible = 1109
        }
        public enum Genders
        {
            [Display(Name = "XY [Male]")] Male,
            [Display(Name = "XX [Female]")] Female
        }
        public enum Countries
        {
            [Display(Name = "Afghanistan")] Afghanistan,
            [Display(Name = "Albania")] Albania,
            [Display(Name = "Algeria")] Algeria,
            [Display(Name = "Argentina")] Argentina,
            [Display(Name = "Armenia")] Armenia,
            [Display(Name = "Australia")] Australia,
            [Display(Name = "Austria")] Austria,
            [Display(Name = "Azerbaijan")] Azerbaijan,
            [Display(Name = "Bahrain")] Bahrain,
            [Display(Name = "Bangladesh")] Bangladesh,
            [Display(Name = "Belarus")] Belarus,
            [Display(Name = "Belgium")] Belgium,
            [Display(Name = "Belize")] Belize,
            [Display(Name = "Bolivarian Republic of Venezuela")] BolivarianRepublicofVenezuela,
            [Display(Name = "Bolivia")] Bolivia,
            [Display(Name = "Bosnia and Herzegovina")] BosniaandHerzegovina,
            [Display(Name = "Botswana")] Botswana,
            [Display(Name = "Brazil")] Brazil,
            [Display(Name = "Brunei Darussalam")] BruneiDarussalam,
            [Display(Name = "Bulgaria")] Bulgaria,
            [Display(Name = "Cambodia")] Cambodia,
            [Display(Name = "Cameroon")] Cameroon,
            [Display(Name = "Canada")] Canada,
            [Display(Name = "Caribbean")] Caribbean,
            [Display(Name = "Chile")] Chile,
            [Display(Name = "China")] China,
            [Display(Name = "Colombia")] Colombia,
            [Display(Name = "Congo")] Congo,
            [Display(Name = "Costa Rica")] CostaRica,
            [Display(Name = "Croatia")] Croatia,
            [Display(Name = "Czech Republic")] CzechRepublic,
            [Display(Name = "Denmark")] Denmark,
            [Display(Name = "Dominican Republic")] DominicanRepublic,
            [Display(Name = "Ecuador")] Ecuador,
            [Display(Name = "Egypt")] Egypt,
            [Display(Name = "El Salvador")] ElSalvador,
            [Display(Name = "Eritrea")] Eritrea,
            [Display(Name = "Estonia")] Estonia,
            [Display(Name = "Ethiopia")] Ethiopia,
            [Display(Name = "Faroe Islands")] FaroeIslands,
            [Display(Name = "Finland")] Finland,
            [Display(Name = "France")] France,
            [Display(Name = "Georgia")] Georgia,
            [Display(Name = "Germany")] Germany,
            [Display(Name = "Greece")] Greece,
            [Display(Name = "Greenland")] Greenland,
            [Display(Name = "Guatemala")] Guatemala,
            [Display(Name = "Haiti")] Haiti,
            [Display(Name = "Honduras")] Honduras,
            [Display(Name = "Hong Kong")] HongKong,
            [Display(Name = "Hungary")] Hungary,
            [Display(Name = "Iceland")] Iceland,
            [Display(Name = "India")] India,
            [Display(Name = "Indonesia")] Indonesia,
            [Display(Name = "Iran")] Iran,
            [Display(Name = "Iraq")] Iraq,
            [Display(Name = "Ireland")] Ireland,
            [Display(Name = "Israel")] Israel,
            [Display(Name = "Italy")] Italy,
            [Display(Name = "Ivory Coast")] IvoryCoast,
            [Display(Name = "Jamaica")] Jamaica,
            [Display(Name = "Japan")] Japan,
            [Display(Name = "Jordan")] Jordan,
            [Display(Name = "Kazakhstan")] Kazakhstan,
            [Display(Name = "Kenya")] Kenya,
            [Display(Name = "Korea")] Korea,
            [Display(Name = "Kuwait")] Kuwait,
            [Display(Name = "Kyrgyzstan")] Kyrgyzstan,
            [Display(Name = "Lao PDR")] LaoPDR,
            [Display(Name = "Latin America")] LatinAmerica,
            [Display(Name = "Latvia")] Latvia,
            [Display(Name = "Lebanon")] Lebanon,
            [Display(Name = "Libya")] Libya,
            [Display(Name = "Liechtenstein")] Liechtenstein,
            [Display(Name = "Lithuania")] Lithuania,
            [Display(Name = "Luxembourg")] Luxembourg,
            [Display(Name = "Macao SAR")] MacaoSAR,
            [Display(Name = "Macedonia")] Macedonia,
            [Display(Name = "Malaysia")] Malaysia,
            [Display(Name = "Maldives")] Maldives,
            [Display(Name = "Mali")] Mali,
            [Display(Name = "Malta")] Malta,
            [Display(Name = "Mexico")] Mexico,
            [Display(Name = "Moldova")] Moldova,
            [Display(Name = "Mongolia")] Mongolia,
            [Display(Name = "Montenegro")] Montenegro,
            [Display(Name = "Morocco")] Morocco,
            [Display(Name = "Myanmar")] Myanmar,
            [Display(Name = "Nepal")] Nepal,
            [Display(Name = "Netherlands")] Netherlands,
            [Display(Name = "New Zealand")] NewZealand,
            [Display(Name = "Nicaragua")] Nicaragua,
            [Display(Name = "Nigeria")] Nigeria,
            [Display(Name = "Norway")] Norway,
            [Display(Name = "Oman")] Oman,
            [Display(Name = "Pakistan")] Pakistan,
            [Display(Name = "Panama")] Panama,
            [Display(Name = "Paraguay")] Paraguay,
            [Display(Name = "Peru")] Peru,
            [Display(Name = "Philippines")] Philippines,
            [Display(Name = "Poland")] Poland,
            [Display(Name = "Portugal")] Portugal,
            [Display(Name = "Principality of Monaco")] PrincipalityofMonaco,
            [Display(Name = "Puerto Rico")] PuertoRico,
            [Display(Name = "Qatar")] Qatar,
            [Display(Name = "Réunion")] Réunion,
            [Display(Name = "Romania")] Romania,
            [Display(Name = "Russia")] Russia,
            [Display(Name = "Rwanda")] Rwanda,
            [Display(Name = "Saudi Arabia")] SaudiArabia,
            [Display(Name = "Senegal")] Senegal,
            [Display(Name = "Serbia")] Serbia,
            [Display(Name = "Serbia and Montenegro")] SerbiaandMontenegro,
            [Display(Name = "Singapore")] Singapore,
            [Display(Name = "Slovakia")] Slovakia,
            [Display(Name = "Slovenia")] Slovenia,
            [Display(Name = "Somalia")] Somalia,
            [Display(Name = "South Africa")] SouthAfrica,
            [Display(Name = "Spain")] Spain,
            [Display(Name = "Sri Lanka")] SriLanka,
            [Display(Name = "Sweden")] Sweden,
            [Display(Name = "Switzerland")] Switzerland,
            [Display(Name = "Syria")] Syria,
            [Display(Name = "Taiwan")] Taiwan,
            [Display(Name = "Tajikistan")] Tajikistan,
            [Display(Name = "Thailand")] Thailand,
            [Display(Name = "Trinidad and Tobago")] TrinidadandTobago,
            [Display(Name = "Tunisia")] Tunisia,
            [Display(Name = "Turkey")] Turkey,
            [Display(Name = "Turkmenistan")] Turkmenistan,
            [Display(Name = "U.A.E.")] UAE,
            [Display(Name = "Ukraine")] Ukraine,
            [Display(Name = "United Kingdom")] UnitedKingdom,
            [Display(Name = "United States")] UnitedStates,
            [Display(Name = "Uruguay")] Uruguay,
            [Display(Name = "Uzbekistan")] Uzbekistan,
            [Display(Name = "Vietnam")] Vietnam,
            [Display(Name = "Yemen")] Yemen,
            [Display(Name = "Zimbabwe")] Zimbabwe
        }
        public enum Races
        {
            [Display(Name = "Arabic")] Arabic,
            [Display(Name = "Asian")] Asian,
            [Display(Name = "Black or African")] BlackOrAfrican,
            [Display(Name = "Indian")] Indian,
            [Display(Name = "Jewish")] Jewish,
            [Display(Name = "Latin or Hispanic")] LatinOrHispanic,
            [Display(Name = "Native American")] NativeAmerican,
            [Display(Name = "Pacific Islander")] PacificIslander,
            [Display(Name = "White or Caucasian")] WhiteOrCaucasian,
            [Display(Name = "Other or Keep Private")] OtherOrKeepPrivate
        }
        public enum Religions
        {
            [Display(Name = "Agnostic")] Agnostic,
            [Display(Name = "Atheist")] Atheist,
            [Display(Name = "Baptist")] Baptist,
            [Display(Name = "Buddhist")] Buddhist,
            [Display(Name = "Catholic")] Catholic,
            [Display(Name = "Evangelical")] Evangelical,
            [Display(Name = "Lutheran")] Lutheran,
            [Display(Name = "Hindu")] Hindu,
            [Display(Name = "Muslim")] Muslim,
            [Display(Name = "Other Christian")] OtherChristian,
            [Display(Name = "Protestant")] Protestant,
            [Display(Name = "Satinism")] Satinism,
            [Display(Name = "Wiccan or New Age")] WiccanOrNewAge,
            [Display(Name = "Other or Keep Private")] OtherOrKeepPrivate
        }
        public enum ExperienceTypes
        {
            [Display(Name = "Heavenly")] Heavenly,
            [Display(Name = "Purgatorial")] Purgatorial,
            [Display(Name = "Hellish")] Hellish,
            [Display(Name = "Other or Unsure")] OtherorUnsure
        }

        public struct NodeProperties
        {
            public const string activateIlluminationControls = "activateIlluminationControls";
            public const string address = "address";
            public const string adorationIframe = "adorationIframe";
            public const string age = "age";
            public const string ageRange = "ageRange";
            public const string alternateName = "alternateName";
            public const string author = "author";

            public const string baseCalculationDate = "baseCalculationDate";
            public const string biography = "biography";
            public const string birthYear = "birthYear";
            public const string bookSet = "bookSet";

            public const string candleOut = "candleOut";
            public const string candle10 = "candle10";
            public const string candle20 = "candle20";
            public const string candle30 = "candle30";
            public const string candle40 = "candle40";
            public const string candle50 = "candle50";
            public const string candle60 = "candle60";
            public const string candle70 = "candle70";
            public const string candle80 = "candle80";
            public const string candle90 = "candle90";
            public const string candle100 = "candle100";
            public const string chapter = "chapter";
            public const string chapters = "chapters";
            public const string compiledStories = "compiledStories";
            public const string contactSummary = "contactSummary";
            public const string content = "content";
            public const string count = "count";
            public const string country = "country";
            public const string currentPercentage = "currentPercentage";

            public const string dateOfMessages = "dateOfMessages";

            public const string email = "email";
            public const string exceptionMessage = "exceptionMessage";
            public const string experiencesByCountry = "experiencesByCountry";
            public const string experiencesByRace = "experiencesByRace";
            public const string experiencesByReligion = "experiencesByReligion";
            public const string experienceType = "experienceType";

            public const string facebookUrl = "facebookUrl";
            public const string females = "females";
            public const string firstName = "firstName";
            public const string formPicker = "formPicker";
            public const string fullName = "fullName";

            public const string gender = "gender";
            public const string generalInfo = "generalInfo";
            public const string googleWalletUrl = "googleWalletUrl";

            public const string heavenly = "heavenly";
            public const string hellish = "hellish";
            public const string hideChildrenFromNavigation = "hideChildrenFromNavigation";

            public const string id = "id";
            public const string illuminationStory = "illuminationStory";
            public const string indexType = "__IndexType";

            public const string label = "label";
            public const string lastName = "lastName";

            public const string mailingAddress = "mailingAddress";
            public const string males = "males";
            public const string member = "member";

            public const string nodeName = "nodeName";
            public const string NodeTypeAlias = "__NodeTypeAlias";

            public const string originalSiteName = "originalSiteName";
            public const string originalSiteUrl = "originalSiteUrl";
            public const string originalSource = "originalSource";
            public const string originallyPostedBy = "originallyPostedBy";
            public const string originalPostUrl = "originalPostUrl";
            public const string other = "other";

            public const string pageImage = "pageImage";
            public const string paypalUrl = "paypalUrl";
            public const string personalArticles = "personalArticles";
            public const string personalPhoto = "personalPhoto";
            public const string phone = "phone";
            public const string prayer = "prayer";
            public const string prayersOfferedFor = "prayersOfferedFor";
            public const string prayerRequestMember = "prayerRequestMember";
            public const string prayerRequests = "prayerRequests";
            public const string prayerTitle = "prayerTitle";
            public const string publishDate = "publishDate";
            public const string purgatorial = "purgatorial";

            public const string race = "race";
            public const string religion = "religion";
            public const string requestDate = "requestDate";

            public const string SaveErrorMessage = "saveErrorMessage";
            public const string showInMinorNavigation = "showInMinorNavigation";
            public const string statsHeavenly = "statsHeavenly";
            public const string statsHellish = "statsHellish";
            public const string statsPurgatorial = "statsPurgatorial";
            public const string statsUnknown = "statsUnknown";
            public const string story = "story";
            public const string subscribed = "subscribed";
            public const string subtitle = "subtitle";

            public const string testament = "testament";
            public const string thumbnail = "thumbnail";
            public const string title = "title";
            public const string topBanner = "topBanner";
            public const string totalPrayersOffered = "totalPrayersOffered";

            public const string umbracoNaviHide = "umbracoNaviHide";

            public const string Verse = "verse";
            public const string Verses = "verses";
            public const string visionarysName = "visionarysName";

            public const string _umb_email = "_umb_email";
            public const string _umb_login = "_umb_login";
        }
        public struct docType
        {
            public const string AddEditIlluminationStory = "addEditIlluminationStory";
            public const string AppStartEvents = "appStartEvents";
            public const string BlockList = "blockList";
            public const string BlockListWithThumbnail = "blockListWithThumbnail";
            public const string Chapter = "chapter";
            public const string ContactUs = "contactUs";
            public const string CreateAccount = "createAccount";
            public const string DataLayer = "dataLayer";
            public const string Donations = "donations";
            public const string EditAccount = "editAccount";
            public const string ErrorMessage = "errorMessage";
            public const string Home = "home";
            public const string IlluminationStatistics = "illuminationStatistics";
            public const string IlluminationStory = "illuminationStory";
            public const string IlluminationStoryList = "illuminationStoryList";
            public const string Login = "login";
            public const string Logout = "logout";
            public const string ManageAccount = "manageAccount";
            public const string Message = "message";
            public const string PrayerList = "prayerList";
            public const string PrayerPledges = "prayerPledges";
            public const string PrayerRequest = "prayerRequest";
            public const string PrayerRequests = "prayerRequests";
            public const string Scripture = "scripture";
            public const string Search = "search";
            public const string Standard = "standard";
            public const string StatsByAge = "statsByAge";
            public const string StatsByCountry = "statsByCountry";
            public const string StatsByExperienceType = "statsByExperienceType";
            public const string StatsByGender = "statsByGender";
            public const string StatsByRace = "statsByRace";
            public const string StatsByReligion = "statsByReligion";
            public const string UDateFoldersyFolderYear = "uDateFoldersyFolderYear";
            public const string Verse = "verse";
            public const string ViewAllMessages = "viewAllMessages";
            public const string Visionary = "visionary";
            public const string VisionaryList = "visionaryList";
            public const string WebmasterMessage = "webmasterMessage";
            public const string WebmasterMessageList = "webmasterMessageList";
        }
        public struct dataType
        {
            public const string ExperienceType = "Experience Type";
            public const string Gender = "Gender";
            public const string Race = "Race";
            public const string Religion = "Religion";
            public const string ScriptureVerse = "scriptureVerse";
            public const string StatisticsBarChart = "statisticsBarChart";
            public const string StatisticsLineChart = "statisticsLineChart";
            public const string StatisticsStackedBarChart = "statisticsStackedBarChart";
        }
        public struct crop
        {
            public const string Portrait_300x400 = "Portrait_300x400";
            public const string Square_500x500 = "Square_500x500";
        }
        public struct Gender
        {
            public const string Male = "XY [Male]";
            public const string Female = "XX [Female]";
        }
        public struct miscellaneous
        {
            public const string Foundation6 = "Foundation6";
            public const string NewTestament = "New Testament";
            public const string OldTestament = "Old Testament";
            public const string OtherOrKeepPrivate = "Other or Keep Private";
            public const string PageNo = "pageNo";
            public const string Path = "__Path";
            public const string ScriptureSet = "Scripture Set";
            public const string ScriptureTestaments = "Scripture Testaments";
            public const string SearchFor = "searchFor";
            public const string SearchIn = "searchIn";
            public const string SortBy = "sortBy";
            public const string ThePentateuchBooks = "The Pentateuch Books";
            public const string TheHistoricalBooks = "The Historic Books";
            public const string TheWisdomBooks = "The Wisdom Books";
            public const string ThePropheticBooks = "The Prophetic Books";
            public const string TheGospels = "The Gospels";
            public const string TheEpistles = "The Epistles";
            public const string UnderMaintenance = "underMaintenance";
            public const string Validate = "validate";
        }
        public struct searchProviders
        {
            public const string ExternalIndex = "ExternalIndex";
            //public const string ArticleSearcher = "ArticleSearcher";
            //public const string ExternalSearcher = "ExternalSearcher";
            //public const string IlluminationStoriesSearcher = "IlluminationStoriesSearcher";
            //public const string InternalMemberSearcher = "InternalMemberSearcher";
            //public const string InternalSearcher = "InternalSearcher";
            //public const string MessagesSearcher = "MessagesSearcher";
            //public const string PrayersSearcher = "PrayersSearcher";
            //public const string ScriptureSearcher = "ScriptureSearcher";
        }
        public struct ExperienceType
        {
            public const string Heavenly = "Heavenly";
            public const string Hellish = "Hellish";
            public const string Other = "Other or Unsure";
            public const string Purgatorial = "Purgatorial";
        }
        public struct ViewByTypes
        {
            public const string Heavenly = "Heavenly";
            public const string Hellish = "Hellish";
            public const string OtherUnsure = "OtherUnsure";
            public const string Purgatorial = "Purgatorial";
            public const string ViewBy = "viewBy";
        }
        public struct SearchIn
        {
            public const string Messages = "messages";
            public const string Articles = "articles";
            public const string Prayers = "prayers";
            public const string Bible = "bible";
            public const string Illuminations = "illuminations";
        }
        #endregion


        #region "Methods"
        public static string ClipString(string text, int length, bool ellipsis)
        {
            text = text.Replace("&nbsp;", " ").Replace("  ", " ");
            if (text.Length > length)
            {
                if (ellipsis)
                {
                    length -= 3;
                }
                int num = text.LastIndexOf(" ", length);
                if (num < 0)
                {
                    num = length;
                }
                text = text.Substring(0, num);
                if (ellipsis)
                {
                    text = text + "...";
                }
            }
            return text;
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
        public static string SerializeValue(string value)
        {
            return JsonConvert.SerializeObject(new[] { value });
        }
        public static List<string> DeserializeValues(string value)
        {
            return JsonConvert.DeserializeObject<List<string>>(value);
        }
        public static string GetPreValueString(string preValueId)
        {
            int Id;
            if (int.TryParse(preValueId, out Id))
            {

                IDataTypeService dataTypeService = Current.Services.DataTypeService;
                IDataType dataType = dataTypeService.GetDataType(Id);
                return dataType.Name;

                //return umbraco.library.GetPreValueAsString(Id);
            }

            return string.Empty;
        }
        #endregion
    }
}






//public static int? GetPrevalueId(string dataTypeName, string dataTypeValue)
//{
//    // Obtain prevalue collection from datatypes
//    IDataTypeService dataTypeService = Current.Services.DataTypeService;
//    IDataType dataType = dataTypeService.GetDataType(dataTypeName);


//    // Returns the ID of the prevalue that matches the string value
//    ValueListConfiguration prevalues = (ValueListConfiguration)dataType.Configuration;
//    int selected = 0;
//    if (prevalues != null && prevalues.Items != null && prevalues.Items.Any())
//    {
//        selected = prevalues.Items.Where(x => x.Value == dataTypeValue).Select(i => i.Id).FirstOrDefault();
//    }
//    return selected;


//    //// Obtain prevalue collection from datatypes
//    //IDataTypeService dtService = ApplicationContext.Current.Services.DataTypeService;
//    //IDataTypeDefinition dtDefinition = dtService.GetDataTypeDefinitionByName(dataTypeName);
//    //PreValueCollection pvCollection = dtService.GetPreValuesCollectionByDataTypeId(dtDefinition.Id);
//    //return pvCollection.PreValuesAsDictionary.FirstOrDefault(preValue => string.Equals(preValue.Value.Value, dataTypeValue)).Value.Id;
//}
//public static int? GetPrevalueIdForIContent(IContent ic, string propertyAlias, string propertyValue)
//{
//    //find the property on the content node by its alias
//    Property prop = ic.Properties.FirstOrDefault(a => a.Alias == propertyAlias);

//    if (prop != null)
//    {
//        //get data type from the property
//        //IDataTypeService dtService = ApplicationContext.Current.Services.DataTypeService;
//        //IDataTypeDefinition dtDefinition = dtService.GetDataTypeDefinitionById(prop.PropertyType.DataTypeDefinitionId);
//        IDataTypeService dtService = Current.Services.DataTypeService;
//        IDataType dataType = dtService.GetDataType(prop.PropertyType.DataTypeId);

//        //Return property value Id
//        //return dtService.GetPreValuesCollectionByDataTypeId(dtDefinition.Id).PreValuesAsDictionary.Where(d => d.Value == propertyValue).Select(f => f.Id).First();
//        if (dataType != null)
//        {
//            ValueListConfiguration valueList = (ValueListConfiguration)dataType.Configuration;
//            return valueList.Items.Where(d => d.Value == propertyValue).Select(f => f.Id).First();
//        }
//    }

//    //Return nothing
//    return null;
//}
//public static int? GetPrevalueIdForIMember(IMember im, string propertyAlias, string propertyValue)
//{
//    if (propertyValue == "")
//    {
//        //Return nothing
//        return null;
//    }
//    else
//    {
//        //find the property on the content node by its alias
//        Property prop = im.Properties.FirstOrDefault(a => a.Alias == propertyAlias);

//        if (prop != null)
//        {
//            IDataTypeService dataTypeService = Current.Services.DataTypeService;
//            IDataType dataType = dataTypeService.GetDataType(propertyAlias);
//            if (dataType != null)
//            {
//                ValueListConfiguration valueList = (ValueListConfiguration)dataType.Configuration;
//                return valueList.Items.Where(d => d.Value == propertyValue).Select(f => f.Id).First();
//            }
//        }

//        //Return nothing
//        return null;
//    }

//}