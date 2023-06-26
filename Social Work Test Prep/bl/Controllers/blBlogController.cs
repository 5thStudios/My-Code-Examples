using System.Collections.Generic;
using Umbraco.Web.Mvc;
using Umbraco.Core;
using System;
using System.Web.Mvc;
using bl.Models;
using System.Web;
using System.Linq;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using ContentModels = Umbraco.Web.PublishedModels;
using Umbraco.Examine;
using Examine;
using Examine.Search;
using System.Web.UI.WebControls;
using Umbraco.Core.Logging;
using Constants = Umbraco.Core.Constants;
using System.Collections.Specialized;

//ServiceContext Services { get; }
//ISqlContext SqlContext { get; }
//UmbracoHelper Umbraco { get; }
//UmbracoContext UmbracoContext { get; }
//IGlobalSettings GlobalSettings { get; }
//IProfilingLogger Logger { get; }
//MembershipHelper Members { get; }


namespace bl.Controllers
{
    public class blBlogController : SurfaceController
    {
        #region "Renders"
        public ActionResult RenderPost(ContentModels.Post ipModel)
        {
            //Instantiate variables
            BlogPost blogPost = new BlogPost();

            try
            {
                //
                blogPost.Title = ipModel.Name;
                blogPost.PostDate = ipModel.PostDate;
                blogPost.Content = ipModel.Content;
                blogPost.Categories = ipModel.Categories.ToList();

                IPublishedContent ipBlog = ipModel.AncestorOrSelf(Models.Common.DocType.Blog);
                blogPost.Blog = new Link(ipBlog.Name, ipBlog.Url());

                if (ExamineManager.Instance.TryGetIndex(Constants.UmbracoIndexes.ExternalIndexName, out var index))
                {
                    /* EXAMINE GUIDES
                     * https://shazwazza.github.io/Examine/searching
                     * https://shazwazza.github.io/Examine/sorting
                     * https://shazwazza.github.io/Examine/indexing
                     * https://shazwazza.github.io/Examine/configuration
                     * */


                    //Get searcher
                    ISearcher searcher = index.GetSearcher();

                    //Search by doctype
                    IBooleanOperation examineQuery = searcher.CreateQuery(Models.Common.NodeProperties.Content).NodeTypeAlias(Models.Common.DocType.Post); //.SelectFields([""]);

                    //Sort by date (custom field added in IndexerComponent.cs)
                    SortableField field = new SortableField(Models.Common.NodeProperties.PostDateSortable, SortType.Long);
                    examineQuery.OrderByDescending(field);

                    string[] flds = new string[3] { Models.Common.NodeProperties.Id, Models.Common.NodeProperties.PostDate, Models.Common.NodeProperties.PostDateSortable };
                    examineQuery.SelectFields(flds);

                    //Get all results
                    ISearchResults searchResults = examineQuery.Execute(maxResults: int.MaxValue);
                    ISearchResult[] pagedResults = searchResults.ToArray(); //.Skip(0).Take(0);  //ALLOCATES TO MEMORY.


                    //
                    //StringBuilder sb = new StringBuilder();
                    var record = pagedResults.Where(x => x.Id == ipModel.Id.ToString()).FirstOrDefault();
                    int pageIndex = pagedResults.IndexOf(record);
                    int maxCount = pagedResults.Count();


                    //Previous button
                    if (pageIndex > 0)
                    {
                        int prevRecord = Convert.ToInt32(pagedResults[pageIndex - 1].Id);
                        IPublishedContent ipPrevPg = Umbraco.Content(prevRecord);
                        blogPost.Previous = new bl.Models.Link(ipPrevPg.Name, ipPrevPg.Url());
                        blogPost.ShowPrevBtn = true;
                    }
                    else
                    {
                        blogPost.ShowPrevBtn = false;
                    }


                    //Next button
                    if (pageIndex < maxCount - 1)
                    {
                        int nextRecord = Convert.ToInt32(pagedResults[pageIndex + 1].Id);
                        IPublishedContent ipNextPg = Umbraco.Content(nextRecord);
                        blogPost.Next = new bl.Models.Link(ipNextPg.Name, ipNextPg.Url());
                        blogPost.ShowNextBtn = true;
                    }
                    else
                    {
                        blogPost.ShowNextBtn = false;
                    }
                }

            }
            catch (Exception ex)
            {
                ////Save error to log
                //StringBuilder sb = new StringBuilder();
                //sb.AppendLine("FilterController | RenderFilter()");
                //Common.SaveErrorMessage(ex, sb, typeof(CommonController));
                //blogPost.tempStr = ex.Message;
                Logger.Error<blBlogController>(ex);
            }

            return PartialView(Models.Common.PartialPath.Blog_Post, blogPost);
        }

