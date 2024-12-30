using ECMC_Umbraco.Models;
using Examine;
using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.PublishedModels;
using cm = Umbraco.Cms.Web.Common.PublishedModels;



namespace www.ViewComponents
{
    public class NewsTop5ViewComponent : ViewComponent
    {
        private IUmbracoContextAccessor Context;
        private UmbracoHelper UmbracoHelper;
        private IPublishedValueFallback PublishedValueFallback;
        private readonly IExamineManager ExamineManager;
        private readonly ILogger<NewsTop5ViewComponent> logger;


        public NewsTop5ViewComponent(
            ILogger<NewsTop5ViewComponent> _logger,
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





        public async Task<IViewComponentResult> InvokeAsync(Umbraco.Cms.Core.Models.Blocks.BlockGridItem<NewsTop5> Model)
        {
            //Instantiate variables
            var vmNewsTop5 = new NewsTop5ViewModel();


            try
            {
                //Obtain new picker
                IPublishedContent ipNewsPicker = Model.Content.NewsPicker!;
                var HowManyToShow = Model.Content.HowManyToShow;
                if (HowManyToShow == 0)
                    HowManyToShow = 5;


                //Create unordered list of child nodes
                List<Item> lstItems = new List<Item>();
                foreach (var item in ipNewsPicker.Descendants())
                {
                    if (item.ContentType.Alias.Contains("newsCard"))
                    {
                        lstItems.Add(new Item()
                        {
                            DatePosted = item.Value<DateTime>("postDate"),
                            Id = item.Id
                        });
                    }
                }


                //Sort list and take top 5 and add to list.
                foreach (var item in lstItems.OrderByDescending(x => x.DatePosted).Take(HowManyToShow))
                {
                    var ipItem = UmbracoHelper.Content(item.Id);
                    vmNewsTop5.LstTop5.Add(new UmbracoProject.Models.Link()
                    {
                        Title = ipItem.Name,
                        Summary = ipItem.Value<DateTime>("postDate").ToString("MM-dd-yy"),
                        Url = ipItem.Url()
                    });
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }


            //Return data to partialview
            return View(Common.Partial.NewsTop5List, vmNewsTop5);
        }


        private class Item()
        {
            public DateTime DatePosted { get; set; }
            public int Id { get; set; }
        }

    }
}
