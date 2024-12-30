using Examine;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.PublishedModels;
using www.ViewModels;
using cm = Umbraco.Cms.Web.Common.PublishedModels;




namespace www.ViewComponents
{
    public class TeaserViewComponent : ViewComponent
    {
        private IUmbracoContextAccessor Context;
        private UmbracoHelper UmbracoHelper;
        private IPublishedValueFallback PublishedValueFallback;
        private readonly IExamineManager ExamineManager;
        private readonly ILogger<TeaserViewComponent> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;


        public TeaserViewComponent(
            ILogger<TeaserViewComponent> _logger,
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





        public async Task<IViewComponentResult> InvokeAsync(Umbraco.Cms.Core.Models.Blocks.BlockGridItem<Teaser> bgiTeaser)
        {

            //Instantiate variables
            TeaserViewModel vmTeaser = new TeaserViewModel();

            try
            {
                //
                vmTeaser.CustomScripts = bgiTeaser.Content.CustomScripts;



                int counter = 0;
                foreach (BlockListItem teaserBlock in bgiTeaser.Content.TeaserBlocks)
                {
                    //
                    var cmTeaserBlk = new cm.TeaserBlock(teaserBlock.Content, PublishedValueFallback);

                    //
                    vmTeaser.LstTeaserBlocks.Add(new TeaserBlockViewModel()
                    {
                        Content = cmTeaserBlk.Content,
                        BgColor = cmTeaserBlk.BackgroundColor,
                        BgImageUrl = cmTeaserBlk.BackgroundImage.GetCropUrl(www.Models.Common.Crop.Portrait_600x400),
                        CustomScripts = cmTeaserBlk.CustomScripts,
                        IsImgFirst = (counter >= 2)
                    });

                    counter++;
                    if (counter == 4)
                        counter = 0;
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }

            

            //Return data to partialview
            return View(www.Models.Common.Partial.BvTeaser, vmTeaser);

        }
    }
}
