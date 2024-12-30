using ECMC_Umbraco.Models;
using Examine;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using ContentModels = Umbraco.Cms.Web.Common.PublishedModels;
using Microsoft.AspNetCore.Html;
using Umbraco.Cms.Core.Models;


namespace www.ViewComponents
{
	public class RenderFormViewComponent : ViewComponent
	{
		private IUmbracoContextAccessor Context;
		private UmbracoHelper UmbracoHelper;
		private IPublishedValueFallback PublishedValueFallback;
		private readonly IExamineManager ExamineManager;
		private readonly ILogger<RenderFormViewComponent> logger;
		private readonly ServiceContext _serviceContext;
		private readonly IVariationContextAccessor _variationContextAccessor;


		public RenderFormViewComponent(
			ILogger<RenderFormViewComponent> _logger,
			IExamineManager _ExamineManager,
				IUmbracoContextAccessor _Context,
				UmbracoHelper _UmbracoHelper,
				IPublishedValueFallback _PublishedValueFallback,
				ServiceContext context,
				IVariationContextAccessor variationContextAccessor)
		{
			ExamineManager = _ExamineManager ?? throw new ArgumentNullException(nameof(_ExamineManager));
			Context = _Context;
			UmbracoHelper = _UmbracoHelper;
			PublishedValueFallback = _PublishedValueFallback;
			_serviceContext = context;
			_variationContextAccessor = variationContextAccessor;
            logger = _logger;

        }





		public async Task<IViewComponentResult> InvokeAsync(IPublishedContent ipModel, bool IsTemplateA)
		{

			//Instantiate variables



			try
			{
				
			}
			catch (Exception ex)
			{
				logger.LogError(ex, ex.ToString());
			}



            //Return data to partialview
            return View("");
			
		}
	}
}
