using ECMC_Umbraco.Models;
using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco.Extensions;
using ContentModels = Umbraco.Cms.Web.Common.PublishedModels;

namespace www.ViewComponents
{
    public class TooltipsViewComponent : ViewComponent
    {
        private IUmbracoContextAccessor Context;
        private readonly ILogger<TooltipsViewComponent> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;


        public TooltipsViewComponent(
            ILogger<TooltipsViewComponent> _logger,
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
            List<TooltipsViewModel> LstToolTipsVM = new List<TooltipsViewModel>();

            try
            {
                //Obtain root node and tooltips
                IPublishedContent ipRootPg = ipCurrentPg.Root();
                IPublishedContent? ipTooltips = ipRootPg.DescendantsOfType(Common.Doctype.Tooltips)?.FirstOrDefault();

                if (ipTooltips != null)
                {
                    Tooltips cmTooltips = new ContentModels.Tooltips(ipTooltips, new PublishedValueFallback(_serviceContext, _variationContextAccessor));

                    if (cmTooltips.TooltipList != null)
                    {
                        foreach (var tooltip in cmTooltips.TooltipList)
                        {
                            if (tooltip != null)
                            {
                                //Add each tooltip into list
                                LstToolTipsVM.Add(new TooltipsViewModel()
                                {
                                    id = tooltip.Content.Value<string>(Common.Property.TooltipId) ?? "",
                                    title = tooltip.Content.Value<string>(Common.Property.TooltipTitle) ?? "",
                                    text = tooltip.Content.Value<string>(Common.Property.TooltipText) ?? ""
                                });
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }



            //Return data to partialview
            return View(Common.Partial.Tooltips, LstToolTipsVM);
        }
    }
}
