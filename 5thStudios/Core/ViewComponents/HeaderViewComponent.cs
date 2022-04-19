//using JonDJones.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco.Extensions;
using Core.ViewModel;




namespace Core
{
    public class HeaderViewComponent : ViewComponent
    {
        //private IMenuService _menu;
        private IUmbracoContextAccessor _context;

        public HeaderViewComponent(IUmbracoContextAccessor context) { _context = context; }



        public async Task<IViewComponentResult> InvokeAsync(IPublishedContent model)
        {
            Test _test = new Test();
            _test.text = "RESPONSE TEXT FROM HEADERVIEWCOMPONENT.CS WITH NAME: " + model.Name;

            return View(_test);
        }





    }
}
