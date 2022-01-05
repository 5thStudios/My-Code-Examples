using System;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using bl.Models;
using Umbraco.Web;
using System.Linq;
using System.Collections.Generic;
using bl.EF;
using bl.Repositories;
using System.Web;
using Umbraco.Core.Models.PublishedContent;
using ContentModels = Umbraco.Web.PublishedModels;


namespace www.Controllers
{
    public class NavigationController : SurfaceController
    {
        #region "Render ActionResults"
        public ActionResult RenderSideToolNavigation()
        {
            //Instantiate variables
            IPublishedContent ipDashboard = Umbraco.Content((int)(Common.SiteNode.Dashboard));
            SideToolNav sideNav = new SideToolNav();

            //Obtain Dashboard Link
            sideNav.DashboardLink.Title = ipDashboard.Name;
            if (ipDashboard.HasProperty(Common.NodeProperty.NavigationTitleOverride) && ipDashboard.HasValue(Common.NodeProperty.NavigationTitleOverride))
                sideNav.DashboardLink.Title = ipDashboard.Value<string>(Common.NodeProperty.NavigationTitleOverride);
            sideNav.DashboardLink.Url = ipDashboard.Url();
            sideNav.DashboardLink.Icon = ipDashboard.Value<string>(Common.NodeProperty.NavigationIcon);

            //Obtain list of all tools
            foreach (var ipChild in ipDashboard.Children)
            {
                if (ipChild.HasProperty(Common.NodeProperty.ShowInSideNav) && ipChild.Value<bool>(Common.NodeProperty.ShowInSideNav) == true)
                {
                    if (ipChild.HasProperty(Common.NodeProperty.IsTool) && ipChild.Value<bool>(Common.NodeProperty.IsTool) == true)
                    {
                        bl.Models.Link link = new Link();
                        link.Title = ipChild.Name;
                        if (ipChild.HasProperty(Common.NodeProperty.NavigationTitleOverride) && ipChild.HasValue(Common.NodeProperty.NavigationTitleOverride))
                            link.Title = ipChild.Value<string>(Common.NodeProperty.NavigationTitleOverride);
                        link.Url = ipChild.Url();
                        link.Icon = ipChild.Value<string>(Common.NodeProperty.NavigationIcon);
                        sideNav.LstToolLinks.Add(link);
                    }
                }
            }

            //Obtain list of all admin tools
            foreach (var ipChild in ipDashboard.Children)
            {
                if (ipChild.HasProperty(Common.NodeProperty.ShowInSideNav) && ipChild.Value<bool>(Common.NodeProperty.ShowInSideNav) == true)
                {
                    if (ipChild.HasProperty(Common.NodeProperty.IsTool) && ipChild.Value<bool>(Common.NodeProperty.IsTool) == false)
                    {
                        bl.Models.Link link = new Link();
                        link.Title = ipChild.Name;
                        if (ipChild.HasProperty(Common.NodeProperty.NavigationTitleOverride) && ipChild.HasValue(Common.NodeProperty.NavigationTitleOverride))
                            link.Title = ipChild.Value<string>(Common.NodeProperty.NavigationTitleOverride);
                        link.Url = ipChild.Url();
                        link.Icon = ipChild.Value<string>(Common.NodeProperty.NavigationIcon);
                        sideNav.LstAdminLinks.Add(link);
                    }
                }
            }

            //Return data with partial view
            return PartialView(Common.PartialPath.Aside, sideNav);
        }
        #endregion




    }
}



//ServiceContext Services { get; }
//ISqlContext SqlContext { get; }
//UmbracoHelper Umbraco { get; }
//UmbracoContext UmbracoContext { get; }
//IGlobalSettings GlobalSettings { get; }
//IProfilingLogger Logger { get; }
//MembershipHelper Members { get; }