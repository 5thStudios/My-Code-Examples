using System.Collections.Generic;
using Umbraco.Web.Mvc;
using System.Text;
using Umbraco.Core;
using System;
using System.Web.Mvc;
using bl.Models;
using System.Web;
using System.Linq;
using Umbraco.Core.Models.PublishedContent;
using cm = Umbraco.Web.PublishedModels;
using Umbraco.Web;
using Umbraco.Core.Logging;
using bl.EF;
using Repositories;
using Umbraco.Web.PublishedModels;
using System.Collections.Specialized;
using System.Web.Security;

namespace bl.Controllers
{
    public class blExamController : SurfaceController
    {
        #region "Properties"
        private EF_SwtpDb _context;
        private IExamAnswerRepository repoExamAnswer;
        private IExamAnswerSetRepository repoExamAnswerSet;
        private IExamModeRepository repoExamMode;
        private IExamRecordRepository repoExamRecord;
        private IPurchaseRecordItemRepository repoPurchaseRecordItem;

        public blExamController()
        {
            _context = new EF_SwtpDb();
            repoExamAnswer = new ExamAnswerRepository(_context);
            repoExamAnswerSet = new ExamAnswerSetRepository(_context);
            repoExamMode = new ExamModeRepository(_context);
            repoExamRecord = new ExamRecordRepository(_context);
            repoPurchaseRecordItem = new PurchaseRecordItemRepository(_context);
        }
        #endregion



