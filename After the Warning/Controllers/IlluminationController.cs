using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using ContentModels = Umbraco.Web.PublishedModels;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Examine;
using Examine.Providers;
using Newtonsoft.Json;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.Security;
using formulate.app.Helpers;
using formulate.core.Models;
using formulate.app.Types;
using Examine.Search;
using Umbraco.Examine;
using Umbraco.Core.Composing;
using System.Net.Mail;
using System.Web.Hosting;
using System.Net.Mime;
//using UmbracoExamine;
//using Umbraco.Web.Extensions;
//using Examine.SearchCriteria;


namespace Controllers
{
    public class IlluminationController : SurfaceController
    {
        #region "Renders"
        public ActionResult RenderInstructions()
        {
            return PartialView("~/Views/Partials/IlluminationStories/_illuminationStoryInstructions.cshtml");
        }
        public ActionResult RenderStory(IMember member, UmbracoHelper Umbraco)
        {
            //Instantiate variables
            illuminationStory illuminationStory = new Models.illuminationStory();


            try
            {
                //Obtain the illumination story
                Udi storyUdi = member.GetValue<Udi>(ContentModels.Member.GetModelPropertyType(x => x.IlluminationStory).Alias); //PropertyTypeAlias
                IPublishedContent ipIlluminationStory = Umbraco.Content(storyUdi);

                //Obtain the content models
                //illuminationStory.IsPublished = ipIlluminationStory.IsPublished();
                if (ipIlluminationStory.IsPublished())
                {
                    illuminationStory.IsPublished = true;
                    illuminationStory.CmIpIlluminationStory = new ContentModels.IlluminationStory(ipIlluminationStory);
                    illuminationStory.CmMember = new ContentModels.Member(illuminationStory.CmIpIlluminationStory.Member);

                    //Add data to story model
                    StringBuilder sbAuthor = new StringBuilder();
                    sbAuthor.Append(illuminationStory.CmMember.FirstName);
                    sbAuthor.Append("&nbsp;&nbsp;&nbsp;");
                    sbAuthor.Append(illuminationStory.CmMember.LastName);
                    sbAuthor.Append(".");
                    illuminationStory.Author = sbAuthor.ToString();
                }
                //else
                //{

                //}



                return PartialView("~/Views/Partials/IlluminationStories/_showIlluminationStory.cshtml", illuminationStory);
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"IlluminationController.cs : RenderStory()");
                //sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(illuminationStory));
                //sb.AppendLine("member:" + Newtonsoft.Json.JsonConvert.SerializeObject(member));
                Common.SaveErrorMessage(ex, sb, typeof(IlluminationController));

                ModelState.AddModelError("", "*An error occured while retrieving your story.");
                return PartialView("~/Views/Partials/IlluminationStories/_showIlluminationStory.cshtml", illuminationStory);
            }
        }
        public ActionResult RenderForm(IMember member, UmbracoHelper Umbraco, Boolean editMode = false)
        {
            //Instantiate variables
            illuminationStory illuminationStory = new Models.illuminationStory();

            ////  JF- FOR EMERGENCY USE ONLY TO DELETE CORRUPTED MEMBER FILE
            //Services.MemberService.Delete(member);

            try
            {
                //Use a loop to create an age array from 1 to 120
                illuminationStory.lstAge.Add("");
                int i = 1;
                while (i <= 120)
                {
                    illuminationStory.lstAge.Add(i.ToString());
                    i++;
                }

                //Get member as iPublished
                //var memberShipHelper = new Umbraco.Web.Security.MembershipHelper(Umbraco.Web.UmbracoContext.Current);
                var ipMember = Members.GetById(member.Id);

                //Add data to story model
                StringBuilder sbAuthor = new StringBuilder();
                sbAuthor.Append(ipMember.Value<String>(ContentModels.Member.GetModelPropertyType(x => x.FirstName).Alias));
                sbAuthor.Append("&nbsp;&nbsp;&nbsp;");
                sbAuthor.Append(ipMember.Value<String>(ContentModels.Member.GetModelPropertyType(x => x.LastName).Alias));
                sbAuthor.Append(".");
                illuminationStory.Author = sbAuthor.ToString();

                illuminationStory.memberId = member.Id;

                //Determine which we need to show an edit view or not.
                //if (editMode)
                //{
                //    //Obtain the story from the member
                //    Udi storyUdi = member.GetValue<Udi>(ContentModels.Member.GetModelPropertyType(x => x.IlluminationStory).Alias);
                //    IPublishedContent ipIlluminationStory = Umbraco.Content(storyUdi);

                //    //Extract data from story & member
                //    illuminationStory.storyId = ipIlluminationStory.Id;
                //    //illuminationStory.Title = ipIlluminationStory.Value<string>(Common.NodeProperties.title);
                //    illuminationStory.Story = ipIlluminationStory.Value<string>(Common.NodeProperties.story);
                //    if (member.GetValue<string>(Common.NodeProperties.age) != null)
                //    {
                //        illuminationStory.Age = member.GetValue<int>(Common.NodeProperties.age);
                //    }

                //    //Set the experience type
                //    string experienceType = ipIlluminationStory.Value<string>(Common.NodeProperties.experienceType);
                //    illuminationStory.ExperienceType = (Common.ExperienceTypes)System.Enum.Parse(typeof(Common.ExperienceTypes), experienceType.Replace(" ", ""));

                //    //Set the gender
                //    string gender = member.GetValue<string>(Common.NodeProperties.gender);
                //    switch (gender)
                //    {
                //        case "XX [Female]":
                //            illuminationStory.Gender = Common.Genders.Female;
                //            break;
                //        case "XY [Male]":
                //            illuminationStory.Gender = Common.Genders.Male;
                //            break;
                //    }

                //    //Set the race
                //    string race = Common.DeserializeValues(member.GetValue<string>(Common.NodeProperties.race)).FirstOrDefault();
                //    switch (race)
                //    {
                //        case "Native American":
                //            illuminationStory.Race = Common.Races.NativeAmerican;
                //            break;
                //        case "Arabic":
                //            illuminationStory.Race = Common.Races.Arabic;
                //            break;
                //        case "Asian":
                //            illuminationStory.Race = Common.Races.Asian;
                //            break;
                //        case "Black or African":
                //            illuminationStory.Race = Common.Races.BlackOrAfrican;
                //            break;
                //        case "Indian":
                //            illuminationStory.Race = Common.Races.Indian;
                //            break;
                //        case "Jewish":
                //            illuminationStory.Race = Common.Races.Jewish;
                //            break;
                //        case "Latin or Hispanic":
                //            illuminationStory.Race = Common.Races.LatinOrHispanic;
                //            break;
                //        case "Other or Keep Private":
                //            illuminationStory.Race = Common.Races.OtherOrKeepPrivate;
                //            break;
                //        case "Pacific Islander":
                //            illuminationStory.Race = Common.Races.PacificIslander;
                //            break;
                //        case "White or Caucasian":
                //            illuminationStory.Race = Common.Races.WhiteOrCaucasian;
                //            break;
                //    }





                //    //Set the religion
                //    string religion = Common.DeserializeValues(member.GetValue<string>(Common.NodeProperties.religion)).FirstOrDefault();
                //    switch (religion)
                //    {
                //        case "Agnostic":
                //            illuminationStory.Religion = Common.Religions.Agnostic;
                //            break;
                //        case "Atheist":
                //            illuminationStory.Religion = Common.Religions.Atheist;
                //            break;
                //        case "Baptist":
                //            illuminationStory.Religion = Common.Religions.Baptist;
                //            break;
                //        case "Buddhist":
                //            illuminationStory.Religion = Common.Religions.Buddhist;
                //            break;
                //        case "Catholic":
                //            illuminationStory.Religion = Common.Religions.Catholic;
                //            break;
                //        case "Evangelical":
                //            illuminationStory.Religion = Common.Religions.Evangelical;
                //            break;
                //        case "Hindu":
                //            illuminationStory.Religion = Common.Religions.Hindu;
                //            break;
                //        case "Lutheran":
                //            illuminationStory.Religion = Common.Religions.Lutheran;
                //            break;
                //        case "Muslim":
                //            illuminationStory.Religion = Common.Religions.Muslim;
                //            break;
                //        case "Other Christian":
                //            illuminationStory.Religion = Common.Religions.OtherChristian;
                //            break;
                //        case "Other or Keep Private":
                //            illuminationStory.Religion = Common.Religions.OtherOrKeepPrivate;
                //            break;
                //        case "Protestant":
                //            illuminationStory.Religion = Common.Religions.Protestant;
                //            break;
                //        case "Satinism":
                //            illuminationStory.Religion = Common.Religions.Satinism;
                //            break;
                //        case "Wiccan or New Age":
                //            illuminationStory.Religion = Common.Religions.WiccanOrNewAge;
                //            break;
                //    }






                //    //Set the experience type
                //    string country = member.GetValue<string>(Common.NodeProperties.country);
                //    if (country != null)
                //    {
                //        illuminationStory.Country = (Common.Countries)System.Enum.Parse(typeof(Common.Countries), country.Replace(" ", "").Replace(".", ""));
                //    }

                //    //Add data to partial view
                //    return PartialView("~/Views/Partials/IlluminationStories/_editIlluminationStory.cshtml", illuminationStory);
                //}
                //else
                //{
                //Add data to partial view
                return PartialView("~/Views/Partials/IlluminationStories/_addIlluminationStory.cshtml", illuminationStory);
                //}
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"IlluminationController.cs : RenderForm()");
                sb.AppendLine("editMode:" + editMode.ToString());
                sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(illuminationStory));
                sb.AppendLine("member:" + Newtonsoft.Json.JsonConvert.SerializeObject(member));
                Common.SaveErrorMessage(ex, sb, typeof(IlluminationController));


                ModelState.AddModelError("", "*An error occured while creating a form to submit your story.");
                return PartialView("~/Views/Partials/IlluminationStories/_addIlluminationStory.cshtml", illuminationStory);
            }
        }
        public ActionResult RenderList(MembershipHelper membershipHelper)
        {
            //Instantiate variables
            var illuminationStoryList = new Models.IlluminationStoryList();

            try
            {
                //Determine viewBy settings
                if (!string.IsNullOrEmpty(Request.QueryString[Common.ViewByTypes.ViewBy]))
                {
                    illuminationStoryList.ViewBy = Request.QueryString[Common.ViewByTypes.ViewBy];
                }




                //Get all items
                if (ExamineManager.Instance.TryGetIndex(Common.searchProviders.ExternalIndex, out var index))
                {
                    //
                    ISearcher searcher = index.GetSearcher();
                    IBooleanOperation query = searcher.CreateQuery(null, BooleanOperation.Or)
                        .NodeTypeAlias(Common.docType.IlluminationStory);
                    //var query = searcher.CreateQuery(null, BooleanOperation.Or)
                    //    .NodeTypeAlias(Common.docType.IlluminationStory)
                    //    .OrderByDescending(new SortableField[] { new SortableField("publishDate") });
                    ISearchResults isResults = query.Execute(Int32.MaxValue);

                    if (isResults.Any())
                    {
                        //Get total experiences.
                        foreach (SearchResult sRecord in isResults)
                        {
                            switch (Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(sRecord[Common.NodeProperties.experienceType]).FirstOrDefault())
                            {
                                case Common.ExperienceType.Heavenly:
                                    illuminationStoryList.HeavenlyExperienceCount += 1;
                                    break;
                                case Common.ExperienceType.Hellish:
                                    illuminationStoryList.HellishExperienceCount += 1;
                                    break;
                                case Common.ExperienceType.Purgatorial:
                                    illuminationStoryList.PurgatorialExperienceCount += 1;
                                    break;
                                default:
                                    illuminationStoryList.OtherExperienceCount += 1;
                                    break;
                            }
                        }


                        //Rebuild results
                        if (!string.IsNullOrEmpty(illuminationStoryList.ViewBy))
                        {
                            //Add any viewBy parameters
                            switch (illuminationStoryList.ViewBy)
                            {
                                case Common.ViewByTypes.Heavenly:
                                    query.And().Field(Common.NodeProperties.experienceType, Common.ExperienceType.Heavenly);
                                    break;
                                case Common.ViewByTypes.Hellish:
                                    query.And().Field(Common.NodeProperties.experienceType, Common.ExperienceType.Hellish);
                                    break;
                                case Common.ViewByTypes.Purgatorial:
                                    query.And().Field(Common.NodeProperties.experienceType, Common.ExperienceType.Purgatorial);
                                    break;
                                case Common.ViewByTypes.OtherUnsure:
                                    query.And().Field(Common.NodeProperties.experienceType, Common.ExperienceType.Other);
                                    break;
                                default:
                                    break;
                            }


                            //Obtain updated result with query
                            isResults = query.Execute();
                        }


                        //Get item counts and total experiences.
                        illuminationStoryList.Pagination.itemsPerPage = 50;
                        illuminationStoryList.Pagination.totalItems = isResults.Count();


                        //Determine how many pages/items to skip and take, as well as the total page count for the search result.
                        if (illuminationStoryList.Pagination.totalItems > illuminationStoryList.Pagination.itemsPerPage)
                        {
                            illuminationStoryList.Pagination.totalPages = (int)Math.Ceiling((double)illuminationStoryList.Pagination.totalItems / (double)illuminationStoryList.Pagination.itemsPerPage);
                        }
                        else
                        {
                            illuminationStoryList.Pagination.itemsPerPage = illuminationStoryList.Pagination.totalItems;
                            illuminationStoryList.Pagination.totalPages = 1;
                        }


                        //Determine current page number 
                        var pageNo = 1;
                        if (!string.IsNullOrEmpty(Request.QueryString[Common.miscellaneous.PageNo]))
                        {
                            int.TryParse(Request.QueryString[Common.miscellaneous.PageNo], out pageNo);
                            if (pageNo <= 0 || pageNo > illuminationStoryList.Pagination.totalPages)
                            {
                                pageNo = 1;
                            }
                        }
                        illuminationStoryList.Pagination.pageNo = pageNo;


                        //Determine how many pages/items to skip
                        if (illuminationStoryList.Pagination.totalItems > illuminationStoryList.Pagination.itemsPerPage)
                        {
                            illuminationStoryList.Pagination.itemsToSkip = illuminationStoryList.Pagination.itemsPerPage * (pageNo - 1);
                        }

                        //Convert list of SearchResults to list of classes
                        //var k = isResults.ToList();
                        //var i = k.OrderByDescending(x => x["publishDate"]);

                        foreach (SearchResult sRecord in isResults.OrderByDescending(x => x["publishDate"]).Skip(illuminationStoryList.Pagination.itemsToSkip).Take(illuminationStoryList.Pagination.itemsPerPage))
                        {
                            var storyLink = new Models.illuminationStoryLink();
                            storyLink.experienceType = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(sRecord[Common.NodeProperties.experienceType]).FirstOrDefault();
                            storyLink.id = Convert.ToInt32(sRecord.Id);
                            storyLink.title = sRecord[Common.NodeProperties.title];
                            storyLink.url = Umbraco.Content(sRecord.Id).Url();
                            storyLink.PublishDate = Convert.ToDateTime(sRecord["publishDate"]);

                            //Obtain member 
                            ContentModels.Member CmMember;
                            int memberId;
                            StringBuilder sbAuthor = new StringBuilder();

                            if (int.TryParse(sRecord[Common.NodeProperties.member], out memberId))
                            {
                                IPublishedContent ipMember = membershipHelper.GetById(memberId);
                                if (ipMember != null)
                                {
                                    CmMember = new ContentModels.Member(ipMember);

                                    sbAuthor.Append(CmMember.FirstName);
                                    sbAuthor.Append("&nbsp;&nbsp;&nbsp;");
                                    sbAuthor.Append(CmMember.LastName);
                                    sbAuthor.Append(".");
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
                                    }
                                }
                            }

                            storyLink.memberName = sbAuthor.ToString();
                            illuminationStoryList.lstStoryLink.Add(storyLink);
                        }

                    }
                }

                ////Get all items
                //if (ExamineManager.Instance.TryGetIndex(Common.searchProviders.ExternalIndex, out var index))
                //{
                //    //
                //    ISearcher searcher = index.GetSearcher();
                //    IBooleanOperation query = searcher.CreateQuery(null, BooleanOperation.Or)
                //        .NodeTypeAlias(Common.docType.IlluminationStory);
                //    //var query = searcher.CreateQuery(null, BooleanOperation.Or)
                //    //    .NodeTypeAlias(Common.docType.IlluminationStory)
                //    //    .OrderByDescending(new SortableField[] { new SortableField("publishDate") });
                //    ISearchResults isResults = query.Execute(Int32.MaxValue);

                //    if (isResults.Any())
                //    {
                //        //Get total experiences.
                //        foreach (SearchResult sRecord in isResults)
                //        {
                //            switch (Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(sRecord[Common.NodeProperties.experienceType]).FirstOrDefault())
                //            {
                //                case Common.ExperienceType.Heavenly:
                //                    illuminationStoryList.HeavenlyExperienceCount += 1;
                //                    break;
                //                case Common.ExperienceType.Hellish:
                //                    illuminationStoryList.HellishExperienceCount += 1;
                //                    break;
                //                case Common.ExperienceType.Purgatorial:
                //                    illuminationStoryList.PurgatorialExperienceCount += 1;
                //                    break;
                //                default:
                //                    illuminationStoryList.OtherExperienceCount += 1;
                //                    break;
                //            }
                //        }


                //        //Rebuild results
                //        if (!string.IsNullOrEmpty(illuminationStoryList.ViewBy))
                //        {
                //            //Restart query
                //            //query = criteria.Field(Common.NodeProperties.umbracoNaviHide, "0"); //gets all items when this exists for every record.


                //            //Add any viewBy parameters
                //            switch (illuminationStoryList.ViewBy)
                //            {
                //                case Common.ViewByTypes.Heavenly:
                //                    query.And().Field(Common.NodeProperties.experienceType, Common.ExperienceType.Heavenly);
                //                    break;
                //                case Common.ViewByTypes.Hellish:
                //                    query.And().Field(Common.NodeProperties.experienceType, Common.ExperienceType.Hellish);
                //                    break;
                //                case Common.ViewByTypes.Purgatorial:
                //                    query.And().Field(Common.NodeProperties.experienceType, Common.ExperienceType.Purgatorial);
                //                    break;
                //                case Common.ViewByTypes.OtherUnsure:
                //                    query.And().Field(Common.NodeProperties.experienceType, Common.ExperienceType.Other);
                //                    break;
                //                default:
                //                    break;
                //            }


                //            //Obtain updated result with query
                //            isResults = query.Execute();
                //        }


                //        //Get item counts and total experiences.
                //        illuminationStoryList.Pagination.itemsPerPage = 50;
                //        illuminationStoryList.Pagination.totalItems = isResults.Count();


                //        //Determine how many pages/items to skip and take, as well as the total page count for the search result.
                //        if (illuminationStoryList.Pagination.totalItems > illuminationStoryList.Pagination.itemsPerPage)
                //        {
                //            illuminationStoryList.Pagination.totalPages = (int)Math.Ceiling((double)illuminationStoryList.Pagination.totalItems / (double)illuminationStoryList.Pagination.itemsPerPage);
                //        }
                //        else
                //        {
                //            illuminationStoryList.Pagination.itemsPerPage = illuminationStoryList.Pagination.totalItems;
                //            illuminationStoryList.Pagination.totalPages = 1;
                //        }


                //        //Determine current page number 
                //        var pageNo = 1;
                //        if (!string.IsNullOrEmpty(Request.QueryString[Common.miscellaneous.PageNo]))
                //        {
                //            int.TryParse(Request.QueryString[Common.miscellaneous.PageNo], out pageNo);
                //            if (pageNo <= 0 || pageNo > illuminationStoryList.Pagination.totalPages)
                //            {
                //                pageNo = 1;
                //            }
                //        }
                //        illuminationStoryList.Pagination.pageNo = pageNo;


                //        //Determine how many pages/items to skip
                //        if (illuminationStoryList.Pagination.totalItems > illuminationStoryList.Pagination.itemsPerPage)
                //        {
                //            illuminationStoryList.Pagination.itemsToSkip = illuminationStoryList.Pagination.itemsPerPage * (pageNo - 1);
                //        }


                //        //Response.Write("<h3>Records: " + isResults.Count() + "</h3>");
                //        //Response.Write("<h5>" + Newtonsoft.Json.JsonConvert.SerializeObject(isResults) + "</h5>");

                //        //Convert list of SearchResults to list of classes
                //        foreach (SearchResult sRecord in isResults.Skip(illuminationStoryList.Pagination.itemsToSkip).Take(illuminationStoryList.Pagination.itemsPerPage))
                //        {
                //            var storyLink = new Models.illuminationStoryLink();
                //            storyLink.experienceType = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(sRecord[Common.NodeProperties.experienceType]).FirstOrDefault();
                //            storyLink.id = Convert.ToInt32(sRecord.Id);
                //            storyLink.title = sRecord[Common.NodeProperties.title];
                //            storyLink.url = Umbraco.Content(sRecord.Id).Url();


                //            //Obtain member 
                //            ContentModels.Member CmMember;
                //            int memberId;
                //            StringBuilder sbAuthor = new StringBuilder();

                //            if (int.TryParse(sRecord[Common.NodeProperties.member], out memberId))
                //            {
                //                IPublishedContent ipMember = membershipHelper.GetById(memberId);
                //                if (ipMember != null)
                //                {
                //                    CmMember = new ContentModels.Member(ipMember);

                //                    sbAuthor.Append(CmMember.FirstName);
                //                    sbAuthor.Append("&nbsp;&nbsp;&nbsp;");
                //                    sbAuthor.Append(CmMember.LastName);
                //                    sbAuthor.Append(".");
                //                }
                //            }
                //            else
                //            {
                //                //Member id is not an int so attempt to parse as a guid
                //                if (GuidUdi.TryParse(sRecord[Common.NodeProperties.member], out GuidUdi memberUdi))
                //                {
                //                    //IPublishedContent ipMember = Umbraco.Content(memberUdi.Guid);
                //                    IPublishedContent ipMember = membershipHelper.GetById(memberUdi.Guid);
                //                    if (ipMember != null)
                //                    {
                //                        CmMember = new ContentModels.Member(ipMember);

                //                        sbAuthor.Append(CmMember.FirstName);
                //                        sbAuthor.Append("&nbsp;&nbsp;&nbsp;");
                //                        sbAuthor.Append(CmMember.LastName);
                //                        sbAuthor.Append(".");
                //                    }
                //                }
                //            }

                //            storyLink.memberName = sbAuthor.ToString();
                //            illuminationStoryList.lstStoryLink.Add(storyLink);
                //        }

                //    }
                //}


            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"IlluminationController.cs : RenderList()");
                sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(illuminationStoryList));
                Common.SaveErrorMessage(ex, sb, typeof(IlluminationController));


                ModelState.AddModelError("", "*An error occured while creating a list of illumination stories.");
                return CurrentUmbracoPage();
            }


            //Return data to partialview
            return PartialView("~/Views/Partials/IlluminationStories/_illuminationStoryList.cshtml", illuminationStoryList);
        }
        #endregion


        #region "Posts"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddIlluminationStory(Models.illuminationStory model)
        {
            try
            {
                SendIlluminationStoryByEmail(model);
            }
            catch (Exception ex)
            {
                //Save error message to umbraco
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"IlluminationController.cs : SendIlluminationStoryByEmail()");
                sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                Common.SaveErrorMessage(ex, sb, typeof(IlluminationController));
            }


            try
            {
                if (ModelState.IsValid)
                {
                    //Instantiate variables
                    IContentService contentService = Services.ContentService;

                    Guid guidKey = Umbraco.Content((int)Common.siteNode.IlluminationStories).Key;
                    IContent icIllumStory = contentService.Create(
                                            name: "Pending Publication: " + DateTime.Now.ToString(),  //name: model.Title.Substring(0, 1).ToUpper() + model.Title.Substring(1).ToLower(),
                                            parentId: guidKey,
                                            documentTypeAlias: Common.docType.IlluminationStory);

                    IMember member = Services.MemberService.GetById(model.memberId);

                    icIllumStory.SetValue(ContentModels.IlluminationStory.GetModelPropertyType(x => x.Title).Alias, "Pending Publication: " + DateTime.Now.ToString());//model.Title.Substring(0, 1).ToUpper() + model.Title.Substring(1).ToLower());
                    icIllumStory.SetValue(ContentModels.IlluminationStory.GetModelPropertyType(x => x.Story).Alias, model.Story);
                    icIllumStory.SetValue(ContentModels.IlluminationStory.GetModelPropertyType(x => x.Member).Alias, model.memberId);

                    string experienceType = model.ExperienceType.Value.ToString();
                    if ((Common.ExperienceTypes)System.Enum.Parse(typeof(Common.ExperienceTypes), experienceType) == Common.ExperienceTypes.OtherorUnsure) { experienceType = "Other or Unsure"; }
                    string aliasName = ContentModels.IlluminationStory.GetModelPropertyType(x => x.ExperienceType).Alias;
                    icIllumStory.SetValue(aliasName, Common.SerializeValue(experienceType));

                    //Save new Illumination story
                    //var result = contentService.SaveAndPublish(icIllumStory);
                    var result = contentService.Save(icIllumStory);


                    if (result.Success)
                    {
                        //Add demographics to member's records
                        if (member == null)
                        {
                            //Save error message to umbraco
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(@"IlluminationController.cs : AddIlluminationStory()");
                            sb.AppendLine(@"Member Id returned nothing.  Cannot add illumination story to member.");
                            sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                            Exception ex = new Exception();
                            Common.SaveErrorMessage(ex, sb, typeof(IlluminationController));
                        }
                        else
                        {
                            //Add data to member and save
                            member.SetValue(Common.NodeProperties.illuminationStory, new GuidUdi("document", icIllumStory.Key).ToString());
                            member.SetValue(Common.NodeProperties.age, model.lstAge.FirstOrDefault());

                            if (model.Country == null)
                            {
                                member.SetValue(Common.NodeProperties.country, "");
                            }
                            else
                            {
                                member.SetValue(Common.NodeProperties.country, model.Country.GetType().GetMember(model.Country.ToString()).First().GetCustomAttribute<DisplayAttribute>().GetName());
                            }


                            if (model.Gender == null)
                            {
                                member.SetValue(Common.NodeProperties.gender, "");
                            }
                            else
                            {
                                member.SetValue(
                                Common.NodeProperties.gender,
                                model.Gender.GetType().GetMember(model.Gender.ToString()).First().GetCustomAttribute<DisplayAttribute>().GetName());
                            }


                            if (model.Religion == null)
                            {
                                member.SetValue(Common.NodeProperties.religion, "");
                            }
                            else
                            {
                                member.SetValue(
                                Common.NodeProperties.religion,
                                Common.SerializeValue(model.Religion.GetType().GetMember(model.Religion.ToString()).First().GetCustomAttribute<DisplayAttribute>().GetName()));
                            }


                            if (model.Race == null)
                            {
                                member.SetValue(Common.NodeProperties.race, "");
                            }
                            else
                            {
                                member.SetValue(
                                Common.NodeProperties.race,
                                Common.SerializeValue(model.Race.GetType().GetMember(model.Race.ToString()).First().GetCustomAttribute<DisplayAttribute>().GetName()));
                            }


                            //Save all changes
                            Services.MemberService.Save(member);
                        }

                        //Update all statistical data
                        UpdateAllStatistics(Umbraco, Services.MemberService, contentService);

                        //Return to page
                        TempData["IlluminationStoryAddedSuccessfully"] = true;
                        return RedirectToUmbracoPage((int)(Models.Common.siteNode.AddEditIlluminationStory));
                    }
                    else
                    {
                        //Save error message to umbraco
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(@"Controllers/IlluminationController.cs : AddIlluminationStory()");
                        sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                        sb.AppendLine("result:" + Newtonsoft.Json.JsonConvert.SerializeObject(result));
                        Exception ex = new Exception();
                        Common.SaveErrorMessage(ex, sb, typeof(IlluminationController));

                        //Return with error
                        ModelState.AddModelError(string.Empty, "An error occured while submitting your Illumination story.");
                        TempData["showAddEditPnl"] = true;
                        return CurrentUmbracoPage();
                    }
                }
                else
                {
                    TempData["showAddEditPnl"] = true;
                    return CurrentUmbracoPage();
                }

            }
            catch (Exception ex)
            {
                //Save error message to umbraco
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"IlluminationController.cs : AddIlluminationStory()");
                sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                Common.SaveErrorMessage(ex, sb, typeof(IlluminationController));

                ModelState.AddModelError(string.Empty, "An error occured while adding your story.");
                return CurrentUmbracoPage();
            }
        }



        private void SendIlluminationStoryByEmail(Models.illuminationStory model)
        {
            //
            try
            {
                //Open email files.
                string filePath_html = HostingEnvironment.MapPath("~/Emails/IlluminationStory/IlluminationStory.html");
                string filePath_Text = HostingEnvironment.MapPath("~/Emails/IlluminationStory/IlluminationStory.txt");
                string emailBody_Html = System.IO.File.ReadAllText(filePath_html);
                string emailBody_Text = System.IO.File.ReadAllText(filePath_Text);
                string hostUrl = System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Host + "/";
                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(model);

                //Obtain list of email addresses
                List<string> LstEmails = new List<string>();
                string emailList = "saintpiofan@afterthewarning.com;jim.fifth@5thstudios.com;";
                while (emailList.Contains(";;"))
                {
                    emailList = emailList.Replace(";;", ";");
                }
                foreach (string emailAddress in emailList.Split(';'))
                {
                    if (IsValidEmail(emailAddress))
                    {
                        LstEmails.Add(emailAddress);
                    }
                }

                // Insert data into page
                emailBody_Html = emailBody_Html.Replace("[AFTERTHEWARNING_URL]", hostUrl);
                emailBody_Html = emailBody_Html.Replace("[INFO]", model.Story.Replace(Environment.NewLine, "<br/>"));
                emailBody_Html = emailBody_Html.Replace("[JSON]", jsonData);
                emailBody_Html = emailBody_Html.Replace("[DATE]", DateTime.Today.ToString("MMMM d, yyyy"));
                emailBody_Html = emailBody_Html.Replace("[YEAR]", DateTime.Today.Year.ToString());

                emailBody_Text = emailBody_Text.Replace("[AFTERTHEWARNING_URL]", hostUrl);
                emailBody_Text = emailBody_Text.Replace("[INFO]", model.Story.Replace(Environment.NewLine, "<br/>"));
                emailBody_Text = emailBody_Text.Replace("[JSON]", jsonData);
                emailBody_Text = emailBody_Text.Replace("[DATE]", DateTime.Today.ToString("MMMM d, yyyy"));
                emailBody_Text = emailBody_Text.Replace("[YEAR]", DateTime.Today.Year.ToString());

                // Obtain smtp
                SmtpClient smtp = new SmtpClient();

                // Create mail message
                MailMessage Msg = new MailMessage();
                Msg.BodyEncoding = Encoding.UTF8;
                Msg.SubjectEncoding = Encoding.UTF8;
                Msg.Subject = "Illumination Submission | After the Warning";
                Msg.IsBodyHtml = true;
                Msg.Body = "";
                Msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(emailBody_Text, new System.Net.Mime.ContentType(MediaTypeNames.Text.Plain)));
                Msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(emailBody_Html, new System.Net.Mime.ContentType(MediaTypeNames.Text.Html)));

                //Loop through each email address   
                foreach (string emailTo in LstEmails)
                {
                    try
                    {
                        //Clear email list and add new member email
                        Msg.To.Clear();
                        Msg.To.Add(new MailAddress(emailTo));

                        // Send email
                        smtp.Send(Msg);
                    }
                    catch (Exception e)
                    {
                        //Save error message to umbraco
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(@"CustomAPIController.cs : SendEmail()");
                        sb.AppendLine(@"Error sending email to:" + emailTo);
                        Common.SaveErrorMessage(e, sb, typeof(CustomAPIController));
                    }
                }

                //Close connection after emails have been sent.
                smtp.ServicePoint.CloseConnectionGroup(smtp.ServicePoint.ConnectionName);

            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"CommonController : SendEmail()");
                Common.SaveErrorMessage(ex, sb, typeof(CustomAPIController));

                //TempData[Common.Miscellaneous.SubmittedSuccessfully] = false;
                //TempData[Common.Miscellaneous.SubmissionFailed] = true;
                ModelState.AddModelError("", "An error occured while submitting emails.");
            }
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult UpdateIlluminationStory(Models.illuminationStory model)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            //Instantiate variables
        //            IContentService contentService = Services.ContentService;
        //            IContent icIllumStory = contentService.GetById(model.storyId);
        //            IMember member = Services.MemberService.GetById(model.memberId);

        //            //Update data
        //            icIllumStory.SetValue(ContentModels.IlluminationStory.GetModelPropertyType(x => x.Title).Alias, model.Title.Substring(0, 1).ToUpper() + model.Title.Substring(1).ToLower());
        //            icIllumStory.SetValue(ContentModels.IlluminationStory.GetModelPropertyType(x => x.Story).Alias, model.Story);
        //            icIllumStory.SetValue(ContentModels.IlluminationStory.GetModelPropertyType(x => x.Member).Alias, model.memberId);

        //            string experienceType = model.ExperienceType.Value.ToString();
        //            if ((Common.ExperienceTypes)System.Enum.Parse(typeof(Common.ExperienceTypes), experienceType) == Common.ExperienceTypes.OtherorUnsure) { experienceType = "Other or Unsure"; }
        //            string aliasName = ContentModels.IlluminationStory.GetModelPropertyType(x => x.ExperienceType).Alias;
        //            icIllumStory.SetValue(aliasName, Common.SerializeValue(experienceType));


        //            //Save  Illumination story
        //            var result = contentService.SaveAndPublish(icIllumStory);


        //            if (result.Success)
        //            {
        //                //Add demographics to member's records
        //                if (member == null)
        //                {
        //                    //Save error message to umbraco
        //                    StringBuilder sb = new StringBuilder();
        //                    sb.AppendLine(@"Controllers/IlluminationController.cs : UpdateIlluminationStory()");
        //                    sb.AppendLine(@"Member Id returned nothing.  Cannot update illumination story to member.");
        //                    sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
        //                    Exception ex = new Exception();
        //                    Common.SaveErrorMessage(ex, sb, typeof(IlluminationController));
        //                }
        //                else
        //                {
        //                    //Add data to member and save
        //                    member.SetValue(Common.NodeProperties.age, model.Age);

        //                    if (model.Country == null)
        //                    {
        //                        member.SetValue(Common.NodeProperties.country, "");
        //                    }
        //                    else
        //                    {
        //                        member.SetValue(Common.NodeProperties.country, model.Country.GetType().GetMember(model.Country.ToString()).First().GetCustomAttribute<DisplayAttribute>().GetName());
        //                    }


        //                    if (model.Gender == null)
        //                    {
        //                        member.SetValue(Common.NodeProperties.gender, "");
        //                    }
        //                    else
        //                    {
        //                        member.SetValue(
        //                        Common.NodeProperties.gender,
        //                        model.Gender.GetType().GetMember(model.Gender.ToString()).First().GetCustomAttribute<DisplayAttribute>().GetName());
        //                    }


        //                    if (model.Religion == null)
        //                    {
        //                        member.SetValue(Common.NodeProperties.religion, "");
        //                    }
        //                    else
        //                    {
        //                        member.SetValue(
        //                        Common.NodeProperties.religion,
        //                        Common.SerializeValue(model.Religion.GetType().GetMember(model.Religion.ToString()).First().GetCustomAttribute<DisplayAttribute>().GetName()));
        //                    }


        //                    if (model.Race == null)
        //                    {
        //                        member.SetValue(Common.NodeProperties.race, "");
        //                    }
        //                    else
        //                    {
        //                        member.SetValue(
        //                        Common.NodeProperties.race,
        //                        Common.SerializeValue(model.Race.GetType().GetMember(model.Race.ToString()).First().GetCustomAttribute<DisplayAttribute>().GetName()));
        //                    }

        //                    //Save all changes
        //                    Services.MemberService.Save(member);
        //                }

        //                //Update all statistical data
        //                UpdateAllStatistics(Umbraco, Services.MemberService, contentService);

        //                //Return to page
        //                TempData["IlluminationStoryUpdatedSuccessfully"] = true;
        //                return RedirectToUmbracoPage((int)(Models.Common.siteNode.AddEditIlluminationStory));
        //            }
        //            else
        //            {
        //                //Save error message to umbraco
        //                StringBuilder sb = new StringBuilder();
        //                sb.AppendLine(@"Controllers/IlluminationController.cs : UpdateIlluminationStory()");
        //                sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
        //                sb.AppendLine("result:" + Newtonsoft.Json.JsonConvert.SerializeObject(result));
        //                Exception ex = new Exception();
        //                Common.SaveErrorMessage(ex, sb, typeof(IlluminationController));

        //                //Return with error
        //                ModelState.AddModelError(string.Empty, "An error occured while updating your Illumination story.");
        //                TempData["showAddEditPnl"] = true;
        //                return CurrentUmbracoPage();
        //            }
        //        }
        //        else
        //        {
        //            TempData["showAddEditPnl"] = true;
        //            return CurrentUmbracoPage();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //Save error message to umbraco
        //        StringBuilder sb = new StringBuilder();
        //        sb.AppendLine(@"IlluminationController.cs : UpdateIlluminationStory()");
        //        sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
        //        Common.SaveErrorMessage(ex, sb, typeof(IlluminationController));

        //        ModelState.AddModelError(string.Empty, "An error occured while updating your story.");
        //        return CurrentUmbracoPage();
        //    }
        //}
        #endregion


        #region "Methods"
        public static Models.IlluminationStoryContent ObtainIlluminationStoryContent(System.Security.Principal.IPrincipal User, UmbracoHelper umbracoHelper, HtmlHelper Html, ContentModels.IlluminationStory cmModel)
        {
            //
            Models.IlluminationStoryContent PgContent = new Models.IlluminationStoryContent();

            //Redirect to home if illumination settings are not active.
            IPublishedContent ipHome = umbracoHelper.Content((int)(Common.siteNode.Home));
            if (ipHome.Value<Boolean>(Common.NodeProperties.activateIlluminationControls) != true)
            {
                PgContent.RedirectHome = true;
            }
            else if (!User.Identity.IsAuthenticated)
            {
                //Redirect to login page.
                PgContent.Redirect = true;
                PgContent.RedirectTo = umbracoHelper.Content((int)(Models.Common.siteNode.Login)).Url();
            }
            else
            {
                //Obtain page data
                PgContent.Title = cmModel.Title;
                PgContent.ExperienceType = cmModel.ExperienceType;
                PgContent.Story = Html.Raw(Html.ReplaceLineBreaks(cmModel.Story));

                PgContent.CmMember = new ContentModels.Member(cmModel.Member);

                //Add data to story model
                StringBuilder sbAuthor = new StringBuilder();
                sbAuthor.Append(PgContent.CmMember.FirstName);
                sbAuthor.Append("&nbsp;&nbsp;&nbsp;");
                sbAuthor.Append(PgContent.CmMember.LastName);
                sbAuthor.Append(".");
                PgContent.MemberName = sbAuthor.ToString();

                //PgContent.Gender = umbracoHelper.GetPreValueAsString(PgContent.CmMember.Gender);
                if (!string.IsNullOrEmpty(PgContent.CmMember.Gender)) PgContent.Gender = umbracoHelper.GetDictionaryValue(PgContent.CmMember.Gender);
                PgContent.Religion = PgContent.CmMember.Religion;
                PgContent.Country = PgContent.CmMember.Country;


                //Obtain the form and its view model
                ConfiguredFormInfo pickedForm = cmModel.Parent.Value<ConfiguredFormInfo>("formPicker");
                PgContent.PickedForm = cmModel.Parent.Value<ConfiguredFormInfo>("formPicker");
            }


            return PgContent;
        }
        public static Models.IlluminationStoryListContent ObtainIlluminationStoryListContent(UmbracoHelper Umbraco, string Url)
        {
            IlluminationStoryListContent illuminationStoryListContent = new IlluminationStoryListContent();

            //build base url for filter links
            illuminationStoryListContent.baseUrl = Url;
            if (illuminationStoryListContent.baseUrl.Contains("?"))
            {
                illuminationStoryListContent.baseUrl = (illuminationStoryListContent.baseUrl.Substring(0, illuminationStoryListContent.baseUrl.IndexOf("?")));
            }

            //Create links for dropdown and buttons
            illuminationStoryListContent.urlViewAll = illuminationStoryListContent.baseUrl;
            illuminationStoryListContent.urlHeavenly = illuminationStoryListContent.baseUrl + "?viewBy=Heavenly";
            illuminationStoryListContent.urlHellish = illuminationStoryListContent.baseUrl + "?viewBy=Hellish";
            illuminationStoryListContent.urlPurgatorial = illuminationStoryListContent.baseUrl + "?viewBy=Purgatorial";
            illuminationStoryListContent.urlOtherUnsure = illuminationStoryListContent.baseUrl + "?viewBy=OtherUnsure";
            illuminationStoryListContent.urlAddEditIllumStory = Umbraco.Content((int)Common.siteNode.AddEditIlluminationStory).Url();
            illuminationStoryListContent.urlViewIllumStatistics = Umbraco.Content((int)Common.siteNode.IlluminationStatistics).Url();
            //illuminationStoryListContent.urlDownloadStories = UmbracoContext.Current.PublishedContentRequest.PublishedContent.Value<IPublishedContent>(Common.NodeProperties.compiledStories).Url();
            illuminationStoryListContent.urlDownloadStories = Umbraco.AssignedContentItem.Value<IPublishedContent>(Common.NodeProperties.compiledStories).Url();

            return illuminationStoryListContent;
        }
        public static Models.AddEditIlluminationStoryContent DoesStoryExist(UmbracoHelper Umbraco, System.Security.Principal.IPrincipal User, IMemberService memberService)
        {
            //Instantiate variables.
            Models.AddEditIlluminationStoryContent PgContent = new Models.AddEditIlluminationStoryContent();
            //StringBuilder sb = new StringBuilder();
            //sb.AppendLine(@"IlluminationController.cs : DoesStoryExist()");

            try
            {
                //Determine if user already submitted an illumination story AND if it is published
                PgContent.Member = memberService.GetByUsername(User.Identity.Name);
                if (PgContent.Member != null)
                {
                    //sb.AppendLine(@"PgContent.Member.Id: " + PgContent.Member.Id);

                    if (PgContent.Member.GetValue(Common.NodeProperties.illuminationStory) != null)
                    {
                        //sb.AppendLine(@"Member.illuminationStory != null");


                        if (!string.IsNullOrWhiteSpace(PgContent.Member.GetValue<string>(Common.NodeProperties.illuminationStory)))
                        {
                            PgContent.DoesStoryExist = true;

                            //sb.AppendLine(@"ipIlluminationStory: " + PgContent.Member.GetValue<string>(Common.NodeProperties.illuminationStory));

                            //IPublishedContent ipStory = PgContent.Member.GetValue<IPublishedContent>(Common.NodeProperties.illuminationStory);
                            IPublishedContent ipStory = Umbraco.Content(PgContent.Member.GetValue<string>(Common.NodeProperties.illuminationStory));

                            //sb.AppendLine(@"ipStory.IsPublished(): " + ipStory.IsPublished().ToString());

                            if (ipStory != null && ipStory.IsPublished())
                            {
                                PgContent.IsStoryPublished = true;
                                //sb.AppendLine(@"ipStory: " + ipStory.Name);
                                //sb.AppendLine(@"PgContent.IsStoryPublished = true");
                                //sb.AppendLine(@"ipStory.Id: " + ipStory.Id);
                            }

                        }
                    }
                }
            }
            catch { }

            //Common.SaveErrorMessage(new Exception(), sb, typeof(IlluminationController));


            return PgContent;
        }


        public static Models.LineCharts ObtainStatistics_byAge(IPublishedContent ipByAge)
        {
            //Initialize variables.
            Models.LineCharts statsByAge = new Models.LineCharts();
            statsByAge.LstChartData = new List<ChartDataset>();
            statsByAge.LstAgeRange = new List<string>();
            ChartDataset HeavenlyDataset = new ChartDataset("Heavenly", "Heavenly", "#4f81bc");
            ChartDataset HellishDataset = new ChartDataset("Hellish", "Hellish", "#bf4b49");
            ChartDataset PurgatorialDataset = new ChartDataset("Purgatorial", "Purgatorial", "#9bbb57");
            ChartDataset UnknownDataset = new ChartDataset("Unknown", "Unknown/Unsure", "#7f7f7f");
            statsByAge.TotalEntries = 0;

            //Obtain data from node
            foreach (IPublishedElement ipe in ipByAge.Value<IEnumerable<IPublishedElement>>(Common.NodeProperties.statsHeavenly))
            {
                Models.LineChart chart = new Models.LineChart();
                chart.AgeRange = ipe.Value<string>(Common.NodeProperties.ageRange);
                chart.Count = ipe.Value<int>(Common.NodeProperties.count);
                statsByAge.LstAgeStats_Heavenly.Add(chart);
            }
            foreach (IPublishedElement ipe in ipByAge.Value<IEnumerable<IPublishedElement>>(Common.NodeProperties.statsHellish))
            {
                Models.LineChart chart = new Models.LineChart();
                chart.AgeRange = ipe.Value<string>(Common.NodeProperties.ageRange);
                chart.Count = ipe.Value<int>(Common.NodeProperties.count);
                statsByAge.LstAgeStats_Hellish.Add(chart);
            }
            foreach (IPublishedElement ipe in ipByAge.Value<IEnumerable<IPublishedElement>>(Common.NodeProperties.statsPurgatorial))
            {
                Models.LineChart chart = new Models.LineChart();
                chart.AgeRange = ipe.Value<string>(Common.NodeProperties.ageRange);
                chart.Count = ipe.Value<int>(Common.NodeProperties.count);
                statsByAge.LstAgeStats_Purgatorial.Add(chart);
            }
            foreach (IPublishedElement ipe in ipByAge.Value<IEnumerable<IPublishedElement>>(Common.NodeProperties.statsUnknown))
            {
                Models.LineChart chart = new Models.LineChart();
                chart.AgeRange = ipe.Value<string>(Common.NodeProperties.ageRange);
                chart.Count = ipe.Value<int>(Common.NodeProperties.count);
                statsByAge.LstAgeStats_Unknown.Add(chart);
            }

            //Add data to proper datasets
            foreach (LineChart stat in statsByAge.LstAgeStats_Heavenly)
            {
                HeavenlyDataset.LstData.Add(stat.Count); //Add data to list
                statsByAge.TotalEntries += stat.Count; //Increment total entries

                statsByAge.LstAgeRange.Add(stat.AgeRange); //Add text range [all are the same so only need to do this once.]
            }
            foreach (LineChart stat in statsByAge.LstAgeStats_Hellish)
            {
                HellishDataset.LstData.Add(stat.Count);
                statsByAge.TotalEntries += stat.Count;
            }
            foreach (LineChart stat in statsByAge.LstAgeStats_Purgatorial)
            {
                PurgatorialDataset.LstData.Add(stat.Count);
                statsByAge.TotalEntries += stat.Count;
            }
            foreach (LineChart stat in statsByAge.LstAgeStats_Unknown)
            {
                UnknownDataset.LstData.Add(stat.Count);
                statsByAge.TotalEntries += stat.Count;
            }

            //Stringify lists
            statsByAge.jsonAgeRange = Newtonsoft.Json.JsonConvert.SerializeObject(statsByAge.LstAgeRange);
            HeavenlyDataset.JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(HeavenlyDataset.LstData);
            HellishDataset.JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(HellishDataset.LstData);
            PurgatorialDataset.JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(PurgatorialDataset.LstData);
            UnknownDataset.JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(UnknownDataset.LstData);

            //Add Charts to list.
            statsByAge.LstChartData.Add(HeavenlyDataset);
            statsByAge.LstChartData.Add(HellishDataset);
            statsByAge.LstChartData.Add(PurgatorialDataset);
            statsByAge.LstChartData.Add(UnknownDataset);

            //Return stats
            return statsByAge;
        }
        public static Models.StackedBarChart ObtainStatistics_byCountry(IPublishedContent ip)
        {
            //Instantiate variables
            Models.StackedBarChart stats = new Models.StackedBarChart();
            List<string> lstLabels = new List<string>();
            List<int> lstValues_Heavenly = new List<int>();
            List<int> lstValues_Hellish = new List<int>();
            List<int> lstValues_Purgatorial = new List<int>();
            List<int> lstValues_Unknown = new List<int>();
            int totalExperiences = 0;
            int totalPercentage = 0;
            int heavenly = 0;
            int hellish = 0;
            int purgatorial = 0;
            int unknown = 0;

            //Obtain data from ip
            List<Models.ExperienceByCountry> lstExperiences = new List<ExperienceByCountry>();
            foreach (IPublishedElement ipe in ip.Value<IEnumerable<IPublishedElement>>(Common.NodeProperties.experiencesByCountry))
            {
                Models.ExperienceByCountry chart = new Models.ExperienceByCountry();
                chart.Label = ipe.Value<string>(Common.NodeProperties.label);
                chart.Heavenly = ipe.Value<int>(Common.NodeProperties.heavenly);
                chart.Hellish = ipe.Value<int>(Common.NodeProperties.hellish);
                chart.Purgatorial = ipe.Value<int>(Common.NodeProperties.purgatorial);
                chart.Other = ipe.Value<int>(Common.NodeProperties.other);
                lstExperiences.Add(chart);
            }

            //Extract data
            foreach (Models.ExperienceByCountry experience in lstExperiences)
            {
                //Add label to list
                lstLabels.Add(experience.Label);

                //get total
                totalExperiences = experience.Heavenly + experience.Hellish + experience.Purgatorial + experience.Other;

                //get total percentages
                heavenly = Convert.ToInt32(Math.Round(((decimal)experience.Heavenly / totalExperiences) * 100, 0));
                hellish = Convert.ToInt32(Math.Round(((decimal)experience.Hellish / totalExperiences) * 100, 0));
                purgatorial = Convert.ToInt32(Math.Round(((decimal)experience.Purgatorial / totalExperiences) * 100, 0));
                unknown = Convert.ToInt32(Math.Round(((decimal)experience.Other / totalExperiences) * 100, 0));

                totalPercentage = heavenly + hellish + purgatorial + unknown;

                //Add percentages to list
                lstValues_Heavenly.Add(heavenly);
                lstValues_Hellish.Add(hellish);
                lstValues_Purgatorial.Add(purgatorial);
                lstValues_Unknown.Add(unknown + (100 - totalPercentage)); //Add remainder to ensure total = 100%
            }

            //Convert extracted data to json
            stats.Labels = Newtonsoft.Json.JsonConvert.SerializeObject(lstLabels);
            stats.Values_Heavenly = Newtonsoft.Json.JsonConvert.SerializeObject(lstValues_Heavenly);
            stats.Values_Hellish = Newtonsoft.Json.JsonConvert.SerializeObject(lstValues_Hellish);
            stats.Values_Purgatorial = Newtonsoft.Json.JsonConvert.SerializeObject(lstValues_Purgatorial);
            stats.Values_Unknown = Newtonsoft.Json.JsonConvert.SerializeObject(lstValues_Unknown);

            //sets the canvas height to increase bar heights
            stats.Height = lstLabels.Count * 20;

            //Return stats
            return stats;
        }
        public static Models.PieChart ObtainStatistics_byExperienceType(IPublishedContent ip)
        {
            //Instantiate stat object
            PieChart stats = new PieChart();

            //Extractor data from ip
            stats.lstValues.Add(ip.Value<int>(Common.NodeProperties.heavenly));
            stats.lstValues.Add(ip.Value<int>(Common.NodeProperties.hellish));
            stats.lstValues.Add(ip.Value<int>(Common.NodeProperties.purgatorial));
            stats.lstValues.Add(ip.Value<int>(Common.NodeProperties.other));

            //Static data
            stats.lstLabels.Add("Heavenly");
            stats.lstLabels.Add("Purgatorial");
            stats.lstLabels.Add("Hellish");
            stats.lstLabels.Add("Unknown/Unsure");

            List<string> lstBgColors = new List<string>();
            lstBgColors.Add("#4f81bc");
            lstBgColors.Add("#9bbb57");
            lstBgColors.Add("#bf4b49");
            lstBgColors.Add("#7f7f7f");

            List<string> lstHoverBgColors = new List<string>();
            lstHoverBgColors.Add("#8CACD3");
            lstHoverBgColors.Add("#BDD391");
            lstHoverBgColors.Add("#D58987");
            lstHoverBgColors.Add("#b9b9b9");

            //Convert extracted data to json
            stats.BgColors = Newtonsoft.Json.JsonConvert.SerializeObject(lstBgColors);
            stats.HoverBgColors = Newtonsoft.Json.JsonConvert.SerializeObject(lstHoverBgColors);
            stats.Labels = Newtonsoft.Json.JsonConvert.SerializeObject(stats.lstLabels);
            stats.Values = Newtonsoft.Json.JsonConvert.SerializeObject(stats.lstValues);

            //
            foreach (int value in stats.lstValues)
            {
                stats.TotalEntries += value;
            }

            //Return stats
            return stats;
        }
        public static Models.BarChart ObtainStatistics_byGender(IPublishedContent ip)
        {
            //Instantiate variables
            Models.BarChart stats = new Models.BarChart();
            List<Models.ExperienceByGender> lstHeavenly = new List<ExperienceByGender>();
            List<Models.ExperienceByGender> lstHellish = new List<ExperienceByGender>();
            List<Models.ExperienceByGender> lstPurgatorial = new List<ExperienceByGender>();
            List<Models.ExperienceByGender> lstUnknown = new List<ExperienceByGender>();

            //Obtain data from ip
            foreach (IPublishedElement ipe in ip.Value<IEnumerable<IPublishedElement>>(Common.NodeProperties.statsHeavenly))
            {
                Models.ExperienceByGender chart = new Models.ExperienceByGender();
                chart.AgeRange = ipe.Value<string>(Common.NodeProperties.ageRange);
                chart.Males = ipe.Value<int>(Common.NodeProperties.males);
                chart.Females = ipe.Value<int>(Common.NodeProperties.females);
                lstHeavenly.Add(chart);
            }
            foreach (IPublishedElement ipe in ip.Value<IEnumerable<IPublishedElement>>(Common.NodeProperties.statsHellish))
            {
                Models.ExperienceByGender chart = new Models.ExperienceByGender();
                chart.AgeRange = ipe.Value<string>(Common.NodeProperties.ageRange);
                chart.Males = ipe.Value<int>(Common.NodeProperties.males);
                chart.Females = ipe.Value<int>(Common.NodeProperties.females);
                lstHellish.Add(chart);
            }
            foreach (IPublishedElement ipe in ip.Value<IEnumerable<IPublishedElement>>(Common.NodeProperties.statsPurgatorial))
            {
                Models.ExperienceByGender chart = new Models.ExperienceByGender();
                chart.AgeRange = ipe.Value<string>(Common.NodeProperties.ageRange);
                chart.Males = ipe.Value<int>(Common.NodeProperties.males);
                chart.Females = ipe.Value<int>(Common.NodeProperties.females);
                lstPurgatorial.Add(chart);
            }
            foreach (IPublishedElement ipe in ip.Value<IEnumerable<IPublishedElement>>(Common.NodeProperties.statsUnknown))
            {
                Models.ExperienceByGender chart = new Models.ExperienceByGender();
                chart.AgeRange = ipe.Value<string>(Common.NodeProperties.ageRange);
                chart.Males = ipe.Value<int>(Common.NodeProperties.males);
                chart.Females = ipe.Value<int>(Common.NodeProperties.females);
                lstUnknown.Add(chart);
            }

            //Extract data | Labels
            foreach (var stat in lstHeavenly)
            {
                stats.lstLabels.Add(stat.AgeRange);
            }

            //Extract data | Heavenly
            foreach (var stat in lstHeavenly)
            {
                stats.lstValues_Heavenly_Males.Add(stat.Males);
                stats.lstValues_Heavenly_Females.Add(stat.Females);
            }

            //Extract data | Hellish
            foreach (var stat in lstHellish)
            {
                stats.lstValues_Hellish_Males.Add(stat.Males);
                stats.lstValues_Hellish_Females.Add(stat.Females);
            }

            //Extract data | Purgatorial
            foreach (var stat in lstPurgatorial)
            {
                stats.lstValues_Purgatorial_Males.Add(stat.Males);
                stats.lstValues_Purgatorial_Females.Add(stat.Females);
            }

            //Extract data | Unknown
            foreach (var stat in lstUnknown)
            {
                stats.lstValues_Other_Males.Add(stat.Males);
                stats.lstValues_Other_Females.Add(stat.Females);
            }

            //Convert to json
            stats.jsonLabels = Newtonsoft.Json.JsonConvert.SerializeObject(stats.lstLabels);
            stats.jsonValues_Heavenly_Males = Newtonsoft.Json.JsonConvert.SerializeObject(stats.lstValues_Heavenly_Males);
            stats.jsonValues_Heavenly_Females = Newtonsoft.Json.JsonConvert.SerializeObject(stats.lstValues_Heavenly_Females);
            stats.jsonValues_Hellish_Males = Newtonsoft.Json.JsonConvert.SerializeObject(stats.lstValues_Hellish_Males);
            stats.jsonValues_Hellish_Females = Newtonsoft.Json.JsonConvert.SerializeObject(stats.lstValues_Hellish_Females);
            stats.jsonValues_Purgatorial_Males = Newtonsoft.Json.JsonConvert.SerializeObject(stats.lstValues_Purgatorial_Males);
            stats.jsonValues_Purgatorial_Females = Newtonsoft.Json.JsonConvert.SerializeObject(stats.lstValues_Purgatorial_Females);
            stats.jsonValues_Other_Males = Newtonsoft.Json.JsonConvert.SerializeObject(stats.lstValues_Other_Males);
            stats.jsonValues_Other_Females = Newtonsoft.Json.JsonConvert.SerializeObject(stats.lstValues_Other_Females);

            //sets the canvas height to increase bar heights
            stats.Height = lstHeavenly.Count * 20;

            return stats;
        }
        public static Models.StackedBarChart ObtainStatistics_byRace(IPublishedContent ip)
        {
            //Instantiate variables
            List<string> lstLabels = new List<string>();
            List<int> lstValues_Heavenly = new List<int>();
            List<int> lstValues_Hellish = new List<int>();
            List<int> lstValues_Purgatorial = new List<int>();
            List<int> lstValues_Unknown = new List<int>();
            int totalExperiences = 0;
            int totalPercentage = 0;
            int heavenly = 0;
            int hellish = 0;
            int purgatorial = 0;
            int unknown = 0;

            //Obtain data from ip
            List<Models.ExperienceByRace> lstExperiences = new List<Models.ExperienceByRace>();
            foreach (IPublishedElement ipe in ip.Value<IEnumerable<IPublishedElement>>(Common.NodeProperties.experiencesByRace))
            {
                Models.ExperienceByRace chart = new Models.ExperienceByRace();
                chart.Label = ipe.Value<string>(Common.NodeProperties.label);
                chart.Heavenly = ipe.Value<int>(Common.NodeProperties.heavenly);
                chart.Hellish = ipe.Value<int>(Common.NodeProperties.hellish);
                chart.Purgatorial = ipe.Value<int>(Common.NodeProperties.purgatorial);
                chart.Other = ipe.Value<int>(Common.NodeProperties.other);
                lstExperiences.Add(chart);
            }

            //Extract data
            foreach (Models.ExperienceByRace experience in lstExperiences)
            {
                //Add label to list
                lstLabels.Add(experience.Label);

                //get total
                totalExperiences = experience.Heavenly + experience.Hellish + experience.Purgatorial + experience.Other;

                //get total percentages
                if (totalExperiences > 0)
                {
                    heavenly = Convert.ToInt32(Math.Round(((decimal)experience.Heavenly / totalExperiences) * 100, 0));
                    hellish = Convert.ToInt32(Math.Round(((decimal)experience.Hellish / totalExperiences) * 100, 0));
                    purgatorial = Convert.ToInt32(Math.Round(((decimal)experience.Purgatorial / totalExperiences) * 100, 0));
                    unknown = Convert.ToInt32(Math.Round(((decimal)experience.Other / totalExperiences) * 100, 0));
                }

                totalPercentage = heavenly + hellish + purgatorial + unknown;

                //Add percentages to list
                lstValues_Heavenly.Add(heavenly);
                lstValues_Hellish.Add(hellish);
                lstValues_Purgatorial.Add(purgatorial);
                lstValues_Unknown.Add(unknown + (100 - totalPercentage)); //Add remainder to ensure total = 100%
            }

            //Convert extracted data to json
            Models.StackedBarChart stats = new Models.StackedBarChart();
            stats.Labels = Newtonsoft.Json.JsonConvert.SerializeObject(lstLabels);
            stats.Values_Heavenly = Newtonsoft.Json.JsonConvert.SerializeObject(lstValues_Heavenly);
            stats.Values_Hellish = Newtonsoft.Json.JsonConvert.SerializeObject(lstValues_Hellish);
            stats.Values_Purgatorial = Newtonsoft.Json.JsonConvert.SerializeObject(lstValues_Purgatorial);
            stats.Values_Unknown = Newtonsoft.Json.JsonConvert.SerializeObject(lstValues_Unknown);

            //sets the canvas height to increase bar heights
            stats.Height = lstLabels.Count * 20;

            //Return stats
            return stats;
        }
        public static Models.StackedBarChart ObtainStatistics_byReligion(IPublishedContent ip)
        {
            //Instantiate variables
            List<string> lstLabels = new List<string>();
            List<int> lstValues_Heavenly = new List<int>();
            List<int> lstValues_Hellish = new List<int>();
            List<int> lstValues_Purgatorial = new List<int>();
            List<int> lstValues_Unknown = new List<int>();
            int totalExperiences = 0;
            int totalPercentage = 0;
            int heavenly = 0;
            int hellish = 0;
            int purgatorial = 0;
            int unknown = 0;

            //Obtain data from ip
            List<Models.ExperienceByReligion> lstExperiences = new List<ExperienceByReligion>();
            foreach (IPublishedElement ipe in ip.Value<IEnumerable<IPublishedElement>>(Common.NodeProperties.experiencesByReligion))
            {
                Models.ExperienceByReligion chart = new Models.ExperienceByReligion();
                chart.Label = ipe.Value<string>(Common.NodeProperties.label);
                chart.Heavenly = ipe.Value<int>(Common.NodeProperties.heavenly);
                chart.Hellish = ipe.Value<int>(Common.NodeProperties.hellish);
                chart.Purgatorial = ipe.Value<int>(Common.NodeProperties.purgatorial);
                chart.Other = ipe.Value<int>(Common.NodeProperties.other);
                lstExperiences.Add(chart);
            }

            //Extract data
            foreach (Models.ExperienceByReligion experience in lstExperiences)
            {
                //Add label to list
                lstLabels.Add(experience.Label);

                //get total
                totalExperiences = experience.Heavenly + experience.Hellish + experience.Purgatorial + experience.Other;

                //get total percentages
                if (totalExperiences > 0)
                {
                    heavenly = Convert.ToInt32(Math.Round(((decimal)experience.Heavenly / totalExperiences) * 100, 0));
                    hellish = Convert.ToInt32(Math.Round(((decimal)experience.Hellish / totalExperiences) * 100, 0));
                    purgatorial = Convert.ToInt32(Math.Round(((decimal)experience.Purgatorial / totalExperiences) * 100, 0));
                    unknown = Convert.ToInt32(Math.Round(((decimal)experience.Other / totalExperiences) * 100, 0));
                }

                totalPercentage = heavenly + hellish + purgatorial + unknown;

                //Add percentages to list
                lstValues_Heavenly.Add(heavenly);
                lstValues_Hellish.Add(hellish);
                lstValues_Purgatorial.Add(purgatorial);
                lstValues_Unknown.Add(unknown + (100 - totalPercentage)); //Add remainder to ensure total = 100%
            }

            //Convert extracted data to json
            Models.StackedBarChart stats = new Models.StackedBarChart();
            stats.Labels = Newtonsoft.Json.JsonConvert.SerializeObject(lstLabels);
            stats.Values_Heavenly = Newtonsoft.Json.JsonConvert.SerializeObject(lstValues_Heavenly);
            stats.Values_Hellish = Newtonsoft.Json.JsonConvert.SerializeObject(lstValues_Hellish);
            stats.Values_Purgatorial = Newtonsoft.Json.JsonConvert.SerializeObject(lstValues_Purgatorial);
            stats.Values_Unknown = Newtonsoft.Json.JsonConvert.SerializeObject(lstValues_Unknown);

            //sets the canvas height to increase bar heights
            stats.Height = lstLabels.Count * 20;

            //Return stats
            return stats;
        }


        public static string UpdateAllStatistics(UmbracoHelper umbracoHelper, IMemberService memberService, IContentService contentService)
        {
            //Instantiate variables
            Models.IlluminationStats illuminationStats = new Models.IlluminationStats();

            IPublishedContent ipIlluminationStories = umbracoHelper.Content((int)Common.siteNode.IlluminationStories);

            //Loop through all stories
            foreach (IPublishedContent ip in ipIlluminationStories.Children.ToList())
            {
                if (ip.HasValue(Common.NodeProperties.member))
                {
                    //Instantiate variables
                    IPublishedContent ipMember = ip.Value<IPublishedContent>(Common.NodeProperties.member);

                    //Obtain user's experience type
                    illuminationStats.experienceType = ip.Value<string>(Common.NodeProperties.experienceType);

                    //Obtain data for experience types
                    ObtainUpdateStats_forExperienceType(ref illuminationStats);

                    //Consolidate Info: by Country
                    ObtainUpdateStats_forCountry(ref ipMember, ref illuminationStats);

                    //Consolidate Info: by Age
                    ObtainUpdateStats_forAge(ref ipMember, ref illuminationStats);

                    //Consolidate Info: by Gender
                    ObtainUpdateStats_forGender(ref ipMember, ref illuminationStats);

                    //Consolidate Info: by Religion
                    ObtainUpdateStats_forReligion(ref ipMember, ref illuminationStats);

                    //Consolidate Info: by Race
                    ObtainUpdateStats_forRace(ref ipMember, ref illuminationStats);
                }

            }

            //Sort lists
            illuminationStats.LstExperiences_byCountry = illuminationStats.LstExperiences_byCountry.OrderBy(x => x.Label).ToList();

            //Update nodes with new data
            UpdateStatData_byNode(ref illuminationStats, contentService);


            //Generate a pdf version of the stories
            GeneratePdf(umbracoHelper, memberService);

            return JsonConvert.SerializeObject(illuminationStats);
        }
        private static void ObtainUpdateStats_forExperienceType(ref Models.IlluminationStats illuminationStats)
        {
            //Consolidate Info: by Experience Type
            switch (illuminationStats.experienceType)
            {
                case Common.ExperienceType.Heavenly:
                    illuminationStats.experienceType_Heavenly++;
                    break;
                case Common.ExperienceType.Hellish:
                    illuminationStats.experienceType_Hellish++;
                    break;
                case Common.ExperienceType.Purgatorial:
                    illuminationStats.experienceType_Purgatorial++;
                    break;
                case Common.ExperienceType.Other:
                    illuminationStats.experienceType_Other++;
                    break;
                default:
                    break;
            }
        }
        private static void ObtainUpdateStats_forCountry(ref IPublishedContent ipMember, ref Models.IlluminationStats illuminationStats)
        {
            //
            illuminationStats.hasCountry = ipMember.HasValue(Common.NodeProperties.country); ;

            //
            if (illuminationStats.hasCountry)
            {
                Models.ExperienceByCountry experienceByCountry = new ExperienceByCountry();
                string _country = ipMember.Value<string>(Common.NodeProperties.country);

                if (illuminationStats.LstExperiences_byCountry.Any(x => x.Label == _country))
                {
                    experienceByCountry = illuminationStats.LstExperiences_byCountry.Where(x => x.Label == _country).FirstOrDefault();
                }
                else
                {
                    illuminationStats.LstExperiences_byCountry.Add(experienceByCountry);
                    experienceByCountry.Label = _country;
                }


                //Consolidate Info: by Experience Type
                switch (illuminationStats.experienceType)
                {
                    case Common.ExperienceType.Heavenly:
                        experienceByCountry.Heavenly++;
                        break;
                    case Common.ExperienceType.Hellish:
                        experienceByCountry.Hellish++;
                        break;
                    case Common.ExperienceType.Purgatorial:
                        experienceByCountry.Purgatorial++;
                        break;
                    case Common.ExperienceType.Other:
                        experienceByCountry.Other++;
                        break;
                    default:
                        break;
                }
            }
        }
        private static void ObtainUpdateStats_forAge(ref IPublishedContent ipMember, ref Models.IlluminationStats illuminationStats)
        {
            illuminationStats.hasAge = ipMember.HasValue(Common.NodeProperties.age);
            if (illuminationStats.hasAge)
            {
                //
                Models.LineChart experienceByAge = new LineChart();
                illuminationStats.age = ipMember.Value<int>(Common.NodeProperties.age);
                string _ageRange = "";

                //
                if (illuminationStats.age >= 0 && illuminationStats.age <= 4) { _ageRange = "0"; }
                else if (illuminationStats.age >= 5 && illuminationStats.age <= 9) { _ageRange = "5"; }
                else if (illuminationStats.age >= 10 && illuminationStats.age <= 14) { _ageRange = "10"; }
                else if (illuminationStats.age >= 15 && illuminationStats.age <= 19) { _ageRange = "15"; }
                else if (illuminationStats.age >= 20 && illuminationStats.age <= 24) { _ageRange = "20"; }
                else if (illuminationStats.age >= 25 && illuminationStats.age <= 29) { _ageRange = "25"; }
                else if (illuminationStats.age >= 30 && illuminationStats.age <= 34) { _ageRange = "30"; }
                else if (illuminationStats.age >= 35 && illuminationStats.age <= 39) { _ageRange = "35"; }
                else if (illuminationStats.age >= 40 && illuminationStats.age <= 44) { _ageRange = "40"; }
                else if (illuminationStats.age >= 45 && illuminationStats.age <= 49) { _ageRange = "45"; }
                else if (illuminationStats.age >= 50 && illuminationStats.age <= 54) { _ageRange = "50"; }
                else if (illuminationStats.age >= 55 && illuminationStats.age <= 59) { _ageRange = "55"; }
                else if (illuminationStats.age >= 60 && illuminationStats.age <= 64) { _ageRange = "60"; }
                else if (illuminationStats.age >= 65 && illuminationStats.age <= 69) { _ageRange = "65"; }
                else if (illuminationStats.age >= 70 && illuminationStats.age <= 74) { _ageRange = "70"; }
                else if (illuminationStats.age >= 75 && illuminationStats.age <= 79) { _ageRange = "75"; }
                else if (illuminationStats.age >= 80 && illuminationStats.age <= 84) { _ageRange = "80"; }
                else if (illuminationStats.age >= 85 && illuminationStats.age <= 89) { _ageRange = "85"; }
                else if (illuminationStats.age >= 90 && illuminationStats.age <= 94) { _ageRange = "90"; }
                else if (illuminationStats.age >= 95 && illuminationStats.age <= 99) { _ageRange = "95"; }
                else if (illuminationStats.age >= 100) { _ageRange = "100+"; }

                //Consolidate Info: by Experience Type
                switch (illuminationStats.experienceType)
                {
                    case Common.ExperienceType.Heavenly:
                        //
                        experienceByAge = illuminationStats.lstAge_Heavenly.Where(x => x.AgeRange == _ageRange).FirstOrDefault();
                        experienceByAge.Count++;
                        break;
                    case Common.ExperienceType.Hellish:
                        //
                        experienceByAge = illuminationStats.lstAge_Hellish.Where(x => x.AgeRange == _ageRange).FirstOrDefault();
                        experienceByAge.Count++;
                        break;
                    case Common.ExperienceType.Purgatorial:
                        //
                        experienceByAge = illuminationStats.lstAge_Purgatorial.Where(x => x.AgeRange == _ageRange).FirstOrDefault();
                        experienceByAge.Count++;
                        break;
                    case Common.ExperienceType.Other:
                        //
                        experienceByAge = illuminationStats.lstAge_Unknown.Where(x => x.AgeRange == _ageRange).FirstOrDefault();
                        experienceByAge.Count++;
                        break;
                    default:
                        break;
                }
            }
        }
        private static void ObtainUpdateStats_forGender(ref IPublishedContent ipMember, ref Models.IlluminationStats illuminationStats)
        {
            //
            if (illuminationStats.hasAge)
            {
                illuminationStats.hasGender = ipMember.HasValue(Common.NodeProperties.gender);
                if (illuminationStats.hasGender)
                {
                    //
                    ExperienceByGender experienceByGender = new ExperienceByGender();
                    string gender = Common.GetPreValueString(ipMember.Value<string>(Common.NodeProperties.gender));
                    string _ageRange = "";

                    //
                    if (illuminationStats.age >= 0 && illuminationStats.age <= 4) { _ageRange = "0-5"; }
                    else if (illuminationStats.age >= 5 && illuminationStats.age <= 9) { _ageRange = "6-10"; }
                    else if (illuminationStats.age >= 10 && illuminationStats.age <= 14) { _ageRange = "11-15"; }
                    else if (illuminationStats.age >= 15 && illuminationStats.age <= 19) { _ageRange = "16-20"; }
                    else if (illuminationStats.age >= 20 && illuminationStats.age <= 24) { _ageRange = "21-25"; }
                    else if (illuminationStats.age >= 25 && illuminationStats.age <= 29) { _ageRange = "26-30"; }
                    else if (illuminationStats.age >= 30 && illuminationStats.age <= 34) { _ageRange = "31-35"; }
                    else if (illuminationStats.age >= 35 && illuminationStats.age <= 39) { _ageRange = "36-40"; }
                    else if (illuminationStats.age >= 40 && illuminationStats.age <= 44) { _ageRange = "41-45"; }
                    else if (illuminationStats.age >= 45 && illuminationStats.age <= 49) { _ageRange = "46-50"; }
                    else if (illuminationStats.age >= 50 && illuminationStats.age <= 54) { _ageRange = "51-55"; }
                    else if (illuminationStats.age >= 55 && illuminationStats.age <= 59) { _ageRange = "56-60"; }
                    else if (illuminationStats.age >= 60 && illuminationStats.age <= 64) { _ageRange = "61-65"; }
                    else if (illuminationStats.age >= 65 && illuminationStats.age <= 69) { _ageRange = "66-70"; }
                    else if (illuminationStats.age >= 70 && illuminationStats.age <= 74) { _ageRange = "71-75"; }
                    else if (illuminationStats.age >= 75 && illuminationStats.age <= 79) { _ageRange = "76-80"; }
                    else if (illuminationStats.age >= 80 && illuminationStats.age <= 84) { _ageRange = "81-85"; }
                    else if (illuminationStats.age >= 85 && illuminationStats.age <= 89) { _ageRange = "86-90"; }
                    else if (illuminationStats.age >= 90 && illuminationStats.age <= 94) { _ageRange = "91-95"; }
                    else if (illuminationStats.age >= 95) { _ageRange = "96-100+"; }

                    //Consolidate Info: by Experience Type
                    switch (illuminationStats.experienceType)
                    {
                        case Common.ExperienceType.Heavenly:
                            //
                            experienceByGender = illuminationStats.lstGender_Heavenly.Where(x => x.AgeRange == _ageRange).FirstOrDefault();

                            //Increment gender
                            if (gender == Common.Gender.Male)
                            {
                                experienceByGender.Males++;
                            }
                            else
                            {
                                experienceByGender.Females++;
                            }

                            break;
                        case Common.ExperienceType.Hellish:
                            //
                            experienceByGender = illuminationStats.lstGender_Hellish.Where(x => x.AgeRange == _ageRange).FirstOrDefault();

                            //Increment gender
                            if (gender == Common.Gender.Male)
                            {
                                experienceByGender.Males++;
                            }
                            else
                            {
                                experienceByGender.Females++;
                            }

                            break;
                        case Common.ExperienceType.Purgatorial:
                            //
                            experienceByGender = illuminationStats.lstGender_Purgatorial.Where(x => x.AgeRange == _ageRange).FirstOrDefault();

                            //Increment gender
                            if (gender == Common.Gender.Male)
                            {
                                experienceByGender.Males++;
                            }
                            else
                            {
                                experienceByGender.Females++;
                            }

                            break;
                        case Common.ExperienceType.Other:
                            //
                            experienceByGender = illuminationStats.lstGender_Unknown.Where(x => x.AgeRange == _ageRange).FirstOrDefault();

                            //Increment gender
                            if (gender == Common.Gender.Male)
                            {
                                experienceByGender.Males++;
                            }
                            else
                            {
                                experienceByGender.Females++;
                            }

                            break;
                        default:
                            break;
                    }
                }
            }
        }
        private static void ObtainUpdateStats_forRace(ref IPublishedContent ipMember, ref Models.IlluminationStats illuminationStats)
        {
            //
            illuminationStats.hasRace = ipMember.HasValue(Common.NodeProperties.race); ;

            //
            if (illuminationStats.hasRace)
            {
                //
                Models.ExperienceByRace experienceByRace;
                illuminationStats.lstRace = ipMember.Value<IEnumerable<string>>(Common.NodeProperties.race).ToList();

                //Consolidate Info: by Race
                foreach (var race in illuminationStats.lstRace)
                {
                    //
                    experienceByRace = illuminationStats.lstRaces.Where(x => x.Label == race).FirstOrDefault();

                    switch (illuminationStats.experienceType)
                    {
                        case Common.ExperienceType.Heavenly:
                            experienceByRace.Heavenly++;
                            break;
                        case Common.ExperienceType.Hellish:
                            experienceByRace.Hellish++;
                            break;
                        case Common.ExperienceType.Purgatorial:
                            experienceByRace.Purgatorial++;
                            break;
                        case Common.ExperienceType.Other:
                            experienceByRace.Other++;
                            break;
                        default:
                            break;
                    }
                }

            }
        }
        private static void ObtainUpdateStats_forReligion(ref IPublishedContent ipMember, ref Models.IlluminationStats illuminationStats)
        {
            //
            illuminationStats.hasReligion = ipMember.HasValue(Common.NodeProperties.religion); ;

            //
            if (illuminationStats.hasReligion)
            {
                //
                Models.ExperienceByReligion experienceByReligion = new ExperienceByReligion();
                string _religion = ipMember.Value<string>(Common.NodeProperties.religion);


                experienceByReligion = illuminationStats.lstReligions.Where(x => x.Label == _religion).FirstOrDefault();


                //Consolidate Info: by Experience Type
                switch (illuminationStats.experienceType)
                {
                    case Common.ExperienceType.Heavenly:
                        experienceByReligion.Heavenly++;
                        break;
                    case Common.ExperienceType.Hellish:
                        experienceByReligion.Hellish++;
                        break;
                    case Common.ExperienceType.Purgatorial:
                        experienceByReligion.Purgatorial++;
                        break;
                    case Common.ExperienceType.Other:
                        experienceByReligion.Other++;
                        break;
                    default:
                        break;
                }
            }
        }
        private static void UpdateStatData_byNode(ref Models.IlluminationStats illuminationStats, IContentService contentService)
        {
            //Instantiate variables
            IContent icByAge = contentService.GetById((int)Common.siteNode.ByAge);
            IContent icByCountry = contentService.GetById((int)Common.siteNode.ByCountry);
            IContent icByExperienceType = contentService.GetById((int)Common.siteNode.ByExperienceType);
            IContent icByGender = contentService.GetById((int)Common.siteNode.ByGender);
            IContent icByRace = contentService.GetById((int)Common.siteNode.ByRace);
            IContent icByReligion = contentService.GetById((int)Common.siteNode.ByReligion);


            //Update Age stats
            List<Dictionary<string, string>> lstAge_Heavenly = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> lstAge_Hellish = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> lstAge_Purgatorial = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> lstAge_Unknown = new List<Dictionary<string, string>>();
            foreach (LineChart chart in illuminationStats.lstAge_Heavenly)
            {
                lstAge_Heavenly.Add(new Dictionary<string, string>()
                        {
                            {"key",Guid.NewGuid().ToString()},
                            {"name",chart.AgeRange},
                            {"ncContentTypeAlias",Common.dataType.StatisticsLineChart},
                            {"ageRange",chart.AgeRange},
                            {"count",chart.Count.ToString()},
                        });
            }
            foreach (LineChart chart in illuminationStats.lstAge_Hellish)
            {
                lstAge_Hellish.Add(new Dictionary<string, string>()
                        {
                            {"key",Guid.NewGuid().ToString()},
                            {"name",chart.AgeRange},
                            {"ncContentTypeAlias",Common.dataType.StatisticsLineChart},
                            {"ageRange",chart.AgeRange},
                            {"count",chart.Count.ToString()},
                        });
            }
            foreach (LineChart chart in illuminationStats.lstAge_Purgatorial)
            {
                lstAge_Purgatorial.Add(new Dictionary<string, string>()
                        {
                            {"key",Guid.NewGuid().ToString()},
                            {"name",chart.AgeRange},
                            {"ncContentTypeAlias",Common.dataType.StatisticsLineChart},
                            {"ageRange",chart.AgeRange},
                            {"count",chart.Count.ToString()},
                        });
            }
            foreach (LineChart chart in illuminationStats.lstAge_Unknown)
            {
                lstAge_Unknown.Add(new Dictionary<string, string>()
                        {
                            {"key",Guid.NewGuid().ToString()},
                            {"name",chart.AgeRange},
                            {"ncContentTypeAlias",Common.dataType.StatisticsLineChart},
                            {"ageRange",chart.AgeRange},
                            {"count",chart.Count.ToString()},
                        });
            }
            icByAge.SetValue(Common.NodeProperties.statsHeavenly, JsonConvert.SerializeObject(lstAge_Heavenly));
            icByAge.SetValue(Common.NodeProperties.statsHellish, JsonConvert.SerializeObject(lstAge_Hellish));
            icByAge.SetValue(Common.NodeProperties.statsPurgatorial, JsonConvert.SerializeObject(lstAge_Purgatorial));
            icByAge.SetValue(Common.NodeProperties.statsUnknown, JsonConvert.SerializeObject(lstAge_Unknown));
            var result = contentService.SaveAndPublish(icByAge);


            //Update Country stats
            List<Dictionary<string, string>> LstExperiences_byCountry = new List<Dictionary<string, string>>();
            foreach (ExperienceByCountry chart in illuminationStats.LstExperiences_byCountry)
            {
                LstExperiences_byCountry.Add(new Dictionary<string, string>()
                        {
                            {"key",Guid.NewGuid().ToString()},
                            {"name",chart.Label},
                            {"ncContentTypeAlias",Common.dataType.StatisticsStackedBarChart},
                            {"label",chart.Label},
                            {"heavenly",chart.Heavenly.ToString()},
                            {"hellish",chart.Hellish.ToString()},
                            {"purgatorial",chart.Purgatorial.ToString()},
                            {"other",chart.Other.ToString()},
                        });
            }
            icByCountry.SetValue(Common.NodeProperties.experiencesByCountry, JsonConvert.SerializeObject(LstExperiences_byCountry));
            result = contentService.SaveAndPublish(icByCountry);


            //Update Experience Type
            icByExperienceType.SetValue(Common.NodeProperties.heavenly, illuminationStats.experienceType_Heavenly);
            icByExperienceType.SetValue(Common.NodeProperties.hellish, illuminationStats.experienceType_Hellish);
            icByExperienceType.SetValue(Common.NodeProperties.purgatorial, illuminationStats.experienceType_Purgatorial);
            icByExperienceType.SetValue(Common.NodeProperties.other, illuminationStats.experienceType_Other);
            result = contentService.SaveAndPublish(icByExperienceType);


            //Update Gender
            List<Dictionary<string, string>> lstGender_Heavenly = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> lstGender_Hellish = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> lstGender_Purgatorial = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> lstGender_Unknown = new List<Dictionary<string, string>>();
            foreach (ExperienceByGender chart in illuminationStats.lstGender_Heavenly)
            {
                lstGender_Heavenly.Add(new Dictionary<string, string>()
                        {
                            {"key",Guid.NewGuid().ToString()},
                            {"name",chart.AgeRange},
                            {"ncContentTypeAlias",Common.dataType.StatisticsBarChart},
                            {"ageRange",chart.AgeRange},
                            {"males",chart.Males.ToString()},
                            {"females",chart.Females.ToString()},
                        });
            }
            foreach (ExperienceByGender chart in illuminationStats.lstGender_Hellish)
            {
                lstGender_Hellish.Add(new Dictionary<string, string>()
                        {
                            {"key",Guid.NewGuid().ToString()},
                            {"name",chart.AgeRange},
                            {"ncContentTypeAlias",Common.dataType.StatisticsBarChart},
                            {"ageRange",chart.AgeRange},
                            {"males",chart.Males.ToString()},
                            {"females",chart.Females.ToString()},
                        });
            }
            foreach (ExperienceByGender chart in illuminationStats.lstGender_Purgatorial)
            {
                lstGender_Purgatorial.Add(new Dictionary<string, string>()
                        {
                            {"key",Guid.NewGuid().ToString()},
                            {"name",chart.AgeRange},
                            {"ncContentTypeAlias",Common.dataType.StatisticsBarChart},
                            {"ageRange",chart.AgeRange},
                            {"males",chart.Males.ToString()},
                            {"females",chart.Females.ToString()},
                        });
            }
            foreach (ExperienceByGender chart in illuminationStats.lstGender_Unknown)
            {
                lstGender_Unknown.Add(new Dictionary<string, string>()
                        {
                            {"key",Guid.NewGuid().ToString()},
                            {"name",chart.AgeRange},
                            {"ncContentTypeAlias",Common.dataType.StatisticsBarChart},
                            {"ageRange",chart.AgeRange},
                            {"males",chart.Males.ToString()},
                            {"females",chart.Females.ToString()},
                        });
            }
            icByGender.SetValue(Common.NodeProperties.statsHeavenly, JsonConvert.SerializeObject(lstGender_Heavenly));
            icByGender.SetValue(Common.NodeProperties.statsHellish, JsonConvert.SerializeObject(lstGender_Hellish));
            icByGender.SetValue(Common.NodeProperties.statsPurgatorial, JsonConvert.SerializeObject(lstGender_Purgatorial));
            icByGender.SetValue(Common.NodeProperties.statsUnknown, JsonConvert.SerializeObject(lstGender_Unknown));
            result = contentService.SaveAndPublish(icByGender);


            //Update Race stats
            List<Dictionary<string, string>> lstRaces = new List<Dictionary<string, string>>();
            foreach (ExperienceByRace chart in illuminationStats.lstRaces)
            {
                lstRaces.Add(new Dictionary<string, string>()
                        {
                            {"key",Guid.NewGuid().ToString()},
                            {"name",chart.Label},
                            {"ncContentTypeAlias",Common.dataType.StatisticsStackedBarChart},
                            {"label",chart.Label},
                            {"heavenly",chart.Heavenly.ToString()},
                            {"hellish",chart.Hellish.ToString()},
                            {"purgatorial",chart.Purgatorial.ToString()},
                            {"other",chart.Other.ToString()},
                        });
            }
            icByRace.SetValue(Common.NodeProperties.experiencesByRace, JsonConvert.SerializeObject(lstRaces));
            result = contentService.SaveAndPublish(icByRace);


            //Update Religion stats
            List<Dictionary<string, string>> lstReligions = new List<Dictionary<string, string>>();
            foreach (ExperienceByReligion chart in illuminationStats.lstReligions)
            {
                lstReligions.Add(new Dictionary<string, string>()
                        {
                            {"key",Guid.NewGuid().ToString()},
                            {"name",chart.Label},
                            {"ncContentTypeAlias",Common.dataType.StatisticsStackedBarChart},
                            {"label",chart.Label},
                            {"heavenly",chart.Heavenly.ToString()},
                            {"hellish",chart.Hellish.ToString()},
                            {"purgatorial",chart.Purgatorial.ToString()},
                            {"other",chart.Other.ToString()},
                        });
            }
            icByReligion.SetValue(Common.NodeProperties.experiencesByReligion, JsonConvert.SerializeObject(lstReligions));
            result = contentService.SaveAndPublish(icByReligion);
        }


        public static void GeneratePdf(UmbracoHelper umbracoHelper, IMemberService memberService)
        {
            //Obtain the file name and location from Umbraco
            IPublishedContent ipIlluminationStories = umbracoHelper.Content((int)Common.siteNode.IlluminationStories);
            string fileLocation = HttpRuntime.AppDomainAppPath + ipIlluminationStories.Value<IPublishedContent>(Common.NodeProperties.compiledStories).Url().TrimStart('/');
            fileLocation = fileLocation.Replace(@"/", @"\");

            //Create the document
            Document document = CreateDocument(umbracoHelper, memberService);

            //Convert to pdf and save
            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
            renderer.Document = document;
            renderer.RenderDocument();
            renderer.PdfDocument.Save(fileLocation);
        }
        private static Document CreateDocument(UmbracoHelper umbracoHelper, IMemberService memberService)
        {
            // Create a new MigraDoc document
            Document document = new Document();
            document.Info.Title = "The Illumination of Conscience";
            document.Info.Subject = "Testimonials of the Warning";
            document.Info.Author = @"Jim Fifth | AfterTheWarning.com";

            DefineStyles(document);
            DefineCover(document);
            DefineContentSection(document);
            AddContent(document, umbracoHelper, memberService);

            return document;
        }
        private static void DefineStyles(Document document)
        {
            // Get the predefined style Normal.
            Style style = document.Styles["Normal"];
            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "Times New Roman";
            style.ParagraphFormat.SpaceAfter = 3;


            style = document.Styles["Heading1"];
            style.Font.Name = "Tahoma";
            style.Font.Size = 24;
            style.Font.Bold = true;
            style.Font.Color = Colors.Black;
            style.ParagraphFormat.PageBreakBefore = false;
            style.ParagraphFormat.SpaceAfter = 6;


            style = document.Styles["Heading2"];
            style.Font.Size = 18;
            style.Font.Bold = false;
            style.Font.Color = Colors.DarkOrange;
            style.ParagraphFormat.PageBreakBefore = false;
            style.ParagraphFormat.SpaceBefore = 6;
            style.ParagraphFormat.SpaceAfter = 6;


            style = document.Styles["Heading3"];
            style.Font.Size = 10;
            style.Font.Bold = true;
            style.Font.Italic = true;
            style.ParagraphFormat.SpaceBefore = 6;
            style.ParagraphFormat.SpaceAfter = 3;


            style = document.Styles["Heading4"];
            style.Font.Size = 9;
            style.Font.Bold = false;
            style.Font.Italic = false;
            style.Font.Color = Colors.Gray;
            style.ParagraphFormat.SpaceBefore = 6;
            style.ParagraphFormat.SpaceAfter = 6;
            style.ParagraphFormat.Alignment = ParagraphAlignment.Left;


            style = document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);


            style = document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called TOC based on style Normal
            style = document.Styles.AddStyle("TOC", "Normal");
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right, TabLeader.Dots);
            style.ParagraphFormat.Font.Color = Colors.Blue;
        }
        private static void DefineCover(Document document)
        {
            MigraDoc.DocumentObjectModel.Section section = document.AddSection();
            section.PageSetup.PageFormat = PageFormat.Letter;

            MigraDoc.DocumentObjectModel.Shapes.Image image = section.AddImage(HttpRuntime.AppDomainAppPath + @"images\PdfCover.jpg");
            image.Width = "8.5in";
            image.Height = "11in";
            image.RelativeHorizontal = RelativeHorizontal.Page;
            image.RelativeVertical = RelativeVertical.Page;

        }
        private static void DefineContentSection(Document document)
        {
            MigraDoc.DocumentObjectModel.Section section = document.AddSection();
            section.PageSetup.StartingNumber = 1;

            // Add paragraph to footer
            Paragraph paragraph = new Paragraph();
            section.Footers.Primary.Add(paragraph);
            section.Footers.EvenPage.Add(paragraph.Clone());

            paragraph.Format.Alignment = ParagraphAlignment.Right;
            paragraph.AddFormattedText(@"AfterTheWarning.com  |  ", TextFormat.Bold | TextFormat.Italic);
            paragraph.AddPageField();
        }
        private static void AddContent(Document document, UmbracoHelper umbracoHelper, IMemberService memberService)
        {
            //Instantiate variables
            List<IlluminationPdfStat> lstIlluminationPdfStats = ObtainPdfStats(umbracoHelper, memberService);
            string ExperienceType = "";
            Paragraph paragraph;
            Boolean isFirst = true;


            //Add all stories to pdf
            foreach (IlluminationPdfStat stat in lstIlluminationPdfStats)
            {
                //Determine if a new section is to be created
                if (ExperienceType != stat.ExperienceType)
                {
                    //Add page break between sections
                    if (isFirst)
                    {
                        isFirst = false;
                    }
                    else
                    {
                        document.LastSection.AddPageBreak();
                    }

                    //Add experience type as title
                    ExperienceType = stat.ExperienceType;
                    paragraph = document.LastSection.AddParagraph(ExperienceType + " Stories", "Heading1");
                }

                //Add story
                document.LastSection.AddParagraph(stat.Title, "Heading2");
                document.LastSection.AddParagraph("By " + stat.Author, "Heading3");

                //Add statistics
                document.LastSection.AddParagraph("[", "Heading4");
                if (!string.IsNullOrEmpty(stat.ExperienceType))
                {
                    document.LastSection.LastParagraph.AddFormattedText("Experience: ", TextFormat.Bold);
                    document.LastSection.LastParagraph.AddText(stat.ExperienceType + " | ");
                }
                if (stat.Age > 0)
                {
                    document.LastSection.LastParagraph.AddFormattedText("Age: ", TextFormat.Bold);
                    document.LastSection.LastParagraph.AddText(stat.Age.ToString() + " | ");
                }
                if (!string.IsNullOrEmpty(stat.Gender))
                {
                    document.LastSection.LastParagraph.AddFormattedText("Gender: ", TextFormat.Bold);
                    document.LastSection.LastParagraph.AddText(stat.Gender + " | ");
                }
                if (!string.IsNullOrEmpty(stat.Religion))
                {
                    document.LastSection.LastParagraph.AddFormattedText("Religion: ", TextFormat.Bold);
                    document.LastSection.LastParagraph.AddText(stat.Religion + " | ");
                }
                if (stat.Races.Count > 0)
                {
                    document.LastSection.LastParagraph.AddFormattedText("Race: ", TextFormat.Bold);
                    foreach (string race in stat.Races)
                    {
                        document.LastSection.LastParagraph.AddText(race + " ");
                    }
                    document.LastSection.LastParagraph.AddText("| ");
                }
                if (!string.IsNullOrEmpty(stat.Country))
                {
                    document.LastSection.LastParagraph.AddFormattedText("Country: ", TextFormat.Bold);
                    document.LastSection.LastParagraph.AddText(stat.Country + " | ");
                }

                document.LastSection.LastParagraph.AddFormattedText("Id: ", TextFormat.Bold);
                document.LastSection.LastParagraph.AddText(stat.Id.ToString());
                document.LastSection.LastParagraph.AddText("]");


                //Add Story
                document.LastSection.AddParagraph(stat.Story, "Normal");
            }

        }
        private static List<IlluminationPdfStat> ObtainPdfStats(UmbracoHelper umbraco, IMemberService memberService)
        {
            //Instantiate variables
            List<IlluminationPdfStat> lstIlluminationPdfStats = new List<IlluminationPdfStat>();
            IPublishedContent ipIlluminationStories = umbraco.Content((int)Common.siteNode.IlluminationStories);


            //Loop through all stories
            foreach (IPublishedContent ip in ipIlluminationStories.Children.ToList())
            {
                if (ip.HasValue(Common.NodeProperties.member))
                {
                    //Instantiate variables
                    IlluminationPdfStat stat = new IlluminationPdfStat();
                    IPublishedContent ipMember = ip.Value<IPublishedContent>(Common.NodeProperties.member);

                    //Obtain the content models
                    var CmIlluminationStory = new ContentModels.IlluminationStory(ip);
                    var CmMember = new ContentModels.Member(ip.Value<IPublishedContent>(Common.NodeProperties.member));

                    //Obtain the member's name
                    StringBuilder sbAuthor = new StringBuilder();
                    sbAuthor.Append(CmMember.FirstName);
                    sbAuthor.Append("  ");
                    sbAuthor.Append(CmMember.LastName);
                    sbAuthor.Append(".");
                    stat.Author = sbAuthor.ToString();

                    //Obtain the story data
                    stat.Id = CmIlluminationStory.Id;
                    stat.Title = CmIlluminationStory.Title;
                    stat.ExperienceType = CmIlluminationStory.ExperienceType;
                    stat.Story = CmIlluminationStory.Story.Replace("\r\n\r\n", "\r\n").Replace("\r\n\r\n", "\r\n").Replace("\r\n\r\n", "\r\n").Replace("\r\n", "\r\n\r\n");  //set newlines to x2

                    if (CmMember.Age > 0) stat.Age = CmMember.Age;
                    if (!string.IsNullOrEmpty(CmMember.Gender)) stat.Gender = CmMember.Gender;
                    stat.Religion = CmMember.Religion;
                    stat.Races = CmMember.Race.ToList();
                    stat.Country = CmMember.Country;


                    //Add data to list
                    lstIlluminationPdfStats.Add(stat);

                    break;
                }
            }

            //Sort list
            lstIlluminationPdfStats = lstIlluminationPdfStats.OrderBy(x => x.ExperienceType).ThenBy(x => x.Author).ThenBy(x => x.Religion).ThenBy(x => x.Country).ToList();


            return lstIlluminationPdfStats;
        }


        public static Boolean areIlluminationControlsActivated(UmbracoHelper Umbraco)
        {
            //Are Illumination Controls Active
            IPublishedContent ipHome = Umbraco.Content((int)(Common.siteNode.Home));
            return ipHome.Value<Boolean>(Common.NodeProperties.activateIlluminationControls);
        }
        #endregion
    }
}