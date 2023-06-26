using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Umbraco.Core.IO;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Examine;
using Umbraco.Web;
using Umbraco.Web.PublishedModels;

namespace bl.Models
{
    public sealed class Common
    {
        #region "Properties"
        public enum SiteNode : int
        {
            Account = 1190,
            Account_ChangePassword = 5394,
            Account_GuestAccount = 5373,
            Account_MemberExams = 5393,
            Account_PurchaseHistory = 5395,
            Account_ThankYou = 5405,
            Blog = 1375,
            Exam = 5370,
            ExamInstructions = 4190,
            ExamPrep = 1091,
            ExamReview = 5382,
            Exams = 1206,
            ForgotPassword = 5378,
            FreePracticeTest = 1066,
            Home = 1058,
            Login = 1184,
            Media_BlogPosts = 1382,
            ResetPassword = 5410,
            SignUp = 1183,
            SpecialOffers = 1207,
            SWTPPricing = 1115,
            TutoringServices = 183053
        }
        public struct NodeProperties
        {
            public const string AdditionalNotes = "additionalNotes";
            public const string AddQuerystring = "addQuerystring";
            public const string AnswerSets = "answerSets";
            public const string ArticleImage = "articleImage";
            public const string Categories = "categories";
            public const string ClientEmail = "clientEmail";
            public const string Content = "content";
            public const string DisableComments = "disableComments";
            public const string Duration = "duration";
            public const string ExamGroup = "examGroup";
            public const string Exams = "exams";
            public const string Feature01 = "feature01";
            public const string Feature02 = "feature02";
            public const string Feature03 = "feature03";
            public const string FooterLogo = "footerLogo";
            public const string FooterNav = "footerNav";
            public const string From = "from";
            public const string GuestMessage = "guestMessage";
            public const string GuestSubnav = "guestSubnav";
            public const string HideFromCalifornia = "hideFromCalifornia";
            public const string HideFromUSA = "hideFromUSA";
            public const string Id = "id";
            public const string Image = "image";
            public const string Link = "link";
            public const string Links = "links";
            public const string MemberMessage = "memberMessage";
            public const string MemberSubnav = "memberSubnav";
            public const string Message = "message";
            public const string NavigationTitleOverride = "navigationTitleOverride";
            public const string PreviousUrl = "previousUrl";
            public const string Price = "price";
            public const string ProductDescription = "productDescription";
            public const string ProductImage = "productImage";
            public const string ProductName = "productName";
            public const string ProductPrice = "productPrice";
            public const string PostDate = "postDate";
            public const string PostDateSortable = "postDateSortable";
            public const string QuestionText = "questionText";
            public const string Rationale = "rationale";
            public const string RedirectPage = "redirectPage";
            public const string SEOChecker = "SEOChecker";
            public const string SEODescription = "sEODescription";
            public const string ShowInMainNavigation = "showInMainNavigation";
            public const string ShowProductSnippet = "showProductSnippet";
            public const string SiteLogo = "siteLogo";
            public const string Source = "source";
            public const string Subject = "subject";
            public const string SubscriptionTime = "subscriptionTime";
            public const string Subtitle = "subtitle";
            public const string SuggestedStudyDescription = "suggestedStudyDescription";
            public const string SuggestedStudyLinks = "suggestedStudyLinks";
            public const string SuperContentArea = "superContentArea";
            public const string SuperuserPassword = "superuserPassword";
            public const string Testimonials = "testimonials";
            public const string Title = "title";
        }
        public struct DocType
        {
            public const string AnswerSet = "answerSet";
            public const string Blog = "blog";
            public const string Common = "common";
            public const string ContentArea = "contentArea";
            public const string Day = "day";
            public const string Exam = "exam";
            public const string Exams = "exams";
            public const string ExamFree = "examFree";
            public const string ExamInstructions = "examInstructions";
            public const string ExamPaid = "examPaid";
            public const string ExamReview = "examReview";
            public const string Footer = "footer";
            public const string Home = "home";
            public const string Member = "Member";
            public const string Month = "month";
            public const string Offer = "offer";
            public const string Post = "post";
            public const string Question = "question";
            public const string RedirectToPage = "redirectToPage";
            public const string RSS = "rSS";
            public const string SignUp = "signUp";
            public const string SiteSettings = "siteSettings";
            public const string SocialMedia = "socialMedia";
            public const string SpecialOffers = "specialOffers";
            public const string Year = "year";
        }
        public struct Crop
        {
            public const string Portfolio_162x262 = "Portfolio_162x262";
            public const string Square_250x250 = "Square_250x250";
        }
        public static List<string> ExamDocTypes = new List<string> {
            DocType.ExamInstructions,
            DocType.ExamReview,
            DocType.Exam};
        public struct Controller
        {
            public const string Account = "blAccount";
            public const string ApiMigrateData = "ApiMigrateData";
            public const string Blog = "blBlog";
            public const string Common = "blCommon";
            public const string Exam = "blExam";
            public const string Member = "blMember";
        }
        public struct Action
        {
            public const string PrepareExam = "PrepareExam";
            public const string RenderAccountLinks = "RenderAccountLinks";
            public const string RenderPurchasedExams = "RenderPurchasedExams";
            public const string RenderChangePassword = "RenderChangePassword";
            public const string RenderResetPassword = "RenderResetPassword";
            public const string RenderExam = "RenderExam";
            public const string RenderExamResults = "RenderExamResults";
            public const string RenderExamReview = "RenderExamReview";
            public const string RenderExamReviewList = "RenderExamReviewList";
            public const string RenderFooter = "RenderFooter";
            public const string RenderForgotPassword = "RenderForgotPassword";
            public const string RenderHeader = "RenderHeader";
            public const string RenderList = "RenderList";
            public const string RenderLogin = "RenderLogin";
            public const string RenderMobileNavigation = "RenderMobileNavigation";
            public const string RenderPost = "RenderPost";
            public const string RenderPurchaseForm = "RenderPurchaseForm";
            public const string RenderPurchaseHistory = "RenderPurchaseHistory";
            public const string RenderSignUp = "RenderSignUp";
            public const string RenderSignupPanel = "RenderSignupPanel";
            public const string RenderTutoringServices = "RenderTutoringServices";
            public const string SubmitExam = "SubmitExam";
            public const string SubmitExamPurchase_Bundle = "SubmitExamPurchase_Bundle";
            public const string SubmitExamPurchase_CompleteSet = "SubmitExamPurchase_CompleteSet";
            public const string SubmitExamPurchase_Single = "SubmitExamPurchase_Single";
            public const string SubmitExamReview = "SubmitExamReview";
            public const string SubmitLogin = "SubmitLogin";
            public const string SubmitLogout = "SubmitLogout";
            public const string SubmitPasswordChange = "SubmitPasswordChange";
            public const string SubmitPasswordReset = "SubmitPasswordReset";
            public const string SubmitPasswordUpdate = "SubmitPasswordUpdate";
            public const string SubmitSignUp = "SubmitSignUp";
            public const string SubmitTakeExam = "SubmitTakeExam";
            public const string SubmitTutoringRequest = "SubmitTutoringRequest";

        }
        public struct PartialPath
        {
            public const string Account_AcceptedPayments = "~/Views/Partials/Account/AcceptedPayments.cshtml";
            public const string Account_AccountLinks = "~/Views/Partials/Account/AccountLinks.cshtml";
            public const string Account_PurchaseExams = "~/Views/Partials/Account/PurchaseExams.cshtml";
            public const string Account_PurchaseHistory = "~/Views/Partials/Account/PurchaseHistory.cshtml";
            public const string Account_ReviewExams_InProgress = "~/Views/Partials/Account/ReviewExams_InProgress.cshtml";
            public const string Account_ReviewExams_Submitted = "~/Views/Partials/Account/ReviewExams_Submitted.cshtml";
            public const string Account_ReviewExams_Submitted_StudyMode = "~/Views/Partials/Account/ReviewExams_Submitted_StudyMode.cshtml";
            public const string Account_ReviewExams_SubmittedFree = "~/Views/Partials/Account/ReviewExams_SubmittedFree.cshtml";
            public const string Account_TakeExam = "~/Views/Partials/Account/TakeExam.cshtml";
            public const string BlockList_Default = "~/Views/Partials/BlockList/Default.cshtml";
            public const string Blog_Filter = "~/Views/Partials/Blog/Filter.cshtml";
            public const string Blog_List = "~/Views/Partials/Blog/List.cshtml";
            public const string Blog_Post = "~/Views/Partials/Blog/Post.cshtml";
            public const string Common_Footer = "~/Views/Partials/Common/Footer.cshtml";
            public const string Common_GridOutline = "~/Views/Partials/Common/GridOutline.cshtml";
            public const string Common_Header = "~/Views/Partials/Common/Header.cshtml";
            public const string Common_Meta = "~/Views/Partials/Common/CommonMeta.cshtml";
            public const string Common_MobileNav = "~/Views/Partials/Common/MobileNav.cshtml";
            public const string Common_PanelSignUp = "~/Views/Partials/Common/Panel-SignUp.cshtml";
            public const string Exams_ExamInstructions = "~/Views/Partials/Exams/ExamInstructions.cshtml";
            public const string Exams_ExamQuestion = "~/Views/Partials/Exams/ExamQuestion.cshtml";
            public const string Exams_ExamReview = "~/Views/Partials/Exams/ExamReview.cshtml";
            public const string Exams_ExamOverview = "~/Views/Partials/Exams/ExamOverview.cshtml";
            public const string Home_Features = "~/Views/Partials/Home/Features.cshtml";
            public const string Home_Testimonials = "~/Views/Partials/Home/Testimonials.cshtml";
            public const string Membership_ChangePassword = "~/Views/Partials/Membership/ChangePassword.cshtml";
            public const string Membership_ForgotPassword = "~/Views/Partials/Membership/ForgotPassword.cshtml";
            public const string Membership_ResetPassword = "~/Views/Partials/Membership/ResetPassword.cshtml";
            public const string Membership_Login = "~/Views/Partials/Membership/Login.cshtml";
            public const string Membership_SignUp = "~/Views/Partials/Membership/SignUp.cshtml";
            public const string Membership_TutoringServices = "~/Views/Partials/Membership/TutoringServices.cshtml";
            public const string Snippet_Blogpost = "~/Views/Partials/GoogleSnippets/Snippet_Blogpost.cshtml";
            public const string Snippet_Product = "~/Views/Partials/GoogleSnippets/Snippet_Product.cshtml";
            public const string Snippet_SiteLogo = "~/Views/Partials/GoogleSnippets/Snippet_SiteLogo.cshtml";
            public const string UmbracoEditButton = "~/App_Plugins/Flaeng.Umbraco.EditButton/EditButton.cshtml";
        }
        public struct Querystring
        {
            public const string ExamId = "ExamId";
            public const string ExamRecordId = "ExamRecordId";
            public const string ExamType = "ExamType";
            public const string QuestionNo = "QuestionNo";
            public const string Review = "Review";
            public const string ReviewMode = "ReviewMode";
            public const string ContentArea = "ContentArea";
            public const string LstQuestionIDs = "LstQuestionIDs";
            public const string Index = "Index";
            public const string Count = "Count";
        }
        public struct ExamMode
        {
            public const string FreeMode = "FreeMode";
            public const string StudyMode = "StudyMode";
            public const string TimedMode = "TimedMode";
        }
        public struct TempData
        {
            public const string Email = "Email";
            public const string ExamQuestion = "examQuestion";
            public const string LockedOut = "LockedOut";
            public const string LoginSuccess = "LoginSuccess";
            public const string PasswordResetSuccessfully = "PasswordResetSuccessfully";
            public const string RedirectTo = "RedirectTo";
            public const string RedirectToLogin = "RedirectToLogin";
            public const string SessionTimedOut = "SessionTimedOut";
            public const string Success = "Success";
            public const string TimeIsUp = "TimeIsUp";
            public const string UserSuccessfullyCreated = "UserSuccessfullyCreated";
        }
        public struct Misc
        {
            public const string ExamRecordId = "ExamRecordId";
            public const string Footer = "Footer";
            public const string Header = "Header";
            public const string MailChimpApiKey = "MailChimp.Api.Key";
            public const string MailChimpListID = "MailChimp.ListID";
            public const string StripeApiKey = "StripeApiKey";
            public const string StripeEndpointSecret = "StripeEndpointSecret";
            public const string StripePublicApiKey = "StripePublicApiKey";
            public const string StripeSignature = "Stripe-Signature";            
        }
        #endregion


        #region "Methods"
        public static string StripHTML(string input, int? maxLength = null)
        {
            //Remove all tags from incoming string.
            input = Regex.Replace(input.Trim(), "<.*?>", String.Empty);
            input = Regex.Replace(input, @"<[^>]*>", String.Empty); 

            //If provided, truncate incoming string.
            if (maxLength != null)
            {
                if (input.Length > (int)maxLength)
                {
                    input = input.Substring(0, (int)maxLength) + "...";
                }
            }

            return input.Trim();
        }

        //public static void SaveErrorMessage(Exception ex, StringBuilder sb, Type type, bool saveAsWarning = false)
        //{
        //    StringBuilder sbGeneralInfo = new StringBuilder();

        //    try
        //    {
        //        UmbracoHelper umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
        //        bool SaveErrorMsgs = false;
        //        IPublishedContent ipData = umbracoHelper.TypedContentAtRoot().FirstOrDefault(x => x.ContentType.Alias.Equals(DocType.DataLayer));
        //        IPublishedContent ipAppStartEvents = ipData.FirstChild<IPublishedContent>(x => x.ContentType.Alias.Equals(DocType.AppStartEvents));
        //        if (ipAppStartEvents.HasProperty(NodeProperties.SaveErrorMessage))
        //            SaveErrorMsgs = ipAppStartEvents.GetPropertyValue<bool>(NodeProperties.SaveErrorMessage);

        //        if (SaveErrorMsgs)
        //        {
        //            try
        //            {
        //                StackTrace st = new StackTrace(ex, true);
        //                StackFrame frame = st.GetFrame(0);
        //                sbGeneralInfo.AppendLine("fileName: " + frame.GetFileName());
        //                sbGeneralInfo.AppendLine("methodName: " + frame.GetMethod().Name);
        //                sbGeneralInfo.AppendLine("line: " + frame.GetFileLineNumber());
        //                sbGeneralInfo.AppendLine("col: " + frame.GetFileColumnNumber());
        //            }
        //            catch (Exception exc)
        //            {
        //                if (!saveAsWarning)
        //                {
        //                    sbGeneralInfo.AppendLine("Error attempting to add stack information in SaveErrorMessage()");
        //                    sbGeneralInfo.AppendLine(exc.ToString());
        //                }
        //            }
        //            sbGeneralInfo.AppendLine(sb.ToString());

        //            if (saveAsWarning)
        //            {
        //                LogHelper.Warn(type, sbGeneralInfo.ToString());
        //            }
        //            else
        //            {
        //                LogHelper.Error(type, sbGeneralInfo.ToString(), ex);
        //            }
        //        }
        //    }
        //    catch (Exception error)
        //    {
        //        LogHelper.Error(typeof(Common), "Error Saving Exception Message.  Original Data: " + sbGeneralInfo.ToString() + " ||| " + ex.ToString(), error);
        //    }
        //}
        #endregion
    }
}