        #region "Renders"
        public ActionResult PrepareExam(cm.ExamInstructions cmExamInstructions)
        {
            //Instantiate variables
            Random rnd = new Random();
            ExamData examData = new ExamData();

            try
            {
                //Get data from page
                examData.Instructions = cmExamInstructions.Content;
                if (!string.IsNullOrWhiteSpace(cmExamInstructions.Title))
                    examData.Title = cmExamInstructions.Title;
                else
                    examData.Title = cmExamInstructions.Name;


                //Obtain parameter data
                int number;
                if (Int32.TryParse(Request.QueryString[Models.Common.Querystring.ExamId], out number)) examData.ExamId = number;
                if (Request.QueryString[Models.Common.Querystring.ExamType] != null)
                    examData.ExamType = Request.QueryString[Models.Common.Querystring.ExamType];



                //Set parameter defaults
                if (examData.QuestionId == null) examData.QuestionId = 1;
                if (string.IsNullOrEmpty(examData.ExamType)) examData.ExamType = Models.Common.ExamMode.FreeMode;



                //Create ExamRecord
                ExamRecord examRecord = new ExamRecord();
                blMemberController memberController = new blMemberController();
                //examRecord.ExamModeId = repoExamMode.GetIdByMode(examData.ExamType);
                examRecord.Submitted = false;
                examRecord.CreatedDate = DateTime.Now;
                if (examData.ExamType.ToLower() == Models.Common.ExamMode.FreeMode.ToLower())
                {
                    //Create FREE practice test
                    examRecord.ExamModeId = repoExamMode.GetIdByMode(Models.Common.ExamMode.FreeMode);
                    examRecord.ExamId = cmExamInstructions.FreeExam.Id;
                }
                else
                {
                    examRecord.ExamModeId = repoExamMode.GetIdByMode(examData.ExamType);
                    examRecord.ExamId = (int)examData.ExamId;

                    if (examData.ExamType == Models.Common.ExamMode.TimedMode)
                    {
                        //Set time remaining
                        cm.ExamPaid cmExam = new ExamPaid(Umbraco.Content(examRecord.ExamId));
                        string[] _duration = cmExam.Duration.Split(':');
                        examRecord.TimeRemaining = new TimeSpan(Convert.ToInt32(_duration[0]), Convert.ToInt32(_duration[1]), 0);
                    }
                }


                if (memberController.IsMemberLoggedIn())
                {
                    examRecord.MemberId = Members.GetCurrentMemberId();
                    PurchaseRecordItem item = repoPurchaseRecordItem.ObtainRecord_byMemberId_ExamId((int)examRecord.MemberId, examRecord.ExamId);
                    if (item != null)
                    {
                        examRecord.PurchaseRecordId = item.PurchaseRecordId;
                    }
                }



                if (examData.ExamType != Models.Common.ExamMode.FreeMode && examRecord.MemberId == null)
                {
                    //If exam is a paid exam and memberid is null, ensure member is logged out and redirect user to log in before proceeding!!!  **[2023-05-09 Prevents purchased exams from being created without member id.]
                    LogMemberOut();
                    TempData[bl.Models.Common.TempData.RedirectToLogin] = true;
                }
                else
                {
                    //Add exam record to db.
                    repoExamRecord.AddRecord(examRecord);


                    //Save next url querystring data
                    examData.ExamRecordId = examRecord.ExamRecordId;
                    examData.ExamModeId = examRecord.ExamModeId;


                    //Create ExamAnswerSet WITHOUT AnswerSet
                    ExamAnswerSet examAnswerSet = new ExamAnswerSet();
                    examAnswerSet.ExamRecordId = examRecord.ExamRecordId;
                    repoExamAnswerSet.AddRecord(examAnswerSet);


                    //Get list of answer IDs and randomize
                    IPublishedContent ipExam = Umbraco.Content(examRecord.ExamId);
                    List<int> lstAnswerIDs = new List<int>();
                    List<ExamAnswer> LstExamAnswersToAdd = new List<ExamAnswer>();
                    foreach (IPublishedContent ipContentArea in ipExam.Children)
                    {
                        foreach (IPublishedContent ipQuestion in ipContentArea.Children)
                        {
                            //Get question as model
                            cm.Question cmQuestion = new cm.Question(ipQuestion);

                            //Add question ID to list
                            lstAnswerIDs.Add(cmQuestion.Id);

                            //Get question data
                            ExamAnswer examAnswer = new ExamAnswer();
                            examAnswer.ExamAnswerSetId = examAnswerSet.ExamAnswerSetId;
                            examAnswer.ContentAreaId = ipContentArea.Id;
                            examAnswer.QuestionId = cmQuestion.Id;

                            //Create random order for answer render order.
                            List<int> lstAnswerSetIDs = new List<int>();
                            for (int i = 1; i <= cmQuestion.AnswerSets.Count(); i++) { lstAnswerSetIDs.Add(i); }
                            lstAnswerSetIDs = lstAnswerSetIDs.OrderBy(a => rnd.Next()).ToList();
                            examAnswer.AnswerRenderedOrder = String.Join(",", lstAnswerSetIDs.Select(x => x.ToString()).ToArray());

                            //Get index of correct answer.
                            int index = 1;
                            foreach (var _answerSet in cmQuestion.AnswerSets)
                            {
                                if (_answerSet.IsCorrectAnswer)
                                {
                                    examAnswer.CorrectAnswer = index;
                                    break;
                                }
                                index++;
                            }

                            //Add ExamAnswer
                            //repoExamAnswer.AddRecord(examAnswer);

                            //Add ExamAnswer to list
                            LstExamAnswersToAdd.Add(examAnswer);
                        }
                    }

                    //Add all missing records
                    repoExamAnswer.BulkAddRecord(LstExamAnswersToAdd);

                    //
                    lstAnswerIDs = lstAnswerIDs.OrderBy(a => rnd.Next()).ToList();


                    //Update ExamAnswerSet with AnswerSet from list
                    examAnswerSet.AnswerSet = String.Join(",", lstAnswerIDs.Select(x => x.ToString()).ToArray());
                    repoExamAnswerSet.UpdateRecord(examAnswerSet);


                    //Update each ExamAnswer record with question order
                    List<ExamAnswer> LstExamAnswersToUpdate = new List<ExamAnswer>();
                    for (var i = 0; i < lstAnswerIDs.Count(); i++)
                    {
                        bl.EF.ExamAnswer examAnswer = repoExamAnswer.GetRecord_ByQuestionId_ExamAnswerSetId(lstAnswerIDs[i], examAnswerSet.ExamAnswerSetId);
                        examAnswer.QuestionRenderOrder = i;
                        LstExamAnswersToUpdate.Add(examAnswer);
                        //repoExamAnswer.UpdateRecord(examAnswer);
                    }
                    repoExamAnswer.BulkUpdateRecord(LstExamAnswersToUpdate);


                    //Create next pg url
                    NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
                    queryString.Add(Models.Common.Querystring.ExamRecordId, examData.ExamRecordId.ToString());
                    examData.NextUrl = Umbraco.Content((int)bl.Models.Common.SiteNode.Exam).Url() + "?" + System.Web.HttpUtility.UrlDecode(queryString.ToString());
                }


            }
            catch (Exception ex)
            {
                Logger.Error<blExamController>(ex);
            }

            return PartialView(Models.Common.PartialPath.Exams_ExamInstructions, examData);
        }
        public ActionResult RenderExam(bl.Models.ExamQuestion examQuestion)
        {
            try
            {
                //Determine if data already exists or if a new object needs to be created.
                if (TempData[Models.Common.TempData.ExamQuestion] != null)
                {
                    examQuestion = (ExamQuestion)TempData[Models.Common.TempData.ExamQuestion];
                }
                else
                {
                    examQuestion = new bl.Models.ExamQuestion();
                }


                //Extract question # from querystring if exists.  (Usually present when resuming a test via a link.)
                int questionNo;
                if (Int32.TryParse(Request.QueryString[Models.Common.Querystring.QuestionNo], out questionNo)) examQuestion.QuestionNo = questionNo;


                //Obtain parameter data
                if (examQuestion.ExamRecordId == null)
                {
                    int number;
                    if (Int32.TryParse(Request.QueryString[Models.Common.Querystring.ExamRecordId], out number)) examQuestion.ExamRecordId = number;
                }

                //Refresh data if no errors exist
                if (!examQuestion.ShowErrorMsg && examQuestion.ExamRecordId != null)
                {
                    //Obtain exam record and answer set
                    ExamRecord examRecord = repoExamRecord.GetRecord_ById((int)examQuestion.ExamRecordId);
                    ExamAnswerSet examAnswerSet = repoExamAnswerSet.GetRecord_ByExamRecordId((int)examQuestion.ExamRecordId);




                    //Get exam mode
                    if (examRecord.ExamModeId != null)
                    {
                        examQuestion.ExamModeId = (int)examRecord.ExamModeId;
                        examQuestion.ExamMode = repoExamMode.GetModeNameById(examQuestion.ExamModeId);
                        if (examQuestion.ExamMode == "Free Mode") examQuestion.ExamMode = string.Empty;
                    }


                    //Determine if exam is free or not
                    examQuestion.IsFreeExam = (repoExamMode.GetModeById((int)examRecord.ExamModeId) == Models.Common.ExamMode.FreeMode);







                    //StringBuilder sb = new StringBuilder();
                    //sb.AppendLine("IsFreeExam: " + examQuestion.IsFreeExam.ToString());
                    //sb.AppendLine("  |  ExamModeId: " + examRecord.ExamModeId.ToString());
                    //sb.AppendLine("  |  ExamMode.FreeMode: " + Models.Common.ExamMode.FreeMode);
                    //sb.AppendLine("  |  repoExamMode.GetModeById: " + repoExamMode.GetModeById((int)examRecord.ExamModeId));
                    //Logger.Warn<blExamController>(sb.ToString());











                    //Adjust time remaining
                    if (repoExamMode.GetModeById((int)examRecord.ExamModeId) == Models.Common.ExamMode.TimedMode)
                    {
                        examQuestion.TimeRemaining = string.Format("{0:D2}:{1:D2}:{2:D2}", ((TimeSpan)examRecord.TimeRemaining).Hours, ((TimeSpan)examRecord.TimeRemaining).Minutes, ((TimeSpan)examRecord.TimeRemaining).Seconds);
                    }


                    //Obtain exam answer record from answer set
                    List<int> lstExamAnswerSetIDs = examAnswerSet.AnswerSet?.Split(',')?.Select(Int32.Parse)?.ToList();
                    examQuestion.ExamAnswer = repoExamAnswer.GetRecord_ByQuestionId_ExamAnswerSetId(Convert.ToInt32(lstExamAnswerSetIDs[examQuestion.QuestionNo - 1]), examAnswerSet.ExamAnswerSetId);


                    //Get question node and parent test node
                    cm.Question cmQuestion = new cm.Question(Umbraco.Content(examQuestion.ExamAnswer.QuestionId));
                    cm.ExamFree cmExamFree = new cm.ExamFree(Umbraco.Content(examRecord.ExamId));


                    //Get info for mailto
                    examQuestion.QuestionName = cmQuestion.Name;
                    examQuestion.ContentAreaName = cmQuestion.Parent.Name;


                    //Obtain question render order.
                    List<int> lstRenderOrder = examQuestion.ExamAnswer.AnswerRenderedOrder?.Split(',')?.Select(Int32.Parse)?.ToList();


                    //Get Answer Sets in order based on examAnswer.RenderedOrder    //Example data:  4,2,1,3
                    for (int i = 0; i < lstRenderOrder.Count(); i++)
                    {
                        //Get answer set based on index
                        Int32 renderOrderIndex = lstRenderOrder[i];
                        cm.AnswerSet cmAnswerSet = cmQuestion.AnswerSets.ToList()[renderOrderIndex - 1];

                        //Obtain data and add to list.
                        bl.Models.AnswerSet _answerSet = new Models.AnswerSet();
                        _answerSet.Answer = cmAnswerSet.Answer;
                        _answerSet.Rationale = cmAnswerSet.Rationale;
                        _answerSet.IsCorrectAnswer = cmAnswerSet.IsCorrectAnswer;
                        _answerSet.RenderedOrder = renderOrderIndex;
                        _answerSet.IsSelectedAnswer = (examQuestion.ExamAnswer.SelectedAnswer == renderOrderIndex);
                        examQuestion.LstAnswerSets.Add(_answerSet);
                    }


                    //Obtain question data
                    examQuestion.Title = cmExamFree.Name;
                    examQuestion.TotalNoQuestions = lstExamAnswerSetIDs.Count();
                    examQuestion.QuestionText = cmQuestion.QuestionText;
                    examQuestion.Rationale = cmQuestion.Rationale;
                    foreach (var _link in cmQuestion.SuggestedStudyLinks)
                    {
                        bl.Models.Link newLink = new Link();
                        newLink.Name = _link.Name;
                        newLink.Url = _link.Url;
                        newLink.Target = _link.Target;
                        examQuestion.LstSuggestedStudyLinks.Add(newLink);
                    }
                    examQuestion.SuggestedStudyDescription = cmQuestion.SuggestedStudyDescription;
                    examQuestion.Source = cmQuestion.Source;
                    examQuestion.StartTime = DateTime.Now;


                    //Show or hide prev btn
                    if (examQuestion.QuestionNo > 1)
                        examQuestion.ShowPrevious = true;
                    else
                        examQuestion.ShowPrevious = false;


                    //Show or hide next/complete btns
                    if (examQuestion.QuestionNo == lstExamAnswerSetIDs.Count())
                    {
                        examQuestion.ShowNext = false;
                        examQuestion.ShowComplete = true;
                    }
                    else
                    {
                        examQuestion.ShowNext = true;
                        examQuestion.ShowComplete = false;
                        examQuestion.SaveStopBtnUrl = Umbraco.Content((int)bl.Models.Common.SiteNode.Account_MemberExams).Url();


                        //Check if member is logged in.
                        blMemberController memberController = new blMemberController();
                        if (memberController.IsMemberLoggedIn())
                        {
                            examQuestion.SaveStopBtnUrl = Umbraco.Content((int)bl.Models.Common.SiteNode.Account_MemberExams).Url();
                        }
                        else
                        {
                            examQuestion.SaveStopBtnUrl = Umbraco.Content((int)bl.Models.Common.SiteNode.Account_GuestAccount).Url();
                        }
                    }


                    //Obtain list of questions marked for review
                    foreach (ExamAnswer _examAnswer in repoExamAnswer.GetReviewQuestion_ByExamAnswerSetId(examAnswerSet.ExamAnswerSetId))
                    {
                        examQuestion.LstMarkedQuestions.Add(new MarkedQuestion((int)_examAnswer.QuestionRenderOrder, (_examAnswer.SelectedAnswer != null)));
                    }

                }


                //Create subject line for email
                StringBuilder sbEmailSubjectText = new StringBuilder();
                sbEmailSubjectText.Append("mailto:info@socialworktestprep.com?subject=");
                sbEmailSubjectText.Append("Question for Exam:  ");
                sbEmailSubjectText.Append(examQuestion.Title);
                sbEmailSubjectText.Append("  |  ");
                sbEmailSubjectText.Append(examQuestion.ContentAreaName);
                sbEmailSubjectText.Append("  |  ");
                sbEmailSubjectText.Append(examQuestion.QuestionName);
                examQuestion.EmailSubjectText = System.Net.WebUtility.HtmlEncode(sbEmailSubjectText.ToString());
                //}

            }
            catch (Exception ex)
            {
                Logger.Error<blExamController>(ex);
            }

            return PartialView(Models.Common.PartialPath.Exams_ExamQuestion, examQuestion);
        }
        public ActionResult RenderExamResults(string _examRecordId)
        {
            try
            {
                if (string.IsNullOrEmpty(_examRecordId))
                {
                    return null;
                }
                else
                {
                    //try to parse exam id
                    int examRecordId;
                    if (Int32.TryParse(_examRecordId, out examRecordId))
                    {
                        //Obtain exam results
                        return PartialView(Models.Common.PartialPath.Exams_ExamOverview, ObtainExamResultOverview(examRecordId));
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error<blExamController>(ex);
            }


            return null;
        }
        public ActionResult RenderExamReview(bl.Models.ExamQuestion examQuestion)
        {

            try
            {
                //Instantiate variables
                string reviewType = string.Empty;
                int _ExamRecordId;


                //Obtain parameter data from querystring
                if (Int32.TryParse(Request.QueryString[Models.Common.Querystring.ExamRecordId], out _ExamRecordId))
                {
                    //Instantiate variables                  
                    int _ContentAreaId = 0;
                    int _Index = 0;
                    List<int> _LstQuestionIDs = new List<int>();


                    if (Request.QueryString[Models.Common.Querystring.ContentArea] != null)
                    {
                        //RENDER ONLY CONTENT-AREA QUESTIONS        ex:  ?Review=ContentArea&ExamRecordId=1&ContentArea=4225&Index=1&Count=1&LstQuestionIDs=4226,4228
                        //=======================================
                        Int32.TryParse(Request.QueryString[Models.Common.Querystring.ContentArea], out _ContentAreaId);
                        Int32.TryParse(Request.QueryString[Models.Common.Querystring.Index], out _Index);
                        _LstQuestionIDs = Request.QueryString[Models.Common.Querystring.LstQuestionIDs].Split(',').Select(int.Parse).ToList();
                    }


                    //Determine if data already exists or if a new object needs to be created.
                    if (TempData[Models.Common.TempData.ExamQuestion] != null)
                    {
                        examQuestion = (ExamQuestion)TempData[Models.Common.TempData.ExamQuestion];
                    }
                    else
                    {
                        examQuestion = new bl.Models.ExamQuestion();
                    }


                    //Get review type from querystring
                    if (Request.QueryString[Models.Common.Querystring.Review] != null && Request.QueryString[Models.Common.Querystring.Review] != string.Empty)
                        reviewType = Request.QueryString[Models.Common.Querystring.Review];


                    //Obtain exam record and answer set
                    examQuestion.ExamRecordId = _ExamRecordId;
                    ExamRecord examRecord = repoExamRecord.GetRecord_ById((int)examQuestion.ExamRecordId);
                    ExamAnswerSet examAnswerSet = repoExamAnswerSet.GetRecord_ByExamRecordId((int)examQuestion.ExamRecordId);


                    //Obtain question
                    switch (reviewType)
                    {
                        case "Correct":
                            //Obtain only correct questions
                            examQuestion.TotalNoQuestions = repoExamAnswer.GetCount_ByExamAnswerSetId_CorrectAnswers(examAnswerSet.ExamAnswerSetId);
                            examQuestion.ExamAnswer = repoExamAnswer.GetRecord_ByExamAnswerSetId_QuestionNo_CorrectOnly(examAnswerSet.ExamAnswerSetId, examQuestion.QuestionNo - 1);
                            break;

                        case "Incorrect":
                            //Obtain only incorrect questions
                            examQuestion.TotalNoQuestions = repoExamAnswer.GetCount_ByExamAnswerSetId_IncorrectAnswers(examAnswerSet.ExamAnswerSetId);
                            examQuestion.ExamAnswer = repoExamAnswer.GetRecord_ByExamAnswerSetId_QuestionNo_IncorrectOnly(examAnswerSet.ExamAnswerSetId, examQuestion.QuestionNo - 1);
                            break;

                        case "ContentArea":
                            //Obtain only questions from content area.  [IDs provided in querystring]

                            //int _ExamRecordId = 1;
                            //int _ContentAreaId = 4225;
                            //int _Index = 1;
                            //int _Count = 1;
                            //_LstQuestionIDs = '4226,4228';

                            examQuestion.QuestionNo = _Index;
                            examQuestion.TotalNoQuestions = _LstQuestionIDs.Count;
                            examQuestion.ExamAnswer = repoExamAnswer.GetRecord_ByExamAnswerSetId_QuestionId(examAnswerSet.ExamAnswerSetId, _LstQuestionIDs[_Index - 1]);
                            break;

                        default:
                            //Obtain all questions
                            examQuestion.TotalNoQuestions = repoExamAnswer.GetCount_ByExamAnswerSetId(examAnswerSet.ExamAnswerSetId);
                            examQuestion.ExamAnswer = repoExamAnswer.GetRecord_ByExamAnswerSetId_QuestionNo(examAnswerSet.ExamAnswerSetId, examQuestion.QuestionNo - 1);
                            break;
                    }


                    if (examQuestion != null && examQuestion.ExamAnswer != null)
                    {
                        //Get question node and parent test node
                        cm.Question cmQuestion = new cm.Question(Umbraco.Content(examQuestion.ExamAnswer.QuestionId));

                        //Get info for mailto
                        examQuestion.QuestionName = cmQuestion.Name;
                        examQuestion.ContentAreaName = cmQuestion.Parent.Name;

                        //Obtain question render order.
                        List<int> lstRenderOrder = examQuestion.ExamAnswer.AnswerRenderedOrder?.Split(',')?.Select(Int32.Parse)?.ToList();

                        //Get Answer Sets in order based on examAnswer.RenderedOrder    //Example data:  4,2,1,3
                        for (int i = 0; i < lstRenderOrder.Count(); i++)
                        {
                            //Get answer set based on index
                            Int32 renderOrderIndex = lstRenderOrder[i];
                            if ((renderOrderIndex - 1) <= (cmQuestion.AnswerSets.Count() - 1)) /* 2023-06-08 | Check in prep for upcoming changes in question counts. */
                            {
                                cm.AnswerSet cmAnswerSet = cmQuestion.AnswerSets.ToList()[renderOrderIndex - 1];

                                //Obtain data and add to list.
                                bl.Models.AnswerSet _answerSet = new Models.AnswerSet();
                                _answerSet.Answer = cmAnswerSet.Answer;
                                _answerSet.Rationale = cmAnswerSet.Rationale;
                                _answerSet.IsCorrectAnswer = cmAnswerSet.IsCorrectAnswer;
                                _answerSet.RenderedOrder = renderOrderIndex;
                                _answerSet.IsSelectedAnswer = (examQuestion.ExamAnswer.SelectedAnswer == renderOrderIndex);
                                examQuestion.LstAnswerSets.Add(_answerSet);
                            }
                        }

                        //Obtain question data
                        examQuestion.Title = Umbraco.Content(examRecord.ExamId).Name;
                        //examQuestion.TotalNoQuestions = lstExamAnswerSetIDs.Count();
                        examQuestion.QuestionText = cmQuestion.QuestionText;
                        examQuestion.Rationale = cmQuestion.Rationale;
                        foreach (var _link in cmQuestion.SuggestedStudyLinks)
                        {
                            bl.Models.Link newLink = new Link();
                            newLink.Name = _link.Name;
                            newLink.Url = _link.Url;
                            newLink.Target = _link.Target;
                            examQuestion.LstSuggestedStudyLinks.Add(newLink);
                        }
                        examQuestion.SuggestedStudyDescription = cmQuestion.SuggestedStudyDescription;
                        examQuestion.Source = cmQuestion.Source;


                        //Show or hide prev btn
                        if (examQuestion.QuestionNo > 1)
                            examQuestion.ShowPrevious = true;
                        else
                            examQuestion.ShowPrevious = false;


                        //Show or hide next/complete btns
                        if (examQuestion.QuestionNo == examQuestion.TotalNoQuestions)
                        {
                            examQuestion.ShowNext = false;
                            examQuestion.ShowComplete = true;
                        }
                        else
                        {
                            examQuestion.ShowNext = true;
                            examQuestion.ShowComplete = false;
                        }
                    }



                    //Obtain list of questions marked for review
                    if (examAnswerSet != null && examAnswerSet.ExamAnswerSetId > 0)
                    {
                        foreach (ExamAnswer _examAnswer in repoExamAnswer.GetReviewQuestion_ByExamAnswerSetId(examAnswerSet.ExamAnswerSetId))
                        {
                            if (reviewType == "ContentArea")
                            {
                                if (_LstQuestionIDs.Contains(_examAnswer.QuestionId)) //Add only questions listed in category
                                {
                                    examQuestion.LstMarkedQuestions.Add(new MarkedQuestion((int)_examAnswer.QuestionRenderOrder, (_examAnswer.SelectedAnswer != null)));
                                }
                            }
                            else
                            {
                                examQuestion.LstMarkedQuestions.Add(new MarkedQuestion((int)_examAnswer.QuestionRenderOrder, (_examAnswer.SelectedAnswer != null)));
                            }
                        }
                    }



                    //Create subject line for email
                    StringBuilder sbEmailSubjectText = new StringBuilder();
                    sbEmailSubjectText.Append("mailto:info@socialworktestprep.com?subject=");
                    sbEmailSubjectText.Append("Question for Exam:  ");
                    sbEmailSubjectText.Append(examQuestion.Title);
                    sbEmailSubjectText.Append("  |  ");
                    sbEmailSubjectText.Append(examQuestion.ContentAreaName);
                    sbEmailSubjectText.Append("  |  ");
                    sbEmailSubjectText.Append(examQuestion.QuestionName);
                    examQuestion.EmailSubjectText = System.Net.WebUtility.HtmlEncode(sbEmailSubjectText.ToString());

                }
            }
            catch (Exception ex)
            {
                Logger.Error<blExamController>(ex);
            }

            return PartialView(Models.Common.PartialPath.Exams_ExamReview, examQuestion);
        }
        public ActionResult RenderExamReviewList(bool IsSubmitted, string ExamMode)
        {
            //
            if (new blMemberController().IsMemberLoggedIn())
            {
                //
                List<ExamRecord> lstExamRecords = null;
                List<ExamResult> lstExamResults = new List<ExamResult>();
                List<Link> lstLinks = null;
                int memberId = Members.GetCurrentMemberId();
                //int offsetHours = -6;



                if (!IsSubmitted)
                {
                    //Obtain exams in progress
                    lstExamRecords = repoExamRecord.GetRecords_Unsubmitted_ByMemberId(memberId);

                    //Create resume links for each record
                    lstLinks = new List<Link>();
                    foreach (ExamRecord examRecord in lstExamRecords)
                    {
                        if (repoExamAnswerSet.GetRecord_ByExamRecordId(examRecord.ExamRecordId) != null)
                        {
                            //Obtain test name
                            IPublishedContent ipExam = Umbraco.Content(examRecord.ExamId);
                            string testName = ipExam.Name;
                            string subname = "";
                            DateTime? examDate = null;

                            if (examRecord.SubmittedDate != null)
                            {
                                subname = Convert.ToDateTime(examRecord.SubmittedDate).ToString("MMM d, yyyy @ h:mm") +
                                          Convert.ToDateTime(examRecord.SubmittedDate).ToString("tt").ToLower();
                                //subname = Convert.ToDateTime(examRecord.SubmittedDate).AddHours(offsetHours).ToString("MMM d, yyyy @ h:mm") +
                                //          Convert.ToDateTime(examRecord.SubmittedDate).AddHours(offsetHours).ToString("tt").ToLower();
                                examDate = examRecord.SubmittedDate;
                            }

                            else if (examRecord.CreatedDate != null)
                            {
                                subname = Convert.ToDateTime(examRecord.CreatedDate).ToString("MMM d, yyyy @ h:mm") +
                                          Convert.ToDateTime(examRecord.CreatedDate).ToString("tt").ToLower();
                                //subname = Convert.ToDateTime(examRecord.CreatedDate).AddHours(offsetHours).ToString("MMM d, yyyy @ h:mm") +
                                //          Convert.ToDateTime(examRecord.CreatedDate).AddHours(offsetHours).ToString("tt").ToLower();
                                examDate = examRecord.CreatedDate;
                            }





                            //Obtain time remaining if available
                            string timeRemaining = "N/A";
                            if (examRecord.TimeRemaining != null)
                                timeRemaining = string.Format("{0:D2}:{1:D2}:{2:D2}", ((TimeSpan)examRecord.TimeRemaining).Hours, ((TimeSpan)examRecord.TimeRemaining).Minutes, ((TimeSpan)examRecord.TimeRemaining).Seconds);


                            //Get question # user left off with
                            ExamAnswerSet examAnswerSet = repoExamAnswerSet.GetRecord_ByExamRecordId(examRecord.ExamRecordId);

                            IEnumerable<ExamAnswer> lstExamAnswers = repoExamAnswer.GetRecords_ByExamAnswerSetId(examAnswerSet.ExamAnswerSetId);
                            ExamAnswer examAnswer = lstExamAnswers.OrderBy(x => x.QuestionRenderOrder).Where(x => x.SelectedAnswer == null).FirstOrDefault();

                            //Create resume pg url
                            NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
                            queryString.Add(Models.Common.Querystring.ExamRecordId, examRecord.ExamRecordId.ToString());
                            if (examAnswer != null) queryString.Add(Models.Common.Querystring.QuestionNo, (examAnswer.QuestionRenderOrder + 1).ToString());
                            string url = Umbraco.Content((int)bl.Models.Common.SiteNode.Exam).Url() + "?" + System.Web.HttpUtility.UrlDecode(queryString.ToString());

                            //Create resume link
                            Link link = new Link(testName, url);
                            link.Subname = subname;
                            if (examDate != null) link.Date = (DateTime)examDate;
                            link.MiscNote = timeRemaining;
                            lstLinks.Add(link);
                        }

                    }
                }
                else
                {
                    //Obtain submitted exams
                    int examModeId = repoExamMode.GetIdByMode(ExamMode);
                    lstExamRecords = repoExamRecord.GetRecords_Submitted_ByMemberId_ExamModeId(memberId, examModeId);
                }



                //ObtainExamResultOverview(examRecordId)
                if (lstExamRecords == null || lstExamRecords.Count() == 0)
                {
                    //Hide panel
                    return null;
                }
                else
                {
                    //Return correct partialview
                    if (!IsSubmitted)
                    {
                        return PartialView(Models.Common.PartialPath.Account_ReviewExams_InProgress, lstLinks);
                    }
                    else
                    {
                        //Add each record to list
                        foreach (ExamRecord record in lstExamRecords)
                        {
                            //Create exam result overview and add to list
                            lstExamResults.Add(ObtainExamResultOverview(record.ExamRecordId));
                        }

                        switch (ExamMode)
                        {
                            case "StudyMode":
                                return PartialView(Models.Common.PartialPath.Account_ReviewExams_Submitted_StudyMode, lstExamResults);

                            case "TimedMode":
                                return PartialView(Models.Common.PartialPath.Account_ReviewExams_Submitted, lstExamResults);

                            case "FreeMode":
                                return PartialView(Models.Common.PartialPath.Account_ReviewExams_SubmittedFree, lstExamResults);

                            default: return null;
                        }
                    }
                }
            }

            return null;
        }
        public ActionResult RenderPurchasedExams()
        {
            //Instantiate variables
            ExamList examList = new ExamList();

            try
            {
                //Get member Id
                examList.MemberId = Members.GetCurrentMemberId();


                //Get list of all purchases by member
                List<PurchaseRecordItem> lstPurchaseRecordItem = repoPurchaseRecordItem.ObtainRecords_byMemberId(examList.MemberId);
                foreach (PurchaseRecordItem exam in lstPurchaseRecordItem)
                {
                    //Add ONLY if the dictionary does not already contain the exam.
                    if (!examList.ExamDict.ContainsKey(exam.ExamTitle))
                    {
                        examList.ExamDict.Add(exam.ExamTitle, exam.ExamId);
                    }
                }


                //Get list of all exam modes
                List<EF.ExamMode> lstExamModes = repoExamMode.SelectAll();
                foreach (EF.ExamMode mode in lstExamModes.Where(x => x.AbrName != "Free").ToList())
                {
                    examList.ModeDict.Add(mode.Name, mode.Mode);
                }
            }
            catch (Exception ex)
            {
                Logger.Error<blExamController>(ex);
            }


            return PartialView(bl.Models.Common.PartialPath.Account_TakeExam, examList);
        }
        #endregion



        #region "Submits"
        //[ValidateAntiForgeryToken]
        public ActionResult SubmitExam(bl.Models.ExamQuestion model, bool CompleteBtnClicked = false, bool PreviousBtnClicked = false, bool NextBtnClicked = false)
        {
            //
            bl.Models.ExamQuestion examQuestion = model;


            try
            {
                //Clear error flag
                examQuestion.ShowErrorMsg = false;


                //Check if a marked question was clicked
                int _goToQuestionNo;
                if (Int32.TryParse(examQuestion.GoToQuestion, out _goToQuestionNo))
                {
                    //Ignore invalid data and return to selected question
                    examQuestion = new ExamQuestion();
                    examQuestion.ExamRecordId = model.ExamRecordId;
                    examQuestion.QuestionNo = _goToQuestionNo;


                }
                else if (PreviousBtnClicked)
                {

                    //Determine if timed-mode is active and update time remaining.
                    if (repoExamMode.GetModeById((int)model.ExamModeId) == Models.Common.ExamMode.TimedMode)
                    {
                        //Update time remaining
                        ExamRecord _examRecord = repoExamRecord.GetRecord_ById((int)examQuestion.ExamRecordId);
                        _examRecord.TimeRemaining = _examRecord.TimeRemaining - (DateTime.Now - model.StartTime);
                        if (_examRecord.TimeRemaining <= TimeSpan.Zero)
                        {
                            //Mark as submitted if time is expred
                            _examRecord.TimeRemaining = TimeSpan.Zero;
                            _examRecord.SubmittedDate = DateTime.Now;
                            _examRecord.Submitted = true;
                        }

                        repoExamRecord.UpdateRecord(_examRecord);

                        //If submitted, redirect to acct pg.
                        if (_examRecord.Submitted)
                        {
                            //Show msg that time is up
                            TempData.Clear();
                            TempData[Models.Common.TempData.TimeIsUp] = true;

                            return RedirectToUmbracoPage((int)bl.Models.Common.SiteNode.Account_MemberExams);
                        }
                    }


                    //Ignore invalid data and return to previous question
                    examQuestion = new ExamQuestion();
                    examQuestion.ExamRecordId = model.ExamRecordId;
                    examQuestion.QuestionNo = model.QuestionNo - 1;

                }
                else if (examQuestion.ExamAnswer.ReviewQuestion) //(examQuestion.MarkedForReview)
                {
                    //Obtain ExamAnswer record and update data
                    ExamAnswer _examAnswer = repoExamAnswer.GetRecord_ById(model.ExamAnswer.ExamAnswersId);
                    _examAnswer.SelectedAnswer = model.SelectedAnswer;
                    _examAnswer.IsCorrect = (model.SelectedAnswer == _examAnswer.CorrectAnswer);
                    _examAnswer.ReviewQuestion = true;
                    repoExamAnswer.UpdateRecord(_examAnswer);


                    if (repoExamMode.GetModeById((int)model.ExamModeId) == Models.Common.ExamMode.TimedMode)
                    {
                        //Update time remaining
                        ExamRecord _examRecord = repoExamRecord.GetRecord_ById((int)examQuestion.ExamRecordId);
                        _examRecord.TimeRemaining = _examRecord.TimeRemaining - (DateTime.Now - model.StartTime);
                        if (_examRecord.TimeRemaining < TimeSpan.Zero)
                            _examRecord.TimeRemaining = TimeSpan.Zero;

                        repoExamRecord.UpdateRecord(_examRecord);
                    }

                    if (!CompleteBtnClicked)
                    {
                        //Ignore invalid data, record data and proceed to next question
                        examQuestion = new ExamQuestion();
                        examQuestion.ExamRecordId = model.ExamRecordId;
                        examQuestion.QuestionNo = model.QuestionNo + 1;
                    }
                }
                else if (NextBtnClicked)
                {
                    if (!ModelState.IsValid || examQuestion.SelectedAnswer == null)
                    {
                        //Show error msg and stop processing data.
                        examQuestion.ShowErrorMsg = true;
                    }
                    else
                    {

                        //Obtain ExamAnswer record and update data
                        ExamAnswer _examAnswer = repoExamAnswer.GetRecord_ById(model.ExamAnswer.ExamAnswersId);
                        _examAnswer.SelectedAnswer = model.SelectedAnswer;
                        _examAnswer.IsCorrect = (model.SelectedAnswer == _examAnswer.CorrectAnswer);
                        _examAnswer.ReviewQuestion = examQuestion.ExamAnswer.ReviewQuestion;
                        repoExamAnswer.UpdateRecord(_examAnswer);

                        if (repoExamMode.GetModeById((int)model.ExamModeId) == Models.Common.ExamMode.TimedMode)
                        {
                            //Update time remaining
                            ExamRecord _examRecord = repoExamRecord.GetRecord_ById((int)examQuestion.ExamRecordId);
                            _examRecord.TimeRemaining = _examRecord.TimeRemaining - (DateTime.Now - model.StartTime);
                            if (_examRecord.TimeRemaining <= TimeSpan.Zero)
                            {
                                //Mark as submitted if time is expred
                                _examRecord.TimeRemaining = TimeSpan.Zero;
                                _examRecord.SubmittedDate = DateTime.Now;
                                _examRecord.Submitted = true;
                            }

                            repoExamRecord.UpdateRecord(_examRecord);

                            //If submitted, redirect to acct pg.
                            if (_examRecord.Submitted)
                            {
                                //Show msg that time is up
                                TempData.Clear();
                                TempData[Models.Common.TempData.TimeIsUp] = true;

                                return RedirectToUmbracoPage((int)bl.Models.Common.SiteNode.Account_MemberExams);
                            }
                        }

                        //Create new instance of class
                        examQuestion = new ExamQuestion();
                        examQuestion.ExamRecordId = model.ExamRecordId;
                        examQuestion.QuestionNo = model.QuestionNo + 1;
                    }
                }



                if (CompleteBtnClicked)
                {
                    if (!ModelState.IsValid || (examQuestion.SelectedAnswer == null && !examQuestion.ExamAnswer.ReviewQuestion))
                    {
                        //Show error msg and stop processing data.
                        examQuestion.ShowErrorMsg = true;
                    }
                    else
                    {
                        //Clear class
                        //examQuestion = null;

                        //Obtain ExamAnswer record and update data
                        ExamAnswer _examAnswer = repoExamAnswer.GetRecord_ById(model.ExamAnswer.ExamAnswersId);
                        _examAnswer.SelectedAnswer = model.SelectedAnswer;
                        _examAnswer.IsCorrect = (model.SelectedAnswer == _examAnswer.CorrectAnswer);
                        _examAnswer.ReviewQuestion = examQuestion.ExamAnswer.ReviewQuestion; //examQuestion.MarkedForReview;
                        repoExamAnswer.UpdateRecord(_examAnswer);

                        //Obtain ExamRecord and update data
                        ExamRecord _examRecord = repoExamRecord.GetRecord_ById((int)model.ExamRecordId);
                        _examRecord.SubmittedDate = DateTime.Now;
                        _examRecord.Submitted = true;
                        if (repoExamMode.GetModeById((int)model.ExamModeId) == Models.Common.ExamMode.TimedMode) //Update time remaining
                        {
                            _examRecord.TimeRemaining = _examRecord.TimeRemaining - (DateTime.Now - model.StartTime);
                            if (_examRecord.TimeRemaining < TimeSpan.Zero)
                                _examRecord.TimeRemaining = TimeSpan.Zero;
                        }
                        repoExamRecord.UpdateRecord(_examRecord);

                        //Redirect to proper page
                        if (Umbraco.MemberIsLoggedOn())
                        {
                            //if logged in go to account/exams
                            return RedirectToUmbracoPage((int)bl.Models.Common.SiteNode.Account_MemberExams);
                        }
                        else
                        {
                            //Create a Cookie with [ExamRecord].ExamRecordId
                            HttpCookie cookie = new HttpCookie("ExamRecordId");
                            cookie.Value = model.ExamRecordId.ToString();
                            cookie.Expires = DateTime.Now.AddDays(90);
                            Response.Cookies.Add(cookie);

                            //Go to guest acct page
                            return RedirectToUmbracoPage((int)bl.Models.Common.SiteNode.Account_GuestAccount);
                        }
                    }


                }

                //Save updated data to cache
                TempData.Clear();
                TempData[Models.Common.TempData.ExamQuestion] = examQuestion;
            }
            catch (Exception ex)
            {
                Logger.Error<blExamController>(ex);
                Response.Write("ERROR: " + ex.Message);

                //Return original data to cache
                TempData.Clear();
                TempData[Models.Common.TempData.ExamQuestion] = model;
            }



            //Recreate querystring and redirect with it.
            NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString.Add(Models.Common.Querystring.ExamRecordId, examQuestion.ExamRecordId.ToString());
            return RedirectToCurrentUmbracoPage(HttpUtility.UrlDecode(queryString.ToString()));

        }

        //[ValidateAntiForgeryToken]
        public ActionResult SubmitExamReview(bl.Models.ExamQuestion model)
        {
            //
            bl.Models.ExamQuestion examQuestion = null;

            try
            {
                //Check if a marked question was clicked
                int _goToQuestionNo;
                if (Int32.TryParse(model.GoToQuestion, out _goToQuestionNo))
                {
                    //Ignore invalid data and return to selected question
                    examQuestion = new ExamQuestion();
                    examQuestion.ExamRecordId = model.ExamRecordId;
                    examQuestion.QuestionNo = _goToQuestionNo;


                }
                else if (model.PreviousBtnClicked)
                {
                    //Ignore invalid data and return to previous question
                    examQuestion = new ExamQuestion();
                    examQuestion.ExamRecordId = model.ExamRecordId;
                    examQuestion.QuestionNo = model.QuestionNo - 1;


                }
                else if (model.NextBtnClicked)
                {
                    //Create new instance of class
                    examQuestion = new ExamQuestion();
                    examQuestion.ExamRecordId = model.ExamRecordId;
                    examQuestion.QuestionNo = model.QuestionNo + 1;
                }
                else if (model.CompleteBtnClicked)
                {
                    //Redirect to proper page
                    if (Umbraco.MemberIsLoggedOn())
                    {
                        //if logged in go to account/exams
                        return RedirectToUmbracoPage((int)bl.Models.Common.SiteNode.Account_MemberExams);
                    }
                    else
                    {
                        //Go to guest acct page
                        return RedirectToUmbracoPage((int)bl.Models.Common.SiteNode.Account_GuestAccount);
                    }
                }

                //Save updated data to cache
                TempData.Clear();
                TempData[Models.Common.TempData.ExamQuestion] = examQuestion;
            }
            catch (Exception ex)
            {
                Logger.Error<blExamController>(ex);
                Response.Write("ERROR: " + ex.Message);

                //Return original data to cache
                TempData.Clear();
                TempData[Models.Common.TempData.ExamQuestion] = model;
            }


            //Recreate querystring and redirect with it.
            NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);
            if (Request.QueryString[Models.Common.Querystring.ContentArea] != null)
            {
                queryString.Add(Models.Common.Querystring.Review, Models.Common.Querystring.ContentArea);
                queryString.Add(Models.Common.Querystring.ExamRecordId, Request.QueryString[Models.Common.Querystring.ExamRecordId]);
                queryString.Add(Models.Common.Querystring.ContentArea, Request.QueryString[Models.Common.Querystring.ContentArea]);
                queryString.Add(Models.Common.Querystring.Index, examQuestion.QuestionNo.ToString());
                queryString.Add(Models.Common.Querystring.LstQuestionIDs, Request.QueryString[Models.Common.Querystring.LstQuestionIDs]);
            }
            else
            {
                queryString.Add(Models.Common.Querystring.ExamRecordId, examQuestion.ExamRecordId.ToString());
                if (Request.QueryString[Models.Common.Querystring.Review] != null && Request.QueryString[Models.Common.Querystring.Review] != string.Empty)
                    queryString.Add(Models.Common.Querystring.Review, Request.QueryString[Models.Common.Querystring.Review]);
            }


            return RedirectToCurrentUmbracoPage(HttpUtility.UrlDecode(queryString.ToString()));

        }

        //[ValidateAntiForgeryToken]
        public ActionResult SubmitTakeExam(bl.Models.ExamList model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "*Please select an exam and a test mode.");
                return CurrentUmbracoPage();
            }
            else
            {
                try
                {
                    //Recreate querystring and redirect with it.
                    NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);
                    queryString.Add(Models.Common.Querystring.ExamId, model.SelectedExam);
                    queryString.Add(Models.Common.Querystring.ExamType, model.SelectedMode);
                    string url = Umbraco.Content((int)bl.Models.Common.SiteNode.ExamInstructions).Url() + "?" + System.Web.HttpUtility.UrlDecode(queryString.ToString());

                    return Redirect(url);
                }
                catch (Exception ex)
                {
                    Logger.Error<blExamController>(ex);
                    ModelState.AddModelError("", "*An error has occured.  Please try again.");
                    return CurrentUmbracoPage();
                }
            }
        }
        #endregion



        #region "Private Functions"
        private ExamResult ObtainExamResultOverview(int examRecordId)
        {
            //
            ExamResult examResult = new ExamResult();
            examResult.ExamRecordId = examRecordId;

            //Obtain exam records from db.
            ExamRecord examRecord = repoExamRecord.GetRecord_ById(examRecordId);
            ExamAnswerSet examAnswerSet = repoExamAnswerSet.GetRecord_ByExamRecordId(examRecordId);

            //Obtain test data
            IPublishedContent ipExam = Umbraco.Content(examRecord.ExamId);
            examResult.CorrectAnswerCount = repoExamAnswer.GetCount_ByExamAnswerSetId_CorrectAnswers(examAnswerSet.ExamAnswerSetId);
            examResult.TotalAnswerCount = repoExamAnswer.GetCount_ByExamAnswerSetId(examAnswerSet.ExamAnswerSetId);
            //if (examRecord.SubmittedDate != null)
            //{
            //    //Add date/time to title
            //    examResult.Title = ipExam.Name + " - " + Convert.ToDateTime(examRecord.SubmittedDate).ToString("MMM d, yyyy @ h:mm") + Convert.ToDateTime(examRecord.SubmittedDate).ToString("tt").ToLower();
            //}
            //else
            //{
            examResult.Title = ipExam.Name;
            //}
            examResult.SubmittedDate = examRecord.SubmittedDate;


            //Obtain name and counts for content area
            examResult.LstContentAreaResults = new List<ExamResult>();
            foreach (IPublishedContent ipContentArea in ipExam.Children)
            {
                ExamResult _result = new ExamResult();
                _result.Id = ipContentArea.Id;
                _result.Title = ipContentArea.Name;
                _result.CorrectAnswerCount = repoExamAnswer.GetCount_ByExamAnswerSetId_ContentAreaId_CorrectAnswer(examAnswerSet.ExamAnswerSetId, ipContentArea.Id);
                _result.TotalAnswerCount = repoExamAnswer.GetCount_ByExamAnswerSetId_ContentAreaId(examAnswerSet.ExamAnswerSetId, ipContentArea.Id);

                _result.LstQuestionIDs = new HashSet<int>();
                foreach (IPublishedContent ipQuestion in ipContentArea.Children) { _result.LstQuestionIDs.Add(ipQuestion.Id); }

                NameValueCollection caQueryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
                caQueryString.Add(Models.Common.Querystring.Review, Models.Common.Querystring.ContentArea);
                caQueryString.Add(Models.Common.Querystring.ExamRecordId, examResult.ExamRecordId.ToString());
                caQueryString.Add(Models.Common.Querystring.ContentArea, ipContentArea.Id.ToString());
                caQueryString.Add(Models.Common.Querystring.Index, "1");
                caQueryString.Add(Models.Common.Querystring.LstQuestionIDs, string.Join(",", _result.LstQuestionIDs));

                _result.RedirectUrl = Umbraco.Content((int)bl.Models.Common.SiteNode.ExamReview).Url() + "?" + System.Web.HttpUtility.UrlDecode(caQueryString.ToString());

                examResult.LstContentAreaResults.Add(_result);
            }


            //Create next pg url
            NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            queryString.Add(Models.Common.Querystring.ExamRecordId, examRecordId.ToString());
            examResult.RedirectUrl = Umbraco.Content((int)bl.Models.Common.SiteNode.ExamReview).Url() + "?" + System.Web.HttpUtility.UrlDecode(queryString.ToString());

            //
            return examResult;
        }
        private void LogMemberOut()
        {
            // Log member out
            Session.Clear();
            Session.Abandon();
            Roles.DeleteCookie();
            FormsAuthentication.SignOut();
        }
        #endregion
    }
}