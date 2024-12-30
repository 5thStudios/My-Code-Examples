using ECMC_Umbraco.Models;
using Examine;
using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using UmbracoProject.Models;
using ContentModels = Umbraco.Cms.Web.Common.PublishedModels;



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
        private List<int> lstNodeIDs;


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
                IPublishedContent ipHome = ipModel.Root();
                ContentModels.Site cmSite = new ContentModels.Site(ipHome.Value<IPublishedContent>(Common.Property.SiteSettings), new PublishedValueFallback(_serviceContext, _variationContextAccessor));


                //Obtain 
                headerVM.SiteName = cmSite.SiteName;
                headerVM.SiteLogoUrl = cmSite.SiteLogo?.Url();
                headerVM.ToggleSearch = cmSite.ToggleSearch;

                //Create list of active links
                lstNodeIDs = new List<int>();
                foreach (IPublishedContent? ip in ipModel.AncestorsOrSelf())
                {
                    lstNodeIDs.Add(ip.Id);
                }


                if (ipHome.ContentType.Alias == Common.Doctype.HomeAR)
                {
                    //Make recursive call to obtain all navigation links within current year
                    ContentModels.HomeAR cmHome = new ContentModels.HomeAR(ipHome, new PublishedValueFallback(_serviceContext, _variationContextAccessor));
                    foreach (var ipChild in cmHome.CurrentContent.Children.Where(x => x.IsVisible()))
                    {
                        headerVM.LstLinks!.Add(GetAllChildLinks(ipChild));
                    }
                }
                else
                {
                    //Make recursive call to obtain all navigation links starting with children of home
                    foreach (var ipChild in ipModel.Root().Children.Where(x => x.IsVisible()))
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
            if (isMobile)
                return View(Common.Partial.HeaderMbl, headerVM);
            else
                return View(Common.Partial.Header, headerVM);
        }



        private Link GetAllChildLinks(IPublishedContent ipParent)
        {
            //Create new link and obtain data
            Link link = new Link();
            link.Title = ipParent.Name;
            if (ipParent.HasValue(Common.Property.NavigationTitleOverride))
            {
                link.Title = ipParent.Value<string>(Common.Property.NavigationTitleOverride);
            }
            link.Level = ipParent.Level;
            link.Url = ipParent.Url();
            if (ipParent.ContentType.Alias.Contains(Common.Doctype.RedirectTo))
            {
                //If redirect pg, change url to correct url.
                link.Url = ipParent.Value<Umbraco.Cms.Core.Models.Link>(Common.Property.RedirectToPage)?.Url;
            }
            if (lstNodeIDs.Contains(ipParent.Id))
                link.IsActive = true;


            //If children exist, create list of child links
            if (ipParent.Value<bool>(Common.Property.HideChildrenFromNavigation) == false)
            {
                if (ipParent.Children.Any())
                {
                    link.LstChildLinks = new List<Link>();
                    foreach (var ipChild in ipParent.Children.Where(x => x.IsVisible()))
                    {
                        link.LstChildLinks.Add(GetAllChildLinks(ipChild));
                    }
                }
            }


            //if there are additional child navigations added, insert them here.
            if (ipParent.HasValue(Common.Property.ChildNavigations))
            {
                //Obtain list of links
                var links = ipParent.Value<List<Umbraco.Cms.Core.Models.Link>>(Common.Property.ChildNavigations);

                if (links != null)
                {
                    //Instantiate list if null
                    if (link.LstChildLinks is null)
                        link.LstChildLinks = new List<Link>();

                    //Add each link to list.
                    foreach (var _link in links)
                    {
                        link.LstChildLinks?.Add(new Link()
                        {
                            Title = _link.Name,
                            Url = _link.Url,
                            Target = _link.Target
                        });
                    }
                }
            }



            //Return link
            return link;
        }


    }
}
