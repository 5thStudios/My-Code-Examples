using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using www.Models;
using www.Models.PublishedModels;
using www.ViewModels;
using cm = www.Models.PublishedModels;


namespace www.ViewComponents
{
    public class HeroViewComponent : ViewComponent
    {
        private IPublishedValueFallback PublishedValueFallback;
        private readonly ILogger<HeroViewComponent> logger;


        public HeroViewComponent(
            ILogger<HeroViewComponent> _logger,
                IPublishedValueFallback _PublishedValueFallback)
        {
            PublishedValueFallback = _PublishedValueFallback;
            logger = _logger;
        }





        public async Task<IViewComponentResult> InvokeAsync(IPublishedContent ipModel)
        {
            //Instantiate variables
            HeroViewModel heroVM = new HeroViewModel();
            cm.Hero cmHero = new Hero(ipModel, PublishedValueFallback);



            try
            {
                //
                if (ipModel.ContentType.Alias == Common.Doctype.ListOfHomePgs)
                {
                    //Do not show hero panel
                    return Content(string.Empty);
                }
                else
                {
                    //Determine if pg is project pg
                    if (ipModel.ContentType.Alias == Common.Doctype.ProjectPage)
                        heroVM.IsProjectPg = true;



                    if (cmHero.HeroBackgroundImage == null)
                    {
                        //Instantiate models
                        IPublishedContent ipParentHero = ObtainParentHero(ipModel.Parent);
                        cm.Hero cmParentHero = new Hero(ipParentHero, PublishedValueFallback);


                        //Obtain proper bg image
                        if (heroVM.IsProjectPg)
                            heroVM.BgImageUrl = cmParentHero.HeroBackgroundImage?.GetCropUrl(Common.Crop.Project_1903x925);                       
                        else
                            heroVM.BgImageUrl = cmParentHero.HeroBackgroundImage?.GetCropUrl(Common.Crop.Hero_1903x435);
                    }
                    else
                    {
                        //Obtain proper bg image
                        if (heroVM.IsProjectPg)
                            heroVM.BgImageUrl = cmHero.HeroBackgroundImage?.GetCropUrl(Common.Crop.Project_1903x925);                       
                        else
                            heroVM.BgImageUrl = cmHero.HeroBackgroundImage?.GetCropUrl(Common.Crop.Hero_1903x435);

                    }


                    //Get video url if exists
                    if (cmHero.HeroBackgroundVideo != null)
                    {
                        heroVM.BgVideoSrc = cmHero.HeroBackgroundVideo?.Url();
                        heroVM.ShowVideoBg = true;
                    }



                    //Obtain optional values
                    heroVM.Title = cmHero.HeroTitle;
                    if (string.IsNullOrWhiteSpace(heroVM.Title))
                        heroVM.Title = ipModel.Name;

                    heroVM.Subtitle = cmHero.HeroSubtitle; 
                    if (heroVM.IsProjectPg && string.IsNullOrWhiteSpace(heroVM.Subtitle))
                    {
                        cm.ProjectPage cmProjectPg = new ProjectPage(ipModel, PublishedValueFallback);
                        heroVM.Subtitle = cmProjectPg.Location;
                    }

                    //If bio pg, get subtitle
                    if (ipModel.ContentType.Alias == Common.Doctype.Bio && string.IsNullOrWhiteSpace(heroVM.Subtitle))
                    {
                        cm.Bio cmBio = new Bio(ipModel, PublishedValueFallback);
                        heroVM.Subtitle = cmBio.Title;
                    }

                    if (cmHero.HeroButton != null)
                    {
                        heroVM.Button = new Link()
                        {
                            Title = cmHero.HeroButton.Name,
                            Url = cmHero.HeroButton.Url,
                            Target = cmHero.HeroButton.Target
                        };
                    }
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }



            //Return data to partialview
            return View(Common.Partial.Hero, heroVM);
        }

        private IPublishedContent ObtainParentHero(IPublishedContent ipParent)
        {
            //Get 
            cm.Hero cmHero = new Hero(ipParent, PublishedValueFallback);

            if (cmHero.HeroBackgroundImage == null && cmHero.HeroBackgroundImage == null)
            {
                ipParent = ObtainParentHero(ipParent.Parent);
            }

            return ipParent;
        }
    }
}
