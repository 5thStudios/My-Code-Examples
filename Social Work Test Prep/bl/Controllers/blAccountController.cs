using bl.EF;
using bl.Models;
using Repositories;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.PublishedModels;
using cm = Umbraco.Web.PublishedModels;




namespace bl.Controllers
{
    public class blAccountController : SurfaceController
    {
        #region "Properties"
        private EF_SwtpDb _context;
        private IExamAnswerRepository repoExamAnswer;
        private IExamAnswerSetRepository repoExamAnswerSet;
        private IExamModeRepository repoExamMode;
        private IExamRecordRepository repoExamRecord;
        private IPurchaseRecordRepository repoPurchaseRecord;
        private IPurchaseRecordItemRepository repoPurchaseRecordItem;

        public blAccountController()
        {
            _context = new EF_SwtpDb();
            repoExamAnswer = new ExamAnswerRepository(_context);
            repoExamAnswerSet = new ExamAnswerSetRepository(_context);
            repoExamMode = new ExamModeRepository(_context);
            repoExamRecord = new ExamRecordRepository(_context);
            repoPurchaseRecord = new PurchaseRecordRepository(_context);
            repoPurchaseRecordItem = new PurchaseRecordItemRepository(_context);
        }
        #endregion

        #region "Renders"
        public ActionResult RenderPurchaseForm(cm.AccountManagement cmModel = null)
        {
            //Instantiate variables
            List<bl.Models.Region> lstRegions = new List<Region>();
            if (cmModel == null)
                cmModel = new cm.AccountManagement(Umbraco.Content((int)bl.Models.Common.SiteNode.Account));

            try
            {
                //
                foreach (var ipRregion in cmModel.Regions)
                {
                    //
                    cm.Offer cmRegion = new Offer(ipRregion);
                    Region region = new Region();

                    //
                    region.Id = cmRegion.Id;
                    region.Title = cmRegion.Name;
                    region.BundleDiscount = cmRegion.BundleDiscount;

                    foreach (var ipExam in cmRegion.Exams)
                    {
                        //
                        cm.ExamPaid cmExam = new cm.ExamPaid(ipExam);

                        //
                        bl.Models.Exam exam = new Models.Exam();

                        exam.Id = cmExam.Id;
                        exam.Price = cmExam.Price;
                        exam.Title = cmExam.Name;

                        region.TotalCost += cmExam.Price;

                        region.LstExams.Add(exam);
                    }

                    lstRegions.Add(region);
                }
            }
            catch (Exception ex)
            {
                Logger.Error<blAccountController>(ex);
            }


            //return null;
            return PartialView(Models.Common.PartialPath.Account_PurchaseExams, lstRegions);
        }
        public ActionResult RenderPurchaseHistory()
        {
            //Instantiate variables
            List<bl.Models.PurchaseRecord> lstPurchaseRecords = new List<bl.Models.PurchaseRecord>();


            try
            {
                //Get member 
                IPublishedContent ipMember = Members.GetCurrentMember();


                //Get list of all purchases by member
                List<PurchaseRecordItem> lstPurchaseRecordItem = new List<PurchaseRecordItem>();
                if (ipMember != null && ipMember.Id > 0)
                {
                    lstPurchaseRecordItem = repoPurchaseRecordItem.ObtainRecords_byMemberId(ipMember.Id);
                }



                foreach (PurchaseRecordItem exam in lstPurchaseRecordItem)
                {
                    //Obtain purchase record
                    bl.Models.PurchaseRecord purchaseRecord = new bl.Models.PurchaseRecord();
                    purchaseRecord.Id = exam.PurchaseRecordItemId;
                    purchaseRecord.Title = exam.ExamTitle;
                    purchaseRecord.ExpirationDate = exam.ExpirationDate;

                    if (exam.PurchaseRecordId != null)
                    {
                        bl.EF.PurchaseRecord record = repoPurchaseRecord.Obtain_byId((int)exam.PurchaseRecordId);
                        if (record.PurchaseDate != null)
                        {
                            purchaseRecord.PurchaseDate = record.PurchaseDate;
                        }
                    }


                    //Create subject line for email
                    StringBuilder sbEmailSubjectText = new StringBuilder();
                    sbEmailSubjectText.Append("mailto:info@socialworktestprep.com?subject=");
                    sbEmailSubjectText.Append("Extension request for:  ");
                    sbEmailSubjectText.Append(exam.ExamTitle);
                    sbEmailSubjectText.Append("  [");
                    sbEmailSubjectText.Append(exam.ExamId);
                    sbEmailSubjectText.Append("]  |  Member:  ");
                    sbEmailSubjectText.Append(ipMember.Name);
                    sbEmailSubjectText.Append("  [");
                    sbEmailSubjectText.Append(ipMember.Id);
                    sbEmailSubjectText.Append("]");
                    purchaseRecord.ExtensionRequest = System.Net.WebUtility.HtmlEncode(sbEmailSubjectText.ToString());

                    lstPurchaseRecords.Add(purchaseRecord);
                }
            }
            catch (Exception ex)
            {
                Logger.Error<blAccountController>(ex);
            }


            //return null;
            return PartialView(Models.Common.PartialPath.Account_PurchaseHistory, lstPurchaseRecords);
        }
        public ActionResult RenderAccountLinks()
        {
            //Instantiate variables
            cm.AccountManagement cmModel = new cm.AccountManagement(Umbraco.Content((int)bl.Models.Common.SiteNode.Account));
            List<Umbraco.Web.Models.Link> lstCurrentLinks = cmModel.UserLinks.ToList();
            List<bl.Models.Link> lstNewLinks = new List<Link>();

            try
            {
                for (int i = 0; i < lstCurrentLinks.Count(); i++)
                {
                    //Create for primary list
                    bl.Models.Link lnk = new bl.Models.Link(lstCurrentLinks[i].Name, lstCurrentLinks[i].Url);

                    //If the next link has a querystring add as child link and skip from primary list.
                    if (i + 2 <= lstCurrentLinks.Count())
                    {
                        if (lstCurrentLinks[i + 1].Url.Contains("?SavedAndCompleted=true"))
                        {
                            i++;

                            //Only show link if user has saved exams
                            if (ShowLinkForSavedCompleted())
                            {
                                lnk.LstChildLinks = new List<Link>();
                                lnk.LstChildLinks.Add(new bl.Models.Link(lstCurrentLinks[i].Name, lstCurrentLinks[i].Url.Replace("?SavedAndCompleted=true", "")));
                            }
                        }
                    }

                    //Add to primary list
                    lstNewLinks.Add(lnk);
                }
            }
            catch (Exception ex)
            {
                Logger.Error<blAccountController>(ex);
            }


            //return null;
            return PartialView(Models.Common.PartialPath.Account_AccountLinks, lstNewLinks);
        }
        #endregion

