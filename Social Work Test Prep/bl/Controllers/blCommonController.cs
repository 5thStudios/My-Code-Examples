using System.Collections.Generic;
using Umbraco.Web.Mvc;
using System.Text;
using Umbraco.Core;
using Umbraco.Core.Services;
using Umbraco.Core.Models;
using System.Text.RegularExpressions;
using System;
using System.Web.Mvc;
using bl.Models;
using System.Web;
using System.Linq;
using Umbraco.Core.Models.PublishedContent;
using cm = Umbraco.Web.PublishedModels;
using Umbraco.Web;
using Umbraco.ModelsBuilder;
using Umbraco.Web.PublishedModels;
using Umbraco.Core.Logging;
using static Lucene.Net.Index.SegmentReader;
using SEOChecker.Core.Extensions.UmbracoExtensions;
using bl.EF;
using Repositories;

namespace bl.Controllers
{
    public class blCommonController : SurfaceController
    {

        #region "Renders"
        public ActionResult RenderHeader(IPublishedContent ipModel)
        {
            if (bl.Models.Common.ExamDocTypes.Contains(ipModel.ContentType.Alias))
            {
                //Bypass section
                return null;
            }
            else
            {
                //Instantiate variables
                Header header = new Header();
                IPublishedContent ipSiteSettings = Umbraco.ContentAtRoot().FirstOrDefault(x => x.ContentType.Alias.Equals(bl.Models.Common.DocType.SiteSettings));
                cm.Common cmCommon = new cm.Common(ipSiteSettings.Children.FirstOrDefault(x => x.ContentType.Alias.Equals(bl.Models.Common.DocType.Common)));


                try
                {
                    //Verify if user is logged in
                    header.IsLoggedIn = User.Identity.IsAuthenticated;

                    //Obtain header content
                    header.SiteLogo = new Link("SocialWorkTestPrep.com: Get Practiced, Get Licensed", cmCommon.SiteLogo.Url());
                    header.SiteLogo.Width = cmCommon.SiteLogo.MediaItem.GetPropertyValue("umbracoWidth");
                    header.SiteLogo.Height = cmCommon.SiteLogo.MediaItem.GetPropertyValue("umbracoHeight");

                    //Determine if page is the home page and obtain main nav or data depending on page.
                    header.IsHome = (ipModel.ContentType.Alias == Models.Common.DocType.Home);
                    if (header.IsHome)
                    {
                        //Intro content for home page
                        header.Intro = new HtmlString("<p>Prepare for the Social Work Licensing Exam. Take our practice exams. Build confidence and get licensed!</p>");

                        //
                        if (header.IsLoggedIn)
                            header.LnkGetStarted = new Link("", Umbraco.Content((int)(Models.Common.SiteNode.Account)).Url());
                        else
                            header.LnkGetStarted = new Link("", Umbraco.Content((int)(Models.Common.SiteNode.SWTPPricing)).Url());

                        //
                        header.LnkFreePracticeTest = new Link("", Umbraco.Content((int)(Models.Common.SiteNode.FreePracticeTest)).Url());
                    }
                    else
                    {
                        //Get site's root node
                        IPublishedContent ipHome = Umbraco.ContentAtRoot().FirstOrDefault(x => x.ContentType.Alias.Equals(bl.Models.Common.DocType.Home));

                        //Generate list of pages for main navigation
                        header.LstMainNav = new List<Link>();
                        foreach (IPublishedContent ipParent in ipHome.Children)
                        {
                            if (ipParent.Value<bool>(bl.Models.Common.NodeProperties.ShowInMainNavigation) == true)
                            {
                                //Obtain link data
                                Link lnkParent = new Link(ipParent.Name, ipParent.Url());
                                if (ipParent.HasValue(Models.Common.NodeProperties.NavigationTitleOverride))
                                {
                                    lnkParent.Name = ipParent.Value<string>(Models.Common.NodeProperties.NavigationTitleOverride);
                                }
                                if (ipParent.GetDocumentTypeAlias() == Models.Common.DocType.RedirectToPage)
                                {
                                    //Change url to the redirect page
                                    cm.RedirectToPage cmPg = new RedirectToPage(ipParent);
                                    lnkParent.Url = cmPg.RedirectPage.Url();
                                }


                                //
                                if (ipParent.Children.Any())
                                {
                                    //Instantiate new child list
                                    lnkParent.LstChildLinks = new List<Link>();

                                    //Loop through all children
                                    foreach (IPublishedContent ipChild in ipParent.Children)
                                    {
                                        if (ipChild.Value<bool>(bl.Models.Common.NodeProperties.ShowInMainNavigation) == true)
                                        {
                                            //Obtain link data
                                            Link lnkChild = new Link(ipChild.Name, ipChild.Url());
                                            if (ipChild.HasValue(Models.Common.NodeProperties.NavigationTitleOverride))
                                            {
                                                //Change name to override title
                                                lnkChild.Name = ipChild.Value<string>(Models.Common.NodeProperties.NavigationTitleOverride);
                                            }
                                            if (ipChild.GetDocumentTypeAlias() == Models.Common.DocType.RedirectToPage)
                                            {
                                                //Change url to the redirect page
                                                cm.RedirectToPage cmPg = new RedirectToPage(ipChild);
                                                lnkChild.Url = cmPg.RedirectPage.Url();
                                            }

                                            //Add page to list
                                            lnkParent.LstChildLinks.Add(lnkChild);
                                        }
                                    }

                                    //nullify if no children were added
                                    if (!lnkParent.LstChildLinks.Any())
                                    {
                                        lnkParent.LstChildLinks = null;
                                    }
                                }

                                //Add page to list
                                header.LstMainNav.Add(lnkParent);
                            }
                        }
                    }

                    //Obtain minor navigation
                    if (header.IsLoggedIn)
                    {
                        //Generate list
                        int index = 0;
                        foreach (Umbraco.Web.Models.Link lnk in cmCommon.MemberSubnav)
                        {
                            index++;
                            if (index == cmCommon.MemberSubnav.Count())
                                header.LstMinorNav.Add(new Link(lnk.Name, lnk.Url, "btn outline"));
                            else
                                header.LstMinorNav.Add(new Link(lnk.Name, lnk.Url));
                        }

                    }
                    else
                    {
                        //Generate list
                        int index = 0;
                        foreach (Umbraco.Web.Models.Link lnk in cmCommon.GuestSubnav)
                        {
                            index++;
                            if (index == cmCommon.GuestSubnav.Count())
                                header.LstMinorNav.Add(new Link(lnk.Name, lnk.Url, "btn green-light"));
                            else
                                header.LstMinorNav.Add(new Link(lnk.Name, lnk.Url));
                        }
                    }

                }
                catch (Exception ex)
                {
                    ////Save error to log
                    //StringBuilder sb = new StringBuilder();
                    //sb.AppendLine("FilterController | RenderFilter()");
                    //Common.SaveErrorMessage(ex, sb, typeof(CommonController));
                    Logger.Error<blCommonController>(ex);
                }

                return PartialView(Models.Common.PartialPath.Common_Header, header);
            }
        }
        public ActionResult RenderMobileNavigation(IPublishedContent ipModel)
        {
            if (bl.Models.Common.ExamDocTypes.Contains(ipModel.ContentType.Alias))
            {
                //Bypass section
                return null;
            }
            else
            {
                //Instantiate variables
                Header header = new Header();
                IPublishedContent ipSiteSettings = Umbraco.ContentAtRoot().FirstOrDefault(x => x.ContentType.Alias.Equals(bl.Models.Common.DocType.SiteSettings));
                cm.Common cmCommon = new cm.Common(ipSiteSettings.Children.FirstOrDefault(x => x.ContentType.Alias.Equals(bl.Models.Common.DocType.Common)));


                try
                {
                    //Verify if user is logged in
                    header.IsLoggedIn = User.Identity.IsAuthenticated;

                    //Get site's root node
                    IPublishedContent ipHome = Umbraco.ContentAtRoot().FirstOrDefault(x => x.ContentType.Alias.Equals(bl.Models.Common.DocType.Home));

                    //Generate list of pages for main navigation
                    header.LstMainNav = new List<Link>();
                    foreach (IPublishedContent ipParent in ipHome.Children)
                    {
                        if (ipParent.Value<bool>(bl.Models.Common.NodeProperties.ShowInMainNavigation) == true)
                        {
                            //Obtain link data
                            Link lnkParent = new Link(ipParent.Name, ipParent.Url());
                            if (ipParent.HasValue(Models.Common.NodeProperties.NavigationTitleOverride))
                            {
                                lnkParent.Name = ipParent.Value<string>(Models.Common.NodeProperties.NavigationTitleOverride);
                            }

                            //
                            if (ipParent.Children.Any())
                            {
                                //Instantiate new child list
                                lnkParent.LstChildLinks = new List<Link>();

                                //Loop through all children
                                foreach (IPublishedContent ipChild in ipParent.Children)
                                {
                                    if (ipChild.Value<bool>(bl.Models.Common.NodeProperties.ShowInMainNavigation) == true)
                                    {
                                        //Obtain link data
                                        Link lnkChild = new Link(ipChild.Name, ipChild.Url());
                                        if (ipChild.HasValue(Models.Common.NodeProperties.NavigationTitleOverride))
                                        {
                                            lnkChild.Name = ipChild.Value<string>(Models.Common.NodeProperties.NavigationTitleOverride);
                                        }

                                        //Add page to list
                                        lnkParent.LstChildLinks.Add(lnkChild);
                                    }
                                }

                                //nullify if no children were added
                                if (!lnkParent.LstChildLinks.Any())
                                {
                                    lnkParent.LstChildLinks = null;
                                }
                            }

                            //Add page to list
                            header.LstMainNav.Add(lnkParent);
                        }
                    }
                    //}

                    //Obtain minor navigation
                    if (header.IsLoggedIn)
                    {
                        //Generate list
                        cm.AccountManagement cmAcctMngmnt = new AccountManagement(Umbraco.Content((int)bl.Models.Common.SiteNode.Account));
                        foreach (var _link in cmAcctMngmnt.UserLinks)
                        {
                            header.LstMainNav.Add(new Link(_link.Name, _link.Url));
                        }
                    }
                    else
                    {
                        //Generate list
                        foreach (Umbraco.Web.Models.Link lnk in cmCommon.GuestSubnav)
                        {
                            header.LstMinorNav.Add(new Link(lnk.Name, lnk.Url));
                        }
                    }

                }
                catch (Exception ex)
                {
                    Logger.Error<blCommonController>(ex);
                }

                return PartialView(Models.Common.PartialPath.Common_MobileNav, header);
            }
        }
        public ActionResult RenderFooter(IPublishedContent ipModel)
        {
            if (bl.Models.Common.ExamDocTypes.Contains(ipModel.ContentType.Alias))
            {
                //Bypass section
                return null;
            }
            else
            {
                //Instantiate variables
                Models.Footer footer = new Models.Footer();
                IPublishedContent ipSiteSettings = Umbraco.ContentAtRoot().FirstOrDefault(x => x.ContentType.Alias.Equals(Models.Common.DocType.SiteSettings));
                cm.Footer cmFooter = new cm.Footer(ipSiteSettings.Children.FirstOrDefault(x => x.ContentType.Alias.Equals(Models.Common.DocType.Footer)));
                cm.SocialMedia cmSocialMedia = new cm.SocialMedia(ipSiteSettings.Children.FirstOrDefault(x => x.ContentType.Alias.Equals(Models.Common.DocType.SocialMedia)));


                try
                {
                    //Obtain footer content
                    footer.Logo = new Link("SocialWorkTestPrep.com: Get Practiced, Get Licensed", cmFooter.FooterLogo.Url());
                    footer.Description = cmFooter.Content;

                    //Obtain social links and footer nav
                    foreach (cm.ImageLink cmLnk in cmSocialMedia.Links)
                    {
                        Link lnk = new Link(cmLnk.Title, cmLnk.Link.FirstOrDefault().Url);
                        lnk.ImgUrl = cmLnk.Image.Url();
                        footer.LstSocialLinks.Add(lnk);
                    }

                    foreach (Umbraco.Web.Models.Link lnk in cmFooter.FooterNav)
                    {
                        footer.LstFooterNav.Add(new Link(lnk.Name, lnk.Url));
                    }
                }
                catch (Exception ex)
                {
                    ////Save error to log
                    //StringBuilder sb = new StringBuilder();
                    //sb.AppendLine("FilterController | RenderFilter()");
                    //Common.SaveErrorMessage(ex, sb, typeof(CommonController));
                    Logger.Error<blCommonController>(ex);
                }

                return PartialView(Models.Common.PartialPath.Common_Footer, footer);
            }

        }
        public ActionResult RenderSignupPanel()
        {
            //Get signup page
            cm.SignUp cmSignUp = new SignUp(Umbraco.Content((int)bl.Models.Common.SiteNode.SignUp));


            //Display signup page with proper data
            if (Umbraco.MemberIsLoggedOn())
            {
                return PartialView(Models.Common.PartialPath.Common_PanelSignUp, cmSignUp.MemberMessage);
            }
            else
            {
                return PartialView(Models.Common.PartialPath.Common_PanelSignUp, cmSignUp.GuestMessage);
            }
        }
        #endregion


        #region "Static Methods"
        public static Models.TopLevelContent ObtainTopLevelData(IPublishedContent ipModel, string url, UmbracoHelper umbraco)
        {
            //Instantiate variables
            TopLevelContent PgContent = new TopLevelContent();

            try
            {
                //Determine if this page needs to have a querystring added to the url
                //if (!url.Contains("?"))
                //{
                //    if (ipModel.HasProperty(Models.Common.NodeProperties.AddQuerystring) && ipModel.HasValue(Models.Common.NodeProperties.AddQuerystring))
                //    {
                //        //If page does not have querystring, add from umbraco
                //        PgContent.Redirect = true;
                //        PgContent.RedirectTo = ipModel.Url() + "?" + ipModel.Value<string>(Models.Common.NodeProperties.AddQuerystring);
                //    }
                //}

                //Remove comma from url and redirect.
                if (url.Contains(",") || url.Contains("!"))
                {
                    //If page contains a comma, remove and redirect
                    PgContent.Redirect = true;
                    PgContent.RedirectTo = url.Replace(",", "").Replace("!", "");
                }


                if (!PgContent.Redirect)
                {
                    //Obtain model
                    IPublishedContent ipSiteSettings = umbraco.ContentAtRoot().FirstOrDefault(x => x.ContentType.Alias.Equals(bl.Models.Common.DocType.SiteSettings));
                cm.Common cmCommon = new cm.Common(ipSiteSettings.Children.FirstOrDefault(x => x.ContentType.Alias.Equals(bl.Models.Common.DocType.Common)));

                //Show analytics code if not a dev site.
                if (url.Contains("dev.")) { PgContent.ShowAnalytics = false; }
                else if (url.Contains("staging")) { PgContent.ShowAnalytics = false; }
                else if (url.Contains("stage")) { PgContent.ShowAnalytics = false; }
                else if (url.Contains("localhost")) { PgContent.ShowAnalytics = false; }
                else if (url.Contains("azure")) { PgContent.ShowAnalytics = false; }
                else if (url.Contains("5thstudios")) { PgContent.ShowAnalytics = false; }
                else
                {
                    //Obtain meta data from SeoChecker            
                    PgContent.Meta = ipModel.Value<SEOChecker.MVC.MetaData>(bl.Models.Common.NodeProperties.SEOChecker);

                    //Obtain analytics data
                    PgContent.Analytics = cmCommon.Analytics;

                    //Obtain Drift data
                    PgContent.DriftCode = cmCommon.DriftCode;

                    //See if a purchase records needs to be submitted to SEO
                    if (umbraco.MemberIsLoggedOn())
                    {
                        //Instantiate seo push
                        SeoEcommercePush seoEcommercePush = new SeoEcommercePush();

                        //Instantiate repos
                        EF_SwtpDb _context = new EF_SwtpDb();
                        IPurchaseRecordRepository repoPurchaseRecords = new PurchaseRecordRepository(_context);


                        //Get purchase record to add to SEO
                        EF.PurchaseRecord purchaseRecord = repoPurchaseRecords.SubmitPurchaseToSEO(umbraco.MembershipHelper.GetCurrentMemberId());
                        if (purchaseRecord != null)
                        {
                            //Instantiate repos
                            IPurchaseRecordItemRepository repoPurchaseRecordItems = new PurchaseRecordItemRepository(_context);
                            IPurchaseTypeRepository repoPurchaseTypes = new PurchaseTypeRepository(_context);
                            IPurchaseTypeRepository repoPurchaseType = new PurchaseTypeRepository(_context);
                            ICouponRepository repoCoupons = new CouponRepository(_context);

                            //Get user properties
                            seoEcommercePush.user_properties.member_id = umbraco.MembershipHelper.GetCurrentMemberId();
                            seoEcommercePush.user_properties.email = umbraco.MembershipHelper.GetCurrentMember().Name;

                            //Get purchase record if not submitted already.
                            seoEcommercePush.ecommerce.transaction_id = purchaseRecord.PurchaseRecordId; //Receipt or any transaction ID
                            seoEcommercePush.ecommerce.value = purchaseRecord.TotalCost; // Transaction value
                            if (purchaseRecord.CouponId != null)
                            {
                                seoEcommercePush.ecommerce.coupon = repoCoupons.ObtainCode_byId((int)purchaseRecord.CouponId); // "Coupon", if no coupon is applied
                            }

                            //Get each item in purchase and add to list
                            decimal _total = 0;
                            List<PurchaseRecordItem> LstPurchaseRecordItems = repoPurchaseRecordItems.ObtainRecords_byPurchaseRecordId(purchaseRecord.PurchaseRecordId);
                            for (int i = 0; i < LstPurchaseRecordItems.Count; i++)
                            {
                                SeoEcommerceItem seoEcommerceItem = new SeoEcommerceItem();

                                //Add item to seo
                                seoEcommerceItem.item_name = LstPurchaseRecordItems[i].ExamTitle;// Name of the product
                                seoEcommerceItem.item_id = LstPurchaseRecordItems[i].ExamId; // Id of the product, if you don't have an id for each product you can send the product's name
                                seoEcommerceItem.item_category = repoPurchaseType.ObtainType_byId(purchaseRecord.PurchaseTypeId); // Purchase type: Single, Bubdle or Complete

                                if (i == (LstPurchaseRecordItems.Count - 1))
                                {
                                    //last item.  Get remainder of price.
                                    seoEcommerceItem.price = purchaseRecord.TotalCost - _total;
                                }
                                else
                                {
                                    //Round decimal to 2 digits
                                    seoEcommerceItem.price = decimal.Round((purchaseRecord.TotalCost / LstPurchaseRecordItems.Count), 2, MidpointRounding.AwayFromZero); // Price of the product, if you don't have the price you can divide the total amount by the number of products and use that value for all the products
                                    _total += seoEcommerceItem.price;
                                }


                                seoEcommercePush.ecommerce.items.Add(seoEcommerceItem);
                            }

                            //Serialize data for seo
                            StringBuilder sb = new StringBuilder();
                            sb.Append("<script>");
                            sb.Append("window.dataLayer = window.dataLayer || [];");
                            sb.Append("window.dataLayer.push(");
                            sb.Append(Newtonsoft.Json.JsonConvert.SerializeObject(seoEcommercePush));
                            sb.Append(");");
                            sb.Append("</script>");
                            PgContent.SeoEcommerce = sb.ToString();
                            PgContent.SeoEcommerce = PgContent.SeoEcommerce.Replace("_event", "event");


                            //Update record as being submitted to seo
                            purchaseRecord.SubmittedToSEO = true;
                            repoPurchaseRecords.UpdateRecord(purchaseRecord);
                        }
                    }
                }
                }
            }
            catch (Exception ex)
            {
                PgContent.ErrorMsg = ex.ToString();
            }


            return PgContent;
        }
        #endregion
    }
}