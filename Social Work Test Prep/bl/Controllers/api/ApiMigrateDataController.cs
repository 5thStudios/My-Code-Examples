using System.Collections.Generic;
using Umbraco.Web.Mvc;
using System.Text;
using Umbraco.Core;
using Umbraco.Core.Services;
using Umbraco.Core.Models;
using System;
using System.Linq;
using Umbraco.Core.Models.PublishedContent;
using cm = Umbraco.Web.PublishedModels;
using Umbraco.Web;
using Umbraco.Web.PublishedModels;
using bl.Models.api;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;
using Examine;
using Examine.Search;
using Umbraco.Examine;
using System.Web.UI.WebControls;
using bl.EF;
using Repositories;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using bl.Models;
using Umbraco.Core.Logging;
using NPoco.Expressions;
using Superpower.Model;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using Umbraco.Core.Deploy;
using Umbraco.Core.Models.Membership;
using Umbraco.ModelsBuilder;
using static Umbraco.Core.Constants.Conventions;
using SEOChecker.Core.Extensions;
using System.Security.Policy;



namespace bl.api.Controllers
{
    public class ApiMigrateDataController : SurfaceController
    {
        #region "Properties"
        Dictionary<string, Guid> dictNodeGuids = null;

        private readonly EF_SwtpDb _dbContext;
        private IOriginalMemberDataRepository repoOriginalMemberData;
        private IOriginalCmsDataTypeRepository repoOriginalCmsDataType;
        private IOriginalCmsPropertyTypeRepository repoOriginalCmsPropertyType;
        private IOriginalCmsPropertyDataRepository repoOriginalCmsPropertyData;
        private IOriginalUpdateRecordRepository repoOriginalUpdateRecord;
        private IOriginalUmbracoNodeRepository repoOriginalUmbracoNode;
        private IExamIdRelationshipRepository repoExamIdRelationships;
        private IExamRecordRepository repoExamRecords;
        private IExamAnswerSetRepository repoExamAnswerSet;
        private IExamAnswerRepository repoExamAnswer;
        private IExamModeRepository repoExamMode;
        private IOriginalExamBundlesRepository repoOriginalExamBundles;
        private IOriginalPurchaseRecordsRepository repoOriginalPurchaseRecords;
        private IPurchaseRecordRepository repoPurchaseRecord;
        private IPurchaseRecordItemRepository repoPurchaseRecordItems;
        private ICouponRepository repoCoupons;
        private IContentService _contentService { get; set; }
        private ICmsMemberRepository repoCmsMembers { get; set; }
        //private IMediaService _mediaService;

        public ApiMigrateDataController(IContentService contentService, IMediaService mediaService)
        {
            _contentService = contentService;
            _dbContext = new EF_SwtpDb();
            repoOriginalMemberData = new OriginalMemberDataRepository(_dbContext);
            //_mediaService = mediaService;
        }
        #endregion



        #region "Functions"


        /*  Member & Purchase Records   */
        /*=============================================================*/
        public string ImportMembers(List<Original_MemberData> lstOriginalMembers)
        {
            /*      PART 1: ADD ALL EMAILS TO DB... NOT UMBRACO     */
            /*======================================================*/

            Stopwatch sw = Stopwatch.StartNew();
            TimeSpan timeComplete;
            sw.Start();


            //Instantiate variables.
            repoOriginalMemberData = new OriginalMemberDataRepository(new EF_SwtpDb());
            List<Original_MemberData> lstMembers2Add = new List<Original_MemberData>();
            List<string> lstAllOriginalEmailsInDb = repoOriginalMemberData.GetAllEmails();  //Get list of all existing emails in db.  (Not umbraco!)


            //Loop through list and create new list with only those records that are missing from database.
            foreach (Original_MemberData _member in lstOriginalMembers)
            {
                if (!lstAllOriginalEmailsInDb.Contains(_member.Email))
                {
                    lstMembers2Add.Add(_member);
                }
            }


            //Submit all updates in bulk to db.
            if (lstMembers2Add.Count > 0) repoOriginalMemberData.BulkAddRecord(lstMembers2Add);


            //Split time to see where slowness is occuring
            sw.Stop();
            timeComplete = sw.Elapsed;
            string _timeTaken = "  |  Timespan: " + timeComplete.ToString(@"hh\:mm\:ss");


            return "Complete.  Added " + lstMembers2Add.Count.ToString() + " records." + _timeTaken;
        }
        public string ConvertDbMemberToUmbraco()
        {
            /*      PART 2: ADD ALL NEW EMAILS TO UMBRACO AS MEMBER     */
            /*==========================================================*/


            Stopwatch sw = Stopwatch.StartNew();
            TimeSpan timeComplete;
            sw.Start();


            //Get list of all existing emails in db.  (Not umbraco!)
            repoOriginalMemberData = new OriginalMemberDataRepository(new EF_SwtpDb());
            List<string> lstAllOriginalEmailsInDb = repoOriginalMemberData.GetAllEmails();


            //Obtain all existing members in umbraco
            IEnumerable<IMember> lstExistinMembers = Services.MemberService.GetAllMembers();


            //New list of members to be added to umbraco.
            List<string> lstMembers2Add = new List<string>();


            //Loop through list and create new list with only those records that are missing from umbraco.
            foreach (string _originalMemberEmail in lstAllOriginalEmailsInDb)
            {
                if (!lstExistinMembers.Where(x => x.Email == _originalMemberEmail).Any())
                {
                    lstMembers2Add.Add(_originalMemberEmail);
                }
            }


            //Importing all emails as members  [timed]
            int addedSuccessfully = 0;
            int failed = 0;
            foreach (string email in lstMembers2Add)
            {
                if (CreateMember(email))
                {
                    addedSuccessfully++;
                }
                else
                {
                    failed++;
                }
            }



            //Split time to see where slowness is occuring
            sw.Stop();
            timeComplete = sw.Elapsed;
            string _timeTaken = "  |  Timespan: " + timeComplete.ToString(@"hh\:mm\:ss");


            return "db: " + lstAllOriginalEmailsInDb.Count().ToString() + " |  umb: " + lstExistinMembers.Count().ToString() + " |  added: " + addedSuccessfully.ToString() + " |  failed: " + failed.ToString() + _timeTaken;

        }
        public string SaveInternalPurchaseRecords(List<bl.Models.api.PurchaseRecord> _data)
        {
            //IMPORTS MANUALLY ADDED EXAMS
            //==========================================


            Stopwatch sw = Stopwatch.StartNew();
            TimeSpan timeComplete;
            sw.Start();



            //Instantiate variables.
            repoOriginalPurchaseRecords = new OriginalPurchaseRecordsRepository(new EF_SwtpDb());
            List<Original_PurchaseRecords> LstPurchaseRecords = new List<Original_PurchaseRecords>();
            List<string> LstTxnIDs = repoOriginalPurchaseRecords.GetAllTxnIDs();
            int totalSubmitted = 0;
            StringBuilder sb = new StringBuilder();


            //
            int index = 0;
            for (int i = 0; i < _data.Count; i++)
            {
                if (index == 0)
                    LstPurchaseRecords.Clear();  //reset list


                if (!LstTxnIDs.Contains(_data[i].Txn_id))
                {
                    index++;
                    int _itemNumber = 0;

                    Original_PurchaseRecords purchaseRecord = new Original_PurchaseRecords();
                    //if (_data[i].OriginalId != null) purchaseRecord.originalId = _data[i].OriginalId;
                    purchaseRecord.txn_id = _data[i].Txn_id;
                    purchaseRecord.created = _data[i].Created;
                    purchaseRecord.payerEmail = _data[i].PayerEmail;
                    purchaseRecord.itemName = _data[i].ItemName;
                    purchaseRecord.internalPurchase = true;
                    if (int.TryParse(_data[i].ItemNumber, out _itemNumber)) purchaseRecord.itemNumber = _itemNumber;
                    if (!string.IsNullOrEmpty(_data[i].Price)) purchaseRecord.price = Convert.ToDecimal(_data[i].Price); //
                    if (!string.IsNullOrEmpty(_data[i].Discount)) purchaseRecord.discount = Convert.ToDecimal(_data[i].Discount); //

                    LstPurchaseRecords.Add(purchaseRecord);

                    //Submit grouped records in bulk
                    if (index == 100 || i == _data.Count - 1)
                    {
                        index = 0; //reset index

                        try
                        {
                            if (LstPurchaseRecords.Count() > 0) repoOriginalPurchaseRecords.BulkAddRecord(LstPurchaseRecords);
                            totalSubmitted += LstPurchaseRecords.Count();
                        }
                        catch (SqlException exS)
                        {
                            sb.AppendLine("      ");
                            sb.AppendLine("SqlException");
                            sb.AppendLine(exS.ToString());
                        }
                        catch (Exception ex)
                        {
                            sb.AppendLine(Newtonsoft.Json.JsonConvert.SerializeObject(purchaseRecord));
                            sb.AppendLine("      ");
                            sb.AppendLine("Exception");
                            sb.AppendLine(ex.Message);
                            sb.AppendLine(ex.InnerException.ToString());
                        }
                    }
                }
            }


            //Add member IDs to each purchase record if missing.
            var strMemberUpdates = UpdatePurchaseRecordsMemberIDs();



            //Split time to see where slowness is occuring
            sw.Stop();
            timeComplete = sw.Elapsed;
            string _timeTaken = "  |  Timespan: " + timeComplete.ToString(@"hh\:mm\:ss");



            return sb.ToString() + "           Complete.  Added " + totalSubmitted.ToString() + " records out of " + _data.Count().ToString() + ".    " + strMemberUpdates + _timeTaken;
        }
        public string SavePurchaseRecords(List<bl.Models.api.PurchaseRecord> _data)
        {

            Stopwatch sw = Stopwatch.StartNew();
            TimeSpan timeComplete;
            sw.Start();



            //Instantiate variables.
            repoOriginalPurchaseRecords = new OriginalPurchaseRecordsRepository(new EF_SwtpDb());
            List<Original_PurchaseRecords> LstPurchaseRecords = new List<Original_PurchaseRecords>();
            List<Guid> LstGuids = repoOriginalPurchaseRecords.GetAllGuids();
            int totalSubmitted = 0;
            StringBuilder sb = new StringBuilder();


            //Add data to list if it does not exist in db.
            int index = 0;
            for (int i = 0; i < _data.Count; i++)
            {
                //reset list
                if (index == 0)
                    LstPurchaseRecords.Clear();


                //Add if record does not exist
                if (!LstGuids.Contains((Guid)_data[i].OriginalId))
                {
                    index++;
                    int _itemNumber = 0;

                    Original_PurchaseRecords purchaseRecord = new Original_PurchaseRecords();
                    if (_data[i].OriginalId != null) purchaseRecord.originalId = _data[i].OriginalId;
                    purchaseRecord.txn_id = _data[i].Txn_id;
                    purchaseRecord.created = _data[i].Created;
                    purchaseRecord.payerEmail = _data[i].PayerEmail;
                    purchaseRecord.itemName = _data[i].ItemName;
                    purchaseRecord.internalPurchase = _data[i].Internal;
                    if (int.TryParse(_data[i].ItemNumber, out _itemNumber)) purchaseRecord.itemNumber = _itemNumber;
                    if (!string.IsNullOrEmpty(_data[i].Price)) purchaseRecord.price = Convert.ToDecimal(_data[i].Price); //
                    if (!string.IsNullOrEmpty(_data[i].Discount)) purchaseRecord.discount = Convert.ToDecimal(_data[i].Discount); //

                    LstPurchaseRecords.Add(purchaseRecord);

                    //Submit grouped records in bulk
                    if (index == 100 || i == _data.Count - 1)
                    {
                        index = 0; //reset index

                        try
                        {
                            if (LstPurchaseRecords.Count() > 0) repoOriginalPurchaseRecords.BulkAddRecord(LstPurchaseRecords);
                            totalSubmitted += LstPurchaseRecords.Count();
                        }
                        catch (SqlException exS)
                        {
                            sb.AppendLine("      ");
                            sb.AppendLine("SqlException");
                            sb.AppendLine(exS.ToString());
                        }
                        catch (Exception ex)
                        {
                            sb.AppendLine(Newtonsoft.Json.JsonConvert.SerializeObject(purchaseRecord));
                            sb.AppendLine("      ");
                            sb.AppendLine("Exception");
                            sb.AppendLine(ex.Message);
                            //sb.AppendLine(ex.InnerException.ToString());
                        }
                    }
                }
            }


            //Add member IDs to each purchase record if missing.
            var strMemberUpdates = UpdatePurchaseRecordsMemberIDs();



            //Split time to see where slowness is occuring
            sw.Stop();
            timeComplete = sw.Elapsed;
            string _timeTaken = "  |  Timespan: " + timeComplete.ToString(@"hh\:mm\:ss");



            return "Import Purchase Records: " + sb.ToString() + "  Complete.  Added " + totalSubmitted.ToString() + " records out of " + _data.Count().ToString() + ".    " + strMemberUpdates + _timeTaken;
        }
        public string UpdatePurchaseRecordsMemberIDs()
        {
            int added = 0;
            int failed = 0;

            //Obtain all purchase records
            repoOriginalPurchaseRecords = new OriginalPurchaseRecordsRepository(new EF_SwtpDb());
            List<Original_PurchaseRecords> LstAllPurchaseRecords = repoOriginalPurchaseRecords.GetAll().Where(x => x.memberId == null).ToList();

            //Obtain all existing members in umbraco
            IEnumerable<IMember> LstMembers = Services.MemberService.GetAllMembers();


            //Update each record with member id
            for (int i = 0; i < LstAllPurchaseRecords.Count(); i++)
            {
                try
                {
                    LstAllPurchaseRecords[i].memberId = LstMembers.Where(x => x.Email == LstAllPurchaseRecords[i].payerEmail.ToLower()).FirstOrDefault().Id;
                    added++;
                }
                catch
                {
                    failed++;

                    //
                    try
                    {
                        if (CreateMember(LstAllPurchaseRecords[i].payerEmail.ToLower()))
                        {
                            LstAllPurchaseRecords[i].memberId = LstMembers.Where(x => x.Email == LstAllPurchaseRecords[i].payerEmail.ToLower()).FirstOrDefault().Id;
                            added++;
                            failed--;
                        }
                    }
                    catch { }

                }
            }


            //
            repoOriginalPurchaseRecords.BulkUpdateRecord(LstAllPurchaseRecords);


            return "Member ID Updates:  Updated " + added.ToString() + " records with " + failed.ToString() + " failed attempts.";
        }





        /*  Import Client Exams [Raw]   */
        /*=============================================================*/
        public string ObtainLatestUpdates(List<UpdateRecord> _data)
        {
            //Time how long this takes
            Stopwatch sw = Stopwatch.StartNew();
            TimeSpan timeTaken;
            sw.Start();

            //Example incoming data:
            //  [{"nodeId":15804412,"updateDate":"2023-03-07T15:10:01.66"}]



            //Instantiate variables.
            repoOriginalUpdateRecord = new OriginalUpdateRecordRepository(new EF_SwtpDb());
            List<Original_UpdateRecord> LstAllExistingRecords = repoOriginalUpdateRecord.GetAll();
            List<Original_UpdateRecord> LstAddRecords = new List<Original_UpdateRecord>();
            List<Original_UpdateRecord> LstUpdateRecords = new List<Original_UpdateRecord>();


            //Determine if record is to be added or updated
            foreach (var _updateRecord in _data)
            {
                if (LstAllExistingRecords.Where(x => x.nodeId == _updateRecord.nodeId).Any())
                {
                    Original_UpdateRecord _oldRecord = LstAllExistingRecords.FirstOrDefault(x => x.nodeId == _updateRecord.nodeId);
                    //Record exists.  see if it needs updating.
                    if (_oldRecord.updateDate != _updateRecord.updateDate)
                    {
                        //Update and add to list
                        _oldRecord.updateDate = _updateRecord.updateDate;
                        LstUpdateRecords.Add(_oldRecord);
                    }
                }
                else
                {
                    Original_UpdateRecord updateRecord = new Original_UpdateRecord();
                    updateRecord.nodeId = _updateRecord.nodeId;
                    updateRecord.updateDate = _updateRecord.updateDate;
                    LstAddRecords.Add(updateRecord);
                }
            }


            //Submit all records in bulk
            if (LstAddRecords.Count() > 0) repoOriginalUpdateRecord.BulkAddRecord(LstAddRecords);
            if (LstUpdateRecords.Count() > 0) repoOriginalUpdateRecord.BulkUpdateRecord(LstUpdateRecords);


            sw.Stop();
            timeTaken = sw.Elapsed;
            string timeLaps = "  |  Timespan: " + timeTaken.ToString(@"hh\:mm\:ss");


            return "Complete.  Added " + LstAddRecords.Count().ToString() + "  |  Updated " + LstUpdateRecords.Count().ToString() + "   Time Lapsed: " + timeLaps;
        }
        public string ObtainAllUmbracoNodes(List<UmbracoNode> _data)
        {
            //Time how long this takes
            Stopwatch sw = Stopwatch.StartNew();
            TimeSpan timeTaken;
            sw.Start();

            //Example incoming data:
            //  [{"id":15521808,"parentID":15521807,"level":3,"text":"Jan","createDate":"2023-01-01T01:07:47.9","path":"-1,1148,15521807,15521808"}]



            //Instantiate variables.
            repoOriginalUmbracoNode = new OriginalUmbracoNodeRepository(new EF_SwtpDb());
            HashSet<int> LstNodeIDs = repoOriginalUmbracoNode.GetAllIDs();
            List<Original_UmbracoNode> LstAddUmbracoNodes = new List<Original_UmbracoNode>();


            //Add data into lists if it does not exist in db.  NOTE: No need to do updates.  data does not change after created.
            foreach (var _umbracoNode in _data)
            {
                if (!LstNodeIDs.Contains(_umbracoNode.id))
                {
                    Original_UmbracoNode umbracoNode = new Original_UmbracoNode();
                    umbracoNode.id = _umbracoNode.id;
                    umbracoNode.parentID = _umbracoNode.parentID;
                    umbracoNode.level = _umbracoNode.level;
                    umbracoNode.path = _umbracoNode.path;
                    umbracoNode.text = _umbracoNode.text;
                    umbracoNode.createDate = _umbracoNode.createDate;
                    LstAddUmbracoNodes.Add(umbracoNode);
                }
            }

            //Submit all records in bulk
            if (LstAddUmbracoNodes.Count() > 0) repoOriginalUmbracoNode.BulkAddRecord(LstAddUmbracoNodes);


            sw.Stop();
            timeTaken = sw.Elapsed;
            string timeLaps = "  |  Timespan: " + timeTaken.ToString(@"hh\:mm\:ss");


            return "Complete.  Added " + LstAddUmbracoNodes.Count().ToString() + " Records.  Lapsed: " + timeLaps;
        }
        public string ObtainAllCmsPropertyData(List<Original_CmsPropertyData> _data)
        {
            //Example incoming data:
            //  [{"id":113887973,"contentNodeId":15521809,"propertytypeid":76,"dataInt":null,"dataDate":null,"dataNvarchar":null,"dataNtext":null}]


            //Time how long this takes
            Stopwatch sw = Stopwatch.StartNew();
            //TimeSpan timeTaken;
            TimeSpan time1_4;
            TimeSpan time2_4;
            TimeSpan time3_4 = new TimeSpan();
            TimeSpan time4_4 = new TimeSpan();
            TimeSpan timeComplete;
            sw.Start();


            //Instantiate variables.
            EF_SwtpDb _context = new EF_SwtpDb();
            _context.Database.CommandTimeout = 172800; //2 days
            _context.Configuration.AutoDetectChangesEnabled = false;
            repoOriginalCmsPropertyData = new OriginalCmsPropertyDataRepository(_context);

            HashSet<int> LstIDs = repoOriginalCmsPropertyData.SelectAllIDs();
            List<Original_CmsPropertyData> LstAddCmsPropertyDatas = new List<Original_CmsPropertyData>();
            List<Original_CmsPropertyData> LstUpdateCmsPropertyDatas = new List<Original_CmsPropertyData>();


            //Split time to see where slowness is occuring
            sw.Stop();
            time1_4 = sw.Elapsed;
            sw.Start();


            //Convert data into lists
            foreach (Original_CmsPropertyData _record in _data)
            {
                if (LstIDs.Contains(_record.id))
                {
                    //Update record
                    LstUpdateCmsPropertyDatas.Add(_record);
                }
                else
                {
                    //Add record
                    LstAddCmsPropertyDatas.Add(_record);
                }
            }


            //Split time to see where slowness is occuring
            sw.Stop();
            time2_4 = sw.Elapsed - time1_4;
            sw.Start();





            //Submits
            StringBuilder sb = new StringBuilder();
            try
            {
                //Submit all inserts in bulk
                _context = new EF_SwtpDb(); //Reset context and repo to improve performance.
                _context.Database.CommandTimeout = 172800; //2 days
                _context.Configuration.AutoDetectChangesEnabled = false;
                repoOriginalCmsPropertyData = new OriginalCmsPropertyDataRepository(_context);
                if (LstAddCmsPropertyDatas.Count() > 0) repoOriginalCmsPropertyData.BulkAddRecord(LstAddCmsPropertyDatas);



                //Split time to see where slowness is occuring
                sw.Stop();
                time3_4 = (sw.Elapsed - time2_4) - time1_4; //2.5 min
                sw.Start();



                //Submit all updates in bulk
                _context = new EF_SwtpDb(); //Reset context and repo to improve performance.
                _context.Database.CommandTimeout = 172800; //2 days
                _context.Configuration.AutoDetectChangesEnabled = false;
                repoOriginalCmsPropertyData = new OriginalCmsPropertyDataRepository(_context);
                if (LstUpdateCmsPropertyDatas.Count() > 0) repoOriginalCmsPropertyData.BulkUpdateRecord(LstUpdateCmsPropertyDatas);


            }
            catch (SqlException exS)
            {
                sb.AppendLine("   |||   ");
                sb.AppendLine("SqlException: ");
                sb.AppendLine("   |||   ");
                sb.AppendLine(exS.ToString());
            }
            catch (Exception ex)
            {
                sb.AppendLine("   |||   ");
                sb.AppendLine("Exception: ");
                sb.AppendLine(ex.Message);
                sb.AppendLine("   |||   ");
                sb.AppendLine(ex.InnerException.ToString());
            }



            sw.Stop();
            time4_4 = ((sw.Elapsed - time3_4) - time2_4) - time1_4;
            timeComplete = sw.Elapsed;




            string _timelaps =
                " |||  1/4: " + time1_4.ToString(@"hh\:mm\:ss")
                + " |  2/4: " + time2_4.ToString(@"hh\:mm\:ss")
                + " |  3/4: " + time3_4.ToString(@"hh\:mm\:ss")
                + " |  4/4: " + time4_4.ToString(@"hh\:mm\:ss")
                + " |  Complete: " + timeComplete.ToString(@"hh\:mm\:ss");



            return "Complete.  Added " + LstAddCmsPropertyDatas.Count().ToString() + "  |  Updated " + LstUpdateCmsPropertyDatas.Count().ToString() + _timelaps + sb.ToString();


        }





