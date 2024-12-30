using Examine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Web;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Controllers;
using www.Models;
using www.Models.PublishedModels;
using www.ViewModels;
using static Umbraco.Cms.Core.Constants;
using cm = www.Models.PublishedModels;


namespace www.Controllers
{
    public class BlogController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<BlogController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly Umbraco.Cms.Core.Hosting.IHostingEnvironment _hostingEnvironment;
        private readonly IExamineManager ExamineManager;
        private readonly IPublishedContentQuery publishedContentQuery;


        public BlogController(
                ILogger<BlogController> _logger,
                IExamineManager _ExamineManager,
                IPublishedContentQuery _publishedContentQuery,
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
            ExamineManager = _ExamineManager ?? throw new ArgumentNullException(nameof(_ExamineManager));
            UmbracoHelper = _UmbracoHelper;
            logger = _logger;
            _serviceContext = context;
            _variationContextAccessor = variationContextAccessor;
            _hostingEnvironment = hostingEnvironment;
            publishedContentQuery = _publishedContentQuery;
        }

        public override IActionResult Index()
        {
            //Instantiate variables
            cm.Blog cmBlogList = new cm.Blog(CurrentPage, _publishedValueFallback);
            BlogListViewModel vmBlogList = new BlogListViewModel();
            vmBlogList.SearchQuery = Request.Query[Common.Query.Search];
            vmBlogList.CategoryQuery = Request.Query[Common.Query.Category];


            try
            {
                //Get list of all blog posts in descending date order
                List<IPublishedContent> lstIpBlogPosts = new List<IPublishedContent>();


                if (!string.IsNullOrWhiteSpace(vmBlogList.SearchQuery))
                {
                    vmBlogList.ShowSearchPnl = true;

                    if (ExamineManager.TryGetIndex(UmbracoIndexes.ExternalIndexName, out IIndex? index))
                    {
                        //Get initial order
                        List<int> LstNodeIDs = new List<int>();
                        foreach (var ip in cmBlogList.Children.OrderByDescending(x => x.Value<DateTime>(Common.Property.DatePosted)).ToList())
                        {
                            LstNodeIDs.Add(ip.Id);
                        }


                        //Update query with quotes to allow non-exact searches.  [Without the quotes, only exact words can be found]
                        string searchQuery = "\"" + vmBlogList.SearchQuery + "\"";


                        //Query
                        var queryExecutor = index.Searcher
                                            .CreateQuery(IndexTypes.Content)
                                            .ManagedQuery(vmBlogList.SearchQuery); //.And().GroupedNot(new[] { "nodeTypeAlias" }, lstExcludedAliases.ToArray());

                        IEnumerable<PublishedSearchResult> LstTemp = publishedContentQuery.Search(queryExecutor);

                        foreach (var nodeId in LstNodeIDs)
                        {
                            if (LstTemp.Any(x => x.Content.Id == nodeId))
                            {
                                lstIpBlogPosts.Add(publishedContentQuery.Content(nodeId));
                            }
                        }
                    }
                }
                else if (!string.IsNullOrWhiteSpace(vmBlogList.CategoryQuery))
                {
                    vmBlogList.ShowCategoryPnl = true;

                    //Get list of all blog posts in descending date order
                    List<IPublishedContent> lstTempIp = cmBlogList.Children.OrderByDescending(x => x.Value<DateTime>(Common.Property.DatePosted)).ToList();

                    foreach (var ip in lstTempIp)
                    {
                        cm.BlogPost cmPost = new BlogPost(ip, _publishedValueFallback);
                        if (cmPost.Categories.Contains(vmBlogList.CategoryQuery))
                        {
                            lstIpBlogPosts.Add(ip);
                        }
                    }
                }
                else
                {
                    //Get list of all blog posts in descending date order
                    lstIpBlogPosts = cmBlogList.Children.OrderByDescending(x => x.Value<DateTime>(Common.Property.DatePosted)).ToList();
                }


                foreach (var ipPost in lstIpBlogPosts)
                {
                    //Instantiate local variables
                    cm.BlogPost cmBlogPost = new cm.BlogPost(ipPost, _publishedValueFallback);
                    BlogPostViewModel vmBlogPost = new BlogPostViewModel();


                    //Obtain page titles and descriptors
                    vmBlogPost.Title = cmBlogPost.Name;
                    if (!string.IsNullOrWhiteSpace(cmBlogPost.HeroTitle))
                        vmBlogPost.Title = cmBlogPost.HeroTitle;

                    vmBlogPost.DatePosted = cmBlogPost.DatePosted;

                    vmBlogPost.PostImageUrl = cmBlogPost.PostImage?.GetCropUrl(Common.Crop.BlogThumbnail_660x435);

                    vmBlogPost.Excerpt = cmBlogPost.Excerpt;

                    vmBlogPost.Url = cmBlogPost.Url();


                    //Add to list
                    vmBlogList.LstBlogPosts.Add(vmBlogPost);
                }


                //Obtain list of categories and convert to Category Link list.
                List<string> LstCategories = new List<string>();
                foreach (IPublishedContent ipPost in cmBlogList.Children)
                {
                    cm.BlogPost cmPost = new cm.BlogPost(ipPost, _publishedValueFallback);
                    if (cmPost.Categories != null && cmPost.Categories.Count() > 0)
                    {
                        foreach (string category in cmPost.Categories)
                        {
                            if (!LstCategories.Contains(category))
                                LstCategories.Add(category);
                        }
                    }
                }
                foreach (string category in LstCategories.OrderBy(x => x))
                {
                    vmBlogList.LstCategories.Add(new Link()
                    {
                        Title = category,
                        Url = cmBlogList.Url() + "?category=" + HttpUtility.UrlEncode(category), //Url = cmBlogList.Parent.Url() + string.Format("categories/{0}", Uri.EscapeDataString(category))
                        Class = Common.ConvertToClass(category)
                    });
                }


                //Obtain settings pg
                cm.SiteSettings? cmSettings = Common.ObtainSettingsPg(CurrentPage!, _publishedValueFallback);
                if (cmSettings != null)
                {
                    //Social links
                    foreach (var link in cmSettings.SocialLinks)
                    {
                        cm.ImageLink cmImgLink = new cm.ImageLink(link.Content, _publishedValueFallback);
                        vmBlogList.LstSocialLinks.Add(new Link()
                        {
                            Title = cmImgLink.Link?.Name ?? string.Empty,
                            Url = cmImgLink.Link?.Url ?? string.Empty,
                            Target = cmImgLink.Link?.Target ?? string.Empty,
                            Class = cmImgLink.Class ?? string.Empty
                        });
                    }
                }



            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }



            //Create view model and return to view
            var viewModel = new ComposedPageViewModel<cm.Blog, BlogListViewModel>
            {
                Page = cmBlogList,
                ViewModel = vmBlogList
            };
            return View(Common.View.Blog, viewModel);
        }
    }
}
