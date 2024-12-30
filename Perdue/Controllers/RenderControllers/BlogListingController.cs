using Dragonfly.NetHelpers;
using Dragonfly.UmbracoServices;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Hosting;
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
    public class BlogListingController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<BlogListingController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly Umbraco.Cms.Core.Hosting.IHostingEnvironment _hostingEnvironment;

        public BlogListingController(
                ILogger<BlogListingController> _logger,
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
            var cmPage = new BlogListing(CurrentPage, _publishedValueFallback);
            BlogListingViewModel vmBlogPost = new BlogListingViewModel();


            try
            {
                //var allPosts = BlogHelper.GetBlogPosts(Umbraco.AssignedContentItem).OrderByDescending(n => n.PublishedDate).ToList();


                foreach (var ipPost in cmPage.Children().OrderByDescending(x => x.Value<DateTime>("PublishedDate")))
                {
                    //
                    BlogPost cmPost = new BlogPost(ipPost, _publishedValueFallback);
                    var catClasses = new List<string>();

                    //
                    if (cmPost.PublishedDate.Year < DateTime.Now.AddYears(-3).Year)
                    {
                        catClasses.Add("cat-Archived");
                    }
                    else
                    {
                        foreach (var cat in cmPost.Categories)
                        {
                            string arg = cat.MakeCamelCase().MakeCodeSafe("");
                            catClasses.Add($"cat-{arg}");
                        }
                    }
                    if (!catClasses.Contains("cat-All")) catClasses.Add("cat-All");


                    //
                    vmBlogPost.LstBlogPosts.Add(new BlogListItem()
                    {
                        Title = cmPost.Name,
                        Url = cmPost.Url(),
                        Summary = cmPost.Excerpt,
                        ImgUrl = cmPost.PostCoverImage?.GetCropUrl(Common.Crop.BlogList_320x190),
                        LstCategories = catClasses
                    });

                }


                //Determine which feature buttons to show/hide
                if (vmBlogPost.LstBlogPosts.Any(x => x.LstCategories.Any(y => y.Contains("Featured")))) vmBlogPost.HasFeatured = true;
                if (vmBlogPost.LstBlogPosts.Any(x => x.LstCategories.Any(y => y.Contains("CollegeandUniversity")))) vmBlogPost.HasCollegeAndUniversity = true;
                if (vmBlogPost.LstBlogPosts.Any(x => x.LstCategories.Any(y => y.Contains("Healthcare")))) vmBlogPost.HasHealthcare = true;
                if (vmBlogPost.LstBlogPosts.Any(x => x.LstCategories.Any(y => y.Contains("K12") || y.Contains("K-12")))) vmBlogPost.HasK12 = true;
                if (vmBlogPost.LstBlogPosts.Any(x => x.LstCategories.Any(y => y.Contains("CulinaryTrends")))) vmBlogPost.HasCulinaryTrends = true;
                if (vmBlogPost.LstBlogPosts.Any(x => x.LstCategories.Any(y => y.Contains("MenuClaims")))) vmBlogPost.HasMenuClaims = true;
                if (vmBlogPost.LstBlogPosts.Any(x => x.LstCategories.Any(y => y.Contains("ProfitDrivingTips")))) vmBlogPost.HasProfitDrivingTips = true;
                if (vmBlogPost.LstBlogPosts.Any(x => x.LstCategories.Any(y => y.Contains("Archived")))) vmBlogPost.HasArchived = true;


            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }


            var viewModel = new ComposedPageViewModel<BlogListing, BlogListingViewModel>
            {
                Page = cmPage,
                ViewModel = vmBlogPost
            };

            return View(Common.View.BlogListing, viewModel);
        }
    }
}
