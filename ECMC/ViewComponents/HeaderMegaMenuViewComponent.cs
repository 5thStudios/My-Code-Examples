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
    public class HeaderMegaMenuViewComponent : ViewComponent
    {
        private IUmbracoContextAccessor Context;
        private UmbracoHelper UmbracoHelper;
        private IPublishedValueFallback PublishedValueFallback;
        private readonly IExamineManager ExamineManager;
        private readonly ILogger<HeaderMegaMenuViewComponent> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private List<int> lstNodeIDs;


        public HeaderMegaMenuViewComponent(
            ILogger<HeaderMegaMenuViewComponent> _logger,
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
            HeaderMegaMenuViewModel headerVM = new HeaderMegaMenuViewModel();

            try
            {
                //Obtain content models
                IPublishedContent ipHome = ipModel.Root();
                ContentModels.Site cmSite = new ContentModels.Site(ipHome.Value<IPublishedContent>(Common.Property.SiteSettings), new PublishedValueFallback(_serviceContext, _variationContextAccessor));


                //Obtain 
                headerVM.SiteName = cmSite.SiteName;
                headerVM.SiteLogoUrl = cmSite.SiteLogo?.Url();


                //Make recursive call to obtain all navigation links starting with children of home
                foreach (var ipChild in ipHome.Children.Where(x => x.IsVisible()))
                {
                    headerVM.LstLinks!.Add(GetAllChildLinks(ipChild));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }



            //Return data to partialview
            return View(Common.Partial.HeaderMegaMenu, headerVM);
        }



        private Link GetAllChildLinks(IPublishedContent ipParent)
        {
            //Create new link and obtain data
            Link link = new Link();
            link.Title = ipParent.Name;
            link.Subtitle = ipParent.Name.ToLower().Replace(" ", "-");
            if (ipParent.HasProperty("summary") && ipParent.HasValue("summary"))
                link.Summary = ipParent.Value<string>("summary");

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


            link.ImgUrl = @"<i class=""fa-thin fa-circle-info""></i>";
            if (ipParent.HasProperty("icon") && ipParent.HasValue("icon"))
            {
                link.ImgUrl = ipParent.Value<string>("icon");
            }


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

            //Return link
            return link;
        }

    }
}
