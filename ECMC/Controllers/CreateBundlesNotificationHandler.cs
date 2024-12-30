using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Core.WebAssets;


namespace Umbraco.Docs.Samples.Web.Stylesheets_Javascript
{
    public class CreateBundlesNotificationHandler : INotificationHandler<UmbracoApplicationStartingNotification>
    {
        private readonly IRuntimeMinifier _runtimeMinifier;
        private readonly IRuntimeState _runtimeState;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContext;


        public CreateBundlesNotificationHandler(IRuntimeMinifier runtimeMinifier, IRuntimeState runtimeState, IWebHostEnvironment webHostEnvironment, ILogger<CreateBundlesNotificationHandler> logger,
        ICompositeViewEngine compositeViewEngine,
        IUmbracoContextAccessor umbracoContextAccessor,
        IHttpContextAccessor httpContextAccessor)

        {
            _runtimeMinifier = runtimeMinifier;
            _runtimeState = runtimeState;
            _webHostEnvironment = webHostEnvironment;
            _httpContext = httpContextAccessor;
        }


        public void Handle(UmbracoApplicationStartingNotification notification)
        {



            if (_runtimeState.Level == RuntimeLevel.Run)
            {
                string[] scriptUrl = new[] {
                    "~/js/jquery/jquery-3.7.1.js",
                    "~/js/aos/aos.js",
                    "~/js/slick/slick.js",
                    "~/js/scrollreveal/scrollreveal.js",
                    "~/js/countUp/countUp.js",
                    "~/js/chart.js/chart.js",
                    "~/js/chart.js/chart-utils.min.js",
                    "~/js/simpleMaps/mapdata.js",
                    "~/js/simpleMaps/usmap.js",
                    "~/js/paginathing/paginathing.min.js",
                    "~/js/zurb/what-input.js",
                    "~/js/zurb/foundation.js",
                    "~/js/zurb/popper.js",
                    "~/js/app.js"
                };

                //var userIp = _httpContext.HttpContext?.Request.Host.Value;

                string[] CSSPath;

                if (_webHostEnvironment.EnvironmentName.Equals("Development", StringComparison.OrdinalIgnoreCase) || _webHostEnvironment.EnvironmentName.Equals("Local", StringComparison.OrdinalIgnoreCase))
                    {
                   CSSPath = new[] { "~/css/eif.org/default.css" };
                    }
                    else
                    {
                       CSSPath = new[] { "~/css/eif.org/default.min.css" };
                    }


                    //if (_webHostEnvironment.EnvironmentName.Equals("Development", StringComparison.OrdinalIgnoreCase) || _webHostEnvironment.EnvironmentName.Equals("Local", StringComparison.OrdinalIgnoreCase))
                    //{
                    //    _runtimeMinifier.CreateJsBundle("registered-js-bundle",
                    //    BundlingOptions.NotOptimizedNotComposite,
                    //    scriptUrl);




                    //    _runtimeMinifier.CreateCssBundle("registered-css-bundle",
                    //        BundlingOptions.NotOptimizedNotComposite,
                    //        CSSPath);

                    //}
                    //else {
                    _runtimeMinifier.CreateJsBundle("registered-js-bundle",
                    BundlingOptions.NotOptimizedAndComposite,
                    scriptUrl);


                    _runtimeMinifier.CreateCssBundle("registered-css-bundle",
                        BundlingOptions.NotOptimizedAndComposite,
                        CSSPath);

                //}


            }
        }
    }
}