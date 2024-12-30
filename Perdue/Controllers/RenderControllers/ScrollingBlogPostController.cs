using Dragonfly.NetHelpers;
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
using static System.Net.Mime.MediaTypeNames;
using ContentModels = www.Models.PublishedModels;


namespace www.Controllers
{
    public class ScrollingBlogPostController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<ScrollingBlogPostController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly Umbraco.Cms.Core.Hosting.IHostingEnvironment _hostingEnvironment;

        public ScrollingBlogPostController(
                ILogger<ScrollingBlogPostController> _logger,
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
            //
            var cmPage = new ScrollingBlogPost(CurrentPage, _publishedValueFallback);
            ScrollingBlogPostViewModel vmBlogPost = new ScrollingBlogPostViewModel();


            try
            {
                //Obtain top 5 recent articles
                foreach (var ipPost in cmPage.Parent?.Children().OrderByDescending(x => x.CreateDate).Take(5))
                {
                    if (ipPost.ContentType.Alias == Common.Doctype.BlogPost)
                    {
                        BlogPost cmBlogpost = new BlogPost(ipPost, _publishedValueFallback);
                        vmBlogPost.LstRecentPosts.Add(new Link()
                        {
                            Title = cmBlogpost.Name,
                            Url = cmBlogpost.Url(),
                            Summary = cmBlogpost.PublishedDate.ToString("MM/dd/yyyy")
                        });
                    }
                    else if (ipPost.ContentType.Alias == Common.Doctype.ScrollingBlogPost)
                    {
                        ScrollingBlogPost cmBlogpost = new ScrollingBlogPost(ipPost, _publishedValueFallback);
                        vmBlogPost.LstRecentPosts.Add(new Link()
                        {
                            Title = cmBlogpost.Name,
                            Url = cmBlogpost.Url(),
                            Summary = cmBlogpost.PublishedDate.ToString("MM/dd/yyyy")
                        });
                    }


                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }


            var viewModel = new ComposedPageViewModel<ScrollingBlogPost, ScrollingBlogPostViewModel>
            {
                Page = cmPage,
                ViewModel = vmBlogPost
            };

            return View(Common.View.ScrollingBlogPost, viewModel);
        }
    }
}
