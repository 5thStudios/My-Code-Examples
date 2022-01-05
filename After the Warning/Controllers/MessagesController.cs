using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using ContentModels = Umbraco.Web.PublishedModels;
using Examine;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Examine;
using Examine.Search;


namespace Controllers
{
    public class MessagesController : SurfaceController
    {
        #region "Renders"
        public ActionResult RenderAllMessages(UmbracoHelper Umbraco)
        {
            //Instantiate variables
            MsgList msgList = new MsgList();

            try
            {
                //Get all prayers
                if (ExamineManager.Instance.TryGetIndex(Common.searchProviders.ExternalIndex, out var index))
                {
                    //
                    ISearcher searcher = index.GetSearcher();
                    IBooleanOperation query = searcher.CreateQuery(null, BooleanOperation.Or)
                        .NodeTypeAlias(Common.docType.Message)
                        .Or().NodeTypeAlias(Common.docType.WebmasterMessage);
                    ISearchResults isResults = query.Execute(Int32.MaxValue);


                    if (isResults.Any())
                    {
                        //Get item counts and total experiences.
                        msgList.Pagination.itemsPerPage = 20;
                        msgList.Pagination.totalItems = isResults.Count();


                        //Determine how many pages/items to skip and take, as well as the total page count for the search result.
                        if (msgList.Pagination.totalItems > msgList.Pagination.itemsPerPage)
                        {
                            msgList.Pagination.totalPages = (int)Math.Ceiling((double)msgList.Pagination.totalItems / (double)msgList.Pagination.itemsPerPage);
                        }
                        else
                        {
                            msgList.Pagination.itemsPerPage = msgList.Pagination.totalItems;
                            msgList.Pagination.totalPages = 1;
                        }


                        //Determine current page number 
                        var pageNo = 1;
                        if (!string.IsNullOrEmpty(Request.QueryString[Common.miscellaneous.PageNo]))
                        {
                            int.TryParse(Request.QueryString[Common.miscellaneous.PageNo], out pageNo);
                            if (pageNo <= 0 || pageNo > msgList.Pagination.totalPages)
                            {
                                pageNo = 1;
                            }
                        }
                        msgList.Pagination.pageNo = pageNo;


                        //Determine how many pages/items to skip
                        if (msgList.Pagination.totalItems > msgList.Pagination.itemsPerPage)
                        {
                            msgList.Pagination.itemsToSkip = msgList.Pagination.itemsPerPage * (pageNo - 1);
                        }


                        //Convert list of SearchResults to list of classes
                        foreach (SearchResult sRecord in isResults.Skip(msgList.Pagination.itemsToSkip).Take(msgList.Pagination.itemsPerPage))
                        {
                            var msgLink = new Models.MsgLink();
                            msgLink.Id = Convert.ToInt32(sRecord.Id);
                            msgLink.Title = sRecord[Common.NodeProperties.nodeName];
                            msgLink.Subtitle = sRecord[Common.NodeProperties.subtitle];
                            msgLink.Url = Umbraco.Content(sRecord.Id).Url();
                            //msgLink.Date = Convert.ToDateTime(sRecord.Fields[Common.NodeProperties.publishDate]);
                            msgLink.Dates = (Convert.ToDateTime(sRecord[Common.NodeProperties.publishDate])).ToString("MMMM dd");

                            msgList.lstMsgLinks.Add(msgLink);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"MessageController.cs : RenderAllMessages()");
                sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(msgList));
                Common.SaveErrorMessage(ex, sb, typeof(MessagesController));


                ModelState.AddModelError("", "*An error occured while retrieving all messages.");
                return CurrentUmbracoPage();
            }

            //Return data to partialview
            return PartialView("~/Views/Partials/MessagesFromHeaven/_msgList.cshtml", msgList);
        }
        public ActionResult RenderLatestMessages()
        {
            //Instantiate variables
            //var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
            List<latestUpdates> lstLatestUpdates = new List<latestUpdates>();

            try
            {
                //perform the search
                //first we try to get the index, it is the ExternalIndex as we don't want to return unpublished things it returns the index in the var index
                if (ExamineManager.Instance.TryGetIndex(Common.searchProviders.ExternalIndex, out var index))
                {
                    //
                    ISearcher searcher = index.GetSearcher();
                    IBooleanOperation query = searcher.CreateQuery(null, BooleanOperation.Or)
                        .NodeTypeAlias(Common.docType.Message)
                        .Or().NodeTypeAlias(Common.docType.WebmasterMessage)
                        .Or().NodeTypeAlias(Common.docType.IlluminationStory);
                    ISearchResults isResults = query.Execute(Int32.MaxValue);


                    if (isResults.Any())
                    {
                        //Instantiate variables
                        DateTime msgDate = new DateTime(1900, 1, 1);
                        DateTime prevDate = new DateTime(1900, 1, 1);
                        latestUpdates latestUpdate = new latestUpdates();
                        visionary visionary = new visionary();
                        message message;
                        IPublishedContent ipMsg;
                        IPublishedContent ipVisionary;


                        //Get top 'n' results and determine link structure
                        List<MsgLink> lstMsgLinks = new List<MsgLink>();
                        foreach (SearchResult srResult in isResults)
                        {
                            MsgLink msgLink = new MsgLink();
                            msgLink.Date = Convert.ToDateTime(srResult["publishDate"]);
                            msgLink.Id = Convert.ToInt32(srResult.Id);
                            lstMsgLinks.Add(msgLink);
                        }
                        List<MsgLink> lstSortedMsgs = lstMsgLinks.OrderByDescending(x => x.Date).ToList();

                        foreach (var srResult in lstSortedMsgs.Take(20))
                        {
                            //Obtain message's node
                            ipMsg = Umbraco.Content(Convert.ToInt32(srResult.Id));
                            if (ipMsg != null)
                            {
                                //Obtain date of message
                                msgDate = ipMsg.Value<DateTime>(Common.NodeProperties.publishDate);

                                //Create a new date for messages
                                if (msgDate != prevDate)
                                {
                                    //Update current date
                                    prevDate = msgDate;

                                    //Create new instances for updates and add to list of all updates.
                                    latestUpdate = new latestUpdates();
                                    latestUpdate.datePublished = msgDate;
                                    lstLatestUpdates.Add(latestUpdate);

                                    //Reset the visionary class on every new date change.
                                    visionary = new visionary();
                                }

                                //Obtain current visionary or webmaster
                                if (ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.ContentType.Alias == Common.docType.Visionary) != null)
                                {
                                    if (visionary.id != ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.ContentType.Alias == Common.docType.Visionary).Id)
                                    {
                                        //Obtain visionary node
                                        ipVisionary = ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.ContentType.Alias == Common.docType.Visionary);

                                        //Create new visionary class and add to latest update class
                                        visionary = new visionary();
                                        visionary.id = ipVisionary.Id;
                                        visionary.name = ipVisionary.Name;
                                        visionary.url = ipVisionary.Url();
                                        latestUpdate.lstVisionaries.Add(visionary);
                                    }
                                }
                                else if (ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.ContentType.Alias == Common.docType.WebmasterMessageList) != null)
                                {
                                    if (visionary.id != ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.ContentType.Alias == Common.docType.WebmasterMessageList).Id)
                                    {
                                        //Obtain visionary node
                                        ipVisionary = ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.ContentType.Alias == Common.docType.WebmasterMessageList);

                                        //Create new visionary class and add to latest update class
                                        visionary = new visionary();
                                        visionary.id = ipVisionary.Id;
                                        visionary.name = ipVisionary.Name;
                                        visionary.url = ipVisionary.Url();
                                        latestUpdate.lstVisionaries.Add(visionary);
                                    }
                                }
                                else if (ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.ContentType.Alias == Common.docType.IlluminationStoryList) != null)
                                {
                                    if (visionary.id != ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.ContentType.Alias == Common.docType.IlluminationStoryList).Id)
                                    {
                                        //Obtain visionary node
                                        ipVisionary = ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.ContentType.Alias == Common.docType.IlluminationStoryList);

                                        //Create new visionary class and add to latest update class
                                        visionary = new visionary();
                                        visionary.id = ipVisionary.Id;
                                        visionary.name = ipVisionary.Name;
                                        visionary.url = ipVisionary.Url();
                                        latestUpdate.lstVisionaries.Add(visionary);
                                    }
                                }

                                //Create new message and add to existing visionary class.
                                message = new message();
                                message.id = ipMsg.Id;
                                message.title = ipMsg.Name;
                                message.url = ipMsg.Url();
                                visionary.lstMessages.Add(message);
                            }
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"MessageController.cs : RenderLatestMessages()");
                sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(lstLatestUpdates));
                Common.SaveErrorMessage(ex, sb, typeof(MessagesController));

                ModelState.AddModelError("", "*An error occured while creating the latest message list.");
                return CurrentUmbracoPage();
            }


            //Return data to partialview
            return PartialView("~/Views/Partials/Common/LatestUpdates.cshtml", lstLatestUpdates);
        }
        public ActionResult RenderMsgs_byVisionary(IPublishedContent ipVisionary, UmbracoHelper Umbraco)
        {
            //Instantiate variables
            MsgList msgList = new MsgList();

            try
            {
                msgList.VisionaryName = "working";

                //Get all prayers
                if (ExamineManager.Instance.TryGetIndex(Common.searchProviders.ExternalIndex, out var index))
                {
                    //Create list of fields to search in.              
                    List<string> lstNodeTypes = new List<string>();
                    lstNodeTypes.Add(Common.docType.Message);
                    lstNodeTypes.Add(Common.docType.WebmasterMessage);

                    //Search for multiple doctypes under a parent folder
                    ISearcher searcher = index.GetSearcher();
                    IQuery criteria = searcher.CreateQuery(null, BooleanOperation.And);
                    //Only look for documents of type
                    IBooleanOperation query = criteria.GroupedOr(new string[] { Common.NodeProperties.NodeTypeAlias }, lstNodeTypes.ToArray());
                    //Add filter to only find files under a parent path with a proceeding wildcard
                    query = query.And().Field(Common.miscellaneous.Path, ipVisionary.Path.MultipleCharacterWildcard());
                    ISearchResults isResults = query.Execute(Int32.MaxValue);


                    //Get item counts and total experiences.
                    msgList.Pagination.itemsPerPage = 30;
                    msgList.Pagination.totalItems = isResults.Count();


                    //Determine how many pages/items to skip and take, as well as the total page count for the search result.
                    if (msgList.Pagination.totalItems > msgList.Pagination.itemsPerPage)
                    {
                        msgList.Pagination.totalPages = (int)Math.Ceiling((double)msgList.Pagination.totalItems / (double)msgList.Pagination.itemsPerPage);
                    }
                    else
                    {
                        msgList.Pagination.itemsPerPage = msgList.Pagination.totalItems;
                        msgList.Pagination.totalPages = 1;
                    }


                    //Determine current page number 
                    var pageNo = 1;
                    if (!string.IsNullOrEmpty(Request.QueryString[Common.miscellaneous.PageNo]))
                    {
                        int.TryParse(Request.QueryString[Common.miscellaneous.PageNo], out pageNo);
                        if (pageNo <= 0 || pageNo > msgList.Pagination.totalPages)
                        {
                            pageNo = 1;
                        }
                    }
                    msgList.Pagination.pageNo = pageNo;


                    //Determine how many pages/items to skip
                    if (msgList.Pagination.totalItems > msgList.Pagination.itemsPerPage)
                    {
                        msgList.Pagination.itemsToSkip = msgList.Pagination.itemsPerPage * (pageNo - 1);
                    }

                    List<MsgLink> lstMsgLinks = new List<MsgLink>();
                    //Convert list of SearchResults to list of classes
                    foreach (SearchResult sRecord in isResults) //.Skip(msgList.Pagination.itemsToSkip).Take(msgList.Pagination.itemsPerPage))
                    {
                        var msgLink = new Models.MsgLink();
                        msgLink.Id = Convert.ToInt32(sRecord.Id);
                        msgLink.Title = sRecord[Common.NodeProperties.nodeName];
                        //msgLink.Subtitle = sRecord[Common.NodeProperties.subtitle];
                        msgLink.Url = Umbraco.Content(sRecord.Id).Url();


                        msgLink.Date = Convert.ToDateTime(sRecord[Common.NodeProperties.publishDate]);


                        //Obtain list of all dates
                        List<DateTime> lstDateRange = new List<DateTime>();
                        var ipMsg = Umbraco.Content(sRecord.Id);
                        if (ipMsg.HasValue(Common.NodeProperties.dateOfMessages))
                        { lstDateRange = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DateTime>>(ipMsg.Value<string>(Common.NodeProperties.dateOfMessages)); }


                        //Determine proper date range for messages
                        if (lstDateRange != null && lstDateRange.Count > 0)
                        {
                            if (lstDateRange.Count == 1)
                            {
                                msgLink.Dates = lstDateRange.First().ToString("MMM d, yyyy");
                            }
                            else
                            {
                                StringBuilder sbDateRange = new StringBuilder();
                                sbDateRange.Append(lstDateRange.First().ToString("MMM d"));
                                sbDateRange.Append(" — ");
                                sbDateRange.Append(lstDateRange.Last().ToString("MMM d, yyyy"));

                                msgLink.Dates = sbDateRange.ToString();
                            }
                        }
                        else
                        {
                            msgLink.Dates = Convert.ToDateTime(sRecord[Common.NodeProperties.publishDate]).ToString("MMM d, yyyy");
                        }

                        lstMsgLinks.Add(msgLink);
                    }

                    //Reorder messages by date and obtain only what is to be displayed.
                    msgList.lstMsgLinks = lstMsgLinks.OrderByDescending(x => x.Date).Skip(msgList.Pagination.itemsToSkip).Take(msgList.Pagination.itemsPerPage).ToList();

                }

            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"MessageController.cs : RenderMsgs_byVisionary()");
                //sb.AppendLine("ipVisionary:" + Newtonsoft.Json.JsonConvert.SerializeObject(ipVisionary));
                sb.AppendLine("msgList:" + Newtonsoft.Json.JsonConvert.SerializeObject(msgList));
                Common.SaveErrorMessage(ex, sb, typeof(MessagesController));


                ModelState.AddModelError("", "*An error occured while retrieving messages by visionary.");
            }

            //Return data to partialview
            return PartialView("~/Views/Partials/MessagesFromHeaven/_msgList.cshtml", msgList);
        }
        #endregion


        #region "Methods"
        public static List<latestUpdates> ObtainLatestMessages(UmbracoHelper Umbraco)
        {
            //Instantiate variables
            List<latestUpdates> lstLatestUpdates = new List<latestUpdates>();

            try
            {
                //Get all messages
                if (ExamineManager.Instance.TryGetIndex(Common.searchProviders.ExternalIndex, out var index))
                {
                    //
                    ISearcher searcher = index.GetSearcher();
                    IBooleanOperation query = searcher.CreateQuery(null, BooleanOperation.And)
                        .NodeTypeAlias(Common.docType.Message)
                        .Or().NodeTypeAlias(Common.docType.WebmasterMessage);
                    ISearchResults isResults = query.Execute(Int32.MaxValue);


                    if (isResults.Any())
                    {
                        //Instantiate variables
                        DateTime msgDate = new DateTime(1900, 1, 1);
                        //DateTime prevDate = new DateTime(1900, 1, 1);
                        latestUpdates latestUpdate = new latestUpdates();
                        visionary visionary = new visionary();
                        message message;
                        IPublishedContent ipMsg;
                        IPublishedContent ipVisionary;
                        Boolean isFirst = true;



                        //Get top 'n' results and determine link structure
                        List<MsgLink> lstMsgLinks = new List<MsgLink>();
                        foreach (SearchResult srResult in isResults)
                        {
                            MsgLink msgLink = new MsgLink();
                            msgLink.Date = Convert.ToDateTime(srResult["publishDate"]);
                            msgLink.Id = Convert.ToInt32(srResult.Id);
                            lstMsgLinks.Add(msgLink);
                        }
                        List<MsgLink> lstSortedMsgs = lstMsgLinks.OrderByDescending(x => x.Date).ToList();

                        //foreach (var srResult in lstSortedMsgs.Take(20))



                        //Get top 'n' results and determine link structure
                        //foreach (SearchResult srResult in isResults)
                        foreach (MsgLink msgLink in lstSortedMsgs)
                        {
                            //Obtain message's node
                            ipMsg = Umbraco.Content(msgLink.Id);
                            if (ipMsg != null)
                            {
                                //Only obtain msgs for the latest date, and then exit loop.
                                if (isFirst)
                                {
                                    //Obtain date of latest message
                                    msgDate = msgLink.Date;
                                    isFirst = false;
                                }
                                else
                                {
                                    //Exit loop if a different publish date exists
                                    if (msgDate != msgLink.Date)
                                    {
                                        break;
                                    }
                                }

                                //Create new instances for updates and add to list of all updates.
                                latestUpdate = new latestUpdates();
                                latestUpdate.datePublished = msgDate;
                                lstLatestUpdates.Add(latestUpdate);

                                //Reset the visionary class on every new date change.
                                visionary = new visionary();

                                //Obtain current visionary or webmaster
                                if (ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.ContentType.Alias == Common.docType.Visionary) != null)
                                {
                                    if (visionary.id != ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.ContentType.Alias == Common.docType.Visionary).Id)
                                    {
                                        //Obtain visionary node
                                        ipVisionary = ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.ContentType.Alias == Common.docType.Visionary);

                                        //Create new visionary class and add to latest update class
                                        visionary = new visionary();
                                        visionary.id = ipVisionary.Id;
                                        visionary.name = ipVisionary.Name;
                                        visionary.url = ipVisionary.Url(mode: UrlMode.Absolute);
                                        latestUpdate.lstVisionaries.Add(visionary);
                                    }
                                }
                                else if (ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.ContentType.Alias == Common.docType.WebmasterMessageList) != null)
                                {
                                    if (visionary.id != ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.ContentType.Alias == Common.docType.WebmasterMessageList).Id)
                                    {
                                        //Obtain visionary node
                                        ipVisionary = ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.ContentType.Alias == Common.docType.WebmasterMessageList);

                                        //Create new visionary class and add to latest update class
                                        visionary = new visionary();
                                        visionary.id = ipVisionary.Id;
                                        visionary.name = ipVisionary.Name;
                                        visionary.url = ipVisionary.Url(mode: UrlMode.Absolute);
                                        latestUpdate.lstVisionaries.Add(visionary);
                                    }
                                }

                                //Create new message and add to existing visionary class.
                                message = new message();
                                message.id = ipMsg.Id;
                                message.title = ipMsg.Name;
                                message.url = ipMsg.Url(mode: UrlMode.Absolute);
                                visionary.lstMessages.Add(message);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"MessageController.cs : ObtainLatestMessages()");
                sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(lstLatestUpdates));
                Common.SaveErrorMessage(ex, sb, typeof(MessagesController));
            }

            //
            return lstLatestUpdates;
        }
        public static LatestUpdateList ObtainAllMessages(UmbracoHelper Umbraco, int pageNo = 1)
        {

            //Instantiate variables
            LatestUpdateList lstLatestUpdates = new LatestUpdateList();

            //try
            //{
            if (ExamineManager.Instance.TryGetIndex(Common.searchProviders.ExternalIndex, out var index))
            {
                //
                ISearcher searcher = index.GetSearcher();
                IBooleanOperation query = searcher.CreateQuery(null, BooleanOperation.Or)
                    .NodeTypeAlias(Common.docType.Message)
                    .Or().NodeTypeAlias(Common.docType.WebmasterMessage);
                ISearchResults isResults = query.Execute(Int32.MaxValue);


                if (isResults.Any())
                {
                    //Instantiate variables
                    DateTime msgDate = new DateTime(1900, 1, 1);
                    DateTime prevDate = new DateTime(1900, 1, 1);
                    latestUpdates latestUpdate = new latestUpdates();
                    visionary visionary = new visionary();
                    message message;
                    IPublishedContent ipMsg;
                    IPublishedContent ipVisionary;


                    //
                    List<MsgLink> lstMsgLinks = new List<MsgLink>();
                    foreach (SearchResult srResult in isResults)
                    {
                        MsgLink msgLink = new MsgLink();
                        msgLink.Date = Convert.ToDateTime(srResult["publishDate"]);
                        msgLink.Id = Convert.ToInt32(srResult.Id);
                        lstMsgLinks.Add(msgLink);
                    }
                    List<MsgLink> lstSortedMsgs = lstMsgLinks.OrderByDescending(x => x.Date).ToList();


                    //Get item counts and total experiences.
                    lstLatestUpdates.Pagination.itemsPerPage = 20;
                    lstLatestUpdates.Pagination.totalItems = isResults.Count();


                    //Determine how many pages/items to skip and take, as well as the total page count for the search result.
                    if (lstLatestUpdates.Pagination.totalItems > lstLatestUpdates.Pagination.itemsPerPage)
                    {
                        lstLatestUpdates.Pagination.totalPages = (int)Math.Ceiling((double)lstLatestUpdates.Pagination.totalItems / (double)lstLatestUpdates.Pagination.itemsPerPage);
                    }
                    else
                    {
                        lstLatestUpdates.Pagination.itemsPerPage = lstLatestUpdates.Pagination.totalItems;
                        lstLatestUpdates.Pagination.totalPages = 1;
                    }


                    //Determine current page number 
                    if (pageNo <= 0 || pageNo > lstLatestUpdates.Pagination.totalPages)
                    {
                        pageNo = 1;
                    }
                    lstLatestUpdates.Pagination.pageNo = pageNo;


                    //Determine how many pages/items to skip
                    if (lstLatestUpdates.Pagination.totalItems > lstLatestUpdates.Pagination.itemsPerPage)
                    {
                        lstLatestUpdates.Pagination.itemsToSkip = lstLatestUpdates.Pagination.itemsPerPage * (pageNo - 1);
                    }



                    //Get top 'n' results and determine link structure
                    //foreach (SearchResult srResult in isResults.Take(30))
                    foreach (var srResult in lstSortedMsgs.Skip(lstLatestUpdates.Pagination.itemsToSkip).Take(lstLatestUpdates.Pagination.itemsPerPage))
                    {
                        //Obtain message's node
                        ipMsg = Umbraco.Content(Convert.ToInt32(srResult.Id));
                        if (ipMsg != null)
                        {
                            //Obtain date of message
                            msgDate = ipMsg.Value<DateTime>(Common.NodeProperties.publishDate);

                            //Create a new date for messages
                            if (msgDate != prevDate)
                            {
                                //Update current date
                                prevDate = msgDate;

                                //Create new instances for updates and add to list of all updates.
                                latestUpdate = new latestUpdates();
                                latestUpdate.datePublished = msgDate;
                                lstLatestUpdates.LstLatestUpdates.Add(latestUpdate);

                                //Reset the visionary class on every new date change.
                                visionary = new visionary();
                            }

                            //Obtain current visionary or webmaster
                            if (ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.ContentType.Alias == Common.docType.Visionary) != null)
                            {
                                if (visionary.id != ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.ContentType.Alias == Common.docType.Visionary).Id)
                                {
                                    //Obtain visionary node
                                    ipVisionary = ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.ContentType.Alias == Common.docType.Visionary);

                                    //Create new visionary class and add to latest update class
                                    visionary = new visionary();
                                    visionary.id = ipVisionary.Id;
                                    visionary.name = ipVisionary.Name;
                                    visionary.url = ipVisionary.Url();
                                    latestUpdate.lstVisionaries.Add(visionary);
                                }
                            }
                            else if (ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.ContentType.Alias == Common.docType.WebmasterMessageList) != null)
                            {
                                if (visionary.id != ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.ContentType.Alias == Common.docType.WebmasterMessageList).Id)
                                {
                                    //Obtain visionary node
                                    ipVisionary = ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.ContentType.Alias == Common.docType.WebmasterMessageList);

                                    //Create new visionary class and add to latest update class
                                    visionary = new visionary();
                                    visionary.id = ipVisionary.Id;
                                    visionary.name = ipVisionary.Name;
                                    visionary.url = ipVisionary.Url();
                                    latestUpdate.lstVisionaries.Add(visionary);
                                }
                            }

                            //Create new message and add to existing visionary class.
                            message = new message();
                            message.id = ipMsg.Id;
                            message.title = ipMsg.Name;
                            message.url = ipMsg.Url();
                            visionary.lstMessages.Add(message);
                        }
                    }
                }
            }

            //    //}
            //    //catch (Exception ex)
            //    //{
            //    //    //StringBuilder sb = new StringBuilder();
            //    //    //sb.AppendLine(@"MessageController.cs : RenderLatestMessages()");
            //    //    //sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(lstLatestUpdates));
            //    //    Common.SaveErrorMessage(ex, sb, typeof(MessageController));


            //    //    //ModelState.AddModelError("", "*An error occured while creating the latest message list.");
            //    //    //return CurrentUmbracoPage();
            //    //}


            //    //Return data to partialview
            return lstLatestUpdates;
            //}
        }
        public static VisionaryContent ObtainVisionaryContent(ContentModels.Visionary cmVisionary)
        {
            VisionaryContent visionaryContent = new VisionaryContent();

            visionaryContent.VisionarysName = cmVisionary.VisionarysName;
            visionaryContent.PageImage = cmVisionary.PageImage.GetCropUrl(Common.crop.Portrait_300x400);
            visionaryContent.Religion = cmVisionary.Religion;
            visionaryContent.isOtherOrKeepPrivate = (visionaryContent.Religion == Common.miscellaneous.OtherOrKeepPrivate);
            visionaryContent.Email = cmVisionary.Email;
            visionaryContent.Phone = cmVisionary.Phone;
            visionaryContent.phoneNo = new String(visionaryContent.Phone.Where(Char.IsDigit).ToArray());
            visionaryContent.OriginalSiteUrl = cmVisionary.OriginalSiteUrl;
            visionaryContent.OriginalSiteName = cmVisionary.OriginalSiteName;

            foreach (var record in cmVisionary.Address)
            {
                visionaryContent.isAddressNull = false;
                visionaryContent.strAddress.Append(record.Address + "<br />");
                visionaryContent.strAddress.Append(record.City + ", ");
                visionaryContent.strAddress.Append(record.State + " ");
                visionaryContent.strAddress.Append(record.Postal);
                break;
            }


            return visionaryContent;
        }
        #endregion
    }
}






//public static List<latestUpdates> ObtainLatestMessages(UmbracoHelper Umbraco)
//{
//    //Instantiate variables
//    List<latestUpdates> lstLatestUpdates = new List<latestUpdates>();

//    try
//    {
//        //Get all messages
//        if (ExamineManager.Instance.TryGetIndex(Common.searchProviders.ExternalIndex, out var index))
//        {
//            //
//            ISearcher searcher = index.GetSearcher();
//            IBooleanOperation query = searcher.CreateQuery(null, BooleanOperation.And)
//                .NodeTypeAlias(Common.docType.Message)
//                .Or().NodeTypeAlias(Common.docType.WebmasterMessage);
//            ISearchResults isResults = query.Execute(Int32.MaxValue);


//            if (isResults.Any())
//            {
//                //Instantiate variables
//                DateTime msgDate = new DateTime(1900, 1, 1);
//                //DateTime prevDate = new DateTime(1900, 1, 1);
//                latestUpdates latestUpdate = new latestUpdates();
//                visionary visionary = new visionary();
//                message message;
//                IPublishedContent ipMsg;
//                IPublishedContent ipVisionary;
//                Boolean isFirst = true;


//                //Get top 'n' results and determine link structure
//                foreach (SearchResult srResult in isResults)
//                {
//                    //Obtain message's node
//                    ipMsg = Umbraco.Content(Convert.ToInt32(srResult.Id));
//                    if (ipMsg != null)
//                    {
//                        if (isFirst)
//                        {
//                            //Obtain date of latest message
//                            msgDate = ipMsg.Value<DateTime>(Common.NodeProperties.publishDate);
//                            isFirst = false;
//                        }
//                        else
//                        {
//                            //Exit loop if a different publish date exists
//                            if (msgDate != ipMsg.Value<DateTime>(Common.NodeProperties.publishDate))
//                            {
//                                break;
//                            }
//                        }


//                        //Create a new date for messages
//                        //if (msgDate != prevDate)
//                        //{
//                        //    //Update current date
//                        //    prevDate = msgDate;

//                        //Create new instances for updates and add to list of all updates.
//                        latestUpdate = new latestUpdates();
//                        latestUpdate.datePublished = msgDate;
//                        lstLatestUpdates.Add(latestUpdate);

//                        //Reset the visionary class on every new date change.
//                        visionary = new visionary();
//                        //}

//                        //Obtain current visionary or webmaster
//                        if (ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.ContentType.Alias == Common.docType.Visionary) != null)
//                        {
//                            if (visionary.id != ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.ContentType.Alias == Common.docType.Visionary).Id)
//                            {
//                                //Obtain visionary node
//                                ipVisionary = ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.ContentType.Alias == Common.docType.Visionary);

//                                //Create new visionary class and add to latest update class
//                                visionary = new visionary();
//                                visionary.id = ipVisionary.Id;
//                                visionary.name = ipVisionary.Name;
//                                visionary.url = ipVisionary.Url();
//                                latestUpdate.lstVisionaries.Add(visionary);
//                            }
//                        }
//                        else if (ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.ContentType.Alias == Common.docType.WebmasterMessageList) != null)
//                        {
//                            if (visionary.id != ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.ContentType.Alias == Common.docType.WebmasterMessageList).Id)
//                            {
//                                //Obtain visionary node
//                                ipVisionary = ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.ContentType.Alias == Common.docType.WebmasterMessageList);

//                                //Create new visionary class and add to latest update class
//                                visionary = new visionary();
//                                visionary.id = ipVisionary.Id;
//                                visionary.name = ipVisionary.Name;
//                                visionary.url = ipVisionary.Url();
//                                latestUpdate.lstVisionaries.Add(visionary);
//                            }
//                        }

//                        //Create new message and add to existing visionary class.
//                        message = new message();
//                        message.id = ipMsg.Id;
//                        message.title = ipMsg.Name;
//                        message.url = ipMsg.Url();
//                        visionary.lstMessages.Add(message);
//                    }
//                }
//            }
//        }

//    }
//    catch (Exception ex)
//    {
//        StringBuilder sb = new StringBuilder();
//        sb.AppendLine(@"MessageController.cs : ObtainLatestMessages()");
//        sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(lstLatestUpdates));
//        Common.SaveErrorMessage(ex, sb, typeof(MessagesController));
//    }

//    //
//    return lstLatestUpdates;
//}