        public ActionResult RenderList(ContentModels.Blog cmModel, int pageNo = 1)
        {
            //Instantiate variables
            BlogPostList blogPostList = new BlogPostList();
            List<string> lstCategories = new List<string>();

            try
            {
                //Get blog's base url
                blogPostList.PgUrl = cmModel.Url();


                //Determine if diplaying by page or by 'tag'
                Boolean ShowByTag = ContainsKey(Request.QueryString, "tag");
                string filterTag = "";
                if (ShowByTag)
                {
                    ShowByTag = string.IsNullOrWhiteSpace(Request.QueryString.Get("tag")) ? false : true;

                    if (ShowByTag)
                    {
                        filterTag = Request.QueryString.Get("tag").ToLower();

                        //
                        if (Request.Url.Query.Contains("&") && Request.Url.Query.Contains("=")) 
                        {
                            var _query = Request.Url.Query.Split('=').LastOrDefault();
                            filterTag = _query.Replace("+", " ").Replace("&", "%26");
                        }
                    }                        
                }


                /* EXAMINE GUIDES
                 * https://shazwazza.github.io/Examine/searching
                 * https://shazwazza.github.io/Examine/sorting
                 * https://shazwazza.github.io/Examine/indexing
                 * https://shazwazza.github.io/Examine/configuration
                 * */
                if (ExamineManager.Instance.TryGetIndex(Constants.UmbracoIndexes.ExternalIndexName, out var index))
                {
                    //Get searcher
                    ISearcher searcher = index.GetSearcher();


                    //Search by doctype
                    IBooleanOperation examineQuery = searcher.CreateQuery(Models.Common.NodeProperties.Content).NodeTypeAlias(Models.Common.DocType.Post);


                    //EXAMPLE OF HOW TO FILTER SEARCH BY CATEGORY
                    //    examineQuery = searcher
                    //        .CreateQuery(Models.Common.NodeProperties.Content)
                    //        .NodeTypeAlias(Models.Common.DocType.Post)
                    //        .And().Field(Models.Common.NodeProperties.Categories, Request.QueryString.Get("tag"));


                    //Sort by date (custom field added in IndexerComponent.cs)
                    SortableField field = new SortableField(Models.Common.NodeProperties.PostDateSortable, SortType.Long);
                    examineQuery.OrderByDescending(field);
                    string[] flds = new string[6] {
                        Models.Common.NodeProperties.Id,
                        Models.Common.NodeProperties.PostDate,
                        Models.Common.NodeProperties.PostDateSortable,
                        Models.Common.NodeProperties.Categories,
                        Models.Common.NodeProperties.Content,
                        Models.Common.NodeProperties.ArticleImage };
                    examineQuery.SelectFields(flds);


                    //Get all results upto page count
                    int pageSize = cmModel.PostsPerPage;
                    ISearchResults searchResults = examineQuery.Execute(maxResults: int.MaxValue);
                    List<ISearchResult> filteredSearchResults = new List<ISearchResult>();

                    ////UPDATED SEARCH: GET ONLY WHAT IS NEEDED
                    //ISearchResults searchResults = examineQuery.Execute(maxResults: pageSize * pageNo);


                    //Obtain all categories
                    foreach (ISearchResult result in searchResults)
                    {
                        BlogPost blogpost = new BlogPost();

                        //Get all categories
                        if (result.Values[Models.Common.NodeProperties.Categories] != null)
                        {
                            blogpost.Categories = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(result.Values[Models.Common.NodeProperties.Categories]);

                            //Add categories to list of categories
                            foreach (string category in blogpost.Categories)
                            {
                                if (!lstCategories.Contains(category.ToLower()))
                                {
                                    lstCategories.Add(category.ToLower());
                                    blogPostList.LstCategories.Add(new Tuple<string, string>(category.ToLower(), category));
                                }

                                //Override with extracted querystring if has a special char.
                                if (ShowByTag)
                                {
                                    if (category.ToLower() == filterTag)
                                    {
                                        filteredSearchResults.Add(result);
                                    }
                                }
                            }
                        }
                    }


                    IEnumerable<ISearchResult> pagedResults;
                    if (ShowByTag)
                    {
                        //Show only filtered results
                        pagedResults = filteredSearchResults;
                    }
                    else
                    {
                        //Use the Skip method to tell Lucene to not allocate search results for the first ~n pages
                        pagedResults = searchResults.Skip(pageSize * (pageNo - 1)).Take(pageSize);
                    }


                    //Obtain data for each blog entry
                    foreach (ISearchResult result in pagedResults)
                    {
                        BlogPost blogpost = new BlogPost();

                        //Get all categories
                        if (result.Values[Models.Common.NodeProperties.Categories] != null)
                        {
                            blogpost.Categories = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(result.Values[Models.Common.NodeProperties.Categories]);
                        }

                        //
                        blogpost.PostDate = Convert.ToDateTime(result.Values[Models.Common.NodeProperties.PostDate]);
                        blogpost.Id = Convert.ToInt32(result.Id);


                        //
                        if (!string.IsNullOrWhiteSpace(result.Values[Models.Common.NodeProperties.Content]))
                        {
                            //Remove html and get content till end of sentence within certain width.
                            string content = result.Values[Models.Common.NodeProperties.Content];
                            System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex("<[^>]*>");
                            content = rx.Replace(content, " ");
                            content = content.Replace("/r", " ");
                            content = content.Replace("/n  ", " ");
                            content = content.Replace("  ", " ");
                            content = content.Replace("  ", " ");
                            content = content.Replace("  ", " ");
                            if (content.Length > 250) content = content.Substring(0, 250);
                            int charIndex = content.LastIndexOf(".");
                            if (content.LastIndexOf("?") > charIndex) charIndex = content.LastIndexOf("?");
                            if (content.LastIndexOf("!") > charIndex) charIndex = content.LastIndexOf("!");
                            if (content.LastIndexOf("-") > charIndex) charIndex = content.LastIndexOf("-");
                            blogpost.Content = new HtmlString(content.Substring(0, charIndex + 1));
                        }

                        //
                        //IPublishedContent ipPostImg = Umbraco.Media(result.Values[Models.Common.NodeProperties.ArticleImage]);

                        string articleImg = result.Values[Models.Common.NodeProperties.ArticleImage];
                        IPublishedContent ipPostImg = Umbraco.Media(articleImg);

                        if (ipPostImg == null && !string.IsNullOrEmpty(articleImg))
                        {
                            //Note: api-imported images save differently than regular images.  This splices json and extracts udi key to get image.
                            List<MediaKey> lstMediaKeys = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MediaKey>>(articleImg);
                            if (lstMediaKeys.Any())
                            {
                                var udi = "umb://media/" + lstMediaKeys.FirstOrDefault().mediaKey.ToString().Replace("-", "");
                                ipPostImg = Umbraco.Media(udi);
                            }
                        }


                        if (ipPostImg != null)
                            blogpost.PostImageUrl = ipPostImg.GetCropUrl(bl.Models.Common.Crop.Square_250x250);


                        //
                        IPublishedContent ipPost = Umbraco.Content(blogpost.Id);
                        blogpost.Title = ipPost.Name;
                        blogpost.Blog = new Link(ipPost.Name, ipPost.Url());

                        blogPostList.LstBlogPosts.Add(blogpost);
                    }


                    //Obtain all post links [avoids any filtered results]
                    foreach (ISearchResult result in searchResults)
                    {
                        BlogPost blogpost = new BlogPost();

                        //Obtain data for link
                        blogpost.PostDate = Convert.ToDateTime(result.Values[Models.Common.NodeProperties.PostDate]);
                        blogpost.Id = Convert.ToInt32(result.Id);

                        IPublishedContent ipPost = Umbraco.Content(blogpost.Id);
                        blogpost.Title = ipPost.Name;
                        blogpost.Blog = new Link(ipPost.Name, ipPost.Url());

                        blogPostList.LstAllBlogPosts.Add(blogpost);
                    }



                    //Show pagination data if applicable
                    if (!ShowByTag)
                    {
                        //Instantiate pagination
                        blogPostList.Pagination = new Pagination();


                        //Get item counts and total experiences.
                        blogPostList.Pagination.itemsPerPage = pageSize;
                        blogPostList.Pagination.totalItems = (int)searchResults.TotalItemCount; // isResults.Count();


                        //Determine how many pages/items to skip and take, as well as the total page count for the search result.
                        if (blogPostList.Pagination.totalItems > blogPostList.Pagination.itemsPerPage)
                        {
                            //blogPostList.Pagination.totalPages = (int)Math.Ceiling((double)blogPostList.Pagination.totalItems / (double)blogPostList.Pagination.itemsPerPage);
                            blogPostList.Pagination.totalPages = (int)Math.Floor((double)blogPostList.Pagination.totalItems / (double)blogPostList.Pagination.itemsPerPage);
                        }
                        else
                        {
                            blogPostList.Pagination.itemsPerPage = blogPostList.Pagination.totalItems;
                            blogPostList.Pagination.totalPages = 1;
                        }


                        //Determine current page number 
                        if (pageNo <= 0 || pageNo > blogPostList.Pagination.totalPages)
                        {
                            pageNo = 1;
                        }
                        blogPostList.Pagination.pageNo = pageNo;


                        //Determine how many pages/items to skip
                        if (blogPostList.Pagination.totalItems > blogPostList.Pagination.itemsPerPage)
                        {
                            blogPostList.Pagination.itemsToSkip = blogPostList.Pagination.itemsPerPage * (pageNo - 1);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                //BlogPost blogpost = new BlogPost();
                //blogpost.Title = ex.Message;
                //lstBlogPosts.Add(blogpost);
                Logger.Error<blBlogController>(ex);
            }

            return PartialView(Models.Common.PartialPath.Blog_List, blogPostList);
        }
        #endregion


        #region "Methods"
        public static bool ContainsKey(NameValueCollection collection, string key) => collection.Get(key) != null || collection.AllKeys.Contains(key);
        public static PaginationContent ObtainPaginationContent(Pagination Model, string url)
        {
            //
            PaginationContent paginationContent = new PaginationContent();
            paginationContent.Pagination = Model;

            //build base url for navigation links
            paginationContent.baseUri = new Uri(url);

            // this gets all the query string key value pairs as a collection
            paginationContent.queryString = HttpUtility.ParseQueryString(paginationContent.baseUri.Query);

            // this removes the key if exists (avoids duplicates)
            paginationContent.queryString.Remove("pageNo");
            paginationContent.queryString.Add("pageNo", "");

            // this gets the page path from root without QueryString
            paginationContent.baseUrl = paginationContent.baseUri.GetLeftPart(UriPartial.Path);

            //
            int lastSlash = paginationContent.baseUrl.LastIndexOf('/');
            if (lastSlash == (paginationContent.baseUrl.Length - 1))
            {
                paginationContent.baseUrl = paginationContent.baseUrl.Substring(0, paginationContent.baseUrl.Length - 1);
            }


            //Set values for page indexes
            paginationContent.Pagination.previous = Model.pageNo - 1;
            paginationContent.Pagination.next = Model.pageNo + 1;

            //Determine if prev/next values are correct.
            if (paginationContent.Pagination.previous < 1) { paginationContent.Pagination.previous = 1; }
            if (paginationContent.Pagination.next > Model.totalPages) { paginationContent.Pagination.next = Model.totalPages; }


            return paginationContent;


            /*
                {
                  "baseUri": "https://localhost:44305/blog?page=3",
                  "queryString": [
                    "page",
                    "pageNo"
                  ],
                  "baseUrl": "https://localhost:44305/blog",
                  "Pagination": {
                    "pageNo": 3,
                    "previous": 2,
                    "next": 4,
                    "itemsPerPage": 10,
                    "totalPages": 90,
                    "totalItems": 895,
                    "itemsToSkip": 20
                  }
                }
            */

        }
        #endregion
    }
}