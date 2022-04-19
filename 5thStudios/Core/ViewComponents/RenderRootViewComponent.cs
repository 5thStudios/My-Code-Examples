//using JonDJones.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco.Extensions;

//namespace Core
//{
    public class RenderRootViewComponent : ViewComponent
    {

        //private IUmbracoContextAccessor _context;

        //public RenderRootViewComponent(
        //       IUmbracoContextAccessor context)
        //{
        //    _context = context;
        //}


        //public async Task<IViewComponentResult> InvokeAsync(IPublishedContent currentPage)
        // public IViewComponentResult InvokeAsync(IPublishedContent currentPage)

        public IViewComponentResult InvokeAsync(IPublishedContent currentPage)
        {
            var root = currentPage.Root();
            return View("root", root);
        }

        //private IMenuService _menu;
        //private IUmbracoContextAccessor _context;
        //private string DEFAULT_TEXT = "A RESPONSIVE HTML5 SITE TEMPLATE.MANUFACTURED BY HTML5 UP.";

        //public RenderRootViewComponent(
        //        IMenuService menu,
        //        IUmbracoContextAccessor context)
        //{
        //    _context = context;
        //    _menu = menu;
        //}

        //public async Task<IViewComponentResult> InvokeAsync()
        //{
        //    var content = _context.GetRequiredUmbracoContext().PublishedRequest.PublishedContent;
        //    var keyword = content.Value<string>("keyword");
        //    var header = new HeaderViewModel
        //    {
        //        MenuItems = _menu.Menus,
        //        Title = content.Name,
        //        SubTitle = keyword == null || keyword.IsNullOrWhiteSpace() ? DEFAULT_TEXT : keyword
        //    };

        //    return View(header);
        //}
    }
//}