        /*  Migrate Original Records into Db   */
        /*=============================================================*/
        public string SaveExamBundles(List<ExamBundle> _data)
        {

            Stopwatch sw = Stopwatch.StartNew();
            TimeSpan timeComplete;
            sw.Start();



            //Instantiate variables.
            repoOriginalExamBundles = new OriginalExamBundlesRepository(new EF_SwtpDb());
            List<Original_ExamBundles> LstExamBundles = new List<Original_ExamBundles>();
            IEnumerable<Original_ExamBundles> LstExistingExamBundles = repoOriginalExamBundles.GetAll();



            //Add data to list if it does not exist in db.
            foreach (var _examBundle in _data)
            {
                if (!LstExistingExamBundles.Where(x => x.bundleId == _examBundle.BundleId).Any())
                {
                    Original_ExamBundles examBundle = new Original_ExamBundles();
                    examBundle.bundleName = _examBundle.BundleName;
                    examBundle.bundleId = _examBundle.BundleId;
                    examBundle.aswbExam1 = _examBundle.AswbExam1;
                    examBundle.aswbExam2 = _examBundle.AswbExam2;
                    examBundle.aswbExam3 = _examBundle.AswbExam3;
                    examBundle.aswbExam4 = _examBundle.AswbExam4;
                    examBundle.aswbExam5 = _examBundle.AswbExam5;
                    examBundle.dsmBooster = _examBundle.DsmBooster;
                    examBundle.ethicsBooster = _examBundle.EthicsBooster;
                    examBundle.californiaLawAndEthics = _examBundle.CaliforniaLawAndEthics;
                    LstExamBundles.Add(examBundle);
                }

            }

            //Submit all records in bulk
            if (LstExamBundles.Count() > 0) repoOriginalExamBundles.BulkAddRecord(LstExamBundles);



            //Split time to see where slowness is occuring
            sw.Stop();
            timeComplete = sw.Elapsed;
            string _timeTaken = "  |  Timespan: " + timeComplete.ToString(@"hh\:mm\:ss");



            return "Complete.  Added " + LstExamBundles.Count().ToString() + " records out of " + _data.Count().ToString() + "." + _timeTaken;
        }
        public string AddImportedPurchaseRecordsToSite()
        {

            Stopwatch sw = Stopwatch.StartNew();
            TimeSpan timeComplete;
            sw.Start();



            //Instantiate repositories
            Dictionary<int, string> DictExams = new Dictionary<int, string>();
            repoOriginalPurchaseRecords = new OriginalPurchaseRecordsRepository(new EF_SwtpDb());
            repoOriginalExamBundles = new OriginalExamBundlesRepository(new EF_SwtpDb());
            repoPurchaseRecord = new PurchaseRecordRepository(new EF_SwtpDb());
            repoPurchaseRecordItems = new PurchaseRecordItemRepository(new EF_SwtpDb());
            int updated = 0;
            int failed = 0;
            StringBuilder sb = new StringBuilder();


            //Get all paid exam names/IDs
            IPublishedContent ipExamFolder = Umbraco.Content((int)(Models.Common.SiteNode.Exams));
            foreach (IPublishedContent ipExam in ipExamFolder.DescendantsOfType(Models.Common.DocType.ExamPaid))
                DictExams.Add(ipExam.Id, ipExam.Name);


            //Get list of all Exam Bundles
            List<Original_ExamBundles> LstOrigExamBundles = repoOriginalExamBundles.GetAll().ToList();


            //Obtain all purchase records  [Test mode: add single record.  Once complete change to "GetAll()".
            List<Original_PurchaseRecords> LstAllPurchaseRecords = repoOriginalPurchaseRecords.GetAll().Where(x => x.memberId != null && x.itemNumber != null && !x.isAddedToSite).ToList();

            //  TESTING ONLY!! 
            //List<Original_PurchaseRecords> LstAllPurchaseRecords = new List<Original_PurchaseRecords>();
            //LstAllPurchaseRecords.Add(repoOriginalPurchaseRecords.GetById(17920)); // [Exam Bundle #158]


            foreach (Original_PurchaseRecords purchaseRecord in LstAllPurchaseRecords)
            {
                try
                {
                    //Instantiate variables
                    EF.PurchaseRecord newPurchaseRecord = new EF.PurchaseRecord();
                    Original_ExamBundles examBundle = null;


                    //Check if item number is for a single or bundle purchase
                    if (LstOrigExamBundles.Where(x => x.bundleId == purchaseRecord.itemNumber).Any())
                    {
                        //Is bundle
                        newPurchaseRecord.PurchaseTypeId = 2;
                        examBundle = LstOrigExamBundles.Where(x => x.bundleId == purchaseRecord.itemNumber).FirstOrDefault();
                    }
                    else
                        newPurchaseRecord.PurchaseTypeId = 1; //Is single exam purchase


                    //Create new db PurchaseRecord entry
                    if (purchaseRecord.created != null) newPurchaseRecord.PurchaseDate = (DateTime)purchaseRecord.created;
                    if (purchaseRecord.memberId != null) newPurchaseRecord.MemberId = (int)purchaseRecord.memberId;
                    newPurchaseRecord.BundleDiscount = 0;
                    newPurchaseRecord.CouponDiscount = 0;
                    newPurchaseRecord.TotalDiscount = purchaseRecord.discount;
                    newPurchaseRecord.TotalCost = Convert.ToDecimal(purchaseRecord.price);


                    //Check if exam bundle is California or whole US
                    if (examBundle != null && examBundle.californiaLawAndEthics)
                    {
                        newPurchaseRecord.BundleId = 1238;
                        newPurchaseRecord.BundleTitle = "California";
                    }
                    else
                    {
                        newPurchaseRecord.BundleId = 5398;
                        newPurchaseRecord.BundleTitle = "U.S. & Canada";
                    }


                    //Save original record as metadata.
                    newPurchaseRecord.Metadata = Newtonsoft.Json.JsonConvert.SerializeObject(purchaseRecord);


                    //Add new record to db.
                    repoPurchaseRecord.AddRecord(newPurchaseRecord);


                    //With new purchase record id, add EACH exam to [PurchaseRecordItem]
                    List<PurchaseRecordItem> LstPurchaseRecordItems = new List<PurchaseRecordItem>();
                    if (examBundle == null)
                    {
                        //Is single exam purchase
                        if (DictExams.ContainsValue(purchaseRecord.itemName))
                        {
                            LstPurchaseRecordItems.Add(CreateNewPurchaseRecordItem(newPurchaseRecord, (int)purchaseRecord.memberId, purchaseRecord.itemName, DictExams));
                        }
                    }
                    else
                    {
                        //Is bundle exam purchase
                        if (examBundle.aswbExam1)
                            LstPurchaseRecordItems.Add(CreateNewPurchaseRecordItem(newPurchaseRecord, (int)purchaseRecord.memberId, "ASWB Exam #1", DictExams));
                        if (examBundle.aswbExam2)
                            LstPurchaseRecordItems.Add(CreateNewPurchaseRecordItem(newPurchaseRecord, (int)purchaseRecord.memberId, "ASWB Exam #2", DictExams));
                        if (examBundle.aswbExam3)
                            LstPurchaseRecordItems.Add(CreateNewPurchaseRecordItem(newPurchaseRecord, (int)purchaseRecord.memberId, "ASWB Exam #3", DictExams));
                        if (examBundle.aswbExam4)
                            LstPurchaseRecordItems.Add(CreateNewPurchaseRecordItem(newPurchaseRecord, (int)purchaseRecord.memberId, "ASWB Exam #4", DictExams));
                        if (examBundle.aswbExam5)
                            LstPurchaseRecordItems.Add(CreateNewPurchaseRecordItem(newPurchaseRecord, (int)purchaseRecord.memberId, "ASWB Exam #5", DictExams));
                        if (examBundle.californiaLawAndEthics)
                            LstPurchaseRecordItems.Add(CreateNewPurchaseRecordItem(newPurchaseRecord, (int)purchaseRecord.memberId, "California Law & Ethics", DictExams));
                        if (examBundle.dsmBooster)
                            LstPurchaseRecordItems.Add(CreateNewPurchaseRecordItem(newPurchaseRecord, (int)purchaseRecord.memberId, "DSM Booster", DictExams));
                        if (examBundle.ethicsBooster)
                            LstPurchaseRecordItems.Add(CreateNewPurchaseRecordItem(newPurchaseRecord, (int)purchaseRecord.memberId, "Ethics Booster", DictExams));
                    }


                    //Add to [PurchaseRecordItem]
                    repoPurchaseRecordItems.BulkAddRecord(LstPurchaseRecordItems);


                    //Mark record as applied to db
                    purchaseRecord.isAddedToSite = true;
                    repoOriginalPurchaseRecords.UpdateRecord(purchaseRecord);


                    updated++;
                }
                catch (Exception ex)
                {
                    failed++;
                    sb.AppendLine("  |  ");
                    sb.AppendLine(ex.InnerException.ToString());
                }
            }



            //Split time to see where slowness is occuring
            sw.Stop();
            timeComplete = sw.Elapsed;
            string _timeTaken = "  |  Timespan: " + timeComplete.ToString(@"hh\:mm\:ss");



            return "Complete.  Updated: " + updated.ToString() + "  |  Failed: " + failed.ToString() + sb.ToString() + _timeTaken;
        }
        public string CleanImportedExams()
        {
            Stopwatch sw = Stopwatch.StartNew();
            TimeSpan time1_3;
            TimeSpan time2_3;
            TimeSpan timeComplete;
            sw.Start();


            //Instantiate repositories
            EF_SwtpDb _context = new EF_SwtpDb();
            _context.Database.CommandTimeout = 172800; //2 days
            repoOriginalUmbracoNode = new OriginalUmbracoNodeRepository(_context);


            //Instantiate variables
            IEnumerable<Original_UmbracoNode> Lst_lvl4 = repoOriginalUmbracoNode.SelectAll_Lvl4();
            IEnumerable<Original_UmbracoNode> Lst_lvl5 = repoOriginalUmbracoNode.SelectAll_Lvl5();
            List<Original_UmbracoNode> LstDeletion = new List<Original_UmbracoNode>();


            //Split time to see where slowness is occuring
            sw.Stop();
            time1_3 = sw.Elapsed;
            sw.Start();


            //Determine how many records need to be deleted.
            var index = 0;
            foreach (Original_UmbracoNode record in Lst_lvl4)
            {
                //index++;
                if (!Lst_lvl5.Where(x => x.parentID == record.id).Any())
                {
                    LstDeletion.Add(record);
                }
                //if (index == 10) break;
            }


            //Split time to see where slowness is occuring
            sw.Stop();
            time2_3 = sw.Elapsed - time1_3;
            sw.Start();


            try
            {
                //Delete records
                repoOriginalUmbracoNode.BulkDeleteRecords(LstDeletion);
            }
            catch (Exception ex)
            {
                Logger.Error<ApiMigrateDataController>(ex);
            }


            sw.Stop();
            timeComplete = sw.Elapsed;

            //estimate total time required IF testing with index.
            TimeSpan estimatedTime = timeComplete;
            if (index > 0)
            {
                estimatedTime = TimeSpan.FromTicks(timeComplete.Ticks * (Lst_lvl4.Count() / index));
            }



            return "Lst_lvl4: " + Lst_lvl4.Count() + " | Lst_lvl5: " + Lst_lvl5.Count() + " | LstDeletion: " + LstDeletion.Count()
                + " |  1/3: " + time1_3.ToString(@"hh\:mm\:ss")
                + " |  2/3: " + time2_3.ToString(@"hh\:mm\:ss")
                + " |  Complete: " + timeComplete.ToString(@"hh\:mm\:ss")
                + " |  Estimated Total Time: " + estimatedTime.ToString(@"hh\:mm\:ss");
        }
        public string ExtendAllExams()
        {
            Stopwatch sw = Stopwatch.StartNew();
            TimeSpan time1_3;
            TimeSpan time2_3;
            TimeSpan timeComplete;
            sw.Start();


            //Instantiate repositories
            EF_SwtpDb _context = new EF_SwtpDb();
            _context.Database.CommandTimeout = 172800; //2 days
            repoPurchaseRecord = new PurchaseRecordRepository(_context);
            repoPurchaseRecordItems = new PurchaseRecordItemRepository(_context);


            //Instantiate variables
            Dictionary<int, DateTime> lstPurchaseDates = repoPurchaseRecord.SelectAllPurchaseDates_asDictionary();
            List<PurchaseRecordItem> lstPurchasedExams = repoPurchaseRecordItems.SelectAll();
            List<PurchaseRecordItem> LstToUpdate = new List<PurchaseRecordItem>();


            //Split time to see where slowness is occuring
            sw.Stop();
            time1_3 = sw.Elapsed;
            sw.Start();


            //Loop through lists to find matches by date
            for (int i = 0; i < lstPurchaseDates.Count; i++)
            {
                var pair = lstPurchaseDates.ElementAt(i);
                foreach (var record in lstPurchasedExams.Where(
                    x => x.PurchaseRecordId == pair.Key &&
                    x.ExpirationDate == pair.Value.Date &&
                    x.Extensions == 0))
                {
                    //Update record and add to list for submission.
                    record.ExpirationDate = record.ExpirationDate.AddDays(180);
                    record.Extensions = 1;
                    LstToUpdate.Add(record);
                }
            }



            //Split time to see where slowness is occuring
            sw.Stop();
            time2_3 = sw.Elapsed - time1_3;
            sw.Start();


            //Update all records
            repoPurchaseRecordItems.BulkUpdateRecord(LstToUpdate);



            sw.Stop();
            timeComplete = sw.Elapsed;



            return "lstPurchaseDates: " + lstPurchaseDates.Count() + " | lstPurchasedExams: " + lstPurchasedExams.Count() + " | LstToUpdate: " + LstToUpdate.Count()
                + " |  1/3: " + time1_3.ToString(@"hh\:mm\:ss")
                + " |  2/3: " + time2_3.ToString(@"hh\:mm\:ss")
                + " |  Complete: " + timeComplete.ToString(@"hh\:mm\:ss");
        }
        public string UpdateCouponCounters(List<pwdDiscount> lstPwdDiscounts)
        {
            //Start watch
            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();


            //Instantiate variables.
            repoPurchaseRecord = new PurchaseRecordRepository(new EF_SwtpDb());
            repoCoupons = new CouponRepository(new EF_SwtpDb());
            List<Coupon> lstAllCoupons = repoCoupons.GetAll();
            List<Coupon> lstAddCoupons = new List<Coupon>();
            List<Coupon> lstUpdateCoupons = new List<Coupon>();
            List<EF.PurchaseRecord> lstCurrentPurchaseRecords = repoPurchaseRecord.ObtainAll_withCouponId();
            List<CouponSetting> lstCouponSettings = new List<CouponSetting>();


            try
            {
                //Seperate records into add/update lists
                foreach (pwdDiscount _prevRecord in lstPwdDiscounts)
                {
                    if (lstAllCoupons.Any(x => x.Code == _prevRecord.Code))
                    {
                        //Create record
                        Coupon record = lstAllCoupons.FirstOrDefault(x => x.Code == _prevRecord.Code);
                        record.TimesUsed = _prevRecord.TimesUsed; //apply # of times used in old site
                        record.TimesUsed += lstCurrentPurchaseRecords.Where(x => x.CouponId == record.CouponId).Count(); //Add # of times used in new site.

                        //Add to update list
                        lstUpdateCoupons.Add(record);
                    }
                    else
                    {
                        //Determine type of discount and amount
                        string _discType = "";
                        decimal _discAmt = 0;
                        if (_prevRecord.DiscountAmount != null && _prevRecord.DiscountAmount > 0)
                        {
                            _discAmt = (decimal)_prevRecord.DiscountAmount;
                        }
                        else if (_prevRecord.DiscountPercent != null && _prevRecord.DiscountPercent > 0)
                        {
                            _discAmt = (decimal)_prevRecord.DiscountPercent;
                            _discType = "Percent";
                        }
                        else
                            continue; //skip... missing info.


                        //Create new pre-coupon setting
                        CouponSetting couponSetting = new CouponSetting()
                        {
                            CouponName = _prevRecord.Code,
                            DiscountAmount = _discAmt,
                            DiscountType = _discType,
                            Expires = _prevRecord.ExpireDate,
                            MaxAllowed = (int?)_prevRecord.TimesUsedLimit,
                            Notes = _prevRecord.Notes
                        };

                        //Add to list to be created as new coupon
                        lstCouponSettings.Add(couponSetting);
                    }
                }


                //Create list of new coupons
                foreach (CouponSetting couponSetting in lstCouponSettings)
                {
                    //Instantiate variables
                    bl.EF.Coupon coupon = new bl.EF.Coupon();


                    //Apply updates to record 
                    coupon.Code = couponSetting.CouponName;
                    coupon.CreateDate = DateTime.Now;

                    //Get # of times used
                    coupon.TimesUsed = 0;
                    pwdDiscount _pwdDiscount = lstPwdDiscounts.FirstOrDefault(x => x.Code == couponSetting.CouponName);
                    if (_pwdDiscount != null)
                    {
                        coupon.TimesUsed = _pwdDiscount.TimesUsed;
                    }

                    coupon.CouponTypeId = 1;

                    if (couponSetting.DiscountType == "Percent")
                    {
                        coupon.DiscountByPercentage = true;
                        coupon.DiscountPercent = couponSetting.DiscountAmount;
                        coupon.DiscountAmount = null;
                    }
                    else
                    {
                        coupon.DiscountByPercentage = false;
                        coupon.DiscountAmount = couponSetting.DiscountAmount;
                        coupon.DiscountPercent = null;
                    }

                    coupon.ExpireDate = couponSetting.Expires;
                    if (couponSetting.Expires != null)
                        coupon.CouponTypeId = 2;

                    coupon.TimesUsedLimit = couponSetting.MaxAllowed;
                    if (couponSetting.MaxAllowed != null)
                        coupon.CouponTypeId = 3;

                    coupon.Enabled = true;
                    coupon.Notes = couponSetting.Notes;



                    //Add to list
                    lstAddCoupons.Add(coupon);

                }


                //Submit all updates in bulk
                repoCoupons.BulkAddRecord(lstAddCoupons);
                repoCoupons.BulkUpdateRecord(lstUpdateCoupons);
            }
            catch (Exception ex) { Logger.Error<ApiMigrateDataController>(ex); }



            sw.Stop();
            return "Complete.  Added " + lstAddCoupons.Count().ToString() + " records.  Updated " + lstUpdateCoupons.Count().ToString() + ".   |  Total Time: " + sw.Elapsed.ToString(@"hh\:mm\:ss");
        }
        public string RemoveDuplicateMembers()
        {
            Stopwatch sw = Stopwatch.StartNew();
            TimeSpan time1_3;
            TimeSpan time2_3;
            TimeSpan time3_3;
            TimeSpan timeComplete;
            sw.Start();


            //var a = _dbContext.cmsMembers.AsNoTracking().ToList();
            //var b = _dbContext.cmsMembers.AsNoTracking().GroupBy(x => x.LoginName).Where(y => y.Count() > 1);
            string _delete = "DELETE_";




            //Instantiate repositories
            EF_SwtpDb _context = new EF_SwtpDb();
            _context.Database.CommandTimeout = 172800; //2 days
            repoCmsMembers = new CmsMemberRepository(_context);
            repoExamRecords = new ExamRecordRepository(_context);


            //Obtain data
            HashSet<int?> memberIDsInExams = repoExamRecords.GetAllDistinctIDs();
            List<cmsMember> lstCmsMembers = repoCmsMembers.SelectAll();
            List<cmsMember> lstMembersToUpdate = new List<cmsMember>();




            //Split time to see where slowness is occuring
            sw.Stop();
            time1_3 = sw.Elapsed;
            sw.Start();


            foreach (cmsMember cmsMember in lstCmsMembers)
            {
                if (!cmsMember.LoginName.Contains(_delete))
                {
                    if (!memberIDsInExams.Contains(cmsMember.nodeId))
                    {
                        if (lstCmsMembers.Where(x => x.LoginName == cmsMember.LoginName).Count() > 1)
                        {
                            //Update the username and email
                            cmsMember.LoginName = _delete + cmsMember.LoginName;
                            cmsMember.Email = _delete + cmsMember.Email;
                            lstMembersToUpdate.Add(cmsMember);
                        }
                    }
                }
            }


            //Split time to see where slowness is occuring
            sw.Stop();
            time2_3 = sw.Elapsed - time1_3;
            sw.Start();



            //Update records
            repoCmsMembers.BulkUpdateRecord(lstMembersToUpdate);



            sw.Stop();
            time3_3 = sw.Elapsed - time2_3 - time1_3;
            timeComplete = sw.Elapsed;




            //HashSet<int> memberIDsInExams = repoExamRecords.GetAllDistinctIDs();
            //List<cmsMember> lstCmsMembers = repoCmsMembers.SelectAll();
            //List<cmsMember> lstMembersToUpdate = new List<cmsMember>();

            string counter = memberIDsInExams.Count.ToString() + " | " + lstCmsMembers.Count.ToString() + " | " + lstMembersToUpdate.Count.ToString();


            return "Total records added: " + counter
                + " |  1/3: " + time1_3.ToString(@"hh\:mm\:ss")
                + " |  2/3: " + time2_3.ToString(@"hh\:mm\:ss")
                + " |  3/3: " + time2_3.ToString(@"hh\:mm\:ss")
                + " |  Complete: " + timeComplete.ToString(@"hh\:mm\:ss");

            //return "Unique member count in exams: " + memberIDsInExams.Count.ToString();

            //return Newtonsoft.Json.JsonConvert.SerializeObject(memberIDsInExams);
            //return Newtonsoft.Json.JsonConvert.SerializeObject(memberIDsInExams);
        }
        //public string ConvertImportedExamResults()
        //{
        //    Stopwatch sw = Stopwatch.StartNew();
        //    //TimeSpan timeTaken;
        //    TimeSpan time1_3;
        //    TimeSpan time2_3;
        //    TimeSpan time3_3;
        //    TimeSpan timeComplete;
        //    sw.Start();



