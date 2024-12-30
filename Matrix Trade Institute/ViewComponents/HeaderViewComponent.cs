using Examine;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.PublishedModels;
using www.Models;
using www.ViewModels;
using cm = Umbraco.Cms.Web.Common.PublishedModels;


namespace www.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private IUmbracoContextAccessor Context;
        private UmbracoHelper UmbracoHelper;
        private IPublishedValueFallback PublishedValueFallback;
        private readonly IExamineManager ExamineManager;
        private readonly ILogger<HeaderViewComponent> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private List<int> lstActiveNodeIDs;


        public HeaderViewComponent(
            ILogger<HeaderViewComponent> _logger,
            IExamineManager _ExamineManager,
                IUmbracoContextAccessor _Context,
                UmbracoHelper _UmbracoHelper,
                IPublishedValueFallback _PublishedValueFallback,
                ServiceContext context,
                IVariationContextAccessor variationContextAccessor)
        {
            ExamineManager = _ExamineManager ?? throw new ArgumentNullException(nameof(_ExamineManager));
            Context = _Context;
            UmbracoHelper = _UmbracoHelper;
            PublishedValueFallback = _PublishedValueFallback;
            _serviceContext = context;
            _variationContextAccessor = variationContextAccessor;
            logger = _logger;
        }



        public async Task<IViewComponentResult> InvokeAsync(IPublishedContent ipModel, bool isMobile = false)
        {
            //Instantiate variables
            HeaderViewModel headerVM = new HeaderViewModel();

            try
            {
                //Obtain content models
                cm.Home cmHome = new cm.Home(ipModel.Root(), PublishedValueFallback);
                IPublishedContent ipSiteSettings = UmbracoHelper.ContentAtRoot().FirstOrDefault(x => x.ContentType.Alias.Equals(www.Models.Common.Doctype.SiteSettings));
                cm.Common cmCommon = new cm.Common(ipSiteSettings!.FirstChildOfType(www.Models.Common.Doctype.Common), new PublishedValueFallback(_serviceContext, _variationContextAccessor));



                //Obtain  data
                headerVM.SiteName = cmCommon.SiteName;
                headerVM.SiteLogoUrl = cmCommon.MainLogoHorizontal?.Url();

                headerVM.PhoneTel = cmCommon.PhoneNo;
                string phn = headerVM.PhoneTel!.Substring(1); //remove the leading '1'
                headerVM.PhoneNo = String.Format("{0:###-###-####}", Int64.Parse(phn));


                //Obtain extra nav classes for header.cshtml
                headerVM.NavigationExtraClasses = ipModel.Value<string>(www.Models.Common.Property.NavigationExtraClasses);



                if (ipModel.IsDocumentType(www.Models.Common.Doctype.LandingPage))
                {
                    cm.LandingPage cmLandingPg = new cm.LandingPage(ipModel, PublishedValueFallback);

                    headerVM.IsLandingPage = true;
                    headerVM.CustomNavigation = cmLandingPg.CustomNavigation;
                }
                else
                {
                    //Create list of active links by grabbing ID hierarchy
                    lstActiveNodeIDs = new List<int>();
                    foreach (IPublishedContent? ip in ipModel.AncestorsOrSelf())
                    {
                        lstActiveNodeIDs.Add(ip.Id);
                    }


                    //Make recursive call to obtain all navigation links starting with children of home
                    foreach (var ipChild in ipModel.Root().Children.Where(x => x.IsVisible() && x.Value<bool>(www.Models.Common.Property.ShowInMainNavigation)))
                    {
                        headerVM.LstLinks!.Add(GetAllChildLinks(ipChild));
                    }
                }


            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }



            //Return data to partialview
            return View(www.Models.Common.Partial.Header, headerVM);
        }

        private Link GetAllChildLinks(IPublishedContent ipParent)
        {
            //Create new link and obtain data
            Link link = new Link();
            link.Title = ipParent.Name;
            if (ipParent.HasValue(www.Models.Common.Property.NavigationTitleOverride))
                link.Title = ipParent.Value<string>(www.Models.Common.Property.NavigationTitleOverride);
            link.Level = ipParent.Level;


            //If redirect pg
            if (ipParent.ContentType.Alias == www.Models.Common.Doctype.RedirectPage)
            {
                var cmPg = new RedirectPage(ipParent, PublishedValueFallback);

                //Obtain redirect link
                string _redirectTo = "";
                string _target = "";
                switch (cmPg?.RedirectOption)
                {
                    case www.Models.Common.RedirectTo.RedirectToUrl:
                        _redirectTo = cmPg?.RedirectPageToUrl?.Url ?? "";
                        _target = cmPg?.RedirectPageToUrl?.Target ?? "";
                        break;

                    case www.Models.Common.RedirectTo.ToFirstChild:
                        _redirectTo = cmPg?.Children()?.FirstOrDefault()?.Url() ?? "";
                        break;

                    case www.Models.Common.RedirectTo.ToParent:
                        _redirectTo = cmPg?.Parent?.Url() ?? "";
                        break;

                    case www.Models.Common.RedirectTo.ToHome:
                        _redirectTo = cmPg?.Root().Url() ?? "";
                        break;

                    default:
                        break;
                }

                link.Url = _redirectTo ?? "/";
                link.Target = _target;
            }
            else
            {
                link.Url = ipParent.Url();

                if (lstActiveNodeIDs.Contains(ipParent.Id))
                    link.IsActive = true;
            }


            //If children exist, create list of child links
            if (ipParent.Children.Any())
            {
                link.LstChildLinks = new List<Link>();
                foreach (var ipChild in ipParent.Children.Where(x => x.IsVisible() && x.Value<bool>(www.Models.Common.Property.ShowInMainNavigation)))
                {
                    link.LstChildLinks.Add(GetAllChildLinks(ipChild));
                }
            }



            //Return link
            return link;
        }

    }
}
