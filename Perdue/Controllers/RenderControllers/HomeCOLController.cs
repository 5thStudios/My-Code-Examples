﻿using Dragonfly.NetHelpers;
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
    public class HomeCOLController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<HomeCOLController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly Umbraco.Cms.Core.Hosting.IHostingEnvironment _hostingEnvironment;

        public HomeCOLController(
                ILogger<HomeCOLController> _logger,
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
            //Return
            return View(Common.View.HomeCOL, new ComposedPageViewModel<HomeCol, HomeViewModel>()
            {
                Page = new HomeCol(CurrentPage, _publishedValueFallback),
                ViewModel = new HomeViewModel()
            });
        }
    }
}