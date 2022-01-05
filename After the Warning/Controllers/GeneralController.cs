using Models;
using SEOChecker.Core.Extensions.UmbracoExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using ContentModels = Umbraco.Web.PublishedModels;
using formulate.app.Types;
using System.Text;

namespace Controllers
{
    public class GeneralController : SurfaceController
    {
        public static Models.TopLevelContent ObtainTopLevelData(IPublishedContent ipModel, string url)
        {
            //Instantiate variables
            Models.TopLevelContent PgContent = new TopLevelContent();

            //Show analytics code if not a dev site.
            if (url.Contains("dev.")) { PgContent.ShowAnalytics = false; }
            else if (url.Contains("staging.")) { PgContent.ShowAnalytics = false; }
            else if (url.Contains("localhost")) { PgContent.ShowAnalytics = false; }
            else
            {
                //Obtain meta data from SeoChecker
                PgContent.Meta = ipModel.Value<SEOChecker.MVC.MetaData>("seoChecker");
            }

            return PgContent;
        }
        public static Models.ContactUsContent ObtainContactUsContent(ContentModels.ContactUs cmModel, HtmlHelper Html)
        {

            //Instantiate variables
            Models.ContactUsContent PgContent = new ContactUsContent();

            //Obtain page summary
            PgContent.ContactSummary = Html.Raw(Html.ReplaceLineBreaks(cmModel.ContactSummary));

            //Obtain the form
            PgContent.PickedForm = cmModel.Value<ConfiguredFormInfo>("formPicker");

            return PgContent;
        }
        public static ManageAcctContent ObtainManageAcctContent(System.Security.Principal.IPrincipal User, UmbracoHelper Umbraco, IPublishedContent ipCurrentPg)
        {
            //Instantiate variables
            Models.ManageAcctContent PgContent = new ManageAcctContent();
            PgContent.Inactive = "inactive";

            if (!User.Identity.IsAuthenticated)
            {
                //Redirect to login page.
                PgContent.Redirect = true;
                PgContent.RedirectTo = Umbraco.Content((int)(Models.Common.siteNode.Login)).Url();
            }
            else if (ipCurrentPg.ContentType.Alias == Common.docType.ManageAccount)
            {
                PgContent.Redirect = true;
                PgContent.RedirectTo = ipCurrentPg.Children.First().Url();
            }
            else
            {
                //Instantiate variables.
                IPublishedContent ipHome = Umbraco.Content((int)(Common.siteNode.Home));
                PgContent.CredentialsUrl = Umbraco.Content((int)(Models.Common.siteNode.EditAccount)).Url();
                PgContent.IlluminationStoryUrl = Umbraco.Content((int)(Models.Common.siteNode.AddEditIlluminationStory)).Url();

                //Make fields active if Illumination has occured.
                if (ipHome.Value<Boolean>(Common.NodeProperties.activateIlluminationControls) == true) { PgContent.Inactive = string.Empty; }
            }

            PgContent.IsManageAcctPg = (ipCurrentPg.ContentType.Alias == Common.docType.ManageAccount);

            return PgContent;
        }
        public static MainNavContent ObtainMainNavContent(System.Security.Principal.IPrincipal User, UmbracoHelper Umbraco, IPublishedContent ipPg)
        {
            //Instantiate variables
            MainNavContent mainNavContent = new MainNavContent();

            //
            mainNavContent.ipHome = ipPg.Root();
            mainNavContent.isLoggedIn = User.Identity.IsAuthenticated;

            //Obtain the url to the search page
            mainNavContent.searchUrl = Umbraco.Content((int)Common.siteNode.Search).Url();

            //Make fields active if Illumination has occured.
            mainNavContent.activateIlluminationControls = mainNavContent.ipHome.Value<Boolean>(Common.NodeProperties.activateIlluminationControls);

            //Get main navigation
            foreach (var ipLvl1 in mainNavContent.ipHome.Children.Where(x => x.IsVisible()))
            {
                if (!mainNavContent.activateIlluminationControls)
                { if (ipLvl1.GetDocumentTypeAlias() == Common.docType.IlluminationStoryList) { continue; } }


                //Instantiate variable
                navigationLink lnkLvl1 = new navigationLink();

                //Add data to link record
                lnkLvl1.id = ipLvl1.Id;
                lnkLvl1.level = 1;
                lnkLvl1.name = ipLvl1.Name;
                lnkLvl1.url = ipLvl1.Url();

                //Add level 2 nav
                if (ipLvl1.Value<Boolean>(Common.NodeProperties.hideChildrenFromNavigation) == false)
                {
                    if (ipLvl1.Children.Any(x => x.IsVisible()))
                    {
                        foreach (var ipLvl2 in ipLvl1.Children.Where(x => x.IsVisible()))
                        {
                            //Instantiate variable
                            navigationLink lnkLvl2 = new navigationLink();

                            //Add data to link record
                            lnkLvl2.id = ipLvl2.Id;
                            lnkLvl2.level = 2;
                            lnkLvl2.name = ipLvl2.Name;
                            lnkLvl2.url = ipLvl2.Url();

                            //Add level 3 nav
                            if (ipLvl2.Value<Boolean>(Common.NodeProperties.hideChildrenFromNavigation) == false)
                            {
                                if (ipLvl2.Children.Any(x => x.IsVisible()))
                                {
                                    foreach (var ipLvl3 in ipLvl2.Children.Where(x => x.IsVisible()))
                                    {
                                        //Skip the following doctypes
                                        if (ipLvl3.GetDocumentTypeAlias() == Common.docType.UDateFoldersyFolderYear) { continue; }
                                        //Instantiate variable
                                        navigationLink lnkLvl3 = new navigationLink();

                                        //Add data to link record
                                        lnkLvl3.id = ipLvl3.Id;
                                        lnkLvl3.level = 3;
                                        lnkLvl3.name = ipLvl3.Name;
                                        lnkLvl3.url = ipLvl3.Url();

                                        //Add records to list
                                        lnkLvl2.lstChildLinks.Add(lnkLvl3);
                                    }
                                }
                            }

                            //Add records to list
                            lnkLvl1.lstChildLinks.Add(lnkLvl2);
                        }
                    }
                }

                //Add records to list
                mainNavContent.lstMainNavlinks.Add(lnkLvl1);
            }

            //Get minor navigation
            foreach (var ipLvl1 in mainNavContent.ipHome.Children.Where(x => x.Value<bool>(Common.NodeProperties.showInMinorNavigation) == true))
            {
                //Skip the following if the following exists.
                if (mainNavContent.isLoggedIn)
                {
                    if (ipLvl1.GetDocumentTypeAlias() == Common.docType.Login) { continue; }
                    if (ipLvl1.GetDocumentTypeAlias() == Common.docType.CreateAccount) { continue; }
                }
                else
                {
                    if (ipLvl1.GetDocumentTypeAlias() == Common.docType.Logout) { continue; }
                    if (ipLvl1.GetDocumentTypeAlias() == Common.docType.ManageAccount) { continue; }
                }


                //Instantiate variable
                navigationLink lnkLvl1 = new navigationLink();

                //Add data to link record
                lnkLvl1.id = ipLvl1.Id;
                lnkLvl1.level = 1;
                lnkLvl1.name = ipLvl1.Name;
                lnkLvl1.url = ipLvl1.Url();

                //Add level 2 nav
                if (ipLvl1.Value<Boolean>(Common.NodeProperties.hideChildrenFromNavigation) == false)
                {
                    foreach (var ipLvl2 in ipLvl1.Children.Where(x => x.Value<bool>(Common.NodeProperties.showInMinorNavigation) == true))
                    {

                        if (!mainNavContent.activateIlluminationControls)
                        {
                            if (ipLvl2.GetDocumentTypeAlias() == Common.docType.AddEditIlluminationStory) { continue; }
                        }

                        //Instantiate variable
                        navigationLink lnkLvl2 = new navigationLink();

                        //Add data to link record
                        lnkLvl2.id = ipLvl2.Id;
                        lnkLvl2.level = 2;
                        lnkLvl2.name = ipLvl2.Name;
                        lnkLvl2.url = ipLvl2.Url();

                        //Add level 3 nav
                        if (ipLvl2.Value<Boolean>(Common.NodeProperties.hideChildrenFromNavigation) == false)
                        {
                            foreach (var ipLvl3 in ipLvl2.Children.Where(x => x.Value<bool>(Common.NodeProperties.showInMinorNavigation) == true))
                            {
                                //Instantiate variable
                                navigationLink lnkLvl3 = new navigationLink();

                                //Add data to link record
                                lnkLvl3.id = ipLvl3.Id;
                                lnkLvl3.level = 3;
                                lnkLvl3.name = ipLvl3.Name;
                                lnkLvl3.url = ipLvl3.Url();

                                //Add records to list
                                lnkLvl2.lstChildLinks.Add(lnkLvl3);
                            }
                        }

                        //Add records to list
                        lnkLvl1.lstChildLinks.Add(lnkLvl2);
                    }
                }

                //Add records to list
                mainNavContent.lstMinorNavlinks.Add(lnkLvl1);
            }


            return mainNavContent;
        }
        public static PaginationContent ObtainPaginationContent(Models.Pagination Model, string url)
        {
            //
            PaginationContent paginationContent = new PaginationContent();
            //int index = 1;

            //build base url for navigation links
            paginationContent.baseUri = new Uri(url);

            // this gets all the query string key value pairs as a collection
            paginationContent.queryString = HttpUtility.ParseQueryString(paginationContent.baseUri.Query);

            // this removes the key if exists
            paginationContent.queryString.Remove("pageNo");
            paginationContent.queryString.Add("pageNo", "");

            // this gets the page path from root without QueryString
            paginationContent.baseUrl = paginationContent.baseUri.GetLeftPart(UriPartial.Path);
            paginationContent.baseUrl = paginationContent.queryString.Count > 0 ? String.Format("{0}?{1}", paginationContent.baseUrl, paginationContent.queryString) : paginationContent.baseUrl;

            //Set values for page indexes
            paginationContent.previous = Model.pageNo - 1;
            paginationContent.next = Model.pageNo + 1;

            //Determine if prev/next values are correct.
            if (paginationContent.previous < 1) { paginationContent.previous = 1; }
            if (paginationContent.next > Model.totalPages) { paginationContent.next = Model.totalPages; }


            return paginationContent;
        }
        public static PersonalAccountsContent ObtainPersonalAcctContent(UmbracoHelper Umbraco)
        {
            //Instantiate variables
            PersonalAccountsContent personalAccountsContent = new PersonalAccountsContent();

            try
            {
                //
                IPublishedContent ipHome = Umbraco.Content((int)(Common.siteNode.Home));
                //IPublishedContent ipImg = Umbraco.Media(ipHome.Value<int>(Common.NodeProperties.personalPhoto));
                IPublishedContent ipImg = ipHome.Value<IPublishedContent>(Common.NodeProperties.personalPhoto);

                personalAccountsContent.imgUrl = ipImg.GetCropUrl(Common.crop.Square_500x500);
                personalAccountsContent.lstArticles = ipHome.Value<List<IPublishedContent>>(Common.NodeProperties.personalArticles);
            }
            catch (Exception ex)
            {
                //Save error message to umbraco
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"GeneralController.cs : ObtainPersonalAcctContent()");
                Common.SaveErrorMessage(ex, sb, typeof(GeneralController));
            }

            return personalAccountsContent;
        }
        public static SupportPnlContent ObtainSupportPnlContent(UmbracoHelper Umbraco, int id)
        {
            //Instantiate variables
            SupportPnlContent supportPnlContent = new SupportPnlContent();

            //IPublishedContent currentNode = UmbracoContext.Current.PublishedContentRequest.PublishedContent;
            supportPnlContent.supportUsUrl = "";
            supportPnlContent.showPnl = (id == (int)Common.siteNode.Home);

            //Only retrieve data if the panel can be shown
            if (supportPnlContent.showPnl)
            {
                //Get url to donation page
                supportPnlContent.supportUsUrl = Umbraco.Content((int)Common.siteNode.Donate).Url();
            }

            return supportPnlContent;
        }
        public static TitlePanelContent ObtainTitlePanelContent(IPublishedContent ipModel)
        {
            TitlePanelContent titlePanelContent = new TitlePanelContent();


            //Other variables
            titlePanelContent.heightClass = "narrow";
            List<DateTime> lstDateRange = new List<DateTime>();

            //Obtain page's doctype
            titlePanelContent.docType = ipModel.GetDocumentTypeAlias();

            //Check if the parent doctype should be used instead.
            if (titlePanelContent.docType != Common.docType.Home && ipModel.Parent.GetDocumentTypeAlias() == Common.docType.ManageAccount)
            {
                titlePanelContent.docType = Common.docType.ManageAccount;
            }
            else if (titlePanelContent.docType != Common.docType.Home && ipModel.Parent.GetDocumentTypeAlias() == Common.docType.IlluminationStatistics)
            {
                titlePanelContent.docType = ipModel.Parent.GetDocumentTypeAlias();
            }

            //
            switch (titlePanelContent.docType)
            {
                case Common.docType.Message:
                    //Obtain list of all dates
                    if (ipModel.HasValue(Common.NodeProperties.dateOfMessages))
                    {
                        lstDateRange = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DateTime>>(ipModel.Value<string>(Common.NodeProperties.dateOfMessages));
                    }

                    //Determine proper date range for messages
                    if (lstDateRange != null)
                    {
                        switch (lstDateRange.Count)
                        {
                            case 0: //Leave blank
                                break;

                            case 1:
                                titlePanelContent.sbDateRange.Append(lstDateRange.First().ToString("MMMM d, yyyy"));
                                break;

                            case 2:
                                titlePanelContent.sbDateRange.Append(lstDateRange.First().ToString("MMMM d"));
                                titlePanelContent.sbDateRange.Append(" and ");
                                titlePanelContent.sbDateRange.Append(lstDateRange.Last().ToString("MMMM d, yyyy"));
                                break;

                            default: //More than 2 dates in list
                                titlePanelContent.sbDateRange.Append(lstDateRange.First().ToString("MMMM d"));
                                titlePanelContent.sbDateRange.Append(" thru ");
                                titlePanelContent.sbDateRange.Append(lstDateRange.Last().ToString("MMMM d, yyyy"));
                                break;
                        }
                    }

                    //Obtain visionary's name and page title
                    if (ipModel.Parent.Parent.Parent.Parent.GetDocumentTypeAlias() == Common.docType.Visionary)
                    {
                        titlePanelContent.visionaryName = ipModel.AncestorsOrSelf().FirstOrDefault(x => x.GetDocumentTypeAlias() == Common.docType.Visionary).Value<String>(Common.NodeProperties.visionarysName);
                    }
                    else
                    {
                        titlePanelContent.visionaryName = ipModel.Parent.Parent.Parent.Parent.Name;
                    }

                    titlePanelContent.Name = ipModel.Name;
                    break;


                case Common.docType.WebmasterMessage:
                    //Obtain list of all dates
                    if (ipModel.HasValue(Common.NodeProperties.dateOfMessages))
                    {
                        lstDateRange = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DateTime>>(ipModel.Value<string>(Common.NodeProperties.dateOfMessages));
                    }

                    //Determine proper date range for messages
                    if (lstDateRange != null)
                    {
                        switch (lstDateRange.Count)
                        {
                            case 0: //Leave blank
                                break;

                            case 1:
                                titlePanelContent.sbDateRange.Append(lstDateRange.First().ToString("MMMM dd"));
                                break;

                            case 2:
                                titlePanelContent.sbDateRange.Append(lstDateRange.First().ToString("MMMM dd"));
                                titlePanelContent.sbDateRange.Append(" and ");
                                titlePanelContent.sbDateRange.Append(lstDateRange.Last().ToString("MMMM dd"));
                                break;

                            default: //More than 2 dates in list
                                titlePanelContent.sbDateRange.Append(lstDateRange.First().ToString("MMMM dd"));
                                titlePanelContent.sbDateRange.Append(" thru ");
                                titlePanelContent.sbDateRange.Append(lstDateRange.Last().ToString("MMMM dd"));
                                break;
                        }
                    }

                    //Obtain name
                    titlePanelContent.visionaryName = ipModel.AncestorsOrSelf().FirstOrDefault(x => x.GetDocumentTypeAlias() == Common.docType.WebmasterMessageList).Name;
                    break;


                //case Common.docType.PrayerRequest:
                //    titlePanelContent.strH1 = Model.Value<String>(Common.NodeProperties.prayerTitle);

                //    //Generate user's name
                //    var CmMember = new ContentModels.Member(Umbraco.TypedMember(Model.Value<int>(Common.NodeProperties.prayerRequestMember)));
                //    StringBuilder sbAuthor = new StringBuilder();
                //    sbAuthor.Append(CmMember.FirstName);
                //    sbAuthor.Append("&nbsp;&nbsp;");
                //    sbAuthor.Append(CmMember.LastName);
                //    sbAuthor.Append(".");
                //    titlePanelContent.strH3 = sbAuthor.ToString();
                //    break;


                case Common.docType.Home:
                    titlePanelContent.heightClass = "tall";
                    titlePanelContent.topBanner = ipModel.Value<HtmlString>(Common.NodeProperties.topBanner);
                    break;


                case Common.docType.IlluminationStatistics:
                    titlePanelContent.ParentName = ipModel.Parent.Name;
                    titlePanelContent.Name = ipModel.Name;
                    break;


                case Common.docType.Standard:
                    titlePanelContent.title = ipModel.Value<string>(Common.NodeProperties.title);
                    titlePanelContent.subtitle = ipModel.Value<string>(Common.NodeProperties.subtitle);
                    if (ipModel.HasValue(Common.NodeProperties.originalSource))
                    {
                        foreach (IPublishedElement ipe in ipModel.Value<IEnumerable<IPublishedElement>>(Common.NodeProperties.originalSource))
                        {
                            if (ipe.HasValue(Common.NodeProperties.author))
                            {
                                titlePanelContent.sbCite.Append("by ");
                                titlePanelContent.sbCite.Append(ipe.Value<string>(Common.NodeProperties.author));
                                //titlePanelContent.sbCite.Append(ipe.Value<string>(Common.NodeProperties.author));
                            }
                            break;
                        }
                    }
                    break;


                default:
                    titlePanelContent.Name = ipModel.Name;
                    titlePanelContent.ParentName = ipModel.Parent.Name;
                    titlePanelContent.title = ipModel.Value<string>(Common.NodeProperties.title);
                    titlePanelContent.subtitle = ipModel.Value<string>(Common.NodeProperties.subtitle);
                    break;
            }


            return titlePanelContent;
        }
        public static BlockItemContent ObtainBlockItemContent(UmbracoHelper Umbraco, IPublishedContent ipModel)
        {
            BlockItemContent blockItemContent = new BlockItemContent();

            blockItemContent.AboutImgUrl = ipModel.Value<IPublishedContent>(Common.NodeProperties.thumbnail).GetCropUrl(Common.crop.Square_500x500);
            blockItemContent.Title = ipModel.Value<string>(Common.NodeProperties.title);
            blockItemContent.Url = ipModel.Url();

            return blockItemContent;
        }
    }
}
