using Dragonfly.UmbracoServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Controllers;
using www.Models;
using www.Models.PublishedModels;
using www.ViewModels;


namespace www.Controllers
{
    public class ViewProductImagesController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<ViewProductImagesController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly Umbraco.Cms.Core.Hosting.IHostingEnvironment _hostingEnvironment;

        public ViewProductImagesController(
                ILogger<ViewProductImagesController> _logger,
                ICompositeViewEngine compositeViewEngine,
                IUmbracoContextAccessor umbracoContextAccessor,
                UmbracoHelper _UmbracoHelper,
                IPublishedValueFallback publishedValueFallback,
                ServiceContext context,
                IVariationContextAccessor variationContextAccessor,
                Umbraco.Cms.Core.Hosting.IHostingEnvironment hostingEnvironment
             )
            : base(_logger, compositeViewEngine, umbracoContextAccessor)
        {
            _publishedValueFallback = publishedValueFallback;
            UmbracoHelper = _UmbracoHelper;
            logger = _logger;
            _serviceContext = context;
            _variationContextAccessor = variationContextAccessor;
            _hostingEnvironment = hostingEnvironment;
        }



        public override IActionResult Index()
        {
            //Instantiate initial variables
            var cmPage = new ViewProductImages(CurrentPage, _publishedValueFallback);
            FileHelperService FileHelper = new FileHelperService(_hostingEnvironment);
            ViewProductImagesViewModel vm = new ViewProductImagesViewModel();
            List<string> LstProductCodes = new List<string>();



            try
            {
                //Obtain each product list node
                List<IPublishedContent> lstSiteProductLists = new List<IPublishedContent>();
                lstSiteProductLists.Add(UmbracoHelper.ContentAtRoot()?.FirstOrDefault(x => x.ContentType.Alias == Common.Doctype.Home)?.ChildrenOfType(Common.Doctype.ProductsSection)?.FirstOrDefault());
                lstSiteProductLists.Add(UmbracoHelper.ContentAtRoot()?.FirstOrDefault(x => x.ContentType.Alias == Common.Doctype.HomeCHE)?.ChildrenOfType(Common.Doctype.ProductsSection)?.FirstOrDefault());
                lstSiteProductLists.Add(UmbracoHelper.ContentAtRoot()?.FirstOrDefault(x => x.ContentType.Alias == Common.Doctype.HomeCOL)?.ChildrenOfType(Common.Doctype.ProductsSection)?.FirstOrDefault());


                //Loop through each site product list
                foreach (var ipProducts in lstSiteProductLists)
                {
                    //Loop through each product from within site
                    foreach (var ipProduct in ipProducts.Children.Where(x => x.ContentType.Alias == Common.Doctype.Product))
                    {

                        //Instantiate product model
                        var cmProduct = new Product(ipProduct, _publishedValueFallback);


                        //Ensure no duplicate products are being added
                        if (!LstProductCodes.Contains(cmProduct.ProductCode))
                        {
                            LstProductCodes.Add(cmProduct.ProductCode);
                            vm.ProductCount++;


                            //Create list of image links
                            List<www.Models.Link> ImgLinks = new List<www.Models.Link>();
                            if (!string.IsNullOrWhiteSpace(cmProduct.PrimaryImageUrl))
                            {
                                //Get primary img url
                                ImgLinks.Add(new www.Models.Link()
                                {
                                    ImgUrl = Common.GetImageSrcUrl(cmProduct.PrimaryImageUrl, 300, 300, "#FFFFFF", UmbracoHelper),  
                                    Summary = "", //img.ImgWidth + " x " + img.ImgHeight
                                    Title = "", //img.FileName,
                                    IsActive = true
                                });
                            }
                            if (cmProduct.AdditionalImages != null && cmProduct.AdditionalImages.Count() > 0)
                            {
                                //Get secondary img urls
                                foreach (var img in cmProduct.AdditionalImages)
                                {
                                    ImgLinks.Add(new www.Models.Link()
                                    {
                                        ImgUrl = Common.GetImageSrcUrl(img, 300, 300, "#FFFFFF", UmbracoHelper),
                                        Summary = "", //img.ImgWidth + " x " + img.ImgHeight
                                        Title = "", //img.FileName,
                                        IsActive = false
                                    });
                                }
                            }


                            //Add each image to list
                            foreach (var img in ImgLinks)
                            {
                                vm.LstImageDetails.Add(new Models.ProductTools.ImageDetail()
                                {
                                    ProductName = cmProduct.Title,
                                    ProductCode = cmProduct.ProductCode,
                                    ImgUrl = img.ImgUrl,
                                    ImgSize = img.Summary,
                                    FileName = img.Title,
                                    IsPrimary = img.IsActive,
                                    LastChanged = DateTime.Now
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


            var viewModel = new ComposedPageViewModel<ViewProductImages, ViewProductImagesViewModel>
            {
                Page = cmPage,
                ViewModel = vm
            };

            return View(Common.View.ViewProductImages, viewModel);
        }
    }
}