        #region "Submits"
        [ValidateAntiForgeryToken]
        public ActionResult SubmitExamPurchase_Single(bl.Models.Region Model, string submit)
        {
            if (!User.Identity.IsAuthenticated)
            {
                //Redirect to login page.
                ModelState.AddModelError("", "*Please log into your account.");
                return RedirectToUmbracoPage((int)bl.Models.Common.SiteNode.Login);
            }
            else if (!Model.LstExams.Where(x => x.IsSelected).Any())
            {
                //No exam is selected.
                ModelState.AddModelError("", "*Please select an exam to purchase.");
                return CurrentUmbracoPage();
            }
            else
            {
                //Get selected exam
                bl.Models.Exam exam = Model.LstExams.Where(x => x.IsSelected).FirstOrDefault();


                //Create metadata class
                ProductMeta productMeta = new ProductMeta();
                //      ...Data
                productMeta.ExamCount = "1";
                productMeta.BundleId = Model.Id.ToString();
                productMeta.BundleTitle = Model.Title;
                productMeta.ExamId = exam.Id.ToString();
                productMeta.ExamTitle = exam.Title;
                productMeta.MemberId = Members.GetCurrentMemberId().ToString();
                productMeta.SubmitType = submit;
                //      ...Monetary
                productMeta.CouponCode = Model.CouponCode;
                productMeta.OriginalPrice = exam.Price.ToString("0.00");
                productMeta.BundleDiscount = Model.BundleDiscount.ToString("0.00");
                productMeta.CouponDiscount = Model.CouponDiscount.ToString("0.00");
                productMeta.TotalDiscount = Model.TotalDiscount.ToString("0.00");
                productMeta.TotalCost = Model.TotalCost.ToString("0.00");


                //Convert metadata to dictionary
                Dictionary<string, string> metaDict = productMeta.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .ToDictionary(prop => prop.Name, prop => (string)prop.GetValue(productMeta, null));


                //Add caption if coupon is applied
                string caption = "";
                if (!string.IsNullOrEmpty(productMeta.CouponCode))
                    caption = " - Coupon applied";


                //Create stripe product with custom price to submit.
                ProductCreateOptions productCreateOptions = new ProductCreateOptions
                {
                    Name = submit + " exam purchase for " + Model.Title + caption,
                    Description = exam.Title + caption,
                    Metadata = metaDict,
                    DefaultPriceData = new ProductDefaultPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = Convert.ToInt64(Model.TotalCost * 100),
                    },
                };


                ProductService service = new ProductService();
                Product product = service.Create(productCreateOptions);


                //Create a stripe session with created product
                SessionCreateOptions sessionOptions = new SessionCreateOptions
                {
                    LineItems = new List<SessionLineItemOptions>
                    {
                      new SessionLineItemOptions
                      {
                        Price = product.DefaultPriceId,
                        Quantity = 1,
                      },
                    },
                    Mode = "payment",
                    SuccessUrl = Umbraco.Content((int)bl.Models.Common.SiteNode.Account_ThankYou).Url(mode: UrlMode.Absolute),
                    CancelUrl = Umbraco.Content((int)bl.Models.Common.SiteNode.Account).Url(mode: UrlMode.Absolute),
                    Metadata = metaDict,
                    ClientReferenceId = Members.GetCurrentMemberId().ToString(),
                    CustomerEmail = Members.GetCurrentMember().Name
                };
                SessionService sessionService = new SessionService();
                Session session = sessionService.Create(sessionOptions);


                //Redirects to the checkout page.
                Response.Headers.Add("Location", session.Url);
                return new HttpStatusCodeResult(303);

            }
        }

