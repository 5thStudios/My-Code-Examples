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
using ContentModels = www.Models.PublishedModels;


namespace www.Controllers
{
    public class ViewAllProductsController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<ViewAllProductsController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;

        public ViewAllProductsController(
                ILogger<ViewAllProductsController> _logger,
                ICompositeViewEngine compositeViewEngine,
                IUmbracoContextAccessor umbracoContextAccessor,
                UmbracoHelper _UmbracoHelper,
                IPublishedValueFallback publishedValueFallback,
                ServiceContext context,
                IVariationContextAccessor variationContextAccessor
             )
            : base(_logger, compositeViewEngine, umbracoContextAccessor)
        {
            _publishedValueFallback = publishedValueFallback;
            UmbracoHelper = _UmbracoHelper;
            logger = _logger;
            _serviceContext = context;
            _variationContextAccessor = variationContextAccessor;
        }



        public override IActionResult Index()
        {
            //Instantiate initial variables
            var cmPage = new ViewAllProducts(CurrentPage, _publishedValueFallback);
            ViewAllProductsViewModel vm = null;



            try
            {
                //Create viewmodel
                if (ViewBag.ViewModel != null)
                {
                    vm = ViewBag.ViewModel;
                }
                else
                {
                    //Instantiate new vm
                    vm = new ViewAllProductsViewModel();


                    //Obtain each product list node
                    var ipHomeProductSection = UmbracoHelper.ContentAtRoot()?.FirstOrDefault(x => x.ContentType.Alias == Common.Doctype.Home)?.ChildrenOfType(Common.Doctype.ProductsSection)?.FirstOrDefault();
                    var ipHomeCHEProductSection = UmbracoHelper.ContentAtRoot()?.FirstOrDefault(x => x.ContentType.Alias == Common.Doctype.HomeCHE)?.ChildrenOfType(Common.Doctype.ProductsSection)?.FirstOrDefault();
                    var ipHomeCOLProductSection = UmbracoHelper.ContentAtRoot()?.FirstOrDefault(x => x.ContentType.Alias == Common.Doctype.HomeCOL)?.ChildrenOfType(Common.Doctype.ProductsSection)?.FirstOrDefault();


                    //Create list of sites
                    vm.LstSites.Add(new ViewAllProducts_Sites()
                    {
                        Name = ipHomeProductSection.Value<string>("ProductsWebsiteCode"),
                        ProductsNodeId = ipHomeProductSection.Id,
                        IsActive = false,
                        IpProductList = ipHomeProductSection
                    });
                    vm.LstSites.Add(new ViewAllProducts_Sites()
                    {
                        Name = ipHomeCOLProductSection.Value<string>("ProductsWebsiteCode"),
                        ProductsNodeId = ipHomeCOLProductSection.Id,
                        IsActive = false,
                        IpProductList = ipHomeCOLProductSection
                    });
                    vm.LstSites.Add(new ViewAllProducts_Sites()
                    {
                        Name = ipHomeCHEProductSection.Value<string>("ProductsWebsiteCode"),
                        ProductsNodeId = ipHomeCHEProductSection.Id,
                        IsActive = false,
                        IpProductList = ipHomeCHEProductSection
                    });



                    //Variables
                    vm.ShowImages = true;
                }



                if (vm.LstSites.Any(x => x.IsActive))
                {
                    //Show list of results
                    vm.ShowList = true;
                    vm.LstProducts.Clear();


                    //Obtain product list page
                    IPublishedContent? ipProductList = UmbracoHelper.Content(vm.LstSites.FirstOrDefault(x => x.IsActive)?.ProductsNodeId);
                    if (ipProductList != null)
                    {
                        foreach (var ipProduct in ipProductList.Children().Where(x => x.ContentType.Alias == Common.Doctype.Product))
                        {
                            //Instantiate product model
                            ContentModels.Product cmProduct = new ContentModels.Product(ipProduct, _publishedValueFallback);


                            //Create list of image links
                            List<www.Models.Link> ImgLinks = new List<www.Models.Link>();
                            if (!string.IsNullOrWhiteSpace(cmProduct.PrimaryImageUrl))
                            {
                                //Get primary img url
                                ImgLinks.Add(new www.Models.Link()
                                {
                                    ImgUrl = Common.GetImageSrcUrl(cmProduct.PrimaryImageUrl, 100, 100, "#FFFFFF", UmbracoHelper),  //cmProduct.PrimaryImageUrl + "?v=1&width=100&height=100&quality=50&upscale=false&bgcolor=FFFFFF", 
                                    Summary = "" //img.ImgWidth + " x " + img.ImgHeight
                                });
                            }
                            if (cmProduct.AdditionalImages != null && cmProduct.AdditionalImages.Count() > 0)
                            {
                                //Get secondary img urls
                                foreach (var img in cmProduct.AdditionalImages)
                                {
                                    ImgLinks.Add(new www.Models.Link()
                                    {
                                        ImgUrl = Common.GetImageSrcUrl(img, 100, 100, "#FFFFFF", UmbracoHelper),  //img.FullUrl + "?v=" + img.FileVersion + "&width=100&height=100&quality=50&upscale=false&bgcolor=FFFFFF",
                                        Summary = "" //img.ImgWidth + " x " + img.ImgHeight
                                    });
                                }
                            }


                            //Create list of attributes
                            List<string> LstAttributes = new List<string>();
                            LstAttributes.Add("Preparation : " + Common.SplitCamelCase(cmProduct.AttributePreparation.Replace("Preparation-", "")));
                            LstAttributes.Add("Cooking Status : " + Common.SplitCamelCase(cmProduct.AttributeCookingStatus.Replace("CookingStatus-", "")));
                            LstAttributes.Add("Fresh-Frozen : " + Common.SplitCamelCase(cmProduct.AttributeFreshFrozen.Replace("Fresh-Frozen-", "")));
                            LstAttributes.Add("Proteins : " + Common.SplitCamelCase(cmProduct.AttributeProtein.Replace("Proteins-", "")));
                            LstAttributes.Add("Brands : " + Common.SplitCamelCase(cmProduct.AttributeBrand.Replace("Brand-", "")));
                            foreach (var attrib in cmProduct.Attributes)
                            {
                                LstAttributes.Add("Attributes : " + Common.SplitCamelCase(attrib.Replace("Attributes-", "")));
                            }


                            //Get product type/subtype
                            List<string> LstProductTypes = new List<string>();
                            foreach (var attrib in cmProduct.AttributeProductTypes)
                            {
                                LstProductTypes.Add(Common.SplitCamelCase(attrib.Replace("ProductType-", "")));
                            }
                            string _productType = "";
                            string _productSubtype = "";
                            if (LstProductTypes.Count > 0)
                                _productType = LstProductTypes[0];
                            if (LstProductTypes.Count > 1)
                                _productSubtype = LstProductTypes[1];


                            //Create new product record
                            vm.LstProducts.Add(new www.Models.ProductTools.Product()
                            {
                                EditUrl = "/umbraco#/content/content/edit/" + cmProduct.Id,
                                ViewUrl = cmProduct.Url(mode: UrlMode.Absolute),
                                ViewJsonUrl = "/umbraco/backoffice/Api/PrivateApi/ViewProductData?Data=Json&NodeId=" + cmProduct.Id,

                                ProductName = cmProduct.Title,
                                ProductCode = cmProduct.ProductCode,
                                ProductType = _productType,
                                ProductSubtype = _productSubtype,

                                PrimaryImgLink = new www.Models.Link()
                                {
                                    Url = cmProduct.PrimaryImageUrl,
                                    ImgUrl = Common.GetImageSrcUrl(cmProduct.PrimaryImageUrl, 200, 200, "#FFFFFF", UmbracoHelper) //cmProduct.PrimaryImageUrl + "?v=1&width=200&height=200&quality=50&upscale=false&bgcolor=FFFFFF"
                                },
                                LstImgLinks = ImgLinks,
                                LstAttributes = LstAttributes,

                                Gtin = cmProduct.Gtin,
                                NodeId = cmProduct.Id,

                                Created = cmProduct.CreateDate,
                                Updated = cmProduct.UpdateDate,
                                LastChanged = cmProduct.LastModified
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }


            var viewModel = new ComposedPageViewModel<ViewAllProducts, ViewAllProductsViewModel>
            {
                Page = cmPage,
                ViewModel = vm
            };

            return View(Common.View.ViewAllProducts, viewModel);
        }
    }
}
