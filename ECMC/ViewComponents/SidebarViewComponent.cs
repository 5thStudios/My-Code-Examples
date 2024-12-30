using ECMC_Umbraco.Models;
using Microsoft.AspNetCore.Mvc;
using SEOChecker.Library.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco.Extensions;
using ContentModels = Umbraco.Cms.Web.Common.PublishedModels;

namespace www.ViewComponents
{
    public class SidebarViewComponent : ViewComponent
    {
        private IUmbracoContextAccessor Context;
        private readonly ILogger<SidebarViewComponent> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;


        public SidebarViewComponent(
            ILogger<SidebarViewComponent> _logger,
            ServiceContext serviceContext,
            IUmbracoContextAccessor _Context,
            IVariationContextAccessor variationContextAccessor)
        {
            logger = _logger;
            Context = _Context;
            _serviceContext = serviceContext;
            _variationContextAccessor = variationContextAccessor;
        }




        public async Task<IViewComponentResult> InvokeAsync(IPublishedContent ipCurrentPg)
        {
            //Instantiate ViewModel
            List<SidebarViewModel> LstSidebarVM = new List<SidebarViewModel>();

            try
            {
                ////Obtain root node and SidebarViewComponent
                //IPublishedContent ipRootPg = ipCurrentPg.Root();
                //IPublishedContent? ipSidebarViewComponent = ipRootPg.DescendantsOfType(Common.Doctype.SidebarViewComponent)?.FirstOrDefault();

                //if (ipSidebarViewComponent != null)
                //{
                //    SidebarViewComponent cmSidebarViewComponent = new ContentModels.SidebarViewComponent(ipSidebarViewComponent, new PublishedValueFallback(_serviceContext, _variationContextAccessor));

                //    if (cmSidebarViewComponent.TooltipList != null)
                //    {
                //        foreach (var tooltip in cmSidebarViewComponent.TooltipList)
                //        {
                //            if (tooltip != null)
                //            {
                //                //Add each tooltip into list
                //                LstSidebarViewComponentVM.Add(new SidebarViewComponentViewModel()
                //                {
                //                    id = tooltip.Content.Value<string>(Common.Property.TooltipId) ?? "",
                //                    title = tooltip.Content.Value<string>(Common.Property.TooltipTitle) ?? "",
                //                    text = tooltip.Content.Value<string>(Common.Property.TooltipText) ?? ""
                //                });
                //            }
                //        }
                //    }
                //}

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }



            //Return data to partialview
            return View(Common.Partial.Sidebar, LstSidebarVM);
        }
    }
}