        //    List<object> lstObjects = new List<object>();
        //    int counter = 0;
        //    int error = 0;
        //    //TEST USER
        //    //adivinestrategy@gmail.com - 15524391
        //    //user:		adivinestrategy@gmail.com
        //    //member id:	23843		
        //    //ExamID		7670554	[+]
        //    //exam folder id:		15524392	Exam Member
        //    //						15525751 [+] Exam Score
        //    //							15525752	Answer Folder
        //    //								15525763	Answer



        //    //Instantiate repositories
        //    EF_SwtpDb _context = new EF_SwtpDb(); //Reset context and repo to improve performance.
        //    _context.Database.CommandTimeout = 172800; //2 days
        //    repoOriginalCmsPropertyData = new OriginalCmsPropertyDataRepository(_context);
        //    repoExamIdRelationships = new ExamIdRelationshipRepository(_context);
        //    repoOriginalUmbracoNode = new OriginalUmbracoNodeRepository(_context);
        //    repoOriginalMemberData = new OriginalMemberDataRepository(_context);
        //    repoExamRecords = new ExamRecordRepository(_context);
        //    repoExamAnswerSet = new ExamAnswerSetRepository(_context);
        //    repoExamAnswer = new ExamAnswerRepository(_context);
        //    repoExamMode = new ExamModeRepository(_context);


        //    //Instantiate variables
        //    Dictionary<int, string> DictExams = new Dictionary<int, string>();
        //    List<ExamIDs_Old_New> LstExamIDs = repoExamIdRelationships.SelectAll_ExceptText(); //Get all exam IDs from relationship table
        //    List<Original_UmbracoNode> LstAllOrigUmbNodes_lvl4 = repoOriginalUmbracoNode.SelectAll_Lvl4().ToList();
        //    List<Original_UmbracoNode> LstAllOrigUmbNodes_lvl5 = repoOriginalUmbracoNode.SelectAll_Lvl5().ToList();
        //    List<Original_UmbracoNode> LstAllOrigUmbNodes_lvl6 = repoOriginalUmbracoNode.SelectAll_Lvl6().ToList();
        //    List<Original_UmbracoNode> LstAllOrigUmbNodes_lvl7 = repoOriginalUmbracoNode.SelectAll_Lvl7().ToList();
        //    List<Original_CmsPropertyData> LstCmsProperties = repoOriginalCmsPropertyData.SelectAll().ToList();
        //    List<ExamMode> LstExamModes = repoExamMode.SelectAll();
        //    List<Original_UmbracoNode> LstAllUpdatedRecords = new List<Original_UmbracoNode>();
        //    List<Original_MemberData> LstOriginalMemberData = repoOriginalMemberData.SelectAll();




        //    //Obtain all existing members in umbraco
        //    //List<IMember> lstExistinMembers = Services.MemberService.GetAllMembers().ToList();







        //    //TEMP!!!  
        //    List<IMember> lstExistinMembers = new List<IMember>();
        //    lstExistinMembers.Add(Services.MemberService.GetByEmail("ragillespie.1996@gmail.com"));
        //    lstExistinMembers.Add(Services.MemberService.GetByEmail("Drakefire.rg@gmail.com"));
        //    //lstExistinMembers.Add(Services.MemberService.GetByEmail("Lindsey.thorp@firstinclay.org"));
        //    //lstExistinMembers.Add(Services.MemberService.GetByEmail("linnynewton13@gmail.com"));
        //    //lstExistinMembers.Add(Services.MemberService.GetByEmail("Drakefire.rg@gmail.com"));
        //    //lstExistinMembers.Add(Services.MemberService.GetByEmail("colleen.mishra@gmail.com"));
        //    //lstExistinMembers.Add(Services.MemberService.GetByEmail("kellercm@gmail.com"));
        //    //lstExistinMembers.Add(Services.MemberService.GetByEmail("Bjonescm23@gmail.com"));
        //    //lstExistinMembers.Add(Services.MemberService.GetByEmail("bri_jones23@yahoo.com"));
        //    //lstExistinMembers.Add(Services.MemberService.GetByEmail("Clinical.Soc.Worker@gmail.com"));
        //    //lstExistinMembers.Add(Services.MemberService.GetByEmail("MJZPublic@comcast.net"));
        //    //lstExistinMembers.Add(Services.MemberService.GetByEmail("rae_abern0812@yahoo.com"));
        //    //lstExistinMembers.Add(Services.MemberService.GetByEmail("selmon.lyser2020@gmail.com"));
        //    //lstExistinMembers.Add(Services.MemberService.GetByEmail("selmon.lyser88@yahoo.com"));
        //    //lstExistinMembers.Add(Services.MemberService.GetByEmail("lbrockman@msbranch.org"));
        //    //lstExistinMembers.Add(Services.MemberService.GetByEmail("armed4pony@yahoo.com"));
        //    //lstExistinMembers.Add(Services.MemberService.GetByEmail("toyinidowu140@gmail.com"));
        //    //lstExistinMembers.Add(Services.MemberService.GetByEmail("toyinadetunji@aol.com"));
        //    //lstExistinMembers.Add(Services.MemberService.GetByEmail("raemel30@gmail.com"));
        //    //lstExistinMembers.Add(Services.MemberService.GetByEmail("kimberly.mikkonen@waldenu.edu"));
        //    //lstExistinMembers.Add(Services.MemberService.GetByEmail("adamsloretta8@gmail.com"));
        //    //lstExistinMembers.Add(Services.MemberService.GetByEmail("janelleraenaek@gmail.com"));
        //    //lstExistinMembers.Add(Services.MemberService.GetByEmail("franklinaisha1@gmail.com"));
        //    //lstExistinMembers.Add(Services.MemberService.GetByEmail("meganmccabe89@gmail.com"));
        //    //lstExistinMembers.Add(Services.MemberService.GetByEmail("oghinp32@bellsouth.net"));
        //    //lstExistinMembers.Add(Services.MemberService.GetByEmail("DAYNAMICHELLE00@HOTMAIL.COM"));
        //    //lstExistinMembers.Add(Services.MemberService.GetByEmail("nyeeshaali@gmail.com"));
        //    //lstExistinMembers.Add(Services.MemberService.GetByEmail("meganmccabe89@gmail.com"));
        //    //lstExistinMembers.Add(Services.MemberService.GetByEmail("abraham.sanu@gmail.com"));








        //    //Get all paid exam names/IDs
        //    IPublishedContent ipExamFolder = Umbraco.Content((int)(Models.Common.SiteNode.Exams));
        //    foreach (IPublishedContent ipExam in ipExamFolder.DescendantsOfType(Models.Common.DocType.ExamPaid))
        //        DictExams.Add(ipExam.Id, ipExam.Name);


        //    //Split time to see where slowness is occuring
        //    sw.Stop();
        //    time1_3 = sw.Elapsed;
        //    sw.Restart();




        //    var onlyDo = 0;
        //    foreach (IMember _member in lstExistinMembers)
        //    {


        //        //Refresh all needed contexts
        //        _context = new EF_SwtpDb(); //Reset context and repo to improve performance.
        //        _context.Database.CommandTimeout = 172800; //2 days
        //        repoExamRecords = new ExamRecordRepository(_context);
        //        repoExamAnswerSet = new ExamAnswerSetRepository(_context);
        //        repoExamAnswer = new ExamAnswerRepository(_context);

        //        try
        //        {
        //            if (LstOriginalMemberData.Any(x => x.Email == _member.Email))
        //            {
        //                //onlyDo++;

        //                //Get original member id
        //                int oldMemberId = LstOriginalMemberData.FirstOrDefault(x => x.Email == _member.Email).MemberId;


        //                //Get list of original exam folder records for user (text contains both user email AND old member id.)
        //                List<Original_UmbracoNode> lstLvl4Nodes_filtered = LstAllOrigUmbNodes_lvl4.Where(x => x.text.Contains(_member.Email) && x.text.Contains(oldMemberId.ToString())).ToList();


        //                foreach (Original_UmbracoNode lvl4Record in lstLvl4Nodes_filtered)
        //                {
        //                    counter++;
        //                    LstAllUpdatedRecords.Add(lvl4Record); //Add to list for updating as added to site

        //                    //Get list of original exam records for lvl5 related to lvl4 record
        //                    List<Original_UmbracoNode> lstLvl5Nodes_filtered = LstAllOrigUmbNodes_lvl5.Where(x => x.path.Contains(lvl4Record.path + ",")).OrderBy(x => x.createDate).ToList();

        //                    foreach (Original_UmbracoNode lvl5Record in lstLvl5Nodes_filtered)
        //                    {
        //                        counter++;
        //                        LstAllUpdatedRecords.Add(lvl5Record); //Add to list for updating as added to site

        //                        //Does exam name exist in list of paid exams. (eliminates any old named exams or free ones.)
        //                        if (DictExams.ContainsValue(lvl5Record.text.Split('-').First().Trim()))
        //                        {
        //                            //Pull all data from property repo and validate
        //                            string _tempExamMode = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 116).dataNvarchar;
        //                            string _tempSubscriptionId = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 115).dataNvarchar;
        //                            string _tempExamId = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 87).dataNvarchar;
        //                            string _tempSubmitted = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 89).dataNvarchar;
        //                            bool submitted;
        //                            var result = Boolean.TryParse(_tempSubmitted, out submitted);
        //                            string _tempSubmittedDate = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 172).dataNvarchar;
        //                            DateTime submittedDate;
        //                            bool submittedDateValid = DateTime.TryParse(_tempSubmittedDate, out submittedDate);
        //                            string _tempTimeRemaining = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 92).dataNvarchar;
        //                            TimeSpan _timespan;
        //                            bool _tempTimeRemainingIsValid = TimeSpan.TryParse(_tempTimeRemaining, out _timespan);



        //                            //Create new exam record and add to db
        //                            ExamRecord _examRecord = new ExamRecord();
        //                            _examRecord.ExamModeId = LstExamModes.FirstOrDefault(x => x.Mode == _tempExamMode).ExamModeId;
        //                            _examRecord.MemberId = _member.Id;
        //                            if (!string.IsNullOrEmpty(_tempSubscriptionId)) _examRecord.SubscriptionId = Convert.ToInt32(_tempSubscriptionId);
        //                            _examRecord.ExamId = (int)LstExamIDs.FirstOrDefault(x => x.ExamId_old == Convert.ToInt32(_tempExamId)).ExamId_new;
        //                            _examRecord.CreatedDate = lvl5Record.createDate;
        //                            _examRecord.Submitted = submitted;
        //                            if (submittedDateValid) _examRecord.SubmittedDate = submittedDate;
        //                            if (_tempTimeRemainingIsValid) _examRecord.TimeRemaining = _timespan;
        //                            repoExamRecords.AddRecord(_examRecord);


        //                            //Get list of original exam records for lvl6
        //                            Original_UmbracoNode lvl6Node_filtered = LstAllOrigUmbNodes_lvl6.FirstOrDefault(x => x.parentID == lvl5Record.id);
        //                            LstAllUpdatedRecords.Add(lvl6Node_filtered); //Add to list for updating as added to site
        //                            counter++;

        //                            //Get answerset list as csv and convert to new id list
        //                            Original_CmsPropertyData lvl6CmsPropertyData = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl6Node_filtered.id && x.propertytypeid == 169);
        //                            HashSet<int> lstOldAnswerSet = lvl6CmsPropertyData.dataNtext.Split(',').Select(i => Int32.Parse(i)).ToHashSet();
        //                            HashSet<int> lstNewAnswerSet = new HashSet<int>();
        //                            foreach (int _id in lstOldAnswerSet)
        //                            {
        //                                lstNewAnswerSet.Add((int)LstExamIDs.FirstOrDefault(x => x.QuestionId_old == _id).QuestionId_new);
        //                            }

        //                            //Create new exam answerset
        //                            ExamAnswerSet _examAnswerSet = new ExamAnswerSet();
        //                            _examAnswerSet.ExamRecordId = _examRecord.ExamRecordId;
        //                            _examAnswerSet.AnswerSet = String.Join(",", lstNewAnswerSet.Select(x => x.ToString()).ToArray());
        //                            repoExamAnswerSet.AddRecord(_examAnswerSet);


        //                            //Get all property datas per question and consolidate into list of answers
        //                            List<AnswerRecord_former> lstAnswerRecords = new List<AnswerRecord_former>();
        //                            List<Original_UmbracoNode> lvl7Node_filtered = LstAllOrigUmbNodes_lvl7.Where(x => x.parentID == lvl6Node_filtered.id).ToList();
        //                            foreach (Original_UmbracoNode lvl7Record in lvl7Node_filtered)
        //                            {
        //                                counter++;
        //                                LstAllUpdatedRecords.Add(lvl7Record); //Add to list for updating as added to site

        //                                AnswerRecord_former _answerRecord = new AnswerRecord_former();
        //                                List<Original_CmsPropertyData> lstPropertyData = LstCmsProperties.Where(x => x.contentNodeId == lvl7Record.id).ToList();
        //                                foreach (Original_CmsPropertyData _property in lstPropertyData)
        //                                {
        //                                    counter++;
        //                                    switch (_property.propertytypeid)
        //                                    {
        //                                        case 109: //questionId
        //                                            _answerRecord.oldQuestionId = _property.dataNvarchar;
        //                                            break;
        //                                        case 110: //answerId
        //                                            _answerRecord.answerId = _property.dataNvarchar;
        //                                            break;
        //                                        case 111: //correct
        //                                            _answerRecord.correct = _property.dataNvarchar;
        //                                            break;
        //                                        case 112: //review
        //                                            _answerRecord.review = _property.dataNvarchar;
        //                                            break;
        //                                        case 113: //answersRendered
        //                                            _answerRecord.answersRendered = _property.dataNvarchar;
        //                                            break;
        //                                        case 114: //contentArea
        //                                            _answerRecord.oldContentArea = _property.dataNvarchar;
        //                                            break;
        //                                        default: break;
        //                                    }
        //                                }

        //                                //Convert old/new IDs
        //                                ExamIDs_Old_New examIDs = LstExamIDs.FirstOrDefault(x => x.QuestionId_old == Convert.ToInt32(_answerRecord.oldQuestionId));
        //                                _answerRecord.newQuestionId = (int)examIDs.QuestionId_new;
        //                                _answerRecord.newContentArea = (int)examIDs.ContentId_new;

        //                                lstAnswerRecords.Add(_answerRecord);
        //                            }


        //                            //Create all answer records
        //                            int index = 0;
        //                            foreach (AnswerRecord_former _answerRecord in lstAnswerRecords)
        //                            {
        //                                bool tempBool = false;

        //                                ExamAnswer _examAnswer = new ExamAnswer();
        //                                _examAnswer.ExamAnswerSetId = _examAnswerSet.ExamAnswerSetId;
        //                                _examAnswer.ContentAreaId = _answerRecord.newContentArea;
        //                                _examAnswer.QuestionId = _answerRecord.newQuestionId;
        //                                _examAnswer.QuestionRenderOrder = index;
        //                                _examAnswer.AnswerRenderedOrder = _answerRecord.answersRendered;
        //                                if (!string.IsNullOrEmpty(_answerRecord.answerId)) _examAnswer.SelectedAnswer = Convert.ToInt32(_answerRecord.answerId);
        //                                _examAnswer.CorrectAnswer = GetCorrectAnswerId(_answerRecord.newQuestionId);
        //                                if (bool.TryParse(_answerRecord.correct, out tempBool)) _examAnswer.IsCorrect = tempBool;
        //                                if (bool.TryParse(_answerRecord.review, out tempBool)) _examAnswer.ReviewQuestion = tempBool;
        //                                repoExamAnswer.AddRecord(_examAnswer);
        //                                index++;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                Logger.Warn<ApiMigrateDataController>(Newtonsoft.Json.JsonConvert.SerializeObject(_member));
        //                error++;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger.Error<ApiMigrateDataController>(ex);
        //            Logger.Warn<ApiMigrateDataController>(Newtonsoft.Json.JsonConvert.SerializeObject(_member));
        //            error++;
        //            //break; //===============================================================================================================================================
        //        }


        //        //if (onlyDo >= 10) break;
        //    }



        //    //Split time to see where slowness is occuring
        //    sw.Stop();
        //    time2_3 = sw.Elapsed;
        //    sw.Restart();



        //    //Update all records as being added to site
        //    for (int i = 0; i < LstAllUpdatedRecords.Count(); i++)
        //    {
        //        LstAllUpdatedRecords[i].isAddedToSite = true;
        //    }

        //    try
        //    {
        //        _context = new EF_SwtpDb(); //Reset context and repo to improve performance.
        //        _context.Database.CommandTimeout = 172800; //2 days
        //        repoOriginalUmbracoNode = new OriginalUmbracoNodeRepository(_context);
        //        repoOriginalUmbracoNode.BulkUpdateRecord(LstAllUpdatedRecords);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error<ApiMigrateDataController>(ex);
        //    }







        //    //Split time to see where slowness is occuring
        //    sw.Stop();
        //    time3_3 = sw.Elapsed;
        //    timeComplete = time3_3 + time2_3 + time1_3;


        //    //estimate total time required IF testing with index.
        //    TimeSpan estimatedTime = timeComplete;
        //    if (onlyDo > 0)
        //    {
        //        estimatedTime = TimeSpan.FromTicks((time2_3.Ticks + time3_3.Ticks) * (lstExistinMembers.Count() / onlyDo)) + time1_3;
        //    }



        //    return "Converted records: " + counter.ToString()
        //        + " |  Errors: " + error.ToString()
        //        + " |  1/3: " + time1_3.ToString(@"hh\:mm\:ss")
        //        + " |  2/3: " + time2_3.ToString(@"hh\:mm\:ss")
        //        + " |  2/3: " + time3_3.ToString(@"hh\:mm\:ss")
        //        + " |  Timespan: " + timeComplete.ToString(@"hh\:mm\:ss")
        //        + " |  Estimated Time: " + estimatedTime.ToString(@"hh\:mm\:ss");

        //}
        //public string ConvertImportedExamResults()
        //{
        //    Stopwatch sw = Stopwatch.StartNew();
        //    //TimeSpan timeTaken;
        //    TimeSpan time1_5;
        //    TimeSpan time2_5;
        //    TimeSpan time3_5;
        //    TimeSpan time4_5;
        //    TimeSpan time5_5;
        //    TimeSpan timeComplete;
        //    sw.Start();



        //    List<object> lstObjects = new List<object>();
        //    int counter = 0;
        //    int error = 0;
        //    //TEST USER
        //    //adivinestrategy@gmail.com - 15524391
        //    //user:		adivinestrategy@gmail.com
        //    //member id:	23843		
        //    //ExamID		7670554	[+]
        //    //exam folder id:		15524392	Exam Member
        //    //						15525751 [+] Exam Score
        //    //							15525752	Answer Folder
        //    //								15525763	Answer



        //    //Instantiate repositories
        //    EF_SwtpDb _context = new EF_SwtpDb(); //Reset context and repo to improve performance.
        //    _context.Database.CommandTimeout = 172800; //2 days
        //    repoOriginalCmsPropertyData = new OriginalCmsPropertyDataRepository(_context);
        //    repoExamIdRelationships = new ExamIdRelationshipRepository(_context);
        //    repoOriginalUmbracoNode = new OriginalUmbracoNodeRepository(_context);
        //    repoOriginalMemberData = new OriginalMemberDataRepository(_context);
        //    repoExamRecords = new ExamRecordRepository(_context);
        //    repoExamAnswerSet = new ExamAnswerSetRepository(_context);
        //    repoExamAnswer = new ExamAnswerRepository(_context);
        //    repoExamMode = new ExamModeRepository(_context);


        //    //Instantiate variables
        //    Dictionary<int, string> DictExams = new Dictionary<int, string>();
        //    List<ExamIDs_Old_New> LstExamIDs = repoExamIdRelationships.SelectAll_ExceptText(); //Get all exam IDs from relationship table
        //    List<Original_UmbracoNode> LstAllOrigUmbNodes_lvl4 = repoOriginalUmbracoNode.SelectAll_Lvl4().ToList();
        //    List<Original_UmbracoNode> LstAllOrigUmbNodes_lvl5 = repoOriginalUmbracoNode.SelectAll_Lvl5().ToList();
        //    List<Original_UmbracoNode> LstAllOrigUmbNodes_lvl6 = repoOriginalUmbracoNode.SelectAll_Lvl6().ToList();
        //    List<Original_UmbracoNode> LstAllOrigUmbNodes_lvl7 = repoOriginalUmbracoNode.SelectAll_Lvl7().ToList();
        //    List<Original_CmsPropertyData> LstCmsProperties = repoOriginalCmsPropertyData.SelectAll().ToList();
        //    List<ExamMode> LstExamModes = repoExamMode.SelectAll();
        //    List<Original_UmbracoNode> LstAllUpdatedRecords = new List<Original_UmbracoNode>();
        //    List<Original_MemberData> LstOriginalMemberData = repoOriginalMemberData.SelectAll();
        //    int onlyDo = 0;


        //    //Obtain all existing members in umbraco
        //    List<IMember> lstExistinMembers = Services.MemberService.GetAllMembers().ToList();


        //    ////TEMP!!!  
        //    //List<IMember> lstExistinMembers = new List<IMember>();
        //    //lstExistinMembers.Add(Services.MemberService.GetByEmail("jim.fifth@5thstudios.com"));


        //    //Get all paid exam names/IDs
        //    IPublishedContent ipExamFolder = Umbraco.Content((int)(Models.Common.SiteNode.Exams));
        //    foreach (IPublishedContent ipExam in ipExamFolder.DescendantsOfType(Models.Common.DocType.ExamPaid))
        //        DictExams.Add(ipExam.Id, ipExam.Name);



        //    //=========================================================================================================================================
        //    sw.Stop();
        //    time1_5 = sw.Elapsed;
        //    sw.Restart();
        //    //=========================================================================================================================================














        //    foreach (IMember _member in lstExistinMembers)
        //    {


        //        //Refresh all needed contexts
        //        _context = new EF_SwtpDb(); //Reset context and repo to improve performance.
        //        _context.Database.CommandTimeout = 172800; //2 days
        //        repoExamRecords = new ExamRecordRepository(_context);
        //        repoExamAnswerSet = new ExamAnswerSetRepository(_context);
        //        repoExamAnswer = new ExamAnswerRepository(_context);

        //        try
        //        {
        //            if (LstOriginalMemberData.Any(x => x.Email == _member.Email))
        //            {
        //                onlyDo++;

        //                //Get original member id
        //                int oldMemberId = LstOriginalMemberData.FirstOrDefault(x => x.Email == _member.Email).MemberId;


