using Dragonfly.NetHelpers;
using Dragonfly.UmbracoServices;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using NPoco.fastJSON;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Controllers;
using www.Models;
using www.Models.PublishedModels;
using www.ViewModels;
using static System.Net.Mime.MediaTypeNames;
using ContentModels = www.Models.PublishedModels;
using Umbraco.Cms.Core.Models;


namespace www.Controllers
{
    public class RecipeController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<RecipeController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly Umbraco.Cms.Core.Hosting.IHostingEnvironment _hostingEnvironment;


        public RecipeController(
                ILogger<RecipeController> _logger,
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
            //Instantiate variables
            Recipe cmPage = new Recipe(CurrentPage, _publishedValueFallback);
            RecipeViewModel vmRecipe = new RecipeViewModel();


            try
            {
                //Obtain related recipe links
                if (cmPage.RelatedProducts != null)
                {
                    foreach (var ipProduct in cmPage.RelatedProducts)
                    {
                        ContentModels.Product cmProduct = new Product(ipProduct, _publishedValueFallback);
                        
                        vmRecipe.LstRelatedProducts.Add(new Models.Link()
                        {
                            Title = cmProduct.Title.Replace("®", "<sup>®</sup>").Replace("™", "<sup>™</sup>"),
                            Summary = cmProduct.ProductCode,
                            Url = cmProduct.Url(),
                            ImgUrl = Common.GetImageSrcUrl(cmProduct.PrimaryImageUrl, 600, 400, "#ff0000", UmbracoHelper)
                        });

                        //                          ImgUrl = cmProduct.PrimaryImageUrl
                        //                          imgUrl = Common.GetImageSrcUrl(cmProduct.PrimaryImageUrl, 250, 180, "#f8f9fa", UmbracoHelper);

                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }


            var viewModel = new ComposedPageViewModel<Recipe, RecipeViewModel>
            {
                Page = cmPage,
                ViewModel = vmRecipe
            };

            return View(Common.View.Recipe, viewModel);
        }

        //private string GetImgUrl(string refId)
        //{

        //    //Obtain json file
        //    var responseFileNameFormat = Common.Path.ApiFoodProductSavePath + "{0}.json";
        //    //string refId = "vLsw1C";
        //    var prodFileName = string.Format(responseFileNameFormat, refId);
        //    FileHelperService fileHelper = new FileHelperService(_hostingEnvironment);

        //    FoodProduct foodProduct = Newtonsoft.Json.JsonConvert.DeserializeObject<FoodProduct>(fileHelper.GetTextFileContents(prodFileName));

        //    //return foodProduct.ProductImages[0].FullUrl;
        //    return ObtainImageSrcUrl((ProductImage)foodProduct.ProductImages.Where(x => x.IsPrimary == true).FirstOrDefault(), 0, 0, "#FFFFFF");

        //}

        //private string ObtainImageSrcUrl(ProductImage ProductImage, int Width, int Height, string BgColorForPadding, IEnumerable<KeyValuePair<string, string>> AdditionalParameters = null, int Quality = 100)
        //{
        //    //string text = ProductImage.FullUrl.Replace("https://", "");

        //    string text2 = BgColorForPadding.Replace("#", "");
        //    string text3 = "";
        //    string text4 = "";
        //    if (Width > 0 && Height > 0)
        //    {
        //        text3 = $"&width={Width}&height={Height}";
        //    }
        //    else if (Width == 0 && Height > 0)
        //    {
        //        text3 = $"&height={Height}";
        //    }
        //    else if (Width > 0 && Height == 0)
        //    {
        //        text3 = $"&width={Width}";
        //    }

        //    text4 = $"&quality={Quality}";
        //    string text5 = "";
        //    if (AdditionalParameters != null)
        //    {
        //        foreach (KeyValuePair<string, string> AdditionalParameter in AdditionalParameters)
        //        {
        //            string text6 = "&" + AdditionalParameter.Key + "=" + AdditionalParameter.Value;
        //            text5 += text6;
        //        }
        //    }

        //    //return $"/remote.axd/{text}";
        //    //return $"/remote.axd/{text}?v={ProductImage.FileVersion}{text3}{text4}&upscale=false&bgcolor={text2}{text5}";

        //    return ProductImage.FullUrl + $"?v={ProductImage.FileVersion}{text3}{text4}&upscale=false&bgcolor={text2}{text5}";
        //}


    }
}
