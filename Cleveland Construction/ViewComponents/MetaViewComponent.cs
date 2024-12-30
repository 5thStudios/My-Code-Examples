using Examine;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using www.Models;
using www.ViewModels;
using cm = www.Models.PublishedModels;


namespace www.ViewComponents
{
    public class MetaViewComponent : ViewComponent
    {
        private IUmbracoContextAccessor Context;
        private UmbracoHelper UmbracoHelper;
        private IPublishedValueFallback PublishedValueFallback;
        private readonly IExamineManager ExamineManager;
        private readonly ILogger<MetaViewComponent> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;


        public MetaViewComponent(
            ILogger<MetaViewComponent> _logger,
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





        public async Task<IViewComponentResult> InvokeAsync(IPublishedContent ipModel)
        {

            //Instantiate variables
            MetaViewModel metaVM = new MetaViewModel();

            try
            {
                //Obtain seo models
                cm.SEO cmSEO = new cm.SEO(ipModel, PublishedValueFallback);
                cm.ListOfHomePgs cmRootPg = new cm.ListOfHomePgs(ipModel.Root(), PublishedValueFallback);


                //Extract seo data
                metaVM.GoogleAnalytics = cmRootPg.GoogleAnalytics;
                metaVM.SeoChecker = cmSEO.SEochecker;



                //if (ipModel.HasProperty(Common.Property.SeoChecker))
                //{
                //    metaVM.SeoChecker = ipModel.Value<SEOChecker.Library.Models.MetaData>(Common.Property.SeoChecker);
                //}


                //cm.SEO cmSEO = new cm.SEO(ipModel, PublishedValueFallback);
                //cm.SEO cmRootSEO = new cm.SEO(ipModel.Root(), PublishedValueFallback);

                ////URLs
                //metaVM.Canonical = ipModel.Root().Value<string>(Common.Property.SiteCanonicalDomain);
                //metaVM.PgUrl = ipModel.Url(mode: UrlMode.Absolute);


                ////Generate page title
                //if (ipModel == ipModel.Root())
                //{
                //    metaVM.Title = ipModel.Value<string>(Common.Property.SiteTitle);
                //}
                //else
                //{
                //    metaVM.Title = ipModel.Name + " | " + ipModel.Root().Value<string>(Common.Property.SiteTitle);
                //}


                ////Generate meta description
                //if (ipModel.HasProperty(Common.Property.MetaDescription) && ipModel.HasValue(Common.Property.MetaDescription))
                //{
                //    metaVM.Description = ipModel.Value<string>(Common.Property.MetaDescription);
                //}
                //else
                //{
                //    metaVM.Description = ipModel.Root().Value<string>(Common.Property.SiteDescription);
                //}


                ////Obtain keywords
                //if (ipModel.HasProperty(Common.Property.MetaKeywords) && ipModel.HasValue(Common.Property.MetaKeywords))
                //    metaVM.Keywords = ipModel.Value<string>(Common.Property.MetaKeywords) ?? "";





                //Obtain ogImage
                //if (ipModel.HasProperty(Common.Property.OpenGraphImage) && ipModel.HasValue(Common.Property.OpenGraphImage))
                //{
                //    MediaWithCrops ogImage = ipModel.Value<MediaWithCrops>(Common.Property.OpenGraphImage);
                //    if (ogImage == null)
                //    {
                //        if (ipModel.HasProperty(Common.Property.Photo) && ipModel.HasValue(Common.Property.Photo))
                //        {
                //            //Menu Idea photo
                //            ogImage = ipModel.Value<MediaWithCrops>(Common.Property.Photo);
                //        }
                //        else if (ipModel.HasProperty(Common.Property.PostCoverImage) && ipModel.HasValue(Common.Property.PostCoverImage))
                //        {
                //            //Trend photo
                //            ogImage = ipModel.Value<MediaWithCrops>(Common.Property.PostCoverImage);
                //        }
                //    }

                //    if (ogImage != null)
                //    {
                //        metaVM.ImgUrl = ogImage.GetCropUrl(Common.Crop.SocialShare_1200x630, UrlMode.Absolute) + "&cachebuster=" + DateTime.Now.Ticks;
                //        metaVM.ImgType = ogImage.Value<string>(Common.Property.UmbracoExtension);
                //    }
                //}



                ////Obtain ogTitle
                //if (ipModel.HasProperty(Common.Property.OpenGraphTitle) && ipModel.HasValue(Common.Property.OpenGraphTitle))
                //{
                //    metaVM.OgTitle = ipModel.Value<string>(Common.Property.OpenGraphTitle);
                //}
                //else if (ipModel.HasProperty(Common.Property.MetaTitle) && ipModel.HasValue(Common.Property.MetaTitle))
                //{
                //    metaVM.OgTitle = ipModel.Value<string>(Common.Property.MetaTitle);
                //}
                //else { metaVM.OgTitle = ipModel.Name; }



                ////Obtain ogDescription
                //if (ipModel.HasProperty(Common.Property.OpenGraphDescription) && ipModel.HasValue(Common.Property.OpenGraphDescription))
                //{
                //    metaVM.OgDescription = ipModel.Value<string>(Common.Property.OpenGraphDescription);
                //}
                //else if (ipModel.HasProperty(Common.Property.MetaDescription) && ipModel.HasValue(Common.Property.MetaDescription))
                //{
                //    metaVM.OgDescription = ipModel.Value<string>(Common.Property.MetaDescription);
                //}



                ////Obtain ogType
                //metaVM.OgType = "article";
                //if (ipModel.HasProperty(Common.Property.OpenGraphType) && ipModel.HasValue(Common.Property.OpenGraphType))
                //{
                //    metaVM.OgType = ipModel.Value<string>(Common.Property.OpenGraphType);
                //}


                ////Obtain site title
                //metaVM.OgSiteName = ipModel.Root().Value<string>(Common.Property.SiteTitle);


                ////Determine which fav icons to show
                //switch (ipModel.Root().Value<string>(Common.Property.Site))
                //{
                //    case Common.Site.Cheney:
                //        metaVM.Directory = "/images/icons/che";
                //        break;
                //    case Common.Site.Coleman:
                //        metaVM.Directory = "/images/icons/col";
                //        break;
                //    default:
                //        //use Perdue fav icons as default
                //        break;
                //}
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }



            //Return data to partialview
            return View(Common.Partial.MetaAndAnalytics, metaVM);

        }
    }
}