        //                //Get list of original exam folder records for user (text contains both user email AND old member id.)
        //                List<Original_UmbracoNode> lstLvl4Nodes_filtered = LstAllOrigUmbNodes_lvl4.Where(x => x.text.Contains(_member.Email) && x.text.Contains(oldMemberId.ToString())).ToList();


        //                foreach (Original_UmbracoNode lvl4Record in lstLvl4Nodes_filtered)
        //                {
        //                    counter++;
        //                    LstAllUpdatedRecords.Add(lvl4Record); //Add to list for updating as added to site

        //                    //Get list of original exam records for lvl5 related to lvl4 record
        //                    List<Original_UmbracoNode> lstLvl5Nodes_filtered = LstAllOrigUmbNodes_lvl5.Where(x => x.path.Contains(lvl4Record.path + ",")).OrderBy(x => x.createDate).ToList();

        //                    foreach (Original_UmbracoNode lvl5Record in lstLvl5Nodes_filtered)
        //                    {
        //                        counter++;
        //                        LstAllUpdatedRecords.Add(lvl5Record); //Add to list for updating as added to site

        //                        //Does exam name exist in list of paid exams. (eliminates any old named exams or free ones.)
        //                        if (DictExams.ContainsValue(lvl5Record.text.Split('-').First().Trim()))
        //                        {
        //                            //Pull all data from property repo and validate
        //                            string _tempExamMode = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 116).dataNvarchar;
        //                            string _tempSubscriptionId = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 115).dataNvarchar;
        //                            string _tempExamId = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 87).dataNvarchar;
        //                            string _tempSubmitted = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 89).dataNvarchar;
        //                            bool submitted;
        //                            var result = Boolean.TryParse(_tempSubmitted, out submitted);
        //                            string _tempSubmittedDate = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 172).dataNvarchar;
        //                            DateTime submittedDate;
        //                            bool submittedDateValid = DateTime.TryParse(_tempSubmittedDate, out submittedDate);
        //                            string _tempTimeRemaining = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 92).dataNvarchar;
        //                            TimeSpan _timespan;
        //                            bool _tempTimeRemainingIsValid = TimeSpan.TryParse(_tempTimeRemaining, out _timespan);



        //                            //Create new exam record and add to db
        //                            ExamRecord _examRecord = new ExamRecord();
        //                            _examRecord.ExamModeId = LstExamModes.FirstOrDefault(x => x.Mode == _tempExamMode).ExamModeId;
        //                            _examRecord.MemberId = _member.Id;
        //                            if (!string.IsNullOrEmpty(_tempSubscriptionId)) _examRecord.SubscriptionId = Convert.ToInt32(_tempSubscriptionId);
        //                            _examRecord.ExamId = (int)LstExamIDs.FirstOrDefault(x => x.ExamId_old == Convert.ToInt32(_tempExamId)).ExamId_new;
        //                            _examRecord.CreatedDate = lvl5Record.createDate;
        //                            _examRecord.Submitted = submitted;
        //                            if (submittedDateValid) _examRecord.SubmittedDate = submittedDate;
        //                            if (_tempTimeRemainingIsValid) _examRecord.TimeRemaining = _timespan;
        //                            repoExamRecords.AddRecord(_examRecord);


        //                            //Get list of original exam records for lvl6
        //                            Original_UmbracoNode lvl6Node_filtered = LstAllOrigUmbNodes_lvl6.FirstOrDefault(x => x.parentID == lvl5Record.id);
        //                            LstAllUpdatedRecords.Add(lvl6Node_filtered); //Add to list for updating as added to site
        //                            counter++;

        //                            //Get answerset list as csv and convert to new id list
        //                            Original_CmsPropertyData lvl6CmsPropertyData = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl6Node_filtered.id && x.propertytypeid == 169);
        //                            HashSet<int> lstOldAnswerSet = lvl6CmsPropertyData.dataNtext.Split(',').Select(i => Int32.Parse(i)).ToHashSet();
        //                            HashSet<int> lstNewAnswerSet = new HashSet<int>();
        //                            foreach (int _id in lstOldAnswerSet)
        //                            {
        //                                lstNewAnswerSet.Add((int)LstExamIDs.FirstOrDefault(x => x.QuestionId_old == _id).QuestionId_new);
        //                            }

        //                            //Create new exam answerset
        //                            ExamAnswerSet _examAnswerSet = new ExamAnswerSet();
        //                            _examAnswerSet.ExamRecordId = _examRecord.ExamRecordId;
        //                            _examAnswerSet.AnswerSet = String.Join(",", lstNewAnswerSet.Select(x => x.ToString()).ToArray());
        //                            repoExamAnswerSet.AddRecord(_examAnswerSet);


        //                            //Get all property datas per question and consolidate into list of answers
        //                            List<AnswerRecord_former> lstAnswerRecords = new List<AnswerRecord_former>();
        //                            List<Original_UmbracoNode> lvl7Node_filtered = LstAllOrigUmbNodes_lvl7.Where(x => x.parentID == lvl6Node_filtered.id).ToList();
        //                            foreach (Original_UmbracoNode lvl7Record in lvl7Node_filtered)
        //                            {
        //                                counter++;
        //                                LstAllUpdatedRecords.Add(lvl7Record); //Add to list for updating as added to site

        //                                AnswerRecord_former _answerRecord = new AnswerRecord_former();
        //                                List<Original_CmsPropertyData> lstPropertyData = LstCmsProperties.Where(x => x.contentNodeId == lvl7Record.id).ToList();
        //                                foreach (Original_CmsPropertyData _property in lstPropertyData)
        //                                {
        //                                    counter++;
        //                                    switch (_property.propertytypeid)
        //                                    {
        //                                        case 109: //questionId
        //                                            _answerRecord.oldQuestionId = _property.dataNvarchar;
        //                                            break;
        //                                        case 110: //answerId
        //                                            _answerRecord.answerId = _property.dataNvarchar;
        //                                            break;
        //                                        case 111: //correct
        //                                            _answerRecord.correct = _property.dataNvarchar;
        //                                            break;
        //                                        case 112: //review
        //                                            _answerRecord.review = _property.dataNvarchar;
        //                                            break;
        //                                        case 113: //answersRendered
        //                                            _answerRecord.answersRendered = _property.dataNvarchar;
        //                                            break;
        //                                        case 114: //contentArea
        //                                            _answerRecord.oldContentArea = _property.dataNvarchar;
        //                                            break;
        //                                        default: break;
        //                                    }
        //                                }

        //                                //Convert old/new IDs
        //                                ExamIDs_Old_New examIDs = LstExamIDs.FirstOrDefault(x => x.QuestionId_old == Convert.ToInt32(_answerRecord.oldQuestionId));
        //                                _answerRecord.newQuestionId = (int)examIDs.QuestionId_new;
        //                                _answerRecord.newContentArea = (int)examIDs.ContentId_new;

        //                                lstAnswerRecords.Add(_answerRecord);
        //                            }


        //                            //Create all answer records
        //                            int index = 0;
        //                            foreach (AnswerRecord_former _answerRecord in lstAnswerRecords)
        //                            {
        //                                bool tempBool = false;

        //                                ExamAnswer _examAnswer = new ExamAnswer();
        //                                _examAnswer.ExamAnswerSetId = _examAnswerSet.ExamAnswerSetId;
        //                                _examAnswer.ContentAreaId = _answerRecord.newContentArea;
        //                                _examAnswer.QuestionId = _answerRecord.newQuestionId;
        //                                _examAnswer.QuestionRenderOrder = index;
        //                                _examAnswer.AnswerRenderedOrder = _answerRecord.answersRendered;
        //                                if (!string.IsNullOrEmpty(_answerRecord.answerId)) _examAnswer.SelectedAnswer = Convert.ToInt32(_answerRecord.answerId);
        //                                _examAnswer.CorrectAnswer = GetCorrectAnswerId(_answerRecord.newQuestionId);
        //                                if (bool.TryParse(_answerRecord.correct, out tempBool)) _examAnswer.IsCorrect = tempBool;
        //                                if (bool.TryParse(_answerRecord.review, out tempBool)) _examAnswer.ReviewQuestion = tempBool;
        //                                repoExamAnswer.AddRecord(_examAnswer);
        //                                index++;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                error++;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger.Error<ApiMigrateDataController>(ex);
        //            error++;
        //            //break; //===============================================================================================================================================
        //        }


        //        if (onlyDo >= 10) break;
        //    }






        //    //=========================================================================================================================================
        //    sw.Stop();
        //    time2_5 = sw.Elapsed;
        //    sw.Restart();
        //    //=========================================================================================================================================





        //    foreach (IMember _member in lstExistinMembers)
        //    {


        //        //Refresh all needed contexts
        //        _context = new EF_SwtpDb(); //Reset context and repo to improve performance.
        //        _context.Database.CommandTimeout = 172800; //2 days
        //        repoExamRecords = new ExamRecordRepository(_context);
        //        repoExamAnswerSet = new ExamAnswerSetRepository(_context);
        //        repoExamAnswer = new ExamAnswerRepository(_context);

        //        try
        //        {
        //            if (LstOriginalMemberData.Any(x => x.Email == _member.Email))
        //            {
        //                onlyDo++;

        //                //Get original member id
        //                int oldMemberId = LstOriginalMemberData.FirstOrDefault(x => x.Email == _member.Email).MemberId;


        //                //Get list of original exam folder records for user (text contains both user email AND old member id.)
        //                List<Original_UmbracoNode> lstLvl4Nodes_filtered = LstAllOrigUmbNodes_lvl4.Where(x => x.text.Contains(_member.Email) && x.text.Contains(oldMemberId.ToString())).ToList();


        //                foreach (Original_UmbracoNode lvl4Record in lstLvl4Nodes_filtered)
        //                {
        //                    counter++;
        //                    LstAllUpdatedRecords.Add(lvl4Record); //Add to list for updating as added to site

        //                    //Get list of original exam records for lvl5 related to lvl4 record
        //                    List<Original_UmbracoNode> lstLvl5Nodes_filtered = LstAllOrigUmbNodes_lvl5.Where(x => x.path.Contains(lvl4Record.path + ",")).OrderBy(x => x.createDate).ToList();

        //                    foreach (Original_UmbracoNode lvl5Record in lstLvl5Nodes_filtered)
        //                    {
        //                        counter++;
        //                        LstAllUpdatedRecords.Add(lvl5Record); //Add to list for updating as added to site

        //                        //Does exam name exist in list of paid exams. (eliminates any old named exams or free ones.)
        //                        if (DictExams.ContainsValue(lvl5Record.text.Split('-').First().Trim()))
        //                        {
        //                            //Pull all data from property repo and validate
        //                            string _tempExamMode = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 116).dataNvarchar;
        //                            string _tempSubscriptionId = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 115).dataNvarchar;
        //                            string _tempExamId = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 87).dataNvarchar;
        //                            string _tempSubmitted = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 89).dataNvarchar;
        //                            bool submitted;
        //                            var result = Boolean.TryParse(_tempSubmitted, out submitted);
        //                            string _tempSubmittedDate = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 172).dataNvarchar;
        //                            DateTime submittedDate;
        //                            bool submittedDateValid = DateTime.TryParse(_tempSubmittedDate, out submittedDate);
        //                            string _tempTimeRemaining = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 92).dataNvarchar;
        //                            TimeSpan _timespan;
        //                            bool _tempTimeRemainingIsValid = TimeSpan.TryParse(_tempTimeRemaining, out _timespan);



        //                            //Create new exam record and add to db
        //                            ExamRecord _examRecord = new ExamRecord();
        //                            _examRecord.ExamModeId = LstExamModes.FirstOrDefault(x => x.Mode == _tempExamMode).ExamModeId;
        //                            _examRecord.MemberId = _member.Id;
        //                            if (!string.IsNullOrEmpty(_tempSubscriptionId)) _examRecord.SubscriptionId = Convert.ToInt32(_tempSubscriptionId);
        //                            _examRecord.ExamId = (int)LstExamIDs.FirstOrDefault(x => x.ExamId_old == Convert.ToInt32(_tempExamId)).ExamId_new;
        //                            _examRecord.CreatedDate = lvl5Record.createDate;
        //                            _examRecord.Submitted = submitted;
        //                            if (submittedDateValid) _examRecord.SubmittedDate = submittedDate;
        //                            if (_tempTimeRemainingIsValid) _examRecord.TimeRemaining = _timespan;
        //                            repoExamRecords.AddRecord(_examRecord);


        //                            //Get list of original exam records for lvl6
        //                            Original_UmbracoNode lvl6Node_filtered = LstAllOrigUmbNodes_lvl6.FirstOrDefault(x => x.parentID == lvl5Record.id);
        //                            LstAllUpdatedRecords.Add(lvl6Node_filtered); //Add to list for updating as added to site
        //                            counter++;

        //                            //Get answerset list as csv and convert to new id list
        //                            Original_CmsPropertyData lvl6CmsPropertyData = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl6Node_filtered.id && x.propertytypeid == 169);
        //                            HashSet<int> lstOldAnswerSet = lvl6CmsPropertyData.dataNtext.Split(',').Select(i => Int32.Parse(i)).ToHashSet();
        //                            HashSet<int> lstNewAnswerSet = new HashSet<int>();
        //                            foreach (int _id in lstOldAnswerSet)
        //                            {
        //                                lstNewAnswerSet.Add((int)LstExamIDs.FirstOrDefault(x => x.QuestionId_old == _id).QuestionId_new);
        //                            }

        //                            //Create new exam answerset
        //                            ExamAnswerSet _examAnswerSet = new ExamAnswerSet();
        //                            _examAnswerSet.ExamRecordId = _examRecord.ExamRecordId;
        //                            _examAnswerSet.AnswerSet = String.Join(",", lstNewAnswerSet.Select(x => x.ToString()).ToArray());
        //                            repoExamAnswerSet.AddRecord(_examAnswerSet);


        //                            //Get all property datas per question and consolidate into list of answers
        //                            List<AnswerRecord_former> lstAnswerRecords = new List<AnswerRecord_former>();
        //                            List<Original_UmbracoNode> lvl7Node_filtered = LstAllOrigUmbNodes_lvl7.Where(x => x.parentID == lvl6Node_filtered.id).ToList();
        //                            foreach (Original_UmbracoNode lvl7Record in lvl7Node_filtered)
        //                            {
        //                                counter++;
        //                                LstAllUpdatedRecords.Add(lvl7Record); //Add to list for updating as added to site

        //                                AnswerRecord_former _answerRecord = new AnswerRecord_former();
        //                                List<Original_CmsPropertyData> lstPropertyData = LstCmsProperties.Where(x => x.contentNodeId == lvl7Record.id).ToList();
        //                                foreach (Original_CmsPropertyData _property in lstPropertyData)
        //                                {
        //                                    counter++;
        //                                    switch (_property.propertytypeid)
        //                                    {
        //                                        case 109: //questionId
        //                                            _answerRecord.oldQuestionId = _property.dataNvarchar;
        //                                            break;
        //                                        case 110: //answerId
        //                                            _answerRecord.answerId = _property.dataNvarchar;
        //                                            break;
        //                                        case 111: //correct
        //                                            _answerRecord.correct = _property.dataNvarchar;
        //                                            break;
        //                                        case 112: //review
        //                                            _answerRecord.review = _property.dataNvarchar;
        //                                            break;
        //                                        case 113: //answersRendered
        //                                            _answerRecord.answersRendered = _property.dataNvarchar;
        //                                            break;
        //                                        case 114: //contentArea
        //                                            _answerRecord.oldContentArea = _property.dataNvarchar;
        //                                            break;
        //                                        default: break;
        //                                    }
        //                                }

        //                                //Convert old/new IDs
        //                                ExamIDs_Old_New examIDs = LstExamIDs.FirstOrDefault(x => x.QuestionId_old == Convert.ToInt32(_answerRecord.oldQuestionId));
        //                                _answerRecord.newQuestionId = (int)examIDs.QuestionId_new;
        //                                _answerRecord.newContentArea = (int)examIDs.ContentId_new;

        //                                lstAnswerRecords.Add(_answerRecord);
        //                            }


        //                            //Create all answer records
        //                            int index = 0;
        //                            foreach (AnswerRecord_former _answerRecord in lstAnswerRecords)
        //                            {
        //                                bool tempBool = false;

        //                                ExamAnswer _examAnswer = new ExamAnswer();
        //                                _examAnswer.ExamAnswerSetId = _examAnswerSet.ExamAnswerSetId;
        //                                _examAnswer.ContentAreaId = _answerRecord.newContentArea;
        //                                _examAnswer.QuestionId = _answerRecord.newQuestionId;
        //                                _examAnswer.QuestionRenderOrder = index;
        //                                _examAnswer.AnswerRenderedOrder = _answerRecord.answersRendered;
        //                                if (!string.IsNullOrEmpty(_answerRecord.answerId)) _examAnswer.SelectedAnswer = Convert.ToInt32(_answerRecord.answerId);
        //                                _examAnswer.CorrectAnswer = GetCorrectAnswerId(_answerRecord.newQuestionId);
        //                                if (bool.TryParse(_answerRecord.correct, out tempBool)) _examAnswer.IsCorrect = tempBool;
        //                                if (bool.TryParse(_answerRecord.review, out tempBool)) _examAnswer.ReviewQuestion = tempBool;
        //                                repoExamAnswer.AddRecord(_examAnswer);
        //                                index++;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                error++;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger.Error<ApiMigrateDataController>(ex);
        //            error++;
        //            //break; //===============================================================================================================================================
        //        }



        //    }






        //    //=========================================================================================================================================
        //    sw.Stop();
        //    time3_5 = sw.Elapsed;
        //    sw.Restart();
        //    //=========================================================================================================================================





        //    foreach (IMember _member in lstExistinMembers)
        //    {
        //        //Refresh all needed contexts
        //        _context = new EF_SwtpDb(); //Reset context and repo to improve performance.
        //        _context.Database.CommandTimeout = 172800; //2 days
        //        repoExamRecords = new ExamRecordRepository(_context);
        //        repoExamAnswerSet = new ExamAnswerSetRepository(_context);
        //        repoExamAnswer = new ExamAnswerRepository(_context);

        //        try
        //        {
        //            if (LstOriginalMemberData.Any(x => x.Email == _member.Email))
        //            {
        //                onlyDo++;

        //                //Get original member id
        //                int oldMemberId = LstOriginalMemberData.FirstOrDefault(x => x.Email == _member.Email).MemberId;


        //                //Get list of original exam folder records for user (text contains both user email AND old member id.)
        //                List<Original_UmbracoNode> lstLvl4Nodes_filtered = LstAllOrigUmbNodes_lvl4.Where(x => x.text.Contains(_member.Email) && x.text.Contains(oldMemberId.ToString())).ToList();


        //                foreach (Original_UmbracoNode lvl4Record in lstLvl4Nodes_filtered)
        //                {
        //                    counter++;
        //                    LstAllUpdatedRecords.Add(lvl4Record); //Add to list for updating as added to site

        //                    //Get list of original exam records for lvl5 related to lvl4 record
        //                    List<Original_UmbracoNode> lstLvl5Nodes_filtered = LstAllOrigUmbNodes_lvl5.Where(x => x.path.Contains(lvl4Record.path + ",")).OrderBy(x => x.createDate).ToList();

        //                    foreach (Original_UmbracoNode lvl5Record in lstLvl5Nodes_filtered)
        //                    {
        //                        counter++;
        //                        LstAllUpdatedRecords.Add(lvl5Record); //Add to list for updating as added to site

        //                        //Does exam name exist in list of paid exams. (eliminates any old named exams or free ones.)
        //                        if (DictExams.ContainsValue(lvl5Record.text.Split('-').First().Trim()))
        //                        {
        //                            //Pull all data from property repo and validate
        //                            string _tempExamMode = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 116).dataNvarchar;
        //                            string _tempSubscriptionId = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 115).dataNvarchar;
        //                            string _tempExamId = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 87).dataNvarchar;
        //                            string _tempSubmitted = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 89).dataNvarchar;
        //                            bool submitted;
        //                            var result = Boolean.TryParse(_tempSubmitted, out submitted);
        //                            string _tempSubmittedDate = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 172).dataNvarchar;
        //                            DateTime submittedDate;
        //                            bool submittedDateValid = DateTime.TryParse(_tempSubmittedDate, out submittedDate);
        //                            string _tempTimeRemaining = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 92).dataNvarchar;
        //                            TimeSpan _timespan;
        //                            bool _tempTimeRemainingIsValid = TimeSpan.TryParse(_tempTimeRemaining, out _timespan);



        //                            //Create new exam record and add to db
        //                            ExamRecord _examRecord = new ExamRecord();
        //                            _examRecord.ExamModeId = LstExamModes.FirstOrDefault(x => x.Mode == _tempExamMode).ExamModeId;
        //                            _examRecord.MemberId = _member.Id;
        //                            if (!string.IsNullOrEmpty(_tempSubscriptionId)) _examRecord.SubscriptionId = Convert.ToInt32(_tempSubscriptionId);
        //                            _examRecord.ExamId = (int)LstExamIDs.FirstOrDefault(x => x.ExamId_old == Convert.ToInt32(_tempExamId)).ExamId_new;
        //                            _examRecord.CreatedDate = lvl5Record.createDate;
        //                            _examRecord.Submitted = submitted;
        //                            if (submittedDateValid) _examRecord.SubmittedDate = submittedDate;
        //                            if (_tempTimeRemainingIsValid) _examRecord.TimeRemaining = _timespan;
        //                            repoExamRecords.AddRecord(_examRecord);


        //                            //Get list of original exam records for lvl6
        //                            Original_UmbracoNode lvl6Node_filtered = LstAllOrigUmbNodes_lvl6.FirstOrDefault(x => x.parentID == lvl5Record.id);
        //                            LstAllUpdatedRecords.Add(lvl6Node_filtered); //Add to list for updating as added to site
        //                            counter++;

        //                            //Get answerset list as csv and convert to new id list
        //                            Original_CmsPropertyData lvl6CmsPropertyData = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl6Node_filtered.id && x.propertytypeid == 169);
        //                            HashSet<int> lstOldAnswerSet = lvl6CmsPropertyData.dataNtext.Split(',').Select(i => Int32.Parse(i)).ToHashSet();
        //                            HashSet<int> lstNewAnswerSet = new HashSet<int>();
        //                            foreach (int _id in lstOldAnswerSet)
        //                            {
        //                                lstNewAnswerSet.Add((int)LstExamIDs.FirstOrDefault(x => x.QuestionId_old == _id).QuestionId_new);
        //                            }

        //                            //Create new exam answerset
        //                            ExamAnswerSet _examAnswerSet = new ExamAnswerSet();
        //                            _examAnswerSet.ExamRecordId = _examRecord.ExamRecordId;
        //                            _examAnswerSet.AnswerSet = String.Join(",", lstNewAnswerSet.Select(x => x.ToString()).ToArray());
        //                            repoExamAnswerSet.AddRecord(_examAnswerSet);


        //                            //Get all property datas per question and consolidate into list of answers
        //                            List<AnswerRecord_former> lstAnswerRecords = new List<AnswerRecord_former>();
        //                            List<Original_UmbracoNode> lvl7Node_filtered = LstAllOrigUmbNodes_lvl7.Where(x => x.parentID == lvl6Node_filtered.id).ToList();
        //                            foreach (Original_UmbracoNode lvl7Record in lvl7Node_filtered)
        //                            {
        //                                counter++;
        //                                LstAllUpdatedRecords.Add(lvl7Record); //Add to list for updating as added to site

        //                                AnswerRecord_former _answerRecord = new AnswerRecord_former();
        //                                List<Original_CmsPropertyData> lstPropertyData = LstCmsProperties.Where(x => x.contentNodeId == lvl7Record.id).ToList();
        //                                foreach (Original_CmsPropertyData _property in lstPropertyData)
        //                                {
        //                                    counter++;
        //                                    switch (_property.propertytypeid)
        //                                    {
        //                                        case 109: //questionId
        //                                            _answerRecord.oldQuestionId = _property.dataNvarchar;
        //                                            break;
        //                                        case 110: //answerId
        //                                            _answerRecord.answerId = _property.dataNvarchar;
        //                                            break;
        //                                        case 111: //correct
        //                                            _answerRecord.correct = _property.dataNvarchar;
        //                                            break;
        //                                        case 112: //review
        //                                            _answerRecord.review = _property.dataNvarchar;
        //                                            break;
        //                                        case 113: //answersRendered
        //                                            _answerRecord.answersRendered = _property.dataNvarchar;
        //                                            break;
        //                                        case 114: //contentArea
        //                                            _answerRecord.oldContentArea = _property.dataNvarchar;
        //                                            break;
        //                                        default: break;
        //                                    }
        //                                }

        //                                //Convert old/new IDs
        //                                ExamIDs_Old_New examIDs = LstExamIDs.FirstOrDefault(x => x.QuestionId_old == Convert.ToInt32(_answerRecord.oldQuestionId));
        //                                _answerRecord.newQuestionId = (int)examIDs.QuestionId_new;
        //                                _answerRecord.newContentArea = (int)examIDs.ContentId_new;

        //                                lstAnswerRecords.Add(_answerRecord);
        //                            }


        //                            //Create all answer records
        //                            int index = 0;
        //                            foreach (AnswerRecord_former _answerRecord in lstAnswerRecords)
        //                            {
        //                                bool tempBool = false;

        //                                ExamAnswer _examAnswer = new ExamAnswer();
        //                                _examAnswer.ExamAnswerSetId = _examAnswerSet.ExamAnswerSetId;
        //                                _examAnswer.ContentAreaId = _answerRecord.newContentArea;
        //                                _examAnswer.QuestionId = _answerRecord.newQuestionId;
        //                                _examAnswer.QuestionRenderOrder = index;
        //                                _examAnswer.AnswerRenderedOrder = _answerRecord.answersRendered;
        //                                if (!string.IsNullOrEmpty(_answerRecord.answerId)) _examAnswer.SelectedAnswer = Convert.ToInt32(_answerRecord.answerId);
        //                                _examAnswer.CorrectAnswer = GetCorrectAnswerId(_answerRecord.newQuestionId);
        //                                if (bool.TryParse(_answerRecord.correct, out tempBool)) _examAnswer.IsCorrect = tempBool;
        //                                if (bool.TryParse(_answerRecord.review, out tempBool)) _examAnswer.ReviewQuestion = tempBool;
        //                                repoExamAnswer.AddRecord(_examAnswer);
        //                                index++;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                error++;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger.Error<ApiMigrateDataController>(ex);
        //            error++;
        //            //break; //===============================================================================================================================================
        //        }

        //    }







        //    //=========================================================================================================================================
        //    sw.Stop();
        //    time4_5 = sw.Elapsed;
        //    sw.Restart();
        //    //=========================================================================================================================================





        //    //Update all records as being added to site
        //    for (int i = 0; i < LstAllUpdatedRecords.Count(); i++)
        //    {
        //        LstAllUpdatedRecords[i].isAddedToSite = true;
        //    }

        //    try
        //    {
        //        _context = new EF_SwtpDb(); //Reset context and repo to improve performance.
        //        _context.Database.CommandTimeout = 172800; //2 days
        //        repoOriginalUmbracoNode = new OriginalUmbracoNodeRepository(_context);
        //        repoOriginalUmbracoNode.BulkUpdateRecord(LstAllUpdatedRecords);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error<ApiMigrateDataController>(ex);
        //    }




        //    //=========================================================================================================================================
        //    sw.Stop();
        //    time5_5 = sw.Elapsed;
        //    timeComplete = TimeSpan.FromTicks(time1_5.Ticks + time2_5.Ticks + time3_5.Ticks + time4_5.Ticks + time5_5.Ticks) ;

        //    //estimate total time required IF testing with index.
        //    TimeSpan estimatedTime = timeComplete;
        //    if (onlyDo > 0)
        //    {
        //        estimatedTime = TimeSpan.FromTicks((time2_5.Ticks + time3_5.Ticks + time4_5.Ticks + time5_5.Ticks) * (lstExistinMembers.Count() / onlyDo)) + time1_5;
        //    }
        //    //=========================================================================================================================================







        //    return "Converted records: " + counter.ToString()
        //        + " |  Errors: " + error.ToString()
        //        + " |  1/5: " + time1_5.ToString(@"hh\:mm\:ss")
        //        + " |  2/5: " + time2_5.ToString(@"hh\:mm\:ss")
        //        + " |  2/5: " + time3_5.ToString(@"hh\:mm\:ss")
        //        + " |  2/5: " + time4_5.ToString(@"hh\:mm\:ss")
        //        + " |  2/5: " + time5_5.ToString(@"hh\:mm\:ss")
        //        + " |  Timespan: " + timeComplete.ToString(@"hh\:mm\:ss")
        //        + " |  Estimated Time: " + estimatedTime.ToString(@"hh\:mm\:ss");

        //}





        //public string ConvertImportedExamResults()
        //{
        //    Stopwatch sw = Stopwatch.StartNew();
        //    //TimeSpan timeTaken;
        //    TimeSpan time1_3;
        //    TimeSpan time2_3;
        //    TimeSpan time3_3;
        //    TimeSpan timeComplete;
        //    sw.Start();



        //    List<object> lstObjects = new List<object>();
        //    int counter = 0;
        //    int error = 0;
        //    //TEST USER
        //    //adivinestrategy@gmail.com - 15524391
        //    //user:		adivinestrategy@gmail.com
        //    //member id:	23843		
        //    //ExamID		7670554	[+]
        //    //exam folder id:		15524392	Exam Member
        //    //						15525751 [+] Exam Score
        //    //							15525752	Answer Folder
        //    //								15525763	Answer



        //    //Instantiate repositories
        //    EF_SwtpDb _context = new EF_SwtpDb(); //Reset context and repo to improve performance.
        //    _context.Database.CommandTimeout = 172800; //2 days
        //    repoOriginalCmsPropertyData = new OriginalCmsPropertyDataRepository(_context);
        //    repoExamIdRelationships = new ExamIdRelationshipRepository(_context);
        //    repoOriginalUmbracoNode = new OriginalUmbracoNodeRepository(_context);
        //    repoOriginalMemberData = new OriginalMemberDataRepository(_context);
        //    repoExamRecords = new ExamRecordRepository(_context);
        //    repoExamAnswerSet = new ExamAnswerSetRepository(_context);
        //    repoExamAnswer = new ExamAnswerRepository(_context);
        //    repoExamMode = new ExamModeRepository(_context);


        //    //Instantiate variables
        //    Dictionary<int, string> DictExams = new Dictionary<int, string>();
        //    List<ExamIDs_Old_New> LstExamIDs = repoExamIdRelationships.SelectAll_ExceptText(); //Get all exam IDs from relationship table
        //    List<Original_UmbracoNode> LstAllOrigUmbNodes_lvl4 = repoOriginalUmbracoNode.SelectAll_Lvl4().ToList();
        //    List<Original_UmbracoNode> LstAllOrigUmbNodes_lvl5 = repoOriginalUmbracoNode.SelectAll_Lvl5().ToList();
        //    List<Original_UmbracoNode> LstAllOrigUmbNodes_lvl6 = repoOriginalUmbracoNode.SelectAll_Lvl6().ToList();
        //    List<Original_UmbracoNode> LstAllOrigUmbNodes_lvl7 = repoOriginalUmbracoNode.SelectAll_Lvl7().ToList();
        //    List<Original_CmsPropertyData> LstCmsProperties = repoOriginalCmsPropertyData.SelectAll().ToList();
        //    List<ExamMode> LstExamModes = repoExamMode.SelectAll();
        //    List<Original_UmbracoNode> LstAllUpdatedRecords = new List<Original_UmbracoNode>();
        //    List<Original_MemberData> LstOriginalMemberData = repoOriginalMemberData.SelectAll();


        //    //Obtain all existing members in umbraco
        //    List<IMember> lstExistinMembers = Services.MemberService.GetAllMembers().ToList();


        //    ////TEMP!!!  
        //    //List<IMember> lstExistinMembers = new List<IMember>();
        //    //lstExistinMembers.Add(Services.MemberService.GetByEmail("jim.fifth@5thstudios.com"));


        //    //Get all paid exam names/IDs
        //    IPublishedContent ipExamFolder = Umbraco.Content((int)(Models.Common.SiteNode.Exams));
        //    foreach (IPublishedContent ipExam in ipExamFolder.DescendantsOfType(Models.Common.DocType.ExamPaid))
        //        DictExams.Add(ipExam.Id, ipExam.Name);


        //    //Split time to see where slowness is occuring
        //    sw.Stop();
        //    time1_3 = sw.Elapsed;
        //    sw.Start();




        //    var onlyDo = 0;
        //    foreach (IMember _member in lstExistinMembers)
        //    {


        //        //Refresh all needed contexts
        //        _context = new EF_SwtpDb(); //Reset context and repo to improve performance.
        //        _context.Database.CommandTimeout = 172800; //2 days
        //        repoExamRecords = new ExamRecordRepository(_context);
        //        repoExamAnswerSet = new ExamAnswerSetRepository(_context);
        //        repoExamAnswer = new ExamAnswerRepository(_context);

        //        try
        //        {
        //            if (LstOriginalMemberData.Any(x => x.Email == _member.Email))
        //            {
        //                onlyDo++;

        //                //Get original member id
        //                int oldMemberId = LstOriginalMemberData.FirstOrDefault(x => x.Email == _member.Email).MemberId;


        //                //Get list of original exam folder records for user (text contains both user email AND old member id.)
        //                List<Original_UmbracoNode> lstLvl4Nodes_filtered = LstAllOrigUmbNodes_lvl4.Where(x => x.text.Contains(_member.Email) && x.text.Contains(oldMemberId.ToString())).ToList();


        //                foreach (Original_UmbracoNode lvl4Record in lstLvl4Nodes_filtered)
        //                {
        //                    counter++;
        //                    LstAllUpdatedRecords.Add(lvl4Record); //Add to list for updating as added to site

        //                    //Get list of original exam records for lvl5 related to lvl4 record
        //                    List<Original_UmbracoNode> lstLvl5Nodes_filtered = LstAllOrigUmbNodes_lvl5.Where(x => x.path.Contains(lvl4Record.path + ",")).OrderBy(x => x.createDate).ToList();

        //                    foreach (Original_UmbracoNode lvl5Record in lstLvl5Nodes_filtered)
        //                    {
        //                        counter++;
        //                        LstAllUpdatedRecords.Add(lvl5Record); //Add to list for updating as added to site

        //                        //Does exam name exist in list of paid exams. (eliminates any old named exams or free ones.)
        //                        if (DictExams.ContainsValue(lvl5Record.text.Split('-').First().Trim()))
        //                        {
        //                            //Pull all data from property repo and validate
        //                            string _tempExamMode = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 116).dataNvarchar;
        //                            string _tempSubscriptionId = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 115).dataNvarchar;
        //                            string _tempExamId = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 87).dataNvarchar;
        //                            string _tempSubmitted = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 89).dataNvarchar;
        //                            bool submitted;
        //                            var result = Boolean.TryParse(_tempSubmitted, out submitted);
        //                            string _tempSubmittedDate = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 172).dataNvarchar;
        //                            DateTime submittedDate;
        //                            bool submittedDateValid = DateTime.TryParse(_tempSubmittedDate, out submittedDate);
        //                            string _tempTimeRemaining = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 92).dataNvarchar;
        //                            TimeSpan _timespan;
        //                            bool _tempTimeRemainingIsValid = TimeSpan.TryParse(_tempTimeRemaining, out _timespan);



        //                            //Create new exam record and add to db
        //                            ExamRecord _examRecord = new ExamRecord();
        //                            _examRecord.ExamModeId = LstExamModes.FirstOrDefault(x => x.Mode == _tempExamMode).ExamModeId;
        //                            _examRecord.MemberId = _member.Id;
        //                            if (!string.IsNullOrEmpty(_tempSubscriptionId)) _examRecord.SubscriptionId = Convert.ToInt32(_tempSubscriptionId);
        //                            _examRecord.ExamId = (int)LstExamIDs.FirstOrDefault(x => x.ExamId_old == Convert.ToInt32(_tempExamId)).ExamId_new;
        //                            _examRecord.CreatedDate = lvl5Record.createDate;
        //                            _examRecord.Submitted = submitted;
        //                            if (submittedDateValid) _examRecord.SubmittedDate = submittedDate;
        //                            if (_tempTimeRemainingIsValid) _examRecord.TimeRemaining = _timespan;
        //                            repoExamRecords.AddRecord(_examRecord);


        //                            //Get list of original exam records for lvl6
        //                            Original_UmbracoNode lvl6Node_filtered = LstAllOrigUmbNodes_lvl6.FirstOrDefault(x => x.parentID == lvl5Record.id);
        //                            LstAllUpdatedRecords.Add(lvl6Node_filtered); //Add to list for updating as added to site
        //                            counter++;

        //                            //Get answerset list as csv and convert to new id list
        //                            Original_CmsPropertyData lvl6CmsPropertyData = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl6Node_filtered.id && x.propertytypeid == 169);
        //                            HashSet<int> lstOldAnswerSet = lvl6CmsPropertyData.dataNtext.Split(',').Select(i => Int32.Parse(i)).ToHashSet();
        //                            HashSet<int> lstNewAnswerSet = new HashSet<int>();
        //                            foreach (int _id in lstOldAnswerSet)
        //                            {
        //                                lstNewAnswerSet.Add((int)LstExamIDs.FirstOrDefault(x => x.QuestionId_old == _id).QuestionId_new);
        //                            }

        //                            //Create new exam answerset
        //                            ExamAnswerSet _examAnswerSet = new ExamAnswerSet();
        //                            _examAnswerSet.ExamRecordId = _examRecord.ExamRecordId;
        //                            _examAnswerSet.AnswerSet = String.Join(",", lstNewAnswerSet.Select(x => x.ToString()).ToArray());
        //                            repoExamAnswerSet.AddRecord(_examAnswerSet);


        //                            //Get all property datas per question and consolidate into list of answers
        //                            List<AnswerRecord_former> lstAnswerRecords = new List<AnswerRecord_former>();
        //                            List<Original_UmbracoNode> lvl7Node_filtered = LstAllOrigUmbNodes_lvl7.Where(x => x.parentID == lvl6Node_filtered.id).ToList();
        //                            foreach (Original_UmbracoNode lvl7Record in lvl7Node_filtered)
        //                            {
        //                                counter++;
        //                                LstAllUpdatedRecords.Add(lvl7Record); //Add to list for updating as added to site

        //                                AnswerRecord_former _answerRecord = new AnswerRecord_former();
        //                                List<Original_CmsPropertyData> lstPropertyData = LstCmsProperties.Where(x => x.contentNodeId == lvl7Record.id).ToList();
        //                                foreach (Original_CmsPropertyData _property in lstPropertyData)
        //                                {
        //                                    counter++;
        //                                    switch (_property.propertytypeid)
        //                                    {
        //                                        case 109: //questionId
        //                                            _answerRecord.oldQuestionId = _property.dataNvarchar;
        //                                            break;
        //                                        case 110: //answerId
        //                                            _answerRecord.answerId = _property.dataNvarchar;
        //                                            break;
        //                                        case 111: //correct
        //                                            _answerRecord.correct = _property.dataNvarchar;
        //                                            break;
        //                                        case 112: //review
        //                                            _answerRecord.review = _property.dataNvarchar;
        //                                            break;
        //                                        case 113: //answersRendered
        //                                            _answerRecord.answersRendered = _property.dataNvarchar;
        //                                            break;
        //                                        case 114: //contentArea
        //                                            _answerRecord.oldContentArea = _property.dataNvarchar;
        //                                            break;
        //                                        default: break;
        //                                    }
        //                                }

        //                                //Convert old/new IDs
        //                                ExamIDs_Old_New examIDs = LstExamIDs.FirstOrDefault(x => x.QuestionId_old == Convert.ToInt32(_answerRecord.oldQuestionId));
        //                                _answerRecord.newQuestionId = (int)examIDs.QuestionId_new;
        //                                _answerRecord.newContentArea = (int)examIDs.ContentId_new;

        //                                lstAnswerRecords.Add(_answerRecord);
        //                            }


        //                            //Create all answer records
        //                            int index = 0;
        //                            foreach (AnswerRecord_former _answerRecord in lstAnswerRecords)
        //                            {
        //                                bool tempBool = false;

        //                                ExamAnswer _examAnswer = new ExamAnswer();
        //                                _examAnswer.ExamAnswerSetId = _examAnswerSet.ExamAnswerSetId;
        //                                _examAnswer.ContentAreaId = _answerRecord.newContentArea;
        //                                _examAnswer.QuestionId = _answerRecord.newQuestionId;
        //                                _examAnswer.QuestionRenderOrder = index;
        //                                _examAnswer.AnswerRenderedOrder = _answerRecord.answersRendered;
        //                                if (!string.IsNullOrEmpty(_answerRecord.answerId)) _examAnswer.SelectedAnswer = Convert.ToInt32(_answerRecord.answerId);
        //                                _examAnswer.CorrectAnswer = GetCorrectAnswerId(_answerRecord.newQuestionId);
        //                                if (bool.TryParse(_answerRecord.correct, out tempBool)) _examAnswer.IsCorrect = tempBool;
        //                                if (bool.TryParse(_answerRecord.review, out tempBool)) _examAnswer.ReviewQuestion = tempBool;
        //                                repoExamAnswer.AddRecord(_examAnswer);
        //                                index++;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                error++;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger.Error<ApiMigrateDataController>(ex);
        //            error++;
        //            //break; //===============================================================================================================================================
        //        }


        //        if (onlyDo >= 10) break;
        //    }



        //    //Split time to see where slowness is occuring
        //    sw.Stop();
        //    timeComplete = sw.Elapsed;
        //    time2_3 = timeComplete - time1_3;
        //    sw.Start();



        //    //Update all records as being added to site
        //    for (int i = 0; i < LstAllUpdatedRecords.Count(); i++)
        //    {
        //        LstAllUpdatedRecords[i].isAddedToSite = true;
        //    }

        //    try
        //    {
        //        _context = new EF_SwtpDb(); //Reset context and repo to improve performance.
        //        _context.Database.CommandTimeout = 172800; //2 days
        //        repoOriginalUmbracoNode = new OriginalUmbracoNodeRepository(_context);
        //        repoOriginalUmbracoNode.BulkUpdateRecord(LstAllUpdatedRecords);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error<ApiMigrateDataController>(ex);
        //    }







        //    //Split time to see where slowness is occuring
        //    sw.Stop();
        //    timeComplete = sw.Elapsed;
        //    time3_3 = timeComplete - time2_3 - time1_3;
        //    sw.Start();


        //    //estimate total time required IF testing with index.
        //    TimeSpan estimatedTime = timeComplete;
        //    if (onlyDo > 0)
        //    {
        //        estimatedTime = TimeSpan.FromTicks((time2_3.Ticks + time3_3.Ticks) * (lstExistinMembers.Count() / onlyDo)) + time1_3;
        //    }



        //    return "Converted records: " + counter.ToString()
        //        + " |  Errors: " + error.ToString()
        //        + " |  1/3: " + time1_3.ToString(@"hh\:mm\:ss")
        //        + " |  2/3: " + time2_3.ToString(@"hh\:mm\:ss")
        //        + " |  2/3: " + time3_3.ToString(@"hh\:mm\:ss")
        //        + " |  Timespan: " + timeComplete.ToString(@"hh\:mm\:ss")
        //        + " |  Estimated Time: " + estimatedTime.ToString(@"hh\:mm\:ss");

        //}





        /*  Post-Launch Fixes   */
        /*=============================================================*/
        public string MoveDeletedAcctPurchasesToLive()
        {
            Stopwatch sw = Stopwatch.StartNew();
            TimeSpan time1_3;
            TimeSpan time2_3;
            TimeSpan time3_3;
            TimeSpan timeComplete;
            sw.Start();


            //Instantiate repositories
            EF_SwtpDb _context = new EF_SwtpDb();
            _context.Database.CommandTimeout = 172800; //2 days
            repoCmsMembers = new CmsMemberRepository(_context);
            repoOriginalPurchaseRecords = new OriginalPurchaseRecordsRepository(_context);
            repoPurchaseRecord = new PurchaseRecordRepository(_context);
            repoPurchaseRecordItems = new PurchaseRecordItemRepository(_context);
            repoExamRecords = new ExamRecordRepository(_context);


            //Obtain all data
            List<cmsMember> lstAllMembers = repoCmsMembers.SelectAll();
            List<Original_PurchaseRecords> lstAllOriginalPurchaseRecords = repoOriginalPurchaseRecords.GetAll();
            List<EF.PurchaseRecord> lstAllPurchaseRecords = repoPurchaseRecord.SelectAll();
            List<PurchaseRecordItem> lstAllPurchaseRecordItems = repoPurchaseRecordItems.SelectAll();
            List<ExamRecord> lstExamRecords = repoExamRecords.GetAll().ToList();


            //Instantiate variables
            string delete = "DELETE_";
            List<Original_PurchaseRecords> LstUpdateOriginalPurchaseRecords = new List<Original_PurchaseRecords>();
            List<EF.PurchaseRecord> LstUpdatePurchaseRecords = new List<EF.PurchaseRecord>();
            List<PurchaseRecordItem> LstUpdatePurchaseRecordItems = new List<PurchaseRecordItem>();
            List<ExamRecord> LstUpdateExamRecords = new List<ExamRecord>();





            //Split time to see where slowness is occuring
            sw.Stop();
            time1_3 = sw.Elapsed;
            sw.Restart();



            //Loop through deleted members
            foreach (cmsMember _deletedMember in lstAllMembers.Where(x => x.LoginName.Contains(delete) && x.LoginName.Contains(delete)))
            {
                if (lstAllMembers.Any(x => x.LoginName.Contains(_deletedMember.LoginName.Replace(delete, ""))))
                {
                    //Get live member
                    cmsMember _liveMember = lstAllMembers.FirstOrDefault(x => x.LoginName == _deletedMember.LoginName.Replace(delete, ""));

                    //Obtain all original purchase records to be updated
                    foreach (Original_PurchaseRecords _originalPurchaseRecord in lstAllOriginalPurchaseRecords.Where(x => x.memberId == _deletedMember.nodeId))
                    {
                        //Replace member id and add to list for updating
                        _originalPurchaseRecord.memberId = _liveMember.nodeId;
                        LstUpdateOriginalPurchaseRecords.Add(_originalPurchaseRecord);
                    }

                    //Obtain all livepurchase records to be updated
                    foreach (EF.PurchaseRecord _purchaseRecord in lstAllPurchaseRecords.Where(x => x.MemberId == _deletedMember.nodeId))
                    {
                        //Replace member id and add to list for updating
                        _purchaseRecord.MemberId = _liveMember.nodeId;
                        LstUpdatePurchaseRecords.Add(_purchaseRecord);
                    }

                    //Obtain all livepurchase records to be updated
                    foreach (PurchaseRecordItem _purchaseRecordItem in lstAllPurchaseRecordItems.Where(x => x.MemberId == _deletedMember.nodeId))
                    {
                        //Replace member id and add to list for updating
                        _purchaseRecordItem.MemberId = _liveMember.nodeId;
                        LstUpdatePurchaseRecordItems.Add(_purchaseRecordItem);
                    }

                    //Obtain all livepurchase records to be updated
                    foreach (ExamRecord _examRecord in lstExamRecords.Where(x => x.MemberId == _deletedMember.nodeId))
                    {
                        //Replace member id and add to list for updating
                        _examRecord.MemberId = _liveMember.nodeId;
                        LstUpdateExamRecords.Add(_examRecord);
                    }
                }
            }




            //Split time to see where slowness is occuring
            sw.Stop();
            time2_3 = sw.Elapsed;
            sw.Restart();


            //Update all records
            repoOriginalPurchaseRecords.BulkUpdateRecord(LstUpdateOriginalPurchaseRecords);
            repoPurchaseRecord.BulkUpdateRecord(LstUpdatePurchaseRecords);
            repoPurchaseRecordItems.BulkUpdateRecord(LstUpdatePurchaseRecordItems);
            repoExamRecords.BulkUpdateRecord(LstUpdateExamRecords);



            sw.Stop();
            time3_3 = sw.Elapsed;
            timeComplete = time1_3 + time2_3 + time3_3;


            //Create return string
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("LstUpdateOriginalPurchaseRecords: " + LstUpdateOriginalPurchaseRecords.Count());
            sb.AppendLine("LstUpdatePurchaseRecords: " + LstUpdatePurchaseRecords.Count());
            sb.AppendLine("LstUpdatePurchaseRecordItems: " + LstUpdatePurchaseRecordItems.Count());
            sb.AppendLine("LstUpdateExamRecords: " + LstUpdateExamRecords.Count());
            sb.AppendLine("1/3: " + time1_3.ToString(@"hh\:mm\:ss"));
            sb.AppendLine("2/3: " + time2_3.ToString(@"hh\:mm\:ss"));
            sb.AppendLine("3/3: " + time3_3.ToString(@"hh\:mm\:ss"));
            sb.AppendLine("Complete: " + timeComplete.ToString(@"hh\:mm\:ss"));
            return sb.ToString();
        }
        public string ConvertImportedExamResults()
        {
            Stopwatch sw = Stopwatch.StartNew();
            //TimeSpan timeTaken;
            TimeSpan time1_3;
            TimeSpan time2_3;
            TimeSpan time3_3;
            TimeSpan timeComplete;
            sw.Start();



            List<object> lstObjects = new List<object>();
            int counter = 0;
            int error = 0;
            //TEST USER
            //adivinestrategy@gmail.com - 15524391
            //user:		adivinestrategy@gmail.com
            //member id:	23843		
            //ExamID		7670554	[+]
            //exam folder id:		15524392	Exam Member
            //						15525751 [+] Exam Score
            //							15525752	Answer Folder
            //								15525763	Answer



            //Instantiate repositories
            EF_SwtpDb _context = new EF_SwtpDb(); //Reset context and repo to improve performance.
            _context.Database.CommandTimeout = 172800; //2 days
            repoOriginalCmsPropertyData = new OriginalCmsPropertyDataRepository(_context);
            repoExamIdRelationships = new ExamIdRelationshipRepository(_context);
            repoOriginalUmbracoNode = new OriginalUmbracoNodeRepository(_context);
            repoOriginalMemberData = new OriginalMemberDataRepository(_context);
            repoExamRecords = new ExamRecordRepository(_context);
            repoExamAnswerSet = new ExamAnswerSetRepository(_context);
            repoExamAnswer = new ExamAnswerRepository(_context);
            repoExamMode = new ExamModeRepository(_context);


            //Instantiate variables
            Dictionary<int, string> DictExams = new Dictionary<int, string>();
            List<ExamIDs_Old_New> LstExamIDs = repoExamIdRelationships.SelectAll_ExceptText(); //Get all exam IDs from relationship table
            List<Original_UmbracoNode> LstAllOrigUmbNodes_lvl4 = repoOriginalUmbracoNode.SelectAll_Lvl4().ToList();
            List<Original_UmbracoNode> LstAllOrigUmbNodes_lvl5 = repoOriginalUmbracoNode.SelectAll_Lvl5().ToList();
            List<Original_UmbracoNode> LstAllOrigUmbNodes_lvl6 = repoOriginalUmbracoNode.SelectAll_Lvl6().ToList();
            List<Original_UmbracoNode> LstAllOrigUmbNodes_lvl7 = repoOriginalUmbracoNode.SelectAll_Lvl7().ToList();
            List<Original_CmsPropertyData> LstCmsProperties = repoOriginalCmsPropertyData.SelectAll().ToList();
            List<ExamMode> LstExamModes = repoExamMode.SelectAll();
            List<Original_UmbracoNode> LstAllUpdatedRecords = new List<Original_UmbracoNode>();
            List<Original_MemberData> LstOriginalMemberData = repoOriginalMemberData.SelectAll();



            //Obtain all existing members in umbraco
            //List<IMember> lstExistinMembers = Services.MemberService.GetAllMembers().ToList();



            //TEMP!!!  
            List<IMember> lstExistinMembers = new List<IMember>();
            //lstExistinMembers.Add(Services.MemberService.GetByEmail("ragillespie.1996@gmail.com"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("drakefire.rg@gmail.com"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("Lindsey.thorp@firstinclay.org"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("linnynewton13@gmail.com"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("colleen.mishra@gmail.com"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("kellercm@gmail.com"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("Bjonescm23@gmail.com"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("bri_jones23@yahoo.com"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("Clinical.Soc.Worker@gmail.com"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("MJZPublic@comcast.net"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("rae_abern0812@yahoo.com"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("selmon.lyser2020@gmail.com"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("selmon.lyser88@yahoo.com"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("lbrockman@msbranch.org"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("armed4pony@yahoo.com"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("toyinidowu140@gmail.com"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("toyinadetunji@aol.com"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("raemel30@gmail.com"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("kimberly.mikkonen@waldenu.edu"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("adamsloretta8@gmail.com"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("janelleraenaek@gmail.com"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("franklinaisha1@gmail.com"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("meganmccabe89@gmail.com"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("oghinp32@bellsouth.net"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("DAYNAMICHELLE00@HOTMAIL.COM"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("nyeeshaali@gmail.com"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("meganmccabe89@gmail.com"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("abraham.sanu@gmail.com"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("jelleah.sidney@yahoo.com"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("b.tooson@caregivergrove.com"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("mistertooson525@gmail.com"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("karitomu28@gmail.com"));
            lstExistinMembers.Add(Services.MemberService.GetByEmail("karitomu28@aol.com"));




            //Get all paid exam names/IDs
            IPublishedContent ipExamFolder = Umbraco.Content((int)(Models.Common.SiteNode.Exams));
            foreach (IPublishedContent ipExam in ipExamFolder.DescendantsOfType(Models.Common.DocType.ExamPaid))
                DictExams.Add(ipExam.Id, ipExam.Name);


            //Split time to see where slowness is occuring
            sw.Stop();
            time1_3 = sw.Elapsed;
            sw.Restart();




            var onlyDo = 0;
            foreach (IMember _member in lstExistinMembers)
            {


                //Refresh all needed contexts
                _context = new EF_SwtpDb(); //Reset context and repo to improve performance.
                _context.Database.CommandTimeout = 172800; //2 days
                repoExamRecords = new ExamRecordRepository(_context);
                repoExamAnswerSet = new ExamAnswerSetRepository(_context);
                repoExamAnswer = new ExamAnswerRepository(_context);

                try
                {
                    if (LstOriginalMemberData.Any(x => x.Email == _member.Email))
                    {
                        //Get original member id
                        int oldMemberId = LstOriginalMemberData.FirstOrDefault(x => x.Email == _member.Email).MemberId;


                        //Get list of original exam folder records for user (text contains both user email AND old member id.)
                        List<Original_UmbracoNode> lstLvl4Nodes_filtered = LstAllOrigUmbNodes_lvl4.Where(x => x.text.ToLower().Contains(_member.Email.ToLower()) && x.text.Contains(oldMemberId.ToString())).ToList();

                        if (lstLvl4Nodes_filtered.Count > 0) { onlyDo++; }

                        foreach (Original_UmbracoNode lvl4Record in lstLvl4Nodes_filtered)
                        {
                            counter++;
                            LstAllUpdatedRecords.Add(lvl4Record); //Add to list for updating as added to site

                            //Get list of original exam records for lvl5 related to lvl4 record
                            List<Original_UmbracoNode> lstLvl5Nodes_filtered = LstAllOrigUmbNodes_lvl5.Where(x => x.path.Contains(lvl4Record.path + ",")).OrderBy(x => x.createDate).ToList();

                            foreach (Original_UmbracoNode lvl5Record in lstLvl5Nodes_filtered)
                            {
                                counter++;
                                LstAllUpdatedRecords.Add(lvl5Record); //Add to list for updating as added to site

                                //Does exam name exist in list of paid exams. (eliminates any old named exams or free ones.)
                                if (DictExams.ContainsValue(lvl5Record.text.Split('-').First().Trim()))
                                {
                                    //Pull all data from property repo and validate
                                    string _tempExamMode = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 116).dataNvarchar;
                                    string _tempSubscriptionId = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 115).dataNvarchar;
                                    string _tempExamId = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 87).dataNvarchar;
                                    string _tempSubmitted = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 89).dataNvarchar;
                                    bool submitted;
                                    var result = Boolean.TryParse(_tempSubmitted, out submitted);
                                    string _tempSubmittedDate = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 172).dataNvarchar;
                                    DateTime submittedDate;
                                    bool submittedDateValid = DateTime.TryParse(_tempSubmittedDate, out submittedDate);
                                    string _tempTimeRemaining = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl5Record.id && x.propertytypeid == 92).dataNvarchar;
                                    TimeSpan _timespan;
                                    bool _tempTimeRemainingIsValid = TimeSpan.TryParse(_tempTimeRemaining, out _timespan);



                                    //Create new exam record and add to db
                                    ExamRecord _examRecord = new ExamRecord();
                                    _examRecord.ExamModeId = LstExamModes.FirstOrDefault(x => x.Mode == _tempExamMode).ExamModeId;
                                    _examRecord.MemberId = _member.Id;
                                    if (!string.IsNullOrEmpty(_tempSubscriptionId)) _examRecord.SubscriptionId = Convert.ToInt32(_tempSubscriptionId);
                                    _examRecord.ExamId = (int)LstExamIDs.FirstOrDefault(x => x.ExamId_old == Convert.ToInt32(_tempExamId)).ExamId_new;
                                    _examRecord.CreatedDate = lvl5Record.createDate;
                                    _examRecord.Submitted = submitted;
                                    if (submittedDateValid) _examRecord.SubmittedDate = submittedDate;
                                    if (_tempTimeRemainingIsValid) _examRecord.TimeRemaining = _timespan;
                                    repoExamRecords.AddRecord(_examRecord);


                                    //Get list of original exam records for lvl6
                                    Original_UmbracoNode lvl6Node_filtered = LstAllOrigUmbNodes_lvl6.FirstOrDefault(x => x.parentID == lvl5Record.id);
                                    LstAllUpdatedRecords.Add(lvl6Node_filtered); //Add to list for updating as added to site
                                    counter++;

                                    //Get answerset list as csv and convert to new id list
                                    Original_CmsPropertyData lvl6CmsPropertyData = LstCmsProperties.FirstOrDefault(x => x.contentNodeId == lvl6Node_filtered.id && x.propertytypeid == 169);
                                    HashSet<int> lstOldAnswerSet = lvl6CmsPropertyData.dataNtext.Split(',').Select(i => Int32.Parse(i)).ToHashSet();
                                    HashSet<int> lstNewAnswerSet = new HashSet<int>();
                                    foreach (int _id in lstOldAnswerSet)
                                    {
                                        lstNewAnswerSet.Add((int)LstExamIDs.FirstOrDefault(x => x.QuestionId_old == _id).QuestionId_new);
                                    }

                                    //Create new exam answerset
                                    ExamAnswerSet _examAnswerSet = new ExamAnswerSet();
                                    _examAnswerSet.ExamRecordId = _examRecord.ExamRecordId;
                                    _examAnswerSet.AnswerSet = String.Join(",", lstNewAnswerSet.Select(x => x.ToString()).ToArray());
                                    repoExamAnswerSet.AddRecord(_examAnswerSet);


                                    //Get all property datas per question and consolidate into list of answers
                                    List<AnswerRecord_former> lstAnswerRecords = new List<AnswerRecord_former>();
                                    List<Original_UmbracoNode> lvl7Node_filtered = LstAllOrigUmbNodes_lvl7.Where(x => x.parentID == lvl6Node_filtered.id).ToList();
                                    foreach (Original_UmbracoNode lvl7Record in lvl7Node_filtered)
                                    {
                                        counter++;
                                        LstAllUpdatedRecords.Add(lvl7Record); //Add to list for updating as added to site

                                        AnswerRecord_former _answerRecord = new AnswerRecord_former();
                                        List<Original_CmsPropertyData> lstPropertyData = LstCmsProperties.Where(x => x.contentNodeId == lvl7Record.id).ToList();
                                        foreach (Original_CmsPropertyData _property in lstPropertyData)
                                        {
                                            counter++;
                                            switch (_property.propertytypeid)
                                            {
                                                case 109: //questionId
                                                    _answerRecord.oldQuestionId = _property.dataNvarchar;
                                                    break;
                                                case 110: //answerId
                                                    _answerRecord.answerId = _property.dataNvarchar;
                                                    break;
                                                case 111: //correct
                                                    _answerRecord.correct = _property.dataNvarchar;
                                                    break;
                                                case 112: //review
                                                    _answerRecord.review = _property.dataNvarchar;
                                                    break;
                                                case 113: //answersRendered
                                                    _answerRecord.answersRendered = _property.dataNvarchar;
                                                    break;
                                                case 114: //contentArea
                                                    _answerRecord.oldContentArea = _property.dataNvarchar;
                                                    break;
                                                default: break;
                                            }
                                        }

                                        //Convert old/new IDs
                                        ExamIDs_Old_New examIDs = LstExamIDs.FirstOrDefault(x => x.QuestionId_old == Convert.ToInt32(_answerRecord.oldQuestionId));
                                        _answerRecord.newQuestionId = (int)examIDs.QuestionId_new;
                                        _answerRecord.newContentArea = (int)examIDs.ContentId_new;

                                        lstAnswerRecords.Add(_answerRecord);
                                    }


                                    //Create all answer records
                                    int index = 0;
                                    foreach (AnswerRecord_former _answerRecord in lstAnswerRecords)
                                    {
                                        bool tempBool = false;

                                        ExamAnswer _examAnswer = new ExamAnswer();
                                        _examAnswer.ExamAnswerSetId = _examAnswerSet.ExamAnswerSetId;
                                        _examAnswer.ContentAreaId = _answerRecord.newContentArea;
                                        _examAnswer.QuestionId = _answerRecord.newQuestionId;
                                        _examAnswer.QuestionRenderOrder = index;
                                        _examAnswer.AnswerRenderedOrder = _answerRecord.answersRendered;
                                        if (!string.IsNullOrEmpty(_answerRecord.answerId)) _examAnswer.SelectedAnswer = Convert.ToInt32(_answerRecord.answerId);
                                        _examAnswer.CorrectAnswer = GetCorrectAnswerId(_answerRecord.newQuestionId);
                                        if (bool.TryParse(_answerRecord.correct, out tempBool)) _examAnswer.IsCorrect = tempBool;
                                        if (bool.TryParse(_answerRecord.review, out tempBool)) _examAnswer.ReviewQuestion = tempBool;
                                        repoExamAnswer.AddRecord(_examAnswer);
                                        index++;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //Logger.Warn<ApiMigrateDataController>(Newtonsoft.Json.JsonConvert.SerializeObject(_member));
                        error++;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error<ApiMigrateDataController>(ex);
                    Logger.Warn<ApiMigrateDataController>(Newtonsoft.Json.JsonConvert.SerializeObject(_member));
                    error++;
                    //break; //===============================================================================================================================================
                }


                //if (onlyDo >= 10) break;
            }



            //Split time to see where slowness is occuring
            sw.Stop();
            time2_3 = sw.Elapsed;
            sw.Restart();



            //Update all records as being added to site
            for (int i = 0; i < LstAllUpdatedRecords.Count(); i++)
            {
                LstAllUpdatedRecords[i].isAddedToSite = true;
            }

            try
            {
                _context = new EF_SwtpDb(); //Reset context and repo to improve performance.
                _context.Database.CommandTimeout = 172800; //2 days
                repoOriginalUmbracoNode = new OriginalUmbracoNodeRepository(_context);
                repoOriginalUmbracoNode.BulkUpdateRecord(LstAllUpdatedRecords);
            }
            catch (Exception ex)
            {
                Logger.Error<ApiMigrateDataController>(ex);
            }







            //Split time to see where slowness is occuring
            sw.Stop();
            time3_3 = sw.Elapsed;
            timeComplete = time3_3 + time2_3 + time1_3;


            //estimate total time required IF testing with index.
            TimeSpan estimatedTime = timeComplete;
            if (onlyDo > 0)
            {
                estimatedTime = TimeSpan.FromTicks((time2_3.Ticks + time3_3.Ticks) * (lstExistinMembers.Count() / onlyDo)) + time1_3;
            }





            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Converted records: " + counter.ToString());
            sb.AppendLine("Errors: " + error.ToString());
            sb.AppendLine("Pull data from db: " + time1_3.ToString(@"hh\:mm\:ss"));
            sb.AppendLine("Create new records: " + time2_3.ToString(@"hh\:mm\:ss"));
            sb.AppendLine("Mark records as added: " + time3_3.ToString(@"hh\:mm\:ss"));
            sb.AppendLine("Complete: " + timeComplete.ToString(@"hh\:mm\:ss"));
            sb.AppendLine("Estimated Time: " + estimatedTime.ToString(@"hh\:mm\:ss"));
            return sb.ToString();
        }
        public string AddMissingExamAnswers()
        {
            Stopwatch sw = Stopwatch.StartNew();
            TimeSpan time1_3;
            TimeSpan time2_3;
            TimeSpan time3_3;
            TimeSpan timeComplete;
            sw.Start();


            //Instantiate repositories
            EF_SwtpDb _context = new EF_SwtpDb();
            _context.Database.CommandTimeout = 172800; //2 days
            //repoExamRecords = new ExamRecordRepository(_context);
            repoExamAnswerSet = new ExamAnswerSetRepository(_context);
            repoExamAnswer = new ExamAnswerRepository(_context);


            //Instantiate variables and obtain needed data
            //List<KeyValuePair> LstExamKeys = repoExamRecords.GetAll_Id_ExamId();
            List<ExamAnswerSet> LstAnswerSets = repoExamAnswerSet.GetAll().ToList();
            List<KeyValuePair> LstAnswerKeys = repoExamAnswer.GetAll_ExamAnswerSetId_QuestionId();  // Key=ExamAnswerSetId  Value=QuestionId
            List<ExamAnswer> LstExamAnswersToAdd = new List<ExamAnswer>();
            Random rnd = new Random();
            int _totalAdded = 0;


            //Split time to see where slowness is occuring
            sw.Stop();
            time1_3 = sw.Elapsed;
            sw.Start();


            //
            foreach (ExamAnswerSet _answerSet in LstAnswerSets)
            {
                //Get counts
                int setCount = _answerSet.AnswerSet.Count(x => x == ',') + 1;
                int answerCount = LstAnswerKeys.Where(x => x.IntKey == _answerSet.ExamAnswerSetId && x.IntValue == setCount).Count(); // Key=ExamAnswerSetId  Value=QuestionId

                if (setCount != answerCount)
                {
                    //Split AnswerSet list
                    HashSet<string> lstQuestionIds = (_answerSet.AnswerSet.Split(',')).ToHashSet();

                    //Get examId
                    //int _examId = (int)LstExamKeys.FirstOrDefault(x => x.IntKey.Equals(_answerSet.ExamRecordId)).IntValue;


                    //Create missing ExamAnswer records
                    int renderOrder = 0;
                    foreach (string _id in lstQuestionIds)
                    {
                        if (LstAnswerKeys.Any(x =>  // Key=ExamAnswerSetId  Value=QuestionId
                        x.IntKey == _answerSet.ExamAnswerSetId &&
                        x.IntValue == Convert.ToInt32(_id)))
                        {
                            //Record exists.  skip
                            renderOrder++;
                            continue;
                        }
                        //Get question as model
                        cm.Question cmQuestion = new cm.Question(Umbraco.Content(_id));

                        //Get question data
                        ExamAnswer examAnswer = new ExamAnswer();
                        examAnswer.ExamAnswerSetId = _answerSet.ExamAnswerSetId;
                        examAnswer.ContentAreaId = cmQuestion.Parent.Id;
                        examAnswer.QuestionId = cmQuestion.Id;

                        //
                        examAnswer.QuestionRenderOrder = renderOrder;

                        //Create random order for answer render order.
                        HashSet<int> lstAnswerSetIDs = new HashSet<int>();
                        for (int i = 1; i <= cmQuestion.AnswerSets.Count(); i++) { lstAnswerSetIDs.Add(i); }
                        lstAnswerSetIDs = lstAnswerSetIDs.OrderBy(a => rnd.Next()).ToHashSet();
                        examAnswer.AnswerRenderedOrder = String.Join(",", lstAnswerSetIDs.Select(x => x.ToString()).ToArray());

                        //Get index of correct answer.
                        int index = 1;
                        foreach (var _answer in cmQuestion.AnswerSets)
                        {
                            if (_answer.IsCorrectAnswer)
                            {
                                examAnswer.CorrectAnswer = index;
                                break;
                            }
                            index++;
                        }


                        //Add ExamAnswer to list
                        LstExamAnswersToAdd.Add(examAnswer);


                        _totalAdded++;
                        renderOrder++;
                    }
                }
            }




            //Split time to see where slowness is occuring
            sw.Stop();
            time2_3 = sw.Elapsed - time1_3;
            sw.Start();


            //Add all missing records
            repoExamAnswer.BulkAddRecord(LstExamAnswersToAdd);



            sw.Stop();
            time3_3 = sw.Elapsed - time2_3 - time1_3;
            timeComplete = sw.Elapsed;



            return "Total records added: " + _totalAdded.ToString()
                + " |  1/3: " + time1_3.ToString(@"hh\:mm\:ss")
                + " |  2/3: " + time2_3.ToString(@"hh\:mm\:ss")
                + " |  3/3: " + time2_3.ToString(@"hh\:mm\:ss")
                + " |  Complete: " + timeComplete.ToString(@"hh\:mm\:ss");
        }
        public string PageNotFoundRedirects()
        {
            //Start watch
            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();


            //Instantiate variables.
            PageNotFoundRepository repo = new PageNotFoundRepository(new EF_SwtpDb());
            List<SEOChecker_PageNotFound> LstPagesNotFound = repo.GetAll();
            List<blogRecord> LstBlogRecords = new List<blogRecord>();
            int updated = 0;


            //Get all blog records
            foreach (IPublishedContent ip in Umbraco.Content((int)(bl.Models.Common.SiteNode.Blog)).DescendantsOfType(bl.Models.Common.DocType.Post))
            {
                LstBlogRecords.Add(new blogRecord()
                {
                    Title = ip.Url().TrimEnd('/').Split('/').LastOrDefault(),
                    Url = ip.Url()
                });
            }


            //Generate list of days, months & years
            List<string> LstDays = new List<string>();
            for (int i = 1; i <= 31; i++)
            {
                //LstDays.Add( i.ToString("00") );
                LstDays.Add("/" + i.ToString("00") + "/");
            }
            List<string> LstMonths = new List<string>()
            {
                "january",
                "february",
                "march",
                "april",
                "may",
                "june",
                "july",
                "august",
                "september",
                "october",
                "november",
                "december",
            };
            List<string> LstYears = new List<string>();
            for (int i = 2009; i <= 2025; i++)
            {
                //LstYears.Add( i.ToString() );
                LstYears.Add("/" + i.ToString() + "/");
            }

            //  blog/2009/december/15/new-social-work-podcast-psychoanalytic-treatment-in-contemporary-social-work-practice
            //  blog/2009/march/04/68
            //  blog/2009/march/30/passed
            //  blog/2023/may/01/exam-day-2009

            //  blog/posts/2009/april/virginia-satir
            //  blog/posts/2023/january/10/aswb-exam-practice%C3%AF%C2%BF%C2%BDself-determination
            //  blog/posts/code-of-ethics-confidentiality
            //  blog/posts/meet-the-tutor-susan-mankita

            //  SELECT* FROM[SWTP_U8_live].[dbo].[SEOChecker_PageNotFound] where RedirectUrl = '' and url like '%blog/%'  order by url



            try
            {
                //Update records if they use dates in the url
                foreach (SEOChecker_PageNotFound record in LstPagesNotFound.OrderBy(x => x.Url))
                {
                    if (string.IsNullOrEmpty(record.RedirectUrl))
                    {
                        bool recordUpdated = false;
                        string _tempRecord = record.Url.Replace("blog/posts/", "").Replace("blog/", "");
                        List<string> lstRecordPath = _tempRecord.Split('/').ToList();

                        //Goal: to narrow down list of blog records down to 1
                        //===================================================

                        string _day = string.Empty;
                        string _month = string.Empty;
                        string _year = string.Empty;

                        //Extrapilate date from record
                        foreach (string yr in LstYears)
                        {
                            if (record.Url.Contains(yr))
                            {
                                _year = yr;
                                break;
                            }
                        }
                        foreach (string mo in LstMonths)
                        {
                            if (record.Url.Contains(mo))
                            {
                                _month = mo;
                                break;
                            }
                        }
                        foreach (string day in LstDays)
                        {
                            if (record.Url.Contains(day))
                            {
                                _day = day;
                                break;
                            }
                        }

                        //Proceed if all values have been identified
                        //if (!string.IsNullOrEmpty(_day) && !string.IsNullOrEmpty(_month) && !string.IsNullOrEmpty(_year))
                        if (!string.IsNullOrEmpty(_month) && !string.IsNullOrEmpty(_year))
                        {
                            //Split title to segments
                            List<string> LstOldTitleSegs = record.Url.TrimEnd('/').Split('/').Last().Split('-').ToList();
                            for (int i = 0; i < LstOldTitleSegs.Count; i++)
                            {
                                //split and take only part of string before %
                                if (LstOldTitleSegs[i].Contains("%"))
                                    LstOldTitleSegs[i] = LstOldTitleSegs[i].Split('%').FirstOrDefault();

                                //Keep segment only if alphanumeric
                                if (!LstOldTitleSegs[i].All(char.IsLetterOrDigit))
                                    LstOldTitleSegs[i] = string.Empty;

                                //Skip if segment contains...
                                if (LstOldTitleSegs[i].Contains("quo"))
                                    LstOldTitleSegs[i] = string.Empty;

                                //Skip if segment is less than 4 chars long
                                if (LstOldTitleSegs[i].Length < 4)
                                    LstOldTitleSegs[i] = string.Empty;
                            }


                            //Reduced list
                            List<blogRecord> LstTempBlogRecords = LstBlogRecords.Where(x => x.Url.Contains(_day) && x.Url.Contains(_month) & x.Url.Contains(_year)).ToList();

                            //If none are found, then browden the search to years only (some blog posts had been moved to different dates.
                            if (LstTempBlogRecords.Count == 0)
                                LstTempBlogRecords = LstBlogRecords.Where(x => x.Url.Contains(_year)).ToList();


                            foreach (string _segment in LstOldTitleSegs)
                            {
                                //Narrow down list by title segment
                                LstTempBlogRecords = LstTempBlogRecords.Where(x => x.Title.Contains(_segment)).ToList();

                                //Continue only if any records still exist in list
                                if (LstTempBlogRecords.Count == 0) break;

                                //If count == 1, then a match has been found.
                                if (LstTempBlogRecords.Count == 1)
                                {
                                    record.RedirectUrl = LstTempBlogRecords.FirstOrDefault().Url;
                                    updated++;
                                    recordUpdated = true;
                                    break;
                                }
                            }



                            if (!recordUpdated)
                            {
                                //Try again, but with broader search to years only (some blog posts had been moved to different dates.
                                LstTempBlogRecords = LstBlogRecords.Where(x => x.Url.Contains(_year)).ToList();


                                foreach (string _segment in LstOldTitleSegs)
                                {
                                    //Narrow down list by title segment
                                    LstTempBlogRecords = LstTempBlogRecords.Where(x => x.Title.Contains(_segment)).ToList();

                                    //Continue only if any records still exist in list
                                    if (LstTempBlogRecords.Count == 0) break;

                                    //If count == 1, then a match has been found.
                                    if (LstTempBlogRecords.Count == 1)
                                    {
                                        record.RedirectUrl = LstTempBlogRecords.FirstOrDefault().Url;
                                        updated++;
                                        break;
                                    }
                                }
                            }


                        }
                    }
                }

                //Submit all updates
                repo.BulkUpdateRecord(LstPagesNotFound);

            }
            catch (Exception ex) { Logger.Error<ApiMigrateDataController>(ex); }



            sw.Stop();
            return "Complete.  Updated " + updated.ToString() + " records.   |  Total Time: " + sw.Elapsed.ToString(@"hh\:mm\:ss");
        }
        private class blogRecord
        {
            public string Title { get; set; }
            public string Url { get; set; }
        }
        public string PullDescriptions(List<SeoDescription> _data)
        {
            //Start watch
            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();


            //
            int counter = 0;
            List<SeoDescription> LstUnfound = new List<SeoDescription>();


            //Get all nodes
            List<Link> LstLnks = new List<Link>();
            LstLnks = GetSEONodes_Recursive(Umbraco.Content(1058));


            //
            foreach (SeoDescription record in _data)
            {
                //Get matching record
                int nodeId = -1;
                if (LstLnks.Any(x => x.Name == record.Name))
                {
                    nodeId = LstLnks.FirstOrDefault(x => x.Name == record.Name).Id;
                }
                else if (LstLnks.Any(x => x.Subname == alphaNumOnly(record.Name.ToLower())))
                {
                    nodeId = LstLnks.FirstOrDefault(x => x.Subname == alphaNumOnly(record.Name.ToLower())).Id;
                }

                //
                if (nodeId > -1)
                {
                    IContent ic = _contentService.GetById(nodeId);
                    ic.SetValue(Models.Common.NodeProperties.SEODescription, record.Description);
                    //ic.SetValue(Models.Common.NodeProperties.Price, offer.Price);
                    //ic.SetValue(Models.Common.NodeProperties.Exams, string.Join(",", lstNodes));

                    _contentService.SaveAndPublish(ic);
                    counter++;
                }
                else
                {
                    //Add unfound record to list.
                    LstUnfound.Add(record);
                }
            }





            sw.Stop();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Time: " + sw.Elapsed.ToString(@"hh\:mm\:ss"));
            sb.AppendLine("Updated: " + counter.ToString());
            sb.AppendLine("Unmatched: " + LstUnfound.Count().ToString());
            sb.AppendLine(Newtonsoft.Json.JsonConvert.SerializeObject(LstUnfound));
            //sb.AppendLine(Newtonsoft.Json.JsonConvert.SerializeObject(LstLnks));
            return sb.ToString();
        }
        public string FindMatchingRedirects()
        {
            //Start watch
            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();


            //Instantiate variables.
            PageNotFoundRepository repo = new PageNotFoundRepository(new EF_SwtpDb());
            List<SEOChecker_PageNotFound> LstPagesNotFound = repo.GetAll();
            List<blogRecord> LstBlogRecords = new List<blogRecord>();
            int updated = 0;
            HashSet<int> lstRecordsToDelete = new HashSet<int>();


            try
            {
                //Remove starting / in 'redirectUrl' column
                foreach (SEOChecker_PageNotFound record in LstPagesNotFound)
                {
                    //Remove any starting /'s from url
                   //if (record.RedirectUrl[0] == '/')
                   // {
                        record.RedirectUrl = record.RedirectUrl.TrimStart('/');
                        updated++;
                    //}

                    //string _url = record.Url.TrimStart('/').TrimEnd('/').ToLower();
                    //string _redirectUrl = record.RedirectUrl.TrimStart('/').TrimEnd('/').ToLower();
                    //if (_url == _redirectUrl)
                    //{
                    //    lstRecordsToDelete.Add(record.NotFoundId);
                    //}
                }

                //Submit all updates
                repo.BulkUpdateRecord(LstPagesNotFound);

            }
            catch (Exception ex) { Logger.Error<ApiMigrateDataController>(ex); }



            sw.Stop();
            return 
                "Updated " + updated.ToString() + " records.   |  Delete " + 
                lstRecordsToDelete.Count() + " records  |  Total Time: " + 
                sw.Elapsed.ToString(@"hh\:mm\:ss") + "   |  Delete the following: " + 
                Newtonsoft.Json.JsonConvert.SerializeObject(lstRecordsToDelete);
        }



        #endregion



        #region "Private Methods"
        private List<Link> GetSEONodes_Recursive(IPublishedContent ip)
        {
            List<Link> LstLnks = new List<Link>();


            //Get only nodes with SEO content
            if (ip.HasProperty(Models.Common.NodeProperties.SEODescription))
            {
                LstLnks.Add(new Link()
                {
                    Id = ip.Id,
                    Name = ip.Name,
                    Subname = alphaNumOnly(ip.Name.ToLower())
                });
            }


            //Get all children nodes
            foreach (IPublishedContent ipChild in ip.Children)
            {
                LstLnks.AddRange(GetSEONodes_Recursive(ipChild));
            }


            return LstLnks;
        }
        private string alphaNumOnly(string original)
        {
            if (string.IsNullOrEmpty(original))
                return original;

            StringBuilder sb = new StringBuilder();
            foreach (Char _char in original)
            {
                if ((_char >= 'a' && _char <= 'z') || (_char >= 'A' && _char <= 'Z') || (_char >= '0' && _char <= '9'))
                    sb.Append(_char);
            }
            return sb.ToString();
        }
        private PurchaseRecordItem CreateNewPurchaseRecordItem(EF.PurchaseRecord newPurchaseRecord, int memberId, string examTitle, Dictionary<int, string> DictExams)
        {
            PurchaseRecordItem purchaseRecordItem = new PurchaseRecordItem();
            purchaseRecordItem.PurchaseRecordId = newPurchaseRecord.PurchaseRecordId;
            purchaseRecordItem.MemberId = memberId;
            purchaseRecordItem.ExamId = DictExams.FirstOrDefault(x => x.Value == examTitle).Key;
            purchaseRecordItem.ExamTitle = examTitle;
            purchaseRecordItem.OriginalPrice = 0;
            purchaseRecordItem.Extensions = 0;
            if (newPurchaseRecord.PurchaseDate != null) purchaseRecordItem.ExpirationDate = newPurchaseRecord.PurchaseDate.AddDays(90);
            return purchaseRecordItem;


            //PurchaseRecordItemId	PurchaseRecordId	MemberId	ExamId	ExamTitle	OriginalPrice	Extensions	ExpirationDate
            //1                     1                   84592       1223    DSM Booster 20.00           0           2023 - 05 - 18
        }
        private void Create_SpecialOffer(SpecialOffer offer)
        {
            //Create list of pages by guid IDs [added to bundle]
            List<string> lstNodes = new List<string>();
            foreach (bl.Models.api.Exam exam in offer.LstExams)
            {
                if (dictNodeGuids.ContainsKey(exam.Title))
                {
                    lstNodes.Add("umb://document/" + dictNodeGuids[exam.Title].ToString().Replace("-", ""));
                }
            }

            //Create new node ONLY IF any items exist in list!
            if (lstNodes.Any())
            {
                IPublishedContent ipFolder = Umbraco.Content((int)(Models.Common.SiteNode.SpecialOffers));
                IContent icNode = _contentService.Create(offer.Title, ipFolder.Key, Models.Common.DocType.Offer);
                icNode.SetValue(Models.Common.NodeProperties.SubscriptionTime, offer.SubscriptionTime);
                icNode.SetValue(Models.Common.NodeProperties.Price, offer.Price);
                icNode.SetValue(Models.Common.NodeProperties.ExamGroup, JsonConvert.SerializeObject(new[] { offer.ExamGroup })); //selecting ddl item

                icNode.SetValue(Models.Common.NodeProperties.Exams, string.Join(",", lstNodes));

                _contentService.SaveAndPublish(icNode);
                //oneCreated = true;
            }


            //  INCOMING JSON DATA
            //===================================
            //"Title": "Exam Bundle #1 (ASWB #1 & 2)",
            //"Price": 78,
            //"SubscriptionTime": "90",
            //"LstExams": [
            //  {
            //    "Id": 4190061,
            //    "Title": "ASWB Exam 1",
            //    "Alias": null,
            //    "Duration": null,
            //    "Price": null,
            //    "HideFromCalifornia": false,
            //    "HideFromUSA": false
            //  },
            //  {
            //    "Id": 2750151,
            //    "Title": "ASWB Exam 2",
            //    "Alias": null,
            //    "Duration": null,
            //    "Price": null,
            //    "HideFromCalifornia": false,
            //    "HideFromUSA": false
            //  }
            //],
            //"ExamGroup": "ASWB Exam Bundles"
        }
        private void Create_ExamFree(bl.Models.api.Exam exam)
        {
            var ipFolder = Umbraco.Content((int)(Models.Common.SiteNode.Exams));
            var icNode = _contentService.Create(exam.Title, ipFolder.Key, Models.Common.DocType.ExamFree);
            _contentService.SaveAndPublish(icNode);

            //  INCOMING JSON DATA
            //===================================
            //  "Title": "Free Practice Exam v1.7",
            //  "Alias": "ExamFree",
            //  "Duration": null,
            //  "Price": null,
            //  "HideFromCalifornia": false,
            //  "HideFromUSA": false
        }
        private void Create_ExamPaid(bl.Models.api.Exam exam)
        {
            var ipFolder = Umbraco.Content((int)(Models.Common.SiteNode.Exams));
            var icNode = _contentService.Create(exam.Title, ipFolder.Key, Models.Common.DocType.ExamPaid);
            icNode.SetValue(Models.Common.NodeProperties.Duration, exam.Duration);
            icNode.SetValue(Models.Common.NodeProperties.Price, exam.Price);
            icNode.SetValue(Models.Common.NodeProperties.HideFromCalifornia, exam.HideFromCalifornia);
            icNode.SetValue(Models.Common.NodeProperties.HideFromUSA, exam.HideFromUSA);
            _contentService.SaveAndPublish(icNode);

            //  INCOMING JSON DATA
            //===================================
            //  "Title": "ASWB Exam #1",
            //  "Alias": "ExamPaid",
            //  "Duration": "04:00",
            //  "Price": "39",
            //  "HideFromCalifornia": false,
            //  "HideFromUSA": false
        }
        private IMedia ImportImageByUrl(string imgUrl)
        {
            //Instantiate variables
            IMedia articleImage = null;

            try
            {
                if (!string.IsNullOrEmpty(imgUrl))
                {
                    //Get article img url for main site.  (not test api site)
                    imgUrl = imgUrl.Replace("https://api.swtp.localhost/", "https://socialworktestprep.com/");

                    //Stream File from URL
                    WebRequest webRequest = WebRequest.Create(imgUrl);
                    WebResponse webResponse = webRequest.GetResponse();
                    System.IO.Stream responseStream = webResponse.GetResponseStream();

                    if (responseStream != null)
                    {
                        //Get File Name from Url
                        int lastIndex = imgUrl.LastIndexOf("/") + 1;
                        string filename = imgUrl.Substring(lastIndex, imgUrl.Length - lastIndex);

                        // Open a new stream to the file
                        using (responseStream)
                        {
                            //Initialize a new image in the Blog Posts folder
                            articleImage = Services.MediaService.CreateMediaWithIdentity(filename, (int)Models.Common.SiteNode.Media_BlogPosts, Constants.Conventions.MediaTypes.Image);

                            //Set the property value
                            articleImage.SetValue(
                                Services.ContentTypeBaseServices,
                                Constants.Conventions.Media.File,
                                filename.Replace(" ", "-"),
                                responseStream);

                            //Save the media
                            Services.MediaService.Save(articleImage);
                            return articleImage;
                        }
                    }
                }
            }
            catch { }

            return null;
        }
        private string UpdateImgTagsInText(string content)
        {
            try
            {
                //Instantiate variables
                const string beginning = "src=\"";
                const string end = "\"";
                HashSet<int> lstIndexes = new HashSet<int>();
                List<Tuple<string, string>> LstURLsToChange = new List<Tuple<string, string>>();
                List<Tuple<string, string>> LstUpdatedURLs = new List<Tuple<string, string>>();


                if (content.Contains(beginning))
                {
                    //Create list of indexes to extract URLs from
                    int start = 0;
                    int _index;
                    while ((_index = content.IndexOf(beginning, start)) >= 0)
                    {
                        lstIndexes.Add(_index);
                        start = _index + 1;
                    }

                    //Create list of img URLs as a [before|after] list
                    foreach (int index in lstIndexes)
                    {
                        string urlToUpdate = content.Substring(index + beginning.Length); //get string starting at index
                        int nextIndex = urlToUpdate.IndexOf(end); //get end point index
                        urlToUpdate = urlToUpdate.Substring(0, nextIndex); //get only url

                        //Update url is a relative address.
                        string newUrl = urlToUpdate;
                        if (urlToUpdate.Substring(0, 1) == "/")
                        {
                            newUrl = "https://socialworktestprep.com" + urlToUpdate;
                        }
                        LstURLsToChange.Add(new Tuple<string, string>(urlToUpdate, newUrl)); //add conversion to list
                    }

                    //Import all images and get new URLs
                    foreach (Tuple<string, string> tupUrl in LstURLsToChange)
                    {
                        IMedia img = ImportImageByUrl(tupUrl.Item2);

                        if (img != null && img.GetValue(Constants.Conventions.Media.File) != null)
                            LstUpdatedURLs.Add(
                                new Tuple<string, string>(
                                    tupUrl.Item1,
                                    Umbraco.Media(img.Id).Url()));
                        else
                            LstUpdatedURLs.Add(tupUrl);
                    }

                    //Replace each of the images in content with udpated URLs.
                    foreach (Tuple<string, string> tupUrl in LstUpdatedURLs)
                    {
                        if (@tupUrl.Item1 != @tupUrl.Item2)
                            content = content.Replace(@tupUrl.Item1, @tupUrl.Item2);
                    }
                }
            }
            catch { }

            return content;
        }
        private bool CreateMember(string email)
        {
            try
            {
                // Create member [reduces coding to speed up import process]
                IMember newMember = Services.MemberService.CreateMember(
                    email.ToLower().Trim(),
                    email.ToLower().Trim(),
                    email.ToLower().Trim(),
                    Models.Common.DocType.Member);

                newMember.IsApproved = true;
                newMember.IsLockedOut = true;

                // Save new member
                Services.MemberService.Save(newMember);

                return true;

                //IMember newMember = Services.MemberService.CreateMemberWithIdentity(
                //    email.ToLower().Trim(),
                //    email.ToLower().Trim(),
                //    email.ToLower().Trim(),
                //    Models.Common.DocType.Member);
                // Set member values
                //newMember.SetValue("firstName", firstName);
                //newMember.SetValue("lastName", lastName);
                //Services.MemberService.SavePassword(newMember, "Pa55word");

            }
            catch (Exception ex)
            {
                Response.Write("Error: " + ex.Message);
                Logger.Error<ApiMigrateDataController>(ex);
                return false;
            }
        }
        private int GetCorrectAnswerId(int id)
        {
            //Get question as model
            cm.Question cmQuestion = new cm.Question(Umbraco.Content(id));

            //Get index of correct answer.
            int index = 1;
            foreach (var _answerSet in cmQuestion.AnswerSets)
            {
                if (_answerSet.IsCorrectAnswer)
                {
                    break;
                }
                index++;
            }

            return index;
        }
        public string UpdateBlogPosts_AddThumbnails()
        {
            //Instantiate variables
            StringBuilder sb = new StringBuilder();
            IPublishedContent ipBlog = Umbraco.Content((int)(Models.Common.SiteNode.Blog));
            const string beginning = "src=\"";
            const string end = "\"";


            if (ExamineManager.Instance.TryGetIndex(Constants.UmbracoIndexes.ExternalIndexName, out var index))
            {
                //Get searcher
                ISearcher searcher = index.GetSearcher();


                //Loop through all blog posts
                foreach (IPublishedContent ipPost in ipBlog.DescendantsOfType(Models.Common.DocType.Post))
                {
                    try
                    {
                        //Obtain test post
                        cm.Post cmPost = new Post(Umbraco.Content(ipPost.Id));

                        //Update article image if empty
                        if (cmPost.ArticleImage == null)
                        {
                            //Does content contain an image?
                            if (cmPost.Content.ToString().Contains(beginning))
                            {
                                //Get content as string
                                string _content = cmPost.Content.ToString();

                                //Extract 1st image url from content
                                int _startIndex = _content.IndexOf(beginning);
                                string imageUrl = _content.Substring(_startIndex + beginning.Length); //get string starting at index
                                int _endIndex = imageUrl.IndexOf(end); //get end point index
                                imageUrl = imageUrl.Substring(0, _endIndex); //get only url

                                //Search for image url if exists in cache
                                if (!string.IsNullOrEmpty(imageUrl))
                                {
                                    //Search by doctype and img url
                                    IBooleanOperation examineQuery = searcher.CreateQuery("media").NodeTypeAlias("Image").And().Field("umbracoFile", imageUrl);

                                    string[] flds = new string[2] { "id", "__Key" };
                                    examineQuery.SelectFields(flds);

                                    //Get all results
                                    ISearchResults searchResults = examineQuery.Execute(maxResults: int.MaxValue);

                                    //If any results exist...
                                    if (searchResults.Any())
                                    {
                                        string mediaKey = searchResults.FirstOrDefault().Values["__Key"];
                                        if (!string.IsNullOrEmpty(mediaKey))
                                        {
                                            //Create image udi from media id
                                            int mediaId = Convert.ToInt32(searchResults.FirstOrDefault().Id);
                                            IMedia mediaImg = Services.MediaService.GetById(mediaId);
                                            Udi udi = Udi.Create(Constants.UdiEntityType.Media, mediaImg.Key);

                                            //Edit post
                                            var icNode = _contentService.GetById(cmPost.Id);
                                            icNode.SetValue(Models.Common.NodeProperties.ArticleImage, udi.ToString());
                                            _contentService.SaveAndPublish(icNode);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception exe)
                    {
                        sb.AppendLine("Error  [[[" + exe.Message + "]]]");
                    }
                }
            }

            return sb.ToString();
        }
        #endregion



        #region ToGoThru

        public string ImportBlogPost(bl.Models.api.BlogPost blogPost)
        {
            try
            {
                if (blogPost.PostDate == null)
                {
                    throw new Exception("Postdate empty!!!");
                }
                else
                {
                    //Instantiate variables
                    IPublishedContent ipBlog = Umbraco.Content((int)(Models.Common.SiteNode.Blog));
                    IPublishedContent ipYear = null;
                    IPublishedContent ipMonth = null;
                    IPublishedContent ipDay = null;


                    //Obtain or create year folder
                    if (ipBlog.Children(x => x.Name == blogPost.PostDate.Year.ToString()).Any())
                    {
                        //Get year folder
                        ipYear = ipBlog.Children(x => x.Name == blogPost.PostDate.Year.ToString()).FirstOrDefault();
                    }
                    else
                    {
                        //Create year folder
                        var icNode = _contentService.Create(blogPost.PostDate.Year.ToString(), ipBlog.Key, Models.Common.DocType.Year);
                        _contentService.SaveAndPublish(icNode);
                        ipYear = Umbraco.Content(icNode.Id);
                    }


                    //Obtain or create month folder
                    var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(blogPost.PostDate.Month);
                    if (ipYear.Children(x => x.Name == monthName).Any())
                    {
                        //Get month folder
                        ipMonth = ipYear.Children(x => x.Name == monthName).FirstOrDefault();
                    }
                    else
                    {
                        //Create month folder
                        var icNode = _contentService.Create(monthName, ipYear.Key, Models.Common.DocType.Month);
                        _contentService.SaveAndPublish(icNode);
                        ipMonth = Umbraco.Content(icNode.Id);
                    }


                    //Obtain or create day folder
                    if (ipMonth.Children(x => x.Name == blogPost.PostDate.Day.ToString("00")).Any())
                    {
                        //Get day folder
                        ipDay = ipMonth.Children(x => x.Name == blogPost.PostDate.Day.ToString("00")).FirstOrDefault();
                    }
                    else
                    {
                        //Create day folder
                        var icNode = _contentService.Create(blogPost.PostDate.Day.ToString("00"), ipMonth.Key, Models.Common.DocType.Day);
                        _contentService.SaveAndPublish(icNode);
                        ipDay = Umbraco.Content(icNode.Id);
                    }


                    //Create blog if missing
                    if (!ipDay.Children(x => x.Name == blogPost.Title).Any())
                    {
                        //Attempt to get article image if available
                        IMedia articleImage = ImportImageByUrl(blogPost.ArticleImgUrl);

                        //Import content images and update URLs.
                        blogPost.Content = UpdateImgTagsInText(Newtonsoft.Json.JsonConvert.DeserializeObject<string>(blogPost.Content));

                        //Create blog post
                        var icNode = _contentService.Create(blogPost.Title, ipDay.Key, Models.Common.DocType.Post);
                        icNode.SetValue(Models.Common.NodeProperties.PostDate, blogPost.PostDate);
                        icNode.SetValue(Models.Common.NodeProperties.DisableComments, blogPost.DisableComments);
                        icNode.SetValue(Models.Common.NodeProperties.Content, blogPost.Content);
                        icNode.SetValue(Models.Common.NodeProperties.PreviousUrl, blogPost.PreviousUrl);
                        icNode.AssignTags(Models.Common.NodeProperties.Categories, blogPost.LstCategories); //add list of categories to tags
                        if (articleImage != null)
                            icNode.SetValue(Models.Common.NodeProperties.ArticleImage, Udi.Create(Constants.UdiEntityType.Media, articleImage.Key).ToString());

                        //Save blog post
                        _contentService.SaveAndPublish(icNode);
                    }
                }
            }
            catch (Exception exe)
            {
                return "Error in  [[[" + exe.Message + "]]]   " + Newtonsoft.Json.JsonConvert.SerializeObject(blogPost);
            }

            return string.Empty;
        }
        public string ImportSpecialOffers(List<SpecialOffer> lstSpecialOffers)
        {
            try
            {
                //Obtain list of all exams
                dictNodeGuids = new Dictionary<string, Guid>();
                IPublishedContent ipFolder = Umbraco.Content((int)(Models.Common.SiteNode.Exams));
                foreach (var ipExam in ipFolder.Children)
                {
                    dictNodeGuids.Add(ipExam.Name, ipExam.Key);
                }

                //Create nodes
                foreach (SpecialOffer offer in lstSpecialOffers)
                {
                    Create_SpecialOffer(offer);
                    //if (oneCreated) break;
                }

                return "Import completed successfully!";
            }
            catch
            {
                return "API error";
            }
        }
        public string ImportExams(List<bl.Models.api.Exam> lstExams)
        {
            //Create each exam based on type
            foreach (bl.Models.api.Exam exam in lstExams)
            {
                if (exam.Alias == "ExamFree")
                {
                    Create_ExamFree(exam);
                }
                else if (exam.Alias == "ExamPaid")
                {
                    Create_ExamPaid(exam);
                }
            }
            return "complete";
        }
        public string ImportExamQuestions(List<bl.Models.api.Exam> lstExams)
        {
            //Instantiate variables
            StringBuilder sb = new StringBuilder();
            IPublishedContent ipFolder = Umbraco.Content((int)(Models.Common.SiteNode.Exams));

            try
            {
                foreach (bl.Models.api.Exam _exam in lstExams)
                {
                    //Get exam node by name
                    IPublishedContent ipExam = ipFolder.Children(x => x.Name == _exam.Title).FirstOrDefault();

                    //Add each content area to exam
                    foreach (bl.Models.api.ContentArea _contentArea in _exam.LstContentAreas)
                    {
                        //Instantiate variables
                        IPublishedContent ipContentArea = null;


                        //Obtain or create content area node
                        if (ipExam.Children(x => x.Name == _contentArea.Title).Any())
                        {
                            //Get content area
                            ipContentArea = ipExam.Children(x => x.Name == _contentArea.Title).FirstOrDefault();
                        }
                        else
                        {
                            //Add content area
                            var icContentArea = _contentService.Create(_contentArea.Title, ipExam.Key, Models.Common.DocType.ContentArea);
                            icContentArea.SetValue(Models.Common.NodeProperties.SuperContentArea, _contentArea.SuperContentArea);
                            _contentService.SaveAndPublish(icContentArea);
                            ipContentArea = Umbraco.Content(icContentArea.Id);
                        }

                        //Add each question
                        foreach (bl.Models.api.Question _question in _contentArea.LstQuestions)
                        {
                            //Create question node if it doesn't exist
                            if (!ipContentArea.Children(x => x.Name == _question.Title).Any())
                            {
                                //Add question
                                var icQuestion = _contentService.Create(_question.Title, ipContentArea.Key, Models.Common.DocType.Question);
                                icQuestion.SetValue(Models.Common.NodeProperties.QuestionText, _question.QuestionText);
                                icQuestion.SetValue(Models.Common.NodeProperties.Rationale, _question.Rationale);
                                icQuestion.SetValue(Models.Common.NodeProperties.Source, _question.Source);
                                icQuestion.SetValue(Models.Common.NodeProperties.AdditionalNotes, _question.AdditionalNotes);
                                icQuestion.SetValue(Models.Common.NodeProperties.SuggestedStudyDescription, _question.SuggestedStudyDescription);
                                //icQuestion.SetValue(Models.Common.NodeProperties.SuggestedStudyLinkOld, _question.SuggestedStudyLink);

                                //Add Answers. [Nested Content must be added as a list of dictionaries.]
                                List<Dictionary<string, string>> dictAnswerSet = new List<Dictionary<string, string>>();
                                foreach (bl.Models.api.AnswerSet _answerSet in _question.LstAnswers)
                                {
                                    //Create dictionary entry for answer set
                                    dictAnswerSet.Add(new Dictionary<string, string>()
                                {
                                    {"ncContentTypeAlias",Models.Common.DocType.AnswerSet}, //this is the only "default" value we need to fill for nested item
                                    {"answer", _answerSet.Answer},
                                    {"rationale", _answerSet.Rationale},
                                    {"isCorrectAnswer", Convert.ToInt16(_answerSet.IsCorrectAnswer).ToString()},
                                });
                                }
                                icQuestion.SetValue(Models.Common.NodeProperties.AnswerSets, JsonConvert.SerializeObject(dictAnswerSet));

                                _contentService.SaveAndPublish(icQuestion);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine(ex.ToString());
            }


            sb.AppendLine("IMPORT COMPLETE");
            return sb.ToString();
        }
        public string UpdateExamQuestions_StudyLinks()
        {
            //Instantiate variables
            StringBuilder sb = new StringBuilder();
            IPublishedContent ipFolder = Umbraco.Content((int)(Models.Common.SiteNode.Exams));

            try
            {
                foreach (IPublishedContent ipQuestion in ipFolder.DescendantsOfType(Models.Common.DocType.Question))
                {
                    cm.Question cmQuestion = new cm.Question(ipQuestion);

                    const string beginningHref = "href=\"";
                    const string endOfOpeningTag = ">";
                    const string closingTag = "</a>";
                    const string end = "\"";
                    const string targetAttr = "target=\"";

                    if (!cmQuestion.SuggestedStudyLinks.Any())
                    {
                        //if (!string.IsNullOrEmpty(cmQuestion.SuggestedStudyLinkOld) && cmQuestion.SuggestedStudyLinkOld.Contains(beginningHref))
                        //{
                        //    //Example of data:  <a href="http://www.socialworktoday.com/archive/mayjun2008p16.shtml" target="_blank" title="www.socialworktoday.com">www.socialworktoday.com</a>

                        //    //Extract url from string
                        //    string oldTag = cmQuestion.SuggestedStudyLinkOld;
                        //    int _startIndex = oldTag.IndexOf(beginningHref);
                        //    string href = oldTag.Substring(_startIndex + beginningHref.Length); //get string starting at index
                        //    int _endIndex = href.IndexOf(end); //get end point index
                        //    href = href.Substring(0, _endIndex); //get only url

                        //    //Extract title from string
                        //    oldTag = cmQuestion.SuggestedStudyLinkOld;
                        //    _startIndex = oldTag.IndexOf(endOfOpeningTag);
                        //    string title = oldTag.Substring(_startIndex + endOfOpeningTag.Length); //get string starting at index
                        //    _endIndex = title.IndexOf(closingTag); //get end point index
                        //    title = title.Substring(0, _endIndex); //get only url

                        //    string target = "";
                        //    if (cmQuestion.SuggestedStudyLinkOld.Contains(targetAttr))
                        //    {
                        //        //Extract target from string
                        //        oldTag = cmQuestion.SuggestedStudyLinkOld;
                        //        _startIndex = oldTag.IndexOf(targetAttr);
                        //        target = oldTag.Substring(_startIndex + targetAttr.Length); //get string starting at index
                        //        _endIndex = target.IndexOf(end); //get end point index
                        //        target = target.Substring(0, _endIndex); //get only url
                        //    }



                        //    // Create a list of links
                        //    var lstLinks = new List<Umbraco.Web.Models.Link>
                        //{
                        //    new Umbraco.Web.Models.Link
                        //    {
                        //        Target = target,
                        //        Name = title,
                        //        Url = href,
                        //        Type = LinkType.External
                        //    }
                        //};

                        //    // Serialize the list to JSON
                        //    var jsonLinks = JsonConvert.SerializeObject(lstLinks);

                        //    try
                        //    {
                        //        //Edit post
                        //        var icNode = _contentService.GetById(cmQuestion.Id);
                        //        //icNode.SetValue(Models.Common.NodeProperties.SuggestedStudyLinks, jsonLinks);
                        //        icNode.SetValue(cm.Question.GetModelPropertyType(x => x.SuggestedStudyLinks).Alias, jsonLinks);
                        //        _contentService.SaveAndPublish(icNode);
                        //    }
                        //    catch (Exception ex2)
                        //    {
                        //        sb.AppendLine(ex2.ToString());
                        //    }

                        //}
                    }

                }
            }
            catch (Exception ex)
            {
                sb.AppendLine(ex.ToString());
            }


            sb.AppendLine("UPDATES COMPLETE");
            return sb.ToString();
        }


        public string ConvertMissingPurchaseMemberToUmbraco()
        {
            /*      PART 2B: ADD ALL NEW EMAILS TO UMBRACO AS MEMBER     */
            /*==========================================================*/


            //Obtain all purchase records where member needs to be added to site
            repoOriginalPurchaseRecords = new OriginalPurchaseRecordsRepository(new EF_SwtpDb());
            List<Original_PurchaseRecords> LstAllPurchaseRecords = repoOriginalPurchaseRecords.GetAll().Where(x => x.memberId == null && !string.IsNullOrEmpty(x.payerEmail) && !x.isAddedToSite).ToList();


            //Obtain all existing members in umbraco
            IEnumerable<IMember> lstExistinMembers = Services.MemberService.GetAllMembers();


            //New list of members to be added to umbraco.
            List<string> lstMembers2Add = new List<string>();


            //Loop through list and create new list with only those records that are missing from umbraco.
            foreach (Original_PurchaseRecords record in LstAllPurchaseRecords)
            {
                if (!lstExistinMembers.Where(x => x.Email == record.payerEmail).Any())
                {
                    lstMembers2Add.Add(record.payerEmail);
                }
            }


            //Importing all emails as members  [timed]
            int addedSuccessfully = 0;
            int failed = 0;
            Stopwatch sw = Stopwatch.StartNew();
            TimeSpan timeTaken;
            sw.Start();
            foreach (string email in lstMembers2Add)
            {
                if (CreateMember(email))
                {
                    addedSuccessfully++;
                }
                else
                {
                    failed++;
                }
            }
            sw.Stop();
            timeTaken = sw.Elapsed;


            return "db: " + lstMembers2Add.Count().ToString() + " |  umb: " + lstExistinMembers.Count().ToString() + " |  added: " + addedSuccessfully.ToString() + " |  failed: " + failed.ToString() + " |  Timespan: " + timeTaken.ToString(@"m\:ss\.fff");



            ////For testing only!!!
            //Stopwatch sw = Stopwatch.StartNew();
            //TimeSpan timeTaken;
            //sw.Start();
            //CreateMember("jim.fifth@5thstudios.com");
            //sw.Stop();
            //timeTaken = sw.Elapsed;
            //return "Timespan: " + timeTaken.ToString(@"m\:ss\.fff");


        }
        public string ImportExistingQuestionIDs(List<bl.Models.api.Exam> lstExams)
        {
            //Instantiate variables.
            var repo = new ExamIdRelationshipRepository(new EF_SwtpDb());
            List<ExamIDs_Old_New> LstExamIDs = new List<ExamIDs_Old_New>();


            //Convert data into lists
            foreach (bl.Models.api.Exam _exam in lstExams)
            {
                foreach (var _contentArea in _exam.LstContentAreas)
                {
                    foreach (var _question in _contentArea.LstQuestions)
                    {
                        ExamIDs_Old_New _examIdRecord = new ExamIDs_Old_New();
                        _examIdRecord.ExamId_old = _exam.Id;
                        _examIdRecord.ContentId_old = _contentArea.Id;
                        _examIdRecord.QuestionId_old = _question.Id;
                        _examIdRecord.QuestionText_old = _question.QuestionText;
                        LstExamIDs.Add(_examIdRecord);
                    }
                }
            }

            //Submit all records in bulk
            if (LstExamIDs.Count() > 0) repo.BulkAddRecord(LstExamIDs);


            return "Complete.  Added " + LstExamIDs.Count().ToString() + " records.";
        }
        public string UpdateExistingQuestionIDs()
        {
            //Instantiate variables.
            IExamIdRelationshipRepository repo = new ExamIdRelationshipRepository(new EF_SwtpDb());
            List<ExamIDs_Old_New> LstExamIDs = repo.SelectAll();
            List<Temp_ExamIDs> LstExistingExamIDs = new List<Temp_ExamIDs>();

            //
            IPublishedContent ipExamFolder = Umbraco.Content((int)(Models.Common.SiteNode.Exams));
            foreach (IPublishedContent ipQuestion in ipExamFolder.DescendantsOfType(Models.Common.DocType.Question))
            {
                cm.Question cmQuestion = new cm.Question(ipQuestion);

                //Extract IDs from questions
                Temp_ExamIDs tempExamIDs = new Temp_ExamIDs();
                tempExamIDs.ExamId = cmQuestion.Parent.Parent.Id;
                tempExamIDs.ContentId = cmQuestion.Parent.Id;
                tempExamIDs.QuestionId = cmQuestion.Id;
                tempExamIDs.QuestionText = cmQuestion.QuestionText;
                LstExistingExamIDs.Add(tempExamIDs);
            }

            //
            foreach (var repoRecord in LstExamIDs)
            {
                //Get matching existing question record
                var temp = LstExistingExamIDs.Where(x => x.QuestionText == repoRecord.QuestionText_old).FirstOrDefault();

                //Update record
                repoRecord.ExamId_new = temp.ExamId;
                repoRecord.ContentId_new = temp.ContentId;
                repoRecord.QuestionId_new = temp.QuestionId;
            }


            //Submit all records in bulk
            if (LstExamIDs.Count() > 0) repo.BulkUpdateRecord(LstExamIDs);

            return "Complete.  Updated " + LstExamIDs.Count().ToString() + " records.";
            // return Newtonsoft.Json.JsonConvert.SerializeObject(LstExamIDs);

        }

        public string ImportNonupdatedData(NonupdatedData _data)
        {
            //Instantiate variables.
            repoOriginalCmsDataType = new OriginalCmsDataTypeRepository(new EF_SwtpDb());
            repoOriginalCmsPropertyType = new OriginalCmsPropertyTypeRepository(new EF_SwtpDb());

            List<Original_CmsDataType> LstCmsDataType = new List<Original_CmsDataType>();
            List<Original_CmsPropertyType> LstCmsPropertyType = new List<Original_CmsPropertyType>();


            //Convert data into lists
            foreach (var _dataType in _data.LstCmsDataType)
            {
                Original_CmsDataType cmsDataType = new Original_CmsDataType();
                cmsDataType.nodeId = _dataType.nodeId;
                cmsDataType.dbType = _dataType.dbType;
                LstCmsDataType.Add(cmsDataType);
            }
            foreach (var _propertyType in _data.LstCmsPropertyType)
            {
                Original_CmsPropertyType cmsPropertyType = new Original_CmsPropertyType();
                cmsPropertyType.id = _propertyType.id;
                cmsPropertyType.dataTypeId = _propertyType.dataTypeId;
                cmsPropertyType.contentTypeId = _propertyType.contentTypeId;
                cmsPropertyType.Alias = _propertyType.Alias;
                cmsPropertyType.Name = _propertyType.Name;
                LstCmsPropertyType.Add(cmsPropertyType);
            }

            //Submit all records in bulk
            if (LstCmsDataType.Count() > 0) repoOriginalCmsDataType.BulkAddRecord(LstCmsDataType);
            if (LstCmsPropertyType.Count() > 0) repoOriginalCmsPropertyType.BulkAddRecord(LstCmsPropertyType);


            return "Complete.  Added " + LstCmsDataType.Count().ToString() + " data types and " + LstCmsPropertyType.Count().ToString() + " property types.";
        }



        #endregion
    }

}