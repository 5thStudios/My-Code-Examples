using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Results;
using Umbraco.Web.WebApi;
using bl.Models.api;
using Umbraco.Core.Services;
using Repositories;
using bl.EF;
using Stripe;
using System.IO;
using bl.Controllers;
using Umbraco.Core.Logging;
using Umbraco.Web;
using System.Configuration;
using bl.Models;
using Umbraco.Core;
using System.Data.Entity.Validation;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using System.Diagnostics;
using cm = Umbraco.Web.PublishedModels;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.SignalR.Hosting;
using System.Text;
using static bl.Models.Common;
using static Umbraco.Core.Collections.TopoGraph;
using System.Web.Mvc.Html;
using Umbraco.Web.PublishedModels;
using Serilog;
using Umbraco.ModelsBuilder;

namespace Controllers
{
    public class SwtpApiController : UmbracoApiController
    {
        #region "Properties"
        private EF_SwtpDb _context;
        private ICouponRepository repoCoupon;
        private IPurchaseRecordItemRepository repoPurchaseRecordItem;
        private IPurchaseRecordRepository repoPurchaseRecord;
        private IPurchaseTypeRepository repoPurchaseType;
        private IOriginalPurchaseRecordsRepository repoOrigPurchaseRecords;

        public IContentService _contentService { get; set; }
        private IMediaService _mediaService;
        public SwtpApiController(IContentService contentService, IMediaService mediaService)
        {
            _contentService = contentService;
            _mediaService = mediaService;

            _context = new EF_SwtpDb();
            repoCoupon = new CouponRepository(_context);
            repoPurchaseRecordItem = new PurchaseRecordItemRepository(_context);
            repoPurchaseRecord = new PurchaseRecordRepository(_context);
            repoPurchaseType = new PurchaseTypeRepository(_context);
        }
        //EXPOSED PROPERTIES
        //=====================================
        //ServiceContext Services { get; }
        //ISqlContext SqlContext { get; }
        //UmbracoHelper Umbraco { get; }
        //UmbracoContext UmbracoContext { get; }
        //IGlobalSettings GlobalSettings { get; }
        //IProfilingLogger Logger { get; }
        //MembershipHelper Members { get; }
        #endregion






        #region "API Calls"

        //      https://localhost:44305/umbraco/Api/SwtpApi/ImportMembers
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> ImportMembers()
        {
            //Instantiate variables
            string data = "";

            try
            {
                //Create stopwatch
                Stopwatch sw = Stopwatch.StartNew();
                sw.Start();


                //Extract incoming data from datastream and convert to object
                data = new StreamReader(HttpContext.Current.Request.InputStream).ReadToEnd();
                List<bl.Models.ImportMember> lstImportedMembers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<bl.Models.ImportMember>>(data);


                //Instantiate variables
                repoPurchaseRecordItem = new PurchaseRecordItemRepository(new EF_SwtpDb());
                blMemberController memberController = new blMemberController();
                int member_Created = 0;
                int member_ExistedAlready = 0;
                int member_NotCreated = 0;
                int member_CannotRetrieve = 0;
                int exam_doesNotExist = 0;
                int exam_addedToMember = 0;
                int exam_extendedOnly = 0;


                //Obtain list of exams
                cm.Exams cmExams = new cm.Exams(Umbraco.Content((int)bl.Models.Common.SiteNode.Exams));
                List<IPublishedContent> lstExams = cmExams.Children().ToList();


                //Loop through list of members and create new list with only those records that are missing from umbraco.
                foreach (var record in lstImportedMembers)
                {
                    //Create member if does not exist
                    if (memberController.DoesMemberExist_byEmail(record.User.ToLower().Trim()))
                    {
                        member_ExistedAlready++;
                    }
                    else
                    {
                        if (CreateMember(record))
                            member_Created++;
                        else
                            member_NotCreated++;
                    }


                    //Obtain member
                    IMember member = Services.MemberService.GetByEmail(record.User.ToLower().Trim());


                    if (member == null)
                    {
                        //Cannot retrieve member
                        member_CannotRetrieve++;
                    }
                    else if (!lstExams.Any(x => x.Name == record.ExamName))
                    {
                        //Cannot find exam
                        exam_doesNotExist++;
                    }
                    else
                    {
                        //Obtain single exam
                        var _exam = lstExams.FirstOrDefault(x => x.Name == record.ExamName);


                        try
                        {
                            if (!repoPurchaseRecordItem.DoesRecordExists_byMemberId_ExamId(member.Id, _exam.Id))
                            {
                                //Add new record
                                PurchaseRecordItem newRecord = new PurchaseRecordItem()
                                {
                                    MemberId = member.Id,
                                    ExamId = _exam.Id,
                                    ExamTitle = _exam.Name,
                                    OriginalPrice = 0,
                                    ExpirationDate = DateTime.Now.AddDays(Convert.ToDouble(record.ExtendDaysBy)),
                                    Extensions = 0
                                };
                                repoPurchaseRecordItem.AddRecord(newRecord);
                                exam_addedToMember++;
                            }
                            else
                            {
                                //Extend only
                                PurchaseRecordItem purchaseRecord = repoPurchaseRecordItem.ObtainRecord_byMemberId_ExamId(member.Id, _exam.Id);
                                if (purchaseRecord != null)
                                {
                                    purchaseRecord.Extensions += 1;
                                    if (purchaseRecord.ExpirationDate.Date < DateTime.Today)
                                        purchaseRecord.ExpirationDate = DateTime.Now.AddDays(Convert.ToDouble(record.ExtendDaysBy)); //Add days starting today
                                    else
                                        purchaseRecord.ExpirationDate = purchaseRecord.ExpirationDate.AddDays(Convert.ToDouble(record.ExtendDaysBy)); //Add days to existing date

                                    repoPurchaseRecordItem.UpdateRecord(purchaseRecord);
                                    exam_extendedOnly++;
                                }
                            }
                        }
                        catch (Exception exex)
                        {
                            //Logger.Error<SwtpApiController>(exex, Newtonsoft.Json.JsonConvert.SerializeObject(_importRecord));

                            Dictionary<int, string> dict = new Dictionary<int, string>();
                            dict.Add(0, "Error: ()");
                            dict.Add(1, exex.Message);
                            dict.Add(2, "Record:");
                            dict.Add(3, Newtonsoft.Json.JsonConvert.SerializeObject(record));
                            dict.Add(4, "member.Id:");
                            dict.Add(5, member.Id.ToString());
                            dict.Add(6, "_exam.Id:");
                            dict.Add(7, _exam.Id.ToString());
                            Logger.Error<SwtpApiController>(exex, Newtonsoft.Json.JsonConvert.SerializeObject(dict));
                        }
                    }
                }


                //Split time to see where slowness is occuring
                sw.Stop();


                //Create return response
                List<bl.Models.KeyValuePair> lstResults = new List<bl.Models.KeyValuePair>();
                lstResults.Add(new bl.Models.KeyValuePair() { StrKey = "Time to Complete", StrValue = sw.Elapsed.ToString(@"hh\:mm\:ss") });
                lstResults.Add(new bl.Models.KeyValuePair() { StrKey = "Records to Import", IntValue = lstImportedMembers.Count() });
                lstResults.Add(new bl.Models.KeyValuePair() { StrKey = "Members Created", IntValue = member_Created });
                lstResults.Add(new bl.Models.KeyValuePair() { StrKey = "Members that Already Existed", IntValue = member_ExistedAlready });
                lstResults.Add(new bl.Models.KeyValuePair() { StrKey = "Members Not Created", IntValue = member_NotCreated });
                lstResults.Add(new bl.Models.KeyValuePair() { StrKey = "Failed Retrieving Members", IntValue = member_CannotRetrieve });
                lstResults.Add(new bl.Models.KeyValuePair() { StrKey = "Exam Did Not Exist", IntValue = exam_doesNotExist });
                lstResults.Add(new bl.Models.KeyValuePair() { StrKey = "Exams Added to Members", IntValue = exam_addedToMember });
                lstResults.Add(new bl.Models.KeyValuePair() { StrKey = "Exams Extended Only", IntValue = exam_extendedOnly });

                return Json(Newtonsoft.Json.JsonConvert.SerializeObject(lstResults));

            }
            catch (DbEntityValidationException ev)
            {
                Logger.Error<SwtpApiController>(ev);
                Logger.Error<SwtpApiController>(data);

                Dictionary<int, string> dict = new Dictionary<int, string>();
                dict.Add(0, "Entity Validation Error: ()");
                dict.Add(1, data);

                int index = 2;
                foreach (var eve in ev.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        string msg = string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                        dict.Add(index, msg);
                        index++;
                    }
                }

                return Json(Newtonsoft.Json.JsonConvert.SerializeObject(dict));
            }
            catch (Exception ex)
            {
                Logger.Error<SwtpApiController>(ex);
                Logger.Error<SwtpApiController>(data);

                Dictionary<int, string> dict = new Dictionary<int, string>();
                dict.Add(0, "Error: ()");
                dict.Add(1, data);
                dict.Add(2, ex.Message);

                return Json(Newtonsoft.Json.JsonConvert.SerializeObject(dict));
            }
        }
        private bool CreateMember(bl.Models.ImportMember record)
        {
            try
            {
                // Create member [reduces coding to speed up import process]
                IMember newMember = Services.MemberService.CreateMember(
                  record.User.ToLower().Trim(),
                    record.User.ToLower().Trim(),
                    record.User.ToLower().Trim(),
                    bl.Models.Common.DocType.Member);

                newMember.IsApproved = true;
                newMember.IsLockedOut = false;

                // Save new member
                Services.MemberService.Save(newMember);
                Services.MemberService.SavePassword(newMember, record.Password);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error<SwtpApiController>(ex);
                return false;
            }
        }