        [ValidateAntiForgeryToken]
        public ActionResult SubmitExamPurchase_Bundle(bl.Models.Region Model, string submit)
        {
            if (!User.Identity.IsAuthenticated)
            {
                //Redirect to login page.
                ModelState.AddModelError("", "*Please log into your account.");
                return RedirectToUmbracoPage((int)bl.Models.Common.SiteNode.Login);
            }
            else if (!Model.LstExams.Where(x => x.IsSelected).Any())
            {
                //No exam is selected.
                ModelState.AddModelError("", "*Please select an exam to purchase.");
                return CurrentUmbracoPage();
            }
            else
            {
                //Get selected exam
                List<bl.Models.Exam> lstExam = Model.LstExams.Where(x => x.IsSelected).ToList();


                //List of data for metadata
                List<string> lstExamIDs = new List<string>();
                List<string> lstExamTitles = new List<string>();
                List<string> lstOriginalPrices = new List<string>();
                foreach (var exam in lstExam)
                {
                    lstExamIDs.Add(exam.Id.ToString());
                    lstExamTitles.Add(exam.Title);
                    lstOriginalPrices.Add(exam.Price.ToString("0.00"));
                }


                //Create metadata class
                ProductMeta productMeta = new ProductMeta();
                //      ...Data
                productMeta.ExamCount = lstExam.Count().ToString();
                productMeta.BundleId = Model.Id.ToString();
                productMeta.BundleTitle = Model.Title;
                productMeta.ExamId = Newtonsoft.Json.JsonConvert.SerializeObject(lstExamIDs);
                productMeta.ExamTitle = Newtonsoft.Json.JsonConvert.SerializeObject(lstExamTitles);
                productMeta.MemberId = Members.GetCurrentMemberId().ToString();
                productMeta.SubmitType = submit;
                //      ...Monetary
                productMeta.CouponCode = Model.CouponCode;
                productMeta.OriginalPrice = Newtonsoft.Json.JsonConvert.SerializeObject(lstOriginalPrices);
                productMeta.BundleDiscount = Model.BundleDiscount.ToString("0.00");
                productMeta.CouponDiscount = Model.CouponDiscount.ToString("0.00");
                productMeta.TotalDiscount = Model.TotalDiscount.ToString("0.00");
                productMeta.TotalCost = Model.TotalCost.ToString("0.00");


                //Convert metadata to dictionary
                Dictionary<string, string> metaDict = productMeta.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .ToDictionary(prop => prop.Name, prop => (string)prop.GetValue(productMeta, null));



                //Add caption if coupon is applied
                string caption = "";
                if (!string.IsNullOrEmpty(productMeta.CouponCode))
                    caption = " - Coupon applied";


                //Create stripe product with custom price to submit.
                ProductCreateOptions productCreateOptions = new ProductCreateOptions
                {
                    Name = submit + " exam purchase for " + Model.Title + caption,
                    Description = string.Join(" + ", lstExamTitles),
                    Metadata = metaDict,
                    DefaultPriceData = new ProductDefaultPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = Convert.ToInt64(Model.TotalCost * 100),
                    },
                };
                ProductService service = new ProductService();
                Product product = service.Create(productCreateOptions);


                //Create a stripe session with created product
                SessionCreateOptions sessionOptions = new SessionCreateOptions
                {
                    LineItems = new List<SessionLineItemOptions>
                    {
                      new SessionLineItemOptions
                      {
                        Price = product.DefaultPriceId,
                        Quantity = 1,
                      },
                    },
                    Mode = "payment",
                    SuccessUrl = Umbraco.Content((int)bl.Models.Common.SiteNode.Account_ThankYou).Url(mode: UrlMode.Absolute),
                    CancelUrl = Umbraco.Content((int)bl.Models.Common.SiteNode.Account).Url(mode: UrlMode.Absolute),
                    Metadata = metaDict,
                    ClientReferenceId = Members.GetCurrentMemberId().ToString()
                };
                SessionService sessionService = new SessionService();
                Session session = sessionService.Create(sessionOptions);


                //Redirects to the checkout page.
                Response.Headers.Add("Location", session.Url);
                return new HttpStatusCodeResult(303);

            }
        }

        [ValidateAntiForgeryToken]
        public ActionResult SubmitExamPurchase_CompleteSet(bl.Models.Region Model, string submit)
        {
            if (!User.Identity.IsAuthenticated)
            {
                //Redirect to login page.
                ModelState.AddModelError("", "*Please log into your account.");
                return RedirectToUmbracoPage((int)bl.Models.Common.SiteNode.Login);
            }
            else
            {
                //Get selected exam
                List<bl.Models.Exam> lstExam = Model.LstExams.ToList();


                //List of data for metadata
                List<string> lstExamIDs = new List<string>();
                List<string> lstExamTitles = new List<string>();
                List<string> lstOriginalPrices = new List<string>();
                foreach (var exam in lstExam)
                {
                    lstExamIDs.Add(exam.Id.ToString());
                    lstExamTitles.Add(exam.Title);
                    lstOriginalPrices.Add(exam.Price.ToString("0.00"));
                }


                //Create metadata class
                ProductMeta productMeta = new ProductMeta();
                //      ...Data
                productMeta.ExamCount = lstExam.Count().ToString();
                productMeta.BundleId = Model.Id.ToString();
                productMeta.BundleTitle = Model.Title;
                productMeta.ExamId = Newtonsoft.Json.JsonConvert.SerializeObject(lstExamIDs);
                productMeta.ExamTitle = Newtonsoft.Json.JsonConvert.SerializeObject(lstExamTitles);
                productMeta.MemberId = Members.GetCurrentMemberId().ToString();
                productMeta.SubmitType = submit;
                //      ...Monetary
                productMeta.CouponCode = Model.CouponCode;
                productMeta.OriginalPrice = Newtonsoft.Json.JsonConvert.SerializeObject(lstOriginalPrices);
                productMeta.BundleDiscount = Model.BundleDiscount.ToString("0.00");
                productMeta.CouponDiscount = Model.CouponDiscount.ToString("0.00");
                productMeta.TotalDiscount = Model.TotalDiscount.ToString("0.00");
                productMeta.TotalCost = Model.TotalCost.ToString("0.00");


                //Convert metadata to dictionary
                Dictionary<string, string> metaDict = productMeta.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .ToDictionary(prop => prop.Name, prop => (string)prop.GetValue(productMeta, null));


                //Add caption if coupon is applied
                string caption = "";
                if (!string.IsNullOrEmpty(productMeta.CouponCode))
                    caption = " - Coupon applied";


                //Create stripe product with custom price to submit.
                ProductCreateOptions productCreateOptions = new ProductCreateOptions
                {
                    Name = submit + " bundle purchase for " + Model.Title + caption,
                    Description = string.Join(" + ", lstExamTitles),
                    Metadata = metaDict,
                    DefaultPriceData = new ProductDefaultPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = Convert.ToInt64(Model.TotalCost * 100),
                    },
                };
                ProductService service = new ProductService();
                Product product = service.Create(productCreateOptions);


                //Create a stripe session with created product
                SessionCreateOptions sessionOptions = new SessionCreateOptions
                {
                    LineItems = new List<SessionLineItemOptions>
                    {
                      new SessionLineItemOptions
                      {
                        Price = product.DefaultPriceId,
                        Quantity = 1,
                      },
                    },
                    Mode = "payment",
                    SuccessUrl = Umbraco.Content((int)bl.Models.Common.SiteNode.Account_ThankYou).Url(mode: UrlMode.Absolute),
                    CancelUrl = Umbraco.Content((int)bl.Models.Common.SiteNode.Account).Url(mode: UrlMode.Absolute),
                    Metadata = metaDict,
                    ClientReferenceId = Members.GetCurrentMemberId().ToString()
                };
                SessionService sessionService = new SessionService();
                Session session = sessionService.Create(sessionOptions);


                //Redirects to the checkout page.
                Response.Headers.Add("Location", session.Url);
                return new HttpStatusCodeResult(303);

            }

        }
        #endregion

        #region "Private Functions"
        private bool ShowLinkForSavedCompleted()
        {
            //
            int memberId = Members.GetCurrentMemberId();
            int StudyModeId = repoExamMode.GetIdByMode("StudyMode");
            int TimedModeId = repoExamMode.GetIdByMode("TimedMode");



            //
            if (repoExamRecord.GetRecords_Unsubmitted_ByMemberId(memberId).Any(x => x.ExamModeId == StudyModeId || x.ExamModeId == TimedModeId) == true)
            {
                return true;
            }
            else if (repoExamRecord.GetRecords_Submitted_ByMemberId_ExamModeId(memberId, StudyModeId).Any())
            {
                return true;
            }
            else if (repoExamRecord.GetRecords_Submitted_ByMemberId_ExamModeId(memberId, TimedModeId).Any())
            {
                return true;
            }


            //
            return false;
        }
        #endregion
    }
}