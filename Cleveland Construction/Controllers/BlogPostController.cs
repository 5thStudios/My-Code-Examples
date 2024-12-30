using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Web;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Controllers;
using www.Models;
using www.Models.PublishedModels;
using www.ViewModels;
using cm = www.Models.PublishedModels;


namespace www.Controllers
{
    public class BlogPostController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<BlogPostController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly Umbraco.Cms.Core.Hosting.IHostingEnvironment _hostingEnvironment;

        public BlogPostController(
                ILogger<BlogPostController> _logger,
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
            cm.BlogPost cmBlogPost = new cm.BlogPost(CurrentPage, _publishedValueFallback);
            BlogPostViewModel vmBlogPost = new BlogPostViewModel();


            try
            {
                //Obtain page titles and descriptors
                vmBlogPost.Title = cmBlogPost.Name;
                if (!string.IsNullOrWhiteSpace(cmBlogPost.HeroTitle))
                    vmBlogPost.Title = cmBlogPost.HeroTitle;

                if (!string.IsNullOrWhiteSpace(cmBlogPost.HeroSubtitle))
                    vmBlogPost.Subtitle = cmBlogPost.HeroSubtitle;

                vmBlogPost.DatePosted = cmBlogPost.DatePosted;


                //Author
                if (!string.IsNullOrWhiteSpace(cmBlogPost.Author))
                {
                    vmBlogPost.LnkAuthor = new Link() { Title = cmBlogPost.Author };
                    if (cmBlogPost.AuthorLink != null)
                        vmBlogPost.LnkAuthor.Url = cmBlogPost.AuthorLink.Url();
                }


                //Card Image
                vmBlogPost.PostImageUrl = cmBlogPost.PostImage?.Url();


                //Main Content
                vmBlogPost.MainContent = cmBlogPost.MainContent;


                //Obtain blog navigation links
                List<IPublishedContent> ipLstBlogPosts = cmBlogPost.Parent!.Children.OrderByDescending(x => x.Value<DateTime>(Common.Property.DatePosted)).ToList();
                int index = ipLstBlogPosts.IndexOf(CurrentPage!);
                if (index > 0)
                    vmBlogPost.LnkPrev = new Link()
                    {
                        Title = ipLstBlogPosts[index - 1].Name,
                        Url = ipLstBlogPosts[index - 1].Url()
                    };

                if (index < (ipLstBlogPosts.Count - 1))
                    vmBlogPost.LnkNext = new Link()
                    {
                        Title = ipLstBlogPosts[index + 1].Name,
                        Url = ipLstBlogPosts[index + 1].Url()
                    };
                vmBlogPost.LnkBlog = new Link() { Url = cmBlogPost.Parent.Url() };


                //Obtain list of categories and convert to Category Link list.
                List<string> LstCategories = new List<string>();
                foreach (IPublishedContent ipPost in ipLstBlogPosts)
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
                    vmBlogPost.LstCategories.Add(new Link()
                    {
                        Title = category,
                        Url = cmBlogPost.Parent.Url() + "?category=" + HttpUtility.UrlEncode(category), //cmBlogPost.Parent.Url() + string.Format("categories/{0}", Uri.EscapeDataString(category)),
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
                        vmBlogPost.LstSocialLinks.Add(new Link()
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
            var viewModel = new ComposedPageViewModel<cm.BlogPost, BlogPostViewModel>
            {
                Page = cmBlogPost,
                ViewModel = vmBlogPost
            };
            return View(Common.View.BlogPost, viewModel);
        }
    }
}