        //      https://localhost:44305/umbraco/Api/SwtpApi/ExtendMembers
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> ExtendMembers()
        {
            //Instantiate variables
            string data = "";

            try
            {
                //Extract incoming data from datastream and convert to object
                data = new StreamReader(HttpContext.Current.Request.InputStream).ReadToEnd();
                List<bl.Models.ImportMember> lstImportedMembers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<bl.Models.ImportMember>>(data);


                //Add records to Umbraco
                return Json(ExtendMembersExam(lstImportedMembers));
            }
            catch (DbEntityValidationException ev)
            {
                Logger.Error<SwtpApiController>(ev);
                Logger.Error<SwtpApiController>(data);

                Dictionary<int, string> dict = new Dictionary<int, string>();
                dict.Add(0, "Entity Validation Error: ()");
                dict.Add(1, data);

                int index = 2;
                foreach (var eve in ev.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        string msg = string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                        dict.Add(index, msg);
                        index++;
                    }
                }

                return Json(Newtonsoft.Json.JsonConvert.SerializeObject(dict));
            }
            catch (Exception ex)
            {
                Logger.Error<SwtpApiController>(ex);
                Logger.Error<SwtpApiController>(data);

                Dictionary<int, string> dict = new Dictionary<int, string>();
                dict.Add(0, "Error: ()");
                dict.Add(1, data);
                dict.Add(2, ex.Message);

                return Json(Newtonsoft.Json.JsonConvert.SerializeObject(dict));
            }
        }
        private string ExtendMembersExam(List<bl.Models.ImportMember> lstImportedMembers)
        {
            //Create stopwatch
            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();


            //Instantiate variables
            int member_CannotRetrieve = 0;
            int exam_doesNotExist = 0;
            int exam_extended = 0;
            int exam_notExtended = 0;


            //Obtain list of exams
            cm.Exams cmExams = new cm.Exams(Umbraco.Content((int)bl.Models.Common.SiteNode.Exams));
            List<IPublishedContent> lstExams = cmExams.Children().ToList();


            //Update each record
            foreach (var _importRecord in lstImportedMembers)
            {
                if (!string.IsNullOrEmpty(_importRecord.ExtendDaysBy) && Convert.ToInt16(_importRecord.ExtendDaysBy) > 0)
                {
                    //Obtain member
                    IMember member = Services.MemberService.GetByEmail(_importRecord.User.ToLower().Trim());

                    if (member == null)
                    {
                        //Cannot retrieve member
                        member_CannotRetrieve++;
                    }
                    else if (!lstExams.Any(x => x.Name == _importRecord.ExamName))
                    {
                        //Cannot find exam
                        exam_doesNotExist++;
                    }
                    else
                    {
                        //Get single exam from list
                        var _exam = lstExams.FirstOrDefault(x => x.Name == _importRecord.ExamName);


                        //Retrieve purchase record item for editing.
                        PurchaseRecordItem purchaseRecord = repoPurchaseRecordItem.ObtainRecord_byMemberId_ExamId(member.Id, _exam.Id);
                        if (purchaseRecord != null)
                        {
                            purchaseRecord.Extensions += 1;
                            if (purchaseRecord.ExpirationDate.Date < DateTime.Today)
                                purchaseRecord.ExpirationDate = DateTime.Now.AddDays(Convert.ToDouble(_importRecord.ExtendDaysBy)); //Add days starting today
                            else
                                purchaseRecord.ExpirationDate = purchaseRecord.ExpirationDate.AddDays(Convert.ToDouble(_importRecord.ExtendDaysBy)); //Add days to existing date

                            //Update record
                            repoPurchaseRecordItem.UpdateRecord(purchaseRecord);
                            exam_extended++;
                        }
                        else
                            exam_notExtended++;
                    }
                }
            }


            //Split time to see where slowness is occuring
            sw.Stop();


            //Create return response
            List<bl.Models.KeyValuePair> lstResults = new List<bl.Models.KeyValuePair>();
            lstResults.Add(new bl.Models.KeyValuePair() { StrKey = "Time to Complete", StrValue = sw.Elapsed.ToString(@"hh\:mm\:ss") });
            lstResults.Add(new bl.Models.KeyValuePair() { StrKey = "Records to Import", IntValue = lstImportedMembers.Count() });
            lstResults.Add(new bl.Models.KeyValuePair() { StrKey = "Failed Retrieving Members", IntValue = member_CannotRetrieve });
            lstResults.Add(new bl.Models.KeyValuePair() { StrKey = "Exam Did Not Exist", IntValue = exam_doesNotExist });
            lstResults.Add(new bl.Models.KeyValuePair() { StrKey = "Exams Extended", IntValue = exam_extended });
            lstResults.Add(new bl.Models.KeyValuePair() { StrKey = "Exams Not Extended", IntValue = exam_notExtended });
            return Newtonsoft.Json.JsonConvert.SerializeObject(lstResults);
        }




        //      https://localhost:44305/umbraco/Api/SwtpApi/ValidateCoupon?coupon=INCOMINGTEXT
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> ValidateCoupon(string coupon)
        {
            //Create result data
            CouponResult couponResult = new CouponResult();

            try
            {
                //Record submitted coupon
                couponResult.SearchFor = coupon;

                if (repoCoupon.CodeValid(coupon))
                {
                    couponResult.IsValid = true;
                    couponResult.Coupon = repoCoupon.Obtain_byCode(coupon);
                }
                else
                {
                    couponResult.ErrorMsg = "Invalid coupon.";
                }
            }
            catch (Exception ex)
            {
                //Attach error msg to return
                couponResult.IsValid = false;
                couponResult.ErrorMsg = ex.Message;
            }

            //Return as json result
            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(couponResult));
        }



        //      https://localhost:44305/umbraco/Api/SwtpApi/UpdateCoupon?data=INCOMINGTEXT
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> UpdateCoupon(string data)
        {
            try
            {
                //Convert incoming json to a class
                CouponSetting couponSetting = Newtonsoft.Json.JsonConvert.DeserializeObject<CouponSetting>(data);


                //Create repo
                repoCoupon = new CouponRepository(new EF_SwtpDb());


                //Instantiate variables
                bool IsNew = true;
                bl.EF.Coupon coupon = new bl.EF.Coupon();



                //Obtain record if exists
                if (repoCoupon.CodeValid(couponSetting.CouponName))
                {
                    coupon = repoCoupon.Obtain_byCode(couponSetting.CouponName);
                    IsNew = false;
                }



                //Apply updates to record
                if (IsNew)
                {
                    coupon.Code = couponSetting.CouponName;
                    coupon.CreateDate = DateTime.Now;
                    coupon.TimesUsed = 0;
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



                //Add or Update record
                if (IsNew)
                {
                    repoCoupon.AddRecord(coupon);
                }
                else
                {
                    repoCoupon.UpdateRecord(coupon);
                }

                //Return results
                return Json(Newtonsoft.Json.JsonConvert.SerializeObject(coupon));

            }
            catch (DbEntityValidationException ev)
            {
                Logger.Error<SwtpApiController>(ev);
                Logger.Error<SwtpApiController>(data);

                Dictionary<int, string> dict = new Dictionary<int, string>();
                dict.Add(0, "Entity Validation Error: UpdateCoupon()");
                dict.Add(1, data);

                int index = 2;
                foreach (var eve in ev.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        string msg = string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                        dict.Add(index, msg);
                        index++;
                    }
                }

                return Json(Newtonsoft.Json.JsonConvert.SerializeObject(dict));
            }
            catch (Exception ex)
            {
                Logger.Error<SwtpApiController>(ex);
                Logger.Error<SwtpApiController>(data);

                Dictionary<int, string> dict = new Dictionary<int, string>();
                dict.Add(0, "Error: UpdateCoupon()");
                dict.Add(1, data);
                dict.Add(2, ex.Message);

                return Json(Newtonsoft.Json.JsonConvert.SerializeObject(dict));
            }
        }

        //      https://localhost:44305/umbraco/Api/SwtpApi/Subscriptions_SearchMember?data=INCOMINGTEXT
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> Subscriptions_SearchMember(string data)
        {
            //Instantiate class for subscriptions
            ExtendSubscriptions extendSubscriptions = new ExtendSubscriptions();
            extendSubscriptions.Email = data.Trim();

            try
            {
                //Attempt to get member data
                IMember member = new blMemberController().GetMember_byEmail(extendSubscriptions.Email);


                if (member == null)
                {
                    return null;
                }
                else
                {
                    //Instantiate variables
                    repoPurchaseRecordItem = new PurchaseRecordItemRepository(new EF_SwtpDb());


                    //Set member data
                    extendSubscriptions.IsValidMember = true;
                    extendSubscriptions.MemberId = member.Id;


                    //Obtain list of all exams for dropdown
                    IPublishedContent ipExams = Umbraco.Content((int)bl.Models.Common.SiteNode.Exams);
                    foreach (IPublishedContent ipExam in ipExams.DescendantsOfType(bl.Models.Common.DocType.ExamPaid))
                    {
                        extendSubscriptions.LstBonusExams.Add(new bl.Models.Exam()
                        {
                            Title = ipExam.Name,
                            Id = ipExam.Id //ExamId
                        });
                    }


                    //Obtain list of all exams for member
                    foreach (PurchaseRecordItem record in repoPurchaseRecordItem.ObtainRecords_byMemberId(member.Id))
                    {
                        string status = (record.ExpirationDate.Date >= DateTime.Today) ? "Active" : "Inactive";
                        extendSubscriptions.LstExamSubscription.Add(new ExamSubscription()
                        {
                            Id = record.PurchaseRecordItemId, //PurchaseRecordId
                            IsSelected = false,
                            Title = record.ExamTitle,
                            ExpirationDate = record.ExpirationDate,
                            Status = status
                        });
                    }
                }



            }
            catch (DbEntityValidationException ev)
            {
                Logger.Error<SwtpApiController>(ev);
                Logger.Error<SwtpApiController>(data);

                Dictionary<int, string> dict = new Dictionary<int, string>();
                dict.Add(0, "Entity Validation Error: Subscriptions_SearchMember()");
                dict.Add(1, data);

                int index = 2;
                foreach (var eve in ev.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        string msg = string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                        dict.Add(index, msg);
                        index++;
                    }
                }

                return Json(Newtonsoft.Json.JsonConvert.SerializeObject(dict));
            }
            catch (Exception ex)
            {
                Logger.Error<SwtpApiController>(ex);
                Logger.Error<SwtpApiController>(data);

                Dictionary<int, string> dict = new Dictionary<int, string>();
                dict.Add(0, "Error: Subscriptions_SearchMember()");
                dict.Add(1, data);
                dict.Add(2, ex.Message);

                return Json(Newtonsoft.Json.JsonConvert.SerializeObject(dict));
            }

            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(extendSubscriptions));
        }



        //      https://localhost:44305/umbraco/Api/SwtpApi/Subscriptions_GiveExamToMember?data=INCOMINGTEXT
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> Subscriptions_GiveExamToMember() //(string data)
        {
            string data = "";

            try
            {
                //Extract incoming data from datastream.
                data = new StreamReader(HttpContext.Current.Request.InputStream).ReadToEnd();
                ExtendSubscriptions extendSubscriptions = Newtonsoft.Json.JsonConvert.DeserializeObject<ExtendSubscriptions>(data);

                //Instantiate variables
                repoPurchaseRecordItem = new PurchaseRecordItemRepository(new EF_SwtpDb());

                //Add records if it does not exist.
                if (!repoPurchaseRecordItem.DoesRecordExists_byMemberId_ExamId((int)extendSubscriptions.MemberId, (int)extendSubscriptions.BonusExamId))
                {
                    //Add new record
                    PurchaseRecordItem newRecord = new PurchaseRecordItem()
                    {
                        MemberId = (int)extendSubscriptions.MemberId,
                        ExamId = (int)extendSubscriptions.BonusExamId,
                        ExamTitle = Umbraco.Content(extendSubscriptions.BonusExamId).Name,
                        OriginalPrice = 0,
                        ExpirationDate = DateTime.Now.AddDays(90),
                        Extensions = 0
                    };
                    repoPurchaseRecordItem.AddRecord(newRecord);


                    //Obtain list of all exams for member
                    extendSubscriptions.LstExamSubscription.Clear();
                    foreach (PurchaseRecordItem record in repoPurchaseRecordItem.ObtainRecords_byMemberId((int)extendSubscriptions.MemberId))
                    {
                        string status = (record.ExpirationDate.Date >= DateTime.Today) ? "Active" : "Inactive";
                        extendSubscriptions.LstExamSubscription.Add(new ExamSubscription()
                        {
                            Id = record.PurchaseRecordItemId, //PurchaseRecordId
                            IsSelected = false,
                            Title = record.ExamTitle,
                            ExpirationDate = record.ExpirationDate,
                            Status = status
                        });
                    }
                }

                return Json(Newtonsoft.Json.JsonConvert.SerializeObject(extendSubscriptions));
            }
            catch (DbEntityValidationException ev)
            {
                Logger.Error<SwtpApiController>(ev);
                Logger.Error<SwtpApiController>(data);

                Dictionary<int, string> dict = new Dictionary<int, string>();
                dict.Add(0, "Entity Validation Error: Subscriptions_GiveExamToMember()");
                dict.Add(1, data);

                int index = 2;
                foreach (var eve in ev.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        string msg = string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                        dict.Add(index, msg);
                        index++;
                    }
                }

                return Json(Newtonsoft.Json.JsonConvert.SerializeObject(dict));
            }
            catch (Exception ex)
            {
                Logger.Error<SwtpApiController>(ex);
                Logger.Error<SwtpApiController>(data);

                Dictionary<int, string> dict = new Dictionary<int, string>();
                dict.Add(0, "Error: Subscriptions_GiveExamToMember()");
                dict.Add(1, data);
                dict.Add(2, ex.Message);

                return Json(Newtonsoft.Json.JsonConvert.SerializeObject(dict));
            }
        }



        //      https://localhost:44305/umbraco/Api/SwtpApi/Subscriptions_GiveExamToMember?data=INCOMINGTEXT
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> Subscriptions_ExtendSelectedSubscriptions() //string data)
        {
            string data = "";

            try
            {
                //Extract incoming data from datastream.
                data = new StreamReader(HttpContext.Current.Request.InputStream).ReadToEnd();

                //Instantiate variables
                repoPurchaseRecordItem = new PurchaseRecordItemRepository(new EF_SwtpDb());
                ExtendSubscriptions extendSubscriptions = Newtonsoft.Json.JsonConvert.DeserializeObject<ExtendSubscriptions>(data);


                //Update each record
                foreach (ExamSubscription record in extendSubscriptions.LstExamSubscription.Where(x => x.IsSelected))
                {
                    PurchaseRecordItem purchaseRecord = repoPurchaseRecordItem.ObtainRecord_byId(record.Id);
                    if (purchaseRecord != null)
                    {
                        purchaseRecord.Extensions += 1;
                        if (purchaseRecord.ExpirationDate.Date < DateTime.Today)
                            purchaseRecord.ExpirationDate = DateTime.Now.AddDays(extendSubscriptions.ExtendDays); //Add days starting today
                        else
                            purchaseRecord.ExpirationDate = purchaseRecord.ExpirationDate.AddDays(extendSubscriptions.ExtendDays); //Add days to existing date
                    }
                    repoPurchaseRecordItem.UpdateRecord(purchaseRecord);
                }


                //Obtain list of all exams for member
                extendSubscriptions.LstExamSubscription.Clear();
                foreach (PurchaseRecordItem record in repoPurchaseRecordItem.ObtainRecords_byMemberId((int)extendSubscriptions.MemberId))
                {
                    string status = (record.ExpirationDate.Date >= DateTime.Today) ? "Active" : "Inactive";
                    extendSubscriptions.LstExamSubscription.Add(new ExamSubscription()
                    {
                        Id = record.PurchaseRecordItemId,
                        IsSelected = false,
                        Title = record.ExamTitle,
                        ExpirationDate = record.ExpirationDate,
                        Status = status
                    });
                }


                return Json(Newtonsoft.Json.JsonConvert.SerializeObject(extendSubscriptions));
            }
            catch (DbEntityValidationException ev)
            {
                Logger.Error<SwtpApiController>(ev);
                Logger.Error<SwtpApiController>(data);

                Dictionary<int, string> dict = new Dictionary<int, string>();
                dict.Add(0, "Entity Validation Error: Subscriptions_ExtendSelectedSubscriptions()");
                dict.Add(1, data);

                int index = 2;
                foreach (var eve in ev.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        string msg = string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                        dict.Add(index, msg);
                        index++;
                    }
                }

                return Json(Newtonsoft.Json.JsonConvert.SerializeObject(dict));
            }
            catch (Exception ex)
            {
                Logger.Error<SwtpApiController>(ex);
                Logger.Error<SwtpApiController>(data);

                Dictionary<int, string> dict = new Dictionary<int, string>();
                dict.Add(0, "Error: Subscriptions_ExtendSelectedSubscriptions()");
                dict.Add(1, data);
                dict.Add(2, ex.Message);

                return Json(Newtonsoft.Json.JsonConvert.SerializeObject(dict));
            }
        }




        //      https://localhost:44305/umbraco/Api/SwtpApi/PaypalSearchRecord?data=INCOMINGTEXT
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> PaypalSearchRecord(string data)
        {
            //Instantiate variables
            repoOrigPurchaseRecords = new OriginalPurchaseRecordsRepository(new EF_SwtpDb());


            //Obtain records 
            List<Original_PurchaseRecords> lstPurchaseRecords = repoOrigPurchaseRecords.GetAll_ByTxnId(data);


            //if (lstPurchaseRecords.Count > 0)
            //{
            //    for (int i = 0; i < lstPurchaseRecords.Count; i++)
            //    {
            //        try
            //        {
            //            //Attempt to get member data
            //            IMember member = new MemberController().GetMember_byEmail(extendSubscriptions.Email);

            //            if (member == null)
            //            {

            //            }
            //            else
            //            {

            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            Logger.Error<SwtpApiController>(ex);
            //            Logger.Error<SwtpApiController>(data);

            //        }
            //    }

            //}


            //Return data
            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(lstPurchaseRecords));
        }




        //      https://localhost:44305/umbraco/Api/SwtpApi/Subscriptions_SearchMember?data=INCOMINGTEXT
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]

        public JsonResult<string> PullExamResultsIntoMember(string data)
        {
            //Instantiate class for outbound data
            List<KeyValuePair> LstKeyValuePairs = new List<KeyValuePair>();


            try
            {
                //Attempt to get member data
                IMember _member = new blMemberController().GetMember_byEmail(data.Trim());


                if (_member == null)
                {
                    return null;
                }
                else
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    //TimeSpan timeTaken;
                    TimeSpan time1_5;
                    TimeSpan time2_5;
                    TimeSpan time3_5;
                    TimeSpan time4_5;
                    TimeSpan time5_5;
                    TimeSpan timeComplete;
                    sw.Start();



                    /*
                     * 
                     * OBTAIN ALL NEEDED DATA FROM DATABASE ===============================================================================
                     * 
                     */


                    //Instantiate repositories
                    EF_SwtpDb _context = new EF_SwtpDb(); //Reset context and repo to improve performance.
                    _context.Database.CommandTimeout = 172800; //2 days
                    var repoOriginalCmsPropertyData = new OriginalCmsPropertyDataRepository(_context);
                    var repoExamIdRelationships = new ExamIdRelationshipRepository(_context);
                    var repoOriginalUmbracoNode = new OriginalUmbracoNodeRepository(_context);
                    var repoOriginalMemberData = new OriginalMemberDataRepository(_context);
                    var repoExamRecords = new ExamRecordRepository(_context);
                    var repoExamAnswerSet = new ExamAnswerSetRepository(_context);
                    var repoExamAnswer = new ExamAnswerRepository(_context);
                    var repoExamMode = new ExamModeRepository(_context);
                    var repoPurchaseRecords = new PurchaseRecordRepository(_context);
                    var repoPurchaseRecordItems = new PurchaseRecordItemRepository(_context);


                    //Instantiate variables
                    Dictionary<int, string> DictExams = new Dictionary<int, string>();
                    List<ExamIDs_Old_New> LstExamIDs = repoExamIdRelationships.SelectAll_ExceptText(); //Get all exam IDs from relationship table
                    List<Original_UmbracoNode> LstAllOrigUmbNodes_lvl4 = repoOriginalUmbracoNode.SelectAll_Lvl4_ByEmail(_member.Email).ToList();
                    if (!LstAllOrigUmbNodes_lvl4.Any()) LstAllOrigUmbNodes_lvl4 = repoOriginalUmbracoNode.SelectAll_Lvl4_ByEmail(data.Trim()).ToList(); //search with casing given by user instead of acct.
                    List<Original_UmbracoNode> LstAllOrigUmbNodes_lvl5 = repoOriginalUmbracoNode.SelectAll_Lvl5().ToList();
                    List<Original_UmbracoNode> LstAllOrigUmbNodes_lvl6 = repoOriginalUmbracoNode.SelectAll_Lvl6().ToList();
                    List<Original_UmbracoNode> LstAllOrigUmbNodes_lvl7 = repoOriginalUmbracoNode.SelectAll_Lvl7().ToList();
                    List<Original_CmsPropertyData> LstCmsProperties = repoOriginalCmsPropertyData.SelectAll().ToList();
                    List<Original_MemberData> LstOriginalMemberData = repoOriginalMemberData.SelectAll();
                    List<Original_UmbracoNode> LstAllUpdatedRecords = new List<Original_UmbracoNode>();
                    List<bl.EF.ExamMode> LstExamModes = repoExamMode.SelectAll();
                    int counter = 0;
                    int error = 0;



                    //Get all paid exam names/IDs
                    IPublishedContent ipExamFolder = Umbraco.Content((int)(bl.Models.Common.SiteNode.Exams));
                    foreach (IPublishedContent ipExam in ipExamFolder.DescendantsOfType(bl.Models.Common.DocType.ExamPaid))
                        DictExams.Add(ipExam.Id, ipExam.Name);


                    //Split time to see where slowness is occuring
                    sw.Stop();
                    time1_5 = sw.Elapsed;
                    sw.Restart();



                    //Refresh all needed contexts
                    _context = new EF_SwtpDb(); //Reset context and repo to improve performance.
                    _context.Database.CommandTimeout = 172800; //2 days
                    repoExamRecords = new ExamRecordRepository(_context);
                    repoExamAnswerSet = new ExamAnswerSetRepository(_context);
                    repoExamAnswer = new ExamAnswerRepository(_context);



                    /*
                     * 
                     * IMPORT ALL MISSING EXAM RECORDS INTO MEMBER'S ACCOUNT ===============================================================================
                     * 
                     */



                    try
                    {
                        if (LstOriginalMemberData.Any(x => x.Email == _member.Email))
                        {
                            //Get original member id
                            int oldMemberId = LstOriginalMemberData.FirstOrDefault(x => x.Email == _member.Email).MemberId;


                            //Get list of original exam folder records for user (text contains both user email AND old member id.)
                            List<Original_UmbracoNode> lstLvl4Nodes_filtered = LstAllOrigUmbNodes_lvl4.Where(x => x.text.ToLower().Contains(_member.Email.ToLower()) && x.text.Contains(oldMemberId.ToString())).ToList();


                            foreach (Original_UmbracoNode lvl4Record in lstLvl4Nodes_filtered)
                            {
                                counter++;
                                LstAllUpdatedRecords.Add(lvl4Record); //Add to list for updating as added to site

                                //Get list of original exam records for lvl5 related to lvl4 record
                                List<Original_UmbracoNode> lstLvl5Nodes_filtered = LstAllOrigUmbNodes_lvl5.Where(x => x.path.Contains(lvl4Record.path + ",")).OrderBy(x => x.createDate).ToList();

                                foreach (Original_UmbracoNode lvl5Record in lstLvl5Nodes_filtered)
                                {
                                    try
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
                                            if (LstCmsProperties.Any(x => x.contentNodeId == lvl6Node_filtered.id && x.propertytypeid == 169))
                                            {
                                                try
                                                {
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
                                                catch (Exception ex3)
                                                {
                                                    Logger.Error<ApiMigrateDataController>(ex3);
                                                    Logger.Warn<ApiMigrateDataController>(Newtonsoft.Json.JsonConvert.SerializeObject(_member));
                                                    error++;
                                                }

                                            }
                                        }










                                        //Update all records as being added to site
                                        for (int i = 0; i < LstAllUpdatedRecords.Count(); i++)
                                        {
                                            LstAllUpdatedRecords[i].isAddedToSite = true;
                                        }
                                        _context = new EF_SwtpDb(); //Reset context and repo to improve performance.
                                        _context.Database.CommandTimeout = 172800; //2 days
                                        repoOriginalUmbracoNode = new OriginalUmbracoNodeRepository(_context);
                                        repoOriginalUmbracoNode.BulkUpdateRecord(LstAllUpdatedRecords);
                                        LstAllUpdatedRecords.Clear();






                                    }
                                    catch (Exception ex2)
                                    {
                                        Logger.Error<ApiMigrateDataController>(ex2);
                                        Logger.Warn<ApiMigrateDataController>(Newtonsoft.Json.JsonConvert.SerializeObject(_member));
                                        error++;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //Logger.Warn<ApiMigrateDataController>(Newtonsoft.Json.JsonConvert.SerializeObject(_member));
                            error++;
                        }

                        //EXAMPLE USER
                        //adivinestrategy@gmail.com - 15524391
                        //user:		adivinestrategy@gmail.com
                        //member id:	23843		
                        //ExamID		7670554	[+]
                        //exam folder id:		15524392	Exam Member
                        //						15525751 [+] Exam Score
                        //							15525752	Answer Folder
                        //								15525763	Answer

                    }
                    catch (Exception ex)
                    {
                        Logger.Error<ApiMigrateDataController>(ex);
                        Logger.Warn<ApiMigrateDataController>(Newtonsoft.Json.JsonConvert.SerializeObject(_member));
                        error++;
                    }




                    //Split time to see where slowness is occuring
                    sw.Stop();
                    time2_5 = sw.Elapsed;
                    sw.Restart();





                    /*
                     * 
                     * MARK ALL IMPORTED RECORDS AS BEING ADDED TO SITE ===============================================================================
                     * 
                     */



                    //Update all records as being added to site
                    for (int i = 0; i < LstAllUpdatedRecords.Count(); i++)
                    {
                        LstAllUpdatedRecords[i].isAddedToSite = true;
                    }
                    _context = new EF_SwtpDb(); //Reset context and repo to improve performance.
                    _context.Database.CommandTimeout = 172800; //2 days
                    repoOriginalUmbracoNode = new OriginalUmbracoNodeRepository(_context);
                    repoOriginalUmbracoNode.BulkUpdateRecord(LstAllUpdatedRecords);




                    /*
                     * 
                     * REMOVE ALL DUPLICATE EXAMS FROM MEMBER'S ACCOUNT ===============================================================================
                     * 
                     */


                    List<ExamRecord> LstExams = repoExamRecords.GetAll_ByMemberId(_member.Id);
                    List<ExamRecord> LstRemoveFromMember = new List<ExamRecord>();

                    if (LstExams != null && LstExams.Count() > 1)
                    {
                        //Loop through exams from the beginning
                        for (int i = 0; i < LstExams.Count(); i++)
                        {
                            //Only run if not null (just in case...)
                            if (LstExams[i].MemberId != null)
                            {
                                //Loop through exams from the end
                                for (int x = LstExams.Count() - 1; x > i; x--)
                                {
                                    if (i != x) //Ensure we aren't comparing same record
                                    {
                                        //Only run if not null (just in case...)
                                        if (LstExams[x].MemberId != null)
                                        {
                                            //Delete member from record if bother are duplicates.
                                            if (LstExams[x].CreatedDate == LstExams[i].CreatedDate)
                                            {
                                                if (LstExams[x].ExamId == LstExams[i].ExamId)
                                                {
                                                    LstExams[x].MemberId = null;
                                                    LstRemoveFromMember.Add(LstExams[x]);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //Remove any records in list from member
                    if (LstRemoveFromMember.Any())
                    {
                        repoExamRecords.BulkUpdateRecord(LstRemoveFromMember);
                    }



                    //Split time to see where slowness is occuring
                    sw.Stop();
                    time3_5 = sw.Elapsed;
                    sw.Restart();


                    /*
                     * 
                     * ADD ALL MISSING ANSWERS TO ANSWERSETS ===============================================================================
                     * 
                     */



                    //Get list of answersets for member
                    HashSet<int> lstExamRecordIDs = repoExamRecords.GetAllExamRecordIds_ByMemberId(_member.Id);
                    List<ExamAnswerSet> LstAnswerSets = new List<ExamAnswerSet>();
                    foreach (var _id in lstExamRecordIDs)
                    {
                        LstAnswerSets.Add(repoExamAnswerSet.GetRecord_ByExamRecordId(_id));
                    }


                    List<KeyValuePair> LstAnswerKeys = repoExamAnswer.GetAll_ExamAnswerSetId_QuestionId();  // Key=ExamAnswerSetId  Value=QuestionId
                    List<ExamAnswer> LstExamAnswersToAdd = new List<ExamAnswer>();
                    Random rnd = new Random();
                    int _totalAdded = 0;


                    //Create missing answers for each exam.
                    foreach (ExamAnswerSet _answerSet in LstAnswerSets)
                    {
                        try
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
                        catch (Exception ex5)
                        {
                            Logger.Error<SwtpApiController>(ex5);
                            Logger.Error<SwtpApiController>(Newtonsoft.Json.JsonConvert.SerializeObject(_answerSet));
                        }

                    }


                    //Add all missing records
                    repoExamAnswer.BulkAddRecord(LstExamAnswersToAdd);


                    //Split time to see where slowness is occuring
                    sw.Stop();
                    time4_5 = sw.Elapsed;
                    sw.Restart();


                    /*
                    * 
                    * ADD MISSING PURCHASES  ===============================================================================
                    * 
                    */


                    //Instantiate variables
                    List<bl.EF.PurchaseRecord> LstPurchaseRecords = repoPurchaseRecords.SelectAll_ByMemberId(_member.Id);
                    List<PurchaseRecordItem> LstPurchaseRecordItems = repoPurchaseRecordItems.ObtainRecords_byMemberId(_member.Id);
                    List<PurchaseRecordItem> LstPurchaseRecordItems_TempFromParses = new List<PurchaseRecordItem>();
                    List<ExamRecord> LstExamRecords = repoExamRecords.GetAll_ByMemberId(_member.Id);
                    int PurchasedExamsAdded = 0;
                    int PurchasedExamsUpdated = 0;
                    int ExamsUpdated = 0;


                    //Parse metadata into temp list of purchase records
                    foreach (bl.EF.PurchaseRecord _purchaseRecord in LstPurchaseRecords)
                    {
                        ProductMeta productMeta = null;
                        try
                        {
                            //Deserialize meta
                            productMeta = Newtonsoft.Json.JsonConvert.DeserializeObject<ProductMeta>(_purchaseRecord.Metadata);
                        }
                        catch
                        {
                            //Serialize 1st, then deserialize properly [fixes some issues]
                            string metaSerialized = Newtonsoft.Json.JsonConvert.SerializeObject(_purchaseRecord.Metadata);
                            productMeta = Newtonsoft.Json.JsonConvert.DeserializeObject<ProductMeta>(metaSerialized);
                        }


                        if (productMeta != null)
                        {
                            //Create purchased items and add to temp list
                            if (productMeta.ExamCount == "1")
                            {
                                //Extract meta data
                                var _strExamId = productMeta.ExamId;
                                var _strExamTitle = productMeta.ExamTitle;
                                var _strOriginalPrice = productMeta.OriginalPrice;

                                //If data is in a json array, parse
                                if (_strExamId.Contains("[")) _strExamId = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(productMeta.ExamId).FirstOrDefault();
                                if (_strExamTitle.Contains("[")) _strExamTitle = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(productMeta.ExamTitle).FirstOrDefault();
                                if (_strOriginalPrice.Contains("[")) _strOriginalPrice = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(productMeta.OriginalPrice).FirstOrDefault();

                                //Create purchased item and add to list
                                LstPurchaseRecordItems_TempFromParses.Add(new PurchaseRecordItem()
                                {
                                    PurchaseRecordId = _purchaseRecord.PurchaseRecordId,
                                    MemberId = _purchaseRecord.MemberId,
                                    ExamId = Convert.ToInt32(_strExamId),
                                    ExamTitle = _strExamTitle,
                                    OriginalPrice = Convert.ToDecimal(_strOriginalPrice),
                                    ExpirationDate = DateTime.Today.AddDays(90)
                                });
                            }
                            else
                            {
                                //Split array of items into lists
                                List<string> lstExamIDs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(productMeta.ExamId);
                                List<string> lstExamTitles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(productMeta.ExamTitle);
                                List<string> lstOriginalPrices = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(productMeta.OriginalPrice);

                                //Create each purchased item and add to list
                                for (var i = 0; i < lstExamIDs.Count(); i++)
                                {
                                    //Extract meta data
                                    var _strExamId = lstExamIDs[i];
                                    var _strExamTitle = lstExamTitles[i];
                                    var _strOriginalPrice = lstOriginalPrices[i];

                                    //If data is in a json array, parse
                                    if (_strExamId.Contains("[")) _strExamId = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(productMeta.ExamId).FirstOrDefault();
                                    if (_strExamTitle.Contains("[")) _strExamTitle = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(productMeta.ExamTitle).FirstOrDefault();
                                    if (_strOriginalPrice.Contains("[")) _strOriginalPrice = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(productMeta.OriginalPrice).FirstOrDefault();

                                    //Create purchased item and add to list
                                    LstPurchaseRecordItems_TempFromParses.Add(new PurchaseRecordItem()
                                    {
                                        PurchaseRecordId = _purchaseRecord.PurchaseRecordId,
                                        MemberId = _purchaseRecord.MemberId,
                                        ExamId = Convert.ToInt32(_strExamId),
                                        ExamTitle = _strExamTitle,
                                        OriginalPrice = Convert.ToDecimal(_strOriginalPrice),
                                        ExpirationDate = DateTime.Today.AddDays(90)
                                    });
                                }
                            }
                        }
                    }


                    //Get missing PurchaseRecordIDs and update db
                    foreach (PurchaseRecordItem _purchaseRecordItem in LstPurchaseRecordItems)
                    {
                        //Does purchase record id not exist?
                        if (_purchaseRecordItem.PurchaseRecordId == null)
                        {
                            //First try to match exact record, but if not then get closest with same exam id
                            //if (LstPurchaseRecordItems_TempFromParses.Any(x => x.ExamId == _purchaseRecordItem.ExamId && x.OriginalPrice == _purchaseRecordItem.OriginalPrice))
                            //{
                            //    _purchaseRecordItem.PurchaseRecordId =
                            //        LstPurchaseRecordItems_TempFromParses.FirstOrDefault(x => x.ExamId == _purchaseRecordItem.ExamId && x.OriginalPrice == _purchaseRecordItem.OriginalPrice).PurchaseRecordId;
                            //    PurchasedExamsUpdated++;
                            //}
                            //else if (LstPurchaseRecordItems_TempFromParses.Any(x => x.ExamId == _purchaseRecordItem.ExamId))
                            //{
                            //    _purchaseRecordItem.PurchaseRecordId =
                            //        LstPurchaseRecordItems_TempFromParses.FirstOrDefault(x => x.ExamId == _purchaseRecordItem.ExamId).PurchaseRecordId;
                            //    PurchasedExamsUpdated++;
                            //}

                            PurchaseRecordItem item = repoPurchaseRecordItem.ObtainRecord_byMemberId_ExamId(_purchaseRecordItem.MemberId, _purchaseRecordItem.ExamId);
                            if (item != null)
                                _purchaseRecordItem.PurchaseRecordId = item.PurchaseRecordId;

                        }
                    }
                    repoPurchaseRecordItems.BulkUpdateRecord(LstPurchaseRecordItems);


                    //Add purchase records if missing
                    foreach (PurchaseRecordItem _tempItem in LstPurchaseRecordItems_TempFromParses)
                    {
                        if (!LstPurchaseRecordItems.Any(x => x.ExamId == _tempItem.ExamId && x.PurchaseRecordId == _tempItem.PurchaseRecordId))
                        {
                            repoPurchaseRecordItems.AddRecord(_tempItem);
                            PurchasedExamsAdded++;
                        }
                    }


                    //Update existing exams that are missing purchase record IDs
                    LstPurchaseRecordItems.Clear();
                    LstPurchaseRecordItems = repoPurchaseRecordItems.ObtainRecords_byMemberId(_member.Id);
                    foreach (ExamRecord _examRecord in LstExamRecords)
                    {
                        if (_examRecord.PurchaseRecordId == null)
                        {
                            if (LstPurchaseRecordItems.Any(x => x.ExamId == _examRecord.ExamId && x.PurchaseRecordId != null))
                            {
                                _examRecord.PurchaseRecordId = LstPurchaseRecordItems.FirstOrDefault(x => x.ExamId == _examRecord.ExamId && x.PurchaseRecordId != null).PurchaseRecordId;
                                repoExamRecords.UpdateRecord(_examRecord);
                                ExamsUpdated++;
                            }
                        }
                    }






                    //Split time to see where slowness is occuring
                    sw.Stop();
                    time5_5 = sw.Elapsed;
                    timeComplete = time5_5 + time4_5 + time3_5 + time2_5 + time1_5;





                    /*
                     * 
                     * SHOW RESULTS ===============================================================================
                     * 
                     */


                    //Add results to outpout
                    KeyValuePair keyValuePair = new KeyValuePair();
                    keyValuePair.StrKey = "Exam Records Pulled In";
                    keyValuePair.StrValue = counter.ToString();
                    LstKeyValuePairs.Add(keyValuePair);

                    keyValuePair = new KeyValuePair();
                    keyValuePair.StrKey = "Exam Record Errors";
                    keyValuePair.StrValue = error.ToString();
                    LstKeyValuePairs.Add(keyValuePair);

                    keyValuePair = new KeyValuePair();
                    keyValuePair.StrKey = "Duplicate Exams Deleted";
                    keyValuePair.StrValue = LstRemoveFromMember.Count().ToString();
                    LstKeyValuePairs.Add(keyValuePair);

                    keyValuePair = new KeyValuePair();
                    keyValuePair.StrKey = "Missing Answers Added";
                    keyValuePair.StrValue = _totalAdded.ToString();
                    LstKeyValuePairs.Add(keyValuePair);

                    keyValuePair = new KeyValuePair();
                    keyValuePair.StrKey = "Purchased Exams Added";
                    keyValuePair.StrValue = PurchasedExamsAdded.ToString();
                    LstKeyValuePairs.Add(keyValuePair);

                    keyValuePair = new KeyValuePair();
                    keyValuePair.StrKey = "Purchased Exams Updated with Purchase Id";
                    keyValuePair.StrValue = PurchasedExamsUpdated.ToString();
                    LstKeyValuePairs.Add(keyValuePair);

                    keyValuePair = new KeyValuePair();
                    keyValuePair.StrKey = "Exams Updated with Purchase Id";
                    keyValuePair.StrValue = ExamsUpdated.ToString();
                    LstKeyValuePairs.Add(keyValuePair);

                    keyValuePair = new KeyValuePair();
                    keyValuePair.StrKey = "-------------------";
                    keyValuePair.StrValue = "------------------- ";
                    LstKeyValuePairs.Add(keyValuePair);

                    keyValuePair = new KeyValuePair();
                    keyValuePair.StrKey = "Time to Pull Data";
                    keyValuePair.StrValue = time1_5.ToString(@"hh\:mm\:ss");
                    LstKeyValuePairs.Add(keyValuePair);

                    keyValuePair = new KeyValuePair();
                    keyValuePair.StrKey = "Time to Add Missing Exams";
                    keyValuePair.StrValue = time2_5.ToString(@"hh\:mm\:ss");
                    LstKeyValuePairs.Add(keyValuePair);

                    keyValuePair = new KeyValuePair();
                    keyValuePair.StrKey = "Time to Remove Duplicate Exams";
                    keyValuePair.StrValue = time3_5.ToString(@"hh\:mm\:ss");
                    LstKeyValuePairs.Add(keyValuePair);

                    keyValuePair = new KeyValuePair();
                    keyValuePair.StrKey = "Time to Add Missing Questions";
                    keyValuePair.StrValue = time4_5.ToString(@"hh\:mm\:ss");
                    LstKeyValuePairs.Add(keyValuePair);

                    keyValuePair = new KeyValuePair();
                    keyValuePair.StrKey = "Time to Add Missing Purchases";
                    keyValuePair.StrValue = time5_5.ToString(@"hh\:mm\:ss");
                    LstKeyValuePairs.Add(keyValuePair);

                    keyValuePair = new KeyValuePair();
                    keyValuePair.StrKey = "Total Time to Completed";
                    keyValuePair.StrValue = timeComplete.ToString(@"hh\:mm\:ss");
                    LstKeyValuePairs.Add(keyValuePair);

                }
            }
            catch (DbEntityValidationException ev)
            {
                Logger.Error<SwtpApiController>(ev);
                Logger.Error<SwtpApiController>(data);
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error<SwtpApiController>(ex);
                Logger.Error<SwtpApiController>(data);
                return null;
            }

            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(LstKeyValuePairs));
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




        //      STRIPE WEBHOOK
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public void StripeWebhook()
        {
            try
            {
                //Obtain keys from web.config
                string StripeEndpointSecret = ConfigurationManager.AppSettings[bl.Models.Common.Misc.StripeEndpointSecret];
                StripeConfiguration.ApiKey = ConfigurationManager.AppSettings[bl.Models.Common.Misc.StripeApiKey];


                //Extract incoming data from stripe.
                string jsonMsg = new StreamReader(HttpContext.Current.Request.InputStream).ReadToEnd();


                //Convert stripe data to class
                Event stripeEvent = EventUtility.ConstructEvent(
                    jsonMsg,
                    HttpContext.Current.Request.Headers[bl.Models.Common.Misc.StripeSignature],
                    StripeEndpointSecret);


                // Handle the event
                if (stripeEvent.Type == Events.CheckoutSessionCompleted)
                {
                    //Extract metadata from event
                    Logger.Warn<SwtpApiController>("Incoming Checkout from Stripe");
                    Logger.Warn<SwtpApiController>(Newtonsoft.Json.JsonConvert.SerializeObject(stripeEvent));

                    Stripe.Checkout.Session session = (Stripe.Checkout.Session)stripeEvent.Data.Object;
                    //Dictionary<string, string> metadata = session.Metadata;

                    //Record in log only if logger is set to Warning.
                    Logger.Warn<SwtpApiController>(Newtonsoft.Json.JsonConvert.SerializeObject(session.Metadata));

                    //Convert dictionary to object.
                    string metaSerialized = Newtonsoft.Json.JsonConvert.SerializeObject(session.Metadata);
                    ProductMeta productMeta = Newtonsoft.Json.JsonConvert.DeserializeObject<ProductMeta>(metaSerialized);







                    //Create purchase record and add to database
                    bl.EF.PurchaseRecord purchaseRecord = new bl.EF.PurchaseRecord();
                    try
                    {
                        purchaseRecord.PurchaseDate = DateTime.Now;
                        if (!string.IsNullOrEmpty(productMeta.SubmitType)) purchaseRecord.PurchaseTypeId = repoPurchaseType.ObtainId_byType(productMeta.SubmitType);
                        if (!string.IsNullOrEmpty(productMeta.MemberId)) purchaseRecord.MemberId = Convert.ToInt32(productMeta.MemberId);
                        if (!string.IsNullOrEmpty(productMeta.BundleId)) purchaseRecord.BundleId = Convert.ToInt32(productMeta.BundleId);
                        if (!string.IsNullOrEmpty(productMeta.BundleTitle)) purchaseRecord.BundleTitle = productMeta.BundleTitle;
                        if (!string.IsNullOrEmpty(productMeta.CouponCode))
                        {
                            purchaseRecord.CouponId = repoCoupon.ObtainId_byCode(productMeta.CouponCode);
                            repoCoupon.IncrementTimesUsed(productMeta.CouponCode);  //Update coupon used counter
                        }
                        if (!string.IsNullOrEmpty(productMeta.BundleDiscount)) purchaseRecord.BundleDiscount = Convert.ToDecimal(productMeta.BundleDiscount);
                        if (!string.IsNullOrEmpty(productMeta.CouponDiscount)) purchaseRecord.CouponDiscount = Convert.ToDecimal(productMeta.CouponDiscount);
                        if (!string.IsNullOrEmpty(productMeta.TotalCost)) purchaseRecord.TotalCost = Convert.ToDecimal(productMeta.TotalCost);
                        if (!string.IsNullOrEmpty(productMeta.TotalDiscount)) purchaseRecord.TotalDiscount = Convert.ToDecimal(productMeta.TotalDiscount);
                        purchaseRecord.Metadata = Newtonsoft.Json.JsonConvert.SerializeObject(session.Metadata);
                        purchaseRecord.StripeResponse = Newtonsoft.Json.JsonConvert.SerializeObject(stripeEvent);
                        repoPurchaseRecord.AddRecord(purchaseRecord);
                    }
                    catch (Exception ex1)
                    {
                        Logger.Error<SwtpApiController>("StripeWebhook() Error creating PurchaseRecord. Data: " + Newtonsoft.Json.JsonConvert.SerializeObject(purchaseRecord));
                        Logger.Error<SwtpApiController>(ex1);
                        throw;
                    }



                    if (productMeta.ExamCount == "1")
                    {

                        try
                        {
                            //Extract meta data
                            var _strExamId = productMeta.ExamId;
                            var _strExamTitle = productMeta.ExamTitle;
                            var _strOriginalPrice = productMeta.OriginalPrice;

                            //If data is in a json array, parse
                            if (_strExamId.Contains("[")) _strExamId = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(productMeta.ExamId).FirstOrDefault();
                            if (_strExamTitle.Contains("[")) _strExamTitle = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(productMeta.ExamTitle).FirstOrDefault();
                            if (_strOriginalPrice.Contains("[")) _strOriginalPrice = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(productMeta.OriginalPrice).FirstOrDefault();

                            //Create purchased item and add to database
                            PurchaseRecordItem purchaseRecordItem = new PurchaseRecordItem
                            {
                                PurchaseRecordId = purchaseRecord.PurchaseRecordId,
                                MemberId = purchaseRecord.MemberId,
                                ExamId = Convert.ToInt32(_strExamId),
                                ExamTitle = _strExamTitle,
                                OriginalPrice = Convert.ToDecimal(_strOriginalPrice),
                                ExpirationDate = DateTime.Today.AddDays(90)
                            };
                            repoPurchaseRecordItem.AddRecord(purchaseRecordItem);
                        }
                        catch (Exception ex2)
                        {
                            Logger.Error<SwtpApiController>("StripeWebhook() Error creating single purchase item. PurchaseRecord: "
                                + Newtonsoft.Json.JsonConvert.SerializeObject(purchaseRecord)
                                + "  |  MetaData: " + Newtonsoft.Json.JsonConvert.SerializeObject(productMeta));
                            Logger.Error<SwtpApiController>(ex2);
                            throw;
                        }

                    }
                    else
                    {
                        List<string> lstExamIDs = new List<string>();
                        List<string> lstExamTitles = new List<string>();
                        List<string> lstOriginalPrices = new List<string>();
                        try
                        {
                            //Split array of items into lists
                            lstExamIDs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(productMeta.ExamId);
                            lstExamTitles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(productMeta.ExamTitle);
                            lstOriginalPrices = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(productMeta.OriginalPrice);

                            //Create each purchased item and add to database
                            for (var i = 0; i < lstExamIDs.Count(); i++)
                            {
                                //Extract meta data
                                var _strExamId = lstExamIDs[i];
                                var _strExamTitle = lstExamTitles[i];
                                var _strOriginalPrice = lstOriginalPrices[i];

                                //If data is in a json array, parse
                                if (_strExamId.Contains("[")) _strExamId = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(productMeta.ExamId).FirstOrDefault();
                                if (_strExamTitle.Contains("[")) _strExamTitle = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(productMeta.ExamTitle).FirstOrDefault();
                                if (_strOriginalPrice.Contains("[")) _strOriginalPrice = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(productMeta.OriginalPrice).FirstOrDefault();


                                PurchaseRecordItem purchaseRecordItem = new PurchaseRecordItem
                                {
                                    PurchaseRecordId = purchaseRecord.PurchaseRecordId,
                                    MemberId = purchaseRecord.MemberId,
                                    ExamId = Convert.ToInt32(_strExamId),
                                    ExamTitle = _strExamTitle,
                                    OriginalPrice = Convert.ToDecimal(_strOriginalPrice),
                                    ExpirationDate = DateTime.Today.AddDays(90)
                                };
                                repoPurchaseRecordItem.AddRecord(purchaseRecordItem);
                            }
                        }
                        catch (Exception ex3)
                        {
                            Logger.Error<SwtpApiController>("StripeWebhook() Error creating multiple purchase items. PurchaseRecord: "
                                + Newtonsoft.Json.JsonConvert.SerializeObject(purchaseRecord)
                                + "  |  lstExamIDs: " + Newtonsoft.Json.JsonConvert.SerializeObject(lstExamIDs)
                                + "  |  lstExamTitles: " + Newtonsoft.Json.JsonConvert.SerializeObject(lstExamTitles)
                                + "  |  lstOriginalPrices: " + Newtonsoft.Json.JsonConvert.SerializeObject(lstOriginalPrices));
                            Logger.Error<SwtpApiController>(ex3);
                            throw;
                        }
                    }
                }
                else
                {
                    //Ignore all other incoming responses from Stripe
                    //Logger.Warn<SwtpApiController>("Unhandled event type: {0}", stripeEvent.Type);
                }


                //Return status code as ok
                HttpContext.Current.Response.ContentType = "text/plain";
                HttpContext.Current.Response.StatusCode = 200;
            }
            catch (StripeException e)
            {
                Logger.Error<SwtpApiController>(e);

                //Return status code as error
                HttpContext.Current.Response.ContentType = "text/plain";
                HttpContext.Current.Response.StatusCode = 500;
            }
            catch (Exception ex)
            {
                Logger.Error<SwtpApiController>(ex);

                //Return status code as error
                HttpContext.Current.Response.ContentType = "text/plain";
                HttpContext.Current.Response.StatusCode = 500;
            }
        }
        #endregion









        #region "Test APIs"

        //  /umbraco/Api/SwtpApi/StripeApiTest
        //  https://www.youtube.com/watch?v=_u3of33pZa4
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> StripeApiTest()
        {
            StripeConfiguration.ApiKey = ConfigurationManager.AppSettings[bl.Models.Common.Misc.StripeApiKey];

            var options = new ProductListOptions
            {
                Limit = 3,
            };
            var service = new ProductService();
            StripeList<Product> products = service.List(options);

            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(products));
        }

        //  /umbraco/Api/SwtpApi/ApiTest
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> ApiTest()
        {
            return Json("API reached!!!");
        }

        //  /umbraco/Api/SwtpApi/ApiTest?data=data
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> ApiTest(string data)
        {
            return Json("API reached!!!  " + data);
        }

        //  /umbraco/Api/SwtpApi/ApiTestWithDatastream
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost]
        public JsonResult<string> ApiTestWithDatastream() //Data passed in ajax using data: [json] section!!!
        {
            //Extract incoming data from datastream.
            string jsonMsg = new StreamReader(HttpContext.Current.Request.InputStream).ReadToEnd();

            //Return data for testing
            return Json("API reached!!!  Json data: " + jsonMsg);
        }

        #endregion
    }
}