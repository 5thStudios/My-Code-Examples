using Models;
using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using Umbraco.Web;
using Umbraco.Core;
using ContentModels = Umbraco.Web.PublishedModels;
using Examine;
using Examine.Search;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Examine;
using Umbraco.Web.Security;
using System.Collections.Generic;

namespace Controllers
{
    public class SearchController : SurfaceController
    {
        //Render Search Form
        public ActionResult RenderForm(MembershipHelper membershipHelper, UmbracoHelper umbracoHelper)
        {
            //Instantiate variables
            var searchList = new Models.SearchList();


            try
            {
                //Obtain search parameters
                if (!string.IsNullOrEmpty(Request.QueryString[Common.miscellaneous.SearchIn]))
                {
                    searchList.SearchIn = Request.QueryString[Common.miscellaneous.SearchIn];
                }
                if (!string.IsNullOrEmpty(Request.QueryString[Common.miscellaneous.SearchFor]))
                {
                    searchList.SearchFor = Request.QueryString[Common.miscellaneous.SearchFor];
                }


                if (!String.IsNullOrEmpty(searchList.SearchIn))
                {
                    //Set results section visible
                    searchList.ShowResults = true;


                    //Determine current page number 
                    var pageNo = 1;
                    if (!string.IsNullOrEmpty(Request.QueryString[Common.miscellaneous.PageNo]))
                    {
                        int.TryParse(Request.QueryString[Common.miscellaneous.PageNo], out pageNo);
                    }


                    //Determine what examine to search in
                    switch (searchList.SearchIn)
                    {
                        case Common.SearchIn.Articles:
                            ObtainByArticle(searchList, pageNo, umbracoHelper);
                            break;
                        case Common.SearchIn.Bible:
                            ObtainByScripture(searchList, pageNo, umbracoHelper);
                            break;
                        case Common.SearchIn.Illuminations:
                            ObtainByIlluminationStories(searchList, pageNo, membershipHelper);
                            break;
                        case Common.SearchIn.Messages:
                            ObtainByMessagesFromHeaven(searchList, pageNo);
                            break;
                            //case Common.SearchIn.Prayers:
                            //    ObtainByPrayerCorner(searchList, pageNo);
                            //    break;
                    }
                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"SearchController.cs : RenderForm()");
                sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(searchList));
                Common.SaveErrorMessage(ex, sb, typeof(SearchController));

                ModelState.AddModelError("", "*An error occured while performing your search request.");
            }

            return PartialView("~/Views/Partials/Search/_searchList.cshtml", searchList);
        }


        //Search by
        private void ObtainByIlluminationStories(Models.SearchList searchList, int pageNo, MembershipHelper membershipHelper)
        {
            //Instantiate variables
            //var memberShipHelper = new Umbraco.Web.Security.MembershipHelper(UmbracoContext.Current);
            searchList.ShowIlluminationStories = true;
            searchList.SearchInTitle = "Illumination Stories";

            try
            {
                if (!string.IsNullOrWhiteSpace(searchList.SearchFor))
                {
                    //Create list of fields to search in.              
                    List<string> lstFields = new List<string>();
                    lstFields.Add(Common.NodeProperties.title);
                    lstFields.Add(Common.NodeProperties.experienceType);
                    lstFields.Add(Common.NodeProperties.story);

                    //==========================================================================
                    //it returns the index in the var index
                    if (ExamineManager.Instance.TryGetIndex(Common.searchProviders.ExternalIndex, out var index))
                    {
                        //
                        ISearcher searcher = index.GetSearcher();
                        IBooleanOperation query = searcher.CreateQuery(null, BooleanOperation.And)
                            .NodeTypeAlias(Common.docType.IlluminationStory)
                            .And().GroupedOr(lstFields.ToArray(), searchList.SearchFor.MultipleCharacterWildcard());
                        ISearchResults searchResults = query.Execute(Int32.MaxValue);


                        if (searchResults.Any())
                        {
                            //Get item counts and total experiences.
                            searchList.Pagination.totalItems = searchResults.Count();


                            //Determine how many pages/items to skip and take, as well as the total page count for the search result.
                            if (searchList.Pagination.totalItems > searchList.Pagination.itemsPerPage)
                            {
                                searchList.Pagination.totalPages = (int)Math.Ceiling((double)searchList.Pagination.totalItems / (double)searchList.Pagination.itemsPerPage);
                            }
                            else
                            {
                                searchList.Pagination.itemsPerPage = searchList.Pagination.totalItems;
                                searchList.Pagination.totalPages = 1;
                            }


                            //Determine current page number
                            if (pageNo <= 0 || pageNo > searchList.Pagination.totalPages)
                            {
                                pageNo = 1;
                            }
                            searchList.Pagination.pageNo = pageNo;


                            //Determine how many pages/items to skip
                            if (searchList.Pagination.totalItems > searchList.Pagination.itemsPerPage)
                            {
                                searchList.Pagination.itemsToSkip = searchList.Pagination.itemsPerPage * (pageNo - 1);
                            }


                            //Convert list of SearchResults to list of classes
                            foreach (SearchResult sRecord in searchResults.Skip(searchList.Pagination.itemsToSkip).Take(searchList.Pagination.itemsPerPage))
                            {
                                try
                                {
                                    var storyLink = new Models.illuminationStoryLink();
                                    storyLink.experienceType = Common.DeserializeValues(sRecord[Common.NodeProperties.experienceType]).FirstOrDefault();
                                    storyLink.id = Convert.ToInt32(sRecord.Id);
                                    storyLink.title = sRecord[Common.NodeProperties.title];
                                    storyLink.url = Umbraco.Content(sRecord.Id).Url();


                                    //Obtain member 
                                    StringBuilder sbAuthor = new StringBuilder();
                                    ContentModels.Member CmMember;
                                    int memberId;

                                    if (int.TryParse(sRecord[Common.NodeProperties.member], out memberId))
                                    {
                                        IPublishedContent ipMember = membershipHelper.GetById(memberId);
                                        //CmMember = new ContentModels.Member(ipMember);
                                        if (ipMember != null)
                                        {
                                            CmMember = new ContentModels.Member(ipMember);

                                            sbAuthor.Append(CmMember.FirstName);
                                            sbAuthor.Append("&nbsp;&nbsp;&nbsp;");
                                            sbAuthor.Append(CmMember.LastName);
                                            sbAuthor.Append(".");
                                            storyLink.memberName = sbAuthor.ToString();
                                            storyLink.memberId = CmMember.Id;
                                        }
                                    }
                                    else
                                    {
                                        //Member id is not an int so attempt to parse as a guid
                                        if (GuidUdi.TryParse(sRecord[Common.NodeProperties.member], out GuidUdi memberUdi))
                                        {
                                            //IPublishedContent ipMember = Umbraco.Content(memberUdi.Guid);
                                            IPublishedContent ipMember = membershipHelper.GetById(memberUdi.Guid);
                                            if (ipMember != null)
                                            {
                                                CmMember = new ContentModels.Member(ipMember);

                                                sbAuthor.Append(CmMember.FirstName);
                                                sbAuthor.Append("&nbsp;&nbsp;&nbsp;");
                                                sbAuthor.Append(CmMember.LastName);
                                                sbAuthor.Append(".");
                                                storyLink.memberName = sbAuthor.ToString();
                                                storyLink.memberId = CmMember.Id;
                                            }
                                        }
                                    }

                                    searchList.lstStoryLink.Add(storyLink);
                                }
                                catch (Exception ex)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    sb.AppendLine(@"SearchController.cs : ObtainByIlluminationStories()");
                                    Common.SaveErrorMessage(ex, sb, typeof(SearchController));
                                }
                            }
                        }
                    }
                    //==========================================================================
                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"SearchController.cs : ObtainByIlluminationStories()");
                Common.SaveErrorMessage(ex, sb, typeof(SearchController));
            }
        }
        private void ObtainByMessagesFromHeaven(Models.SearchList searchList, int pageNo)
        {
            //Instantiate variables
            searchList.ShowMsgsFromHeaven = true;
            searchList.SearchInTitle = "Messages from Heaven";

            try
            {
                if (!string.IsNullOrWhiteSpace(searchList.SearchFor))
                {
                    //Create list of fields to search in.              
                    List<string> lstFields = new List<string>();
                    lstFields.Add(Common.NodeProperties.nodeName);
                    lstFields.Add(Common.NodeProperties.subtitle);
                    lstFields.Add(Common.NodeProperties.content);
                    lstFields.Add(Common.NodeProperties.originallyPostedBy);
                    lstFields.Add(Common.NodeProperties.originalPostUrl);

                    //Get all prayers
                    if (ExamineManager.Instance.TryGetIndex(Common.searchProviders.ExternalIndex, out var index))
                    {
                        //
                        ISearcher searcher = index.GetSearcher();
                        IBooleanOperation query = searcher.CreateQuery(null, BooleanOperation.And)
                            .NodeTypeAlias(Common.docType.Message)
                            .And().GroupedOr(lstFields.ToArray(), searchList.SearchFor.MultipleCharacterWildcard());
                        ISearchResults searchResults = query.Execute(Int32.MaxValue);

                        if (searchResults.Any())
                        {
                            //Get item counts and total experiences.
                            searchList.Pagination.totalItems = searchResults.Count();


                            //Determine how many pages/items to skip and take, as well as the total page count for the search result.
                            if (searchList.Pagination.totalItems > searchList.Pagination.itemsPerPage)
                            {
                                searchList.Pagination.totalPages = (int)Math.Ceiling((double)searchList.Pagination.totalItems / (double)searchList.Pagination.itemsPerPage);
                            }
                            else
                            {
                                searchList.Pagination.itemsPerPage = searchList.Pagination.totalItems;
                                searchList.Pagination.totalPages = 1;
                            }


                            //Determine current page number 
                            if (pageNo <= 0 || pageNo > searchList.Pagination.totalPages)
                            {
                                pageNo = 1;
                            }
                            searchList.Pagination.pageNo = pageNo;


                            //Determine how many pages/items to skip
                            if (searchList.Pagination.totalItems > searchList.Pagination.itemsPerPage)
                            {
                                searchList.Pagination.itemsToSkip = searchList.Pagination.itemsPerPage * (pageNo - 1);
                            }


                            //Convert list of SearchResults to list of classes
                            foreach (SearchResult sRecord in searchResults)//.Skip(searchList.Pagination.itemsToSkip).Take(searchList.Pagination.itemsPerPage))
                            {
                                try
                                {
                                    var msgLink = new Models.MsgLink();
                                    msgLink.Id = Convert.ToInt32(sRecord.Id);

                                    msgLink.Subtitle = Umbraco.Content(msgLink.Id).Parent.Parent.Parent.Parent.Name;
                                    msgLink.Title = sRecord[Common.NodeProperties.nodeName];
                                    
                                    msgLink.Url = Umbraco.Content(sRecord.Id).Url();
                                    msgLink.Date = Convert.ToDateTime(sRecord[Common.NodeProperties.publishDate]);
                                    msgLink.Dates = (Convert.ToDateTime(sRecord[Common.NodeProperties.publishDate])).ToString("MMM d, yyyy");

                                    searchList.lstMsgsFromHeavenLinks.Add(msgLink);
                                }
                                catch (Exception ex)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    sb.AppendLine(@"SearchController.cs : ObtainByMessagesFromHeaven()");
                                    Common.SaveErrorMessage(ex, sb, typeof(SearchController));
                                }

                            }
                            searchList.lstMsgsFromHeavenLinks = searchList.lstMsgsFromHeavenLinks.OrderByDescending(x => x.Date).Skip(searchList.Pagination.itemsToSkip).Take(searchList.Pagination.itemsPerPage).ToList();

                        }
                    }
                    //==========================================================================



                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"SearchController.cs : ObtainByMessagesFromHeaven()");
                Common.SaveErrorMessage(ex, sb, typeof(SearchController));
            }
        }
        private void ObtainByArticle(Models.SearchList searchList, int pageNo, UmbracoHelper umbracoHelper)
        {
            //Instantiate variables
            searchList.ShowArticles = true;
            searchList.SearchInTitle = "All Articles";

            try
            {
                if (!string.IsNullOrWhiteSpace(searchList.SearchFor))
                {
                    //Create list of fields to search in.              
                    List<string> lstFields = new List<string>();
                    lstFields.Add(Common.NodeProperties.title);
                    lstFields.Add(Common.NodeProperties.subtitle);
                    lstFields.Add(Common.NodeProperties.content);
                    lstFields.Add(Common.NodeProperties.originalSource);

                    //==========================================================================
                    //it returns the index in the var index
                    if (ExamineManager.Instance.TryGetIndex(Common.searchProviders.ExternalIndex, out var index))
                    {
                        //
                        ISearcher searcher = index.GetSearcher();
                        IBooleanOperation query = searcher.CreateQuery(null, BooleanOperation.And)
                            .NodeTypeAlias(Common.docType.Standard)
                            .And().GroupedOr(lstFields.ToArray(), searchList.SearchFor.MultipleCharacterWildcard());
                        ISearchResults searchResults = query.Execute(Int32.MaxValue);


                        if (searchResults.Any())
                        {
                            //Get item counts and total experiences.
                            searchList.Pagination.totalItems = searchResults.Count();


                            //Determine how many pages/items to skip and take, as well as the total page count for the search result.
                            if (searchList.Pagination.totalItems > searchList.Pagination.itemsPerPage)
                            {
                                searchList.Pagination.totalPages = (int)Math.Ceiling((double)searchList.Pagination.totalItems / (double)searchList.Pagination.itemsPerPage);
                            }
                            else
                            {
                                searchList.Pagination.itemsPerPage = searchList.Pagination.totalItems;
                                searchList.Pagination.totalPages = 1;
                            }


                            //Determine current page number 
                            if (pageNo <= 0 || pageNo > searchList.Pagination.totalPages)
                            {
                                pageNo = 1;
                            }
                            searchList.Pagination.pageNo = pageNo;


                            //Determine how many pages/items to skip
                            if (searchList.Pagination.totalItems > searchList.Pagination.itemsPerPage)
                            {
                                searchList.Pagination.itemsToSkip = searchList.Pagination.itemsPerPage * (pageNo - 1);
                            }


                            //Convert list of SearchResults to list of classes
                            foreach (SearchResult sRecord in searchResults.Skip(searchList.Pagination.itemsToSkip).Take(searchList.Pagination.itemsPerPage))
                            {
                                try
                                {
                                    var msgLink = new Models.ArticleLink();
                                    IPublishedContent ipArticle = umbracoHelper.Content(sRecord.Id);
                                    msgLink.Id = ipArticle.Id;
                                    msgLink.Url = ipArticle.Url();
                                    msgLink.Breadcrumb = GetBreadcrumbForArticle(ipArticle);

                                    searchList.lstArticleLinks.Add(msgLink);
                                }
                                catch (Exception ex)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    sb.AppendLine(@"SearchController.cs : ObtainByArticle()");
                                    Common.SaveErrorMessage(ex, sb, typeof(SearchController));
                                }
                            }
                        }
                    }
                    //==========================================================================
                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"SearchController.cs : ObtainByArticle()");
                Common.SaveErrorMessage(ex, sb, typeof(SearchController));
            }

        }
        private void ObtainByScripture(Models.SearchList searchList, int pageNo, UmbracoHelper umbracoHelper)
        {
            //Instantiate variables
            //var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
            searchList.ShowBible = true;
            searchList.SearchInTitle = "The Scriptures";


            try
            {
                if (!string.IsNullOrWhiteSpace(searchList.SearchFor))
                {
                    //Create list of fields to search in.              
                    List<string> lstFields = new List<string>();
                    lstFields.Add(Common.NodeProperties.Verses);

                    //Get all
                    if (ExamineManager.Instance.TryGetIndex(Common.searchProviders.ExternalIndex, out var index))
                    {
                        //
                        ISearcher searcher = index.GetSearcher();
                        IBooleanOperation query = searcher.CreateQuery(null, BooleanOperation.And)
                            .NodeTypeAlias(Common.docType.Chapter)
                            .And().GroupedOr(lstFields.ToArray(), searchList.SearchFor.MultipleCharacterWildcard());
                        ISearchResults searchResults = query.Execute(Int32.MaxValue);

                        if (searchResults.Any())
                        {
                            //Get item counts and total experiences.
                            searchList.Pagination.itemsPerPage = 30;
                            searchList.Pagination.totalItems = searchResults.Count();


                            //Determine how many pages/items to skip and take, as well as the total page count for the search result.
                            if (searchList.Pagination.totalItems > searchList.Pagination.itemsPerPage)
                            {
                                searchList.Pagination.totalPages = (int)Math.Ceiling((double)searchList.Pagination.totalItems / (double)searchList.Pagination.itemsPerPage);
                            }
                            else
                            {
                                searchList.Pagination.itemsPerPage = searchList.Pagination.totalItems;
                                searchList.Pagination.totalPages = 1;
                            }


                            //Determine current page number 
                            if (pageNo <= 0 || pageNo > searchList.Pagination.totalPages)
                            {
                                pageNo = 1;
                            }
                            searchList.Pagination.pageNo = pageNo;


                            //Determine how many pages/items to skip
                            if (searchList.Pagination.totalItems > searchList.Pagination.itemsPerPage)
                            {
                                searchList.Pagination.itemsToSkip = searchList.Pagination.itemsPerPage * (pageNo - 1);
                            }


                            //Convert list of SearchResults to list of classes
                            foreach (SearchResult sRecord in searchResults.Skip(searchList.Pagination.itemsToSkip).Take(searchList.Pagination.itemsPerPage))
                            {
                                try
                                {
                                    var scriptureLink = new Models.ScriptureLink();
                                    IPublishedContent ipArticle = umbracoHelper.Content(sRecord.Id);
                                    scriptureLink.Id = ipArticle.Id;
                                    scriptureLink.Url = ipArticle.Parent.Url() + "?chapter=" + ipArticle.Name;
                                    scriptureLink.Breadcrumb = GetBreadcrumbForScripture(ipArticle);

                                    searchList.lstBibleLinks.Add(scriptureLink);
                                }
                                catch (Exception ex)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    sb.AppendLine(@"SearchController.cs : ObtainByScripture()");
                                    Common.SaveErrorMessage(ex, sb, typeof(SearchController));
                                }
                            }
                        }
                    }
                    //==========================================================================
                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"SearchController.cs : ObtainByScripture()");
                Common.SaveErrorMessage(ex, sb, typeof(SearchController));
            }

        }
        private string GetBreadcrumbForArticle(IPublishedContent ip)
        {
            //Obtain the breadcrumb ancestors of the current page.
            var lstBreadcrumbs = ip.Ancestors();
            //bool isFirst = true;
            StringBuilder sb = new StringBuilder();


            if (lstBreadcrumbs.Count() > 1)
            {
                foreach (IPublishedContent ipCrumb in lstBreadcrumbs.OrderBy(x => x.Level).ToList())
                {
                    if (ipCrumb.ContentType.Alias != Common.docType.Home)
                    {
                        //
                        sb.Append("<span class='divider'> » </span>");

                        //Add crumb navigation to screen
                        if (ipCrumb.HasProperty(Common.NodeProperties.title))
                        {
                            sb.Append("<span class='breadcrumb'>" + ipCrumb.Value<string>(Common.NodeProperties.title) + "</span>");
                        }
                        else
                        {
                            sb.Append("<span class='breadcrumb'>" + ipCrumb.Name + "</span>");
                        }
                    }
                }
            }


            //
            sb.Append("<span class='divider'> » </span>");

            //Add crumb navigation to screen
            if (ip.HasProperty(Common.NodeProperties.title))
            {
                sb.Append("<span class='breadcrumb'>" + ip.Value<string>(Common.NodeProperties.title) + "</span>");
            }
            else
            {
                sb.Append("<span class='breadcrumb'>" + ip.Name + "</span>");
            }


            return sb.ToString();
        }
        private string GetBreadcrumbForScripture(IPublishedContent ip)
        {
            //Obtain the breadcrumb ancestors of the current page.
            var lstBreadcrumbs = ip.Ancestors();
            //bool isFirst = true;
            StringBuilder sb = new StringBuilder();


            if (lstBreadcrumbs.Count() > 1)
            {
                foreach (IPublishedContent ipCrumb in lstBreadcrumbs.OrderBy(x => x.Level).ToList())
                {
                    if (ipCrumb.ContentType.Alias == Common.docType.Scripture)
                    {
                        //
                        sb.Append("<span class='divider'> » </span>");

                        //Add crumb navigation to screen
                        if (ipCrumb.HasProperty(Common.NodeProperties.title))
                        {
                            sb.Append("<span class='breadcrumb'>" + ipCrumb.Value<string>(Common.NodeProperties.title) + "</span>");
                        }
                        else
                        {
                            sb.Append("<span class='breadcrumb'>" + ipCrumb.Name + "</span>");
                        }
                    }
                }
            }


            //
            sb.Append("<span class='divider'> » </span>");

            //Add crumb navigation to screen
            if (ip.HasProperty(Common.NodeProperties.title))
            {
                sb.Append("<span class='breadcrumb'>Chapter " + ip.Value<string>(Common.NodeProperties.title) + "</span>");
            }
            else
            {
                sb.Append("<span class='breadcrumb'>Chapter " + ip.Name + "</span>");
            }


            return sb.ToString();
        }
    }
}