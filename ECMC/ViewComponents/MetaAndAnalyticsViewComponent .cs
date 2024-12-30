using ECMC_Umbraco.Models;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.PublishedModels;
using ContentModels = Umbraco.Cms.Web.Common.PublishedModels;

namespace www.ViewComponents
{
    public class MetaAndAnalyticsViewComponent : ViewComponent
    {
        private IUmbracoContextAccessor Context;
        private readonly ILogger<MetaAndAnalyticsViewComponent> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;


        public MetaAndAnalyticsViewComponent(
            ILogger<MetaAndAnalyticsViewComponent> _logger,
            ServiceContext serviceContext,
            IUmbracoContextAccessor _Context,
            IVariationContextAccessor variationContextAccessor)
        {
            logger = _logger;
            Context = _Context;
            _serviceContext = serviceContext;
            _variationContextAccessor = variationContextAccessor;
        }





        public async Task<IViewComponentResult> InvokeAsync()
        {
            //Instantiate ViewModel
            MetaAndAnalyticsViewModel vModel = new MetaAndAnalyticsViewModel();


            try
            {
                //Determine if site is live or dev.
                IPublishedContent ipCurrentPg = Context.GetRequiredUmbracoContext().PublishedRequest!.PublishedContent!;
                IPublishedContent ipRootPg = Context.GetRequiredUmbracoContext().PublishedRequest!.PublishedContent!.Root();
                SiteSettings cmSiteSettings = new ContentModels.SiteSettings(ipRootPg.Value<IPublishedContent>(Common.Property.SiteSettings), new PublishedValueFallback(_serviceContext, _variationContextAccessor));



                //Add data to models
                vModel.SiteSettings = cmSiteSettings;
                if (ipCurrentPg.HasValue(Common.Doctype.TitleOverride))
                {
                    vModel.Title = ipCurrentPg.Value<string>(Common.Doctype.TitleOverride) + " | " + cmSiteSettings.SiteName;
                }
                else
                {
                    vModel.Title = ipCurrentPg.Name + " | " + cmSiteSettings.SiteName;
                }

                vModel.Description = ipCurrentPg.Value<string>(Common.Doctype.PageDescription);

                if (ipCurrentPg.Value<bool>(Common.Doctype.HideFromIndexing))
                {
                    vModel.Robots = "noimageindex, noindex, nofollow, nosnippet, noarchive";
                }
                else
                {
                    vModel.Robots = "index, follow";
                }

                if (ipCurrentPg.HasValue(Common.Doctype.KeywordList) && ipCurrentPg?.Value<string[]>(Common.Doctype.KeywordList)?.Length > 0)
                {
                    vModel.Keywords = string.Join(",", ipCurrentPg.Value<string[]>(Common.Doctype.KeywordList));
                }

                vModel.CanonicalUrl = ipRootPg.Url(mode: UrlMode.Absolute).ToLower().Replace("http://", "https://");
                vModel.PageUrl = ipCurrentPg.Url(mode: UrlMode.Absolute).ToLower().Replace("http://", "https://");
                vModel.GoogleSiteVerification = cmSiteSettings.GoogleSiteVerification;

                if (cmSiteSettings.SEoimage != null)
                {
                    vModel.ImageUrl = cmSiteSettings.SEoimage?.GetCropUrl(Common.Crop.SEO_1200x630, UrlMode.Absolute) + "&cachebuster=" + DateTime.Now.Ticks;
                }
                if (ipCurrentPg.HasProperty(Common.Doctype.SeoImageOverride) && ipCurrentPg.HasValue(Common.Doctype.SeoImageOverride))
                {
                    vModel.ImageUrl = ipCurrentPg.Value<MediaWithCrops>(Common.Doctype.SeoImageOverride)?.GetCropUrl(Common.Crop.SEO_1200x630, UrlMode.Absolute) + "&cachebuster=" + DateTime.Now.Ticks;
                }

                if (!string.IsNullOrEmpty(vModel.ImageUrl))
                {
                    if (vModel.ImageUrl.Contains(".gif"))
                    {
                        vModel.ImageType = "image/gif";
                    }
                    else if (vModel.ImageUrl.Contains(".png"))
                    {
                        vModel.ImageType = "image/png";
                    }
                    else if (vModel.ImageUrl.Contains(".jpg") || vModel.ImageUrl.Contains(".jpeg"))
                    {
                        vModel.ImageType = "image/jpeg";
                    }
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }



            //Return data to partialview
            return View(Common.Partial.MetaAndAnalytics, vModel);
        }
    }
}
