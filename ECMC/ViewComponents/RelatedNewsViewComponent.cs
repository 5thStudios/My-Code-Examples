using ECMC_Umbraco.Models;
using Examine;
using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.PublishedModels;
using UmbracoProject.Models;
using cm = Umbraco.Cms.Web.Common.PublishedModels;



namespace www.ViewComponents
{
    public class RelatedNewsViewComponent : ViewComponent
    {
        private IUmbracoContextAccessor Context;
        private UmbracoHelper UmbracoHelper;
        private IPublishedValueFallback PublishedValueFallback;
        private readonly IExamineManager ExamineManager;
        private readonly ILogger<RelatedNewsViewComponent> logger;


        public RelatedNewsViewComponent(
            ILogger<RelatedNewsViewComponent> _logger,
            IExamineManager _ExamineManager,
            IUmbracoContextAccessor _Context,
            UmbracoHelper _UmbracoHelper,
            IPublishedValueFallback _PublishedValueFallback)
        {
            ExamineManager = _ExamineManager ?? throw new ArgumentNullException(nameof(_ExamineManager));
            Context = _Context;
            logger = _logger;
            UmbracoHelper = _UmbracoHelper;
            PublishedValueFallback = _PublishedValueFallback;
        }





        public async Task<IViewComponentResult> InvokeAsync(Umbraco.Cms.Core.Models.Blocks.BlockGridItem<RelatedNews> Model)
        {
            //Instantiate variables
            var vmRelatedNews = new RelatedNewsViewModel();
            List<Link> LstNewsItems = new List<Link>();
            List<string> LstTags = new List<string>();

            try
            {
                //
                vmRelatedNews.RootList = Model.Content.RootList;
                vmRelatedNews.ItemCount = Model.Content.ItemCount;
                if (vmRelatedNews.ItemCount < 4) vmRelatedNews.ItemCount = 4;


                //Obtain tag filtes if applicable
                if (Model.Content.TagsAreasOfInterest != null)
                {
                    foreach (var ip in Model.Content.TagsAreasOfInterest)
                        LstTags.Add(ip.Name);
                }


                //Create full list of news cards without filtering
                foreach (var ipItem in vmRelatedNews.RootList!.Descendants().OrderByDescending(x => x.Value<DateTime>("postDate")))
                {
                    if (!ipItem.ContentType.Alias.Contains("Category"))
                    {
                        //Instantiate news card
                        var cmNewsCard = new NewsCardEif(ipItem, PublishedValueFallback);  //Assuming all NewsCards have the same properties.  Using this model as base model.


                        //Create list of tags
                        List<string> LstCardTags = new List<string>();
                        if (cmNewsCard.AreasOfInterest != null)
                        {
                            foreach (var ip in cmNewsCard.AreasOfInterest)
                                LstCardTags.Add(ip.Name);
                        }


                        //Create list of items
                        LstNewsItems.Add(new UmbracoProject.Models.Link()
                        {
                            Url = cmNewsCard.Url(),
                            ImgUrl = cmNewsCard.PrimaryImage?.GetCropUrl(Common.Crop.LearnMore_500x550),
                            Title = cmNewsCard.Title,
                            Summary = cmNewsCard.Summary,
                            Misc = cmNewsCard.PostDate.ToString("MMM d, yyyy"),
                            LstMisc = LstCardTags,
                            JsonMisc = Newtonsoft.Json.JsonConvert.SerializeObject(LstCardTags)
                        });
                    }
                }


                //
                if (LstTags.Any())
                {
                    //Take filtered list
                    vmRelatedNews.LstNewsItems = LstNewsItems.Where(x => x.LstMisc.Intersect(LstTags).Any()).Take(vmRelatedNews.ItemCount).ToList();
                }
                else
                {
                    //Take unfiltered list
                    vmRelatedNews.LstNewsItems = LstNewsItems.Take(vmRelatedNews.ItemCount).ToList();
                }




                //vmRelatedNews.ItemCount

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }


            //Return data to partialview
            return View(Common.Partial.RelatedNewsList, vmRelatedNews);
        }

    }
}
