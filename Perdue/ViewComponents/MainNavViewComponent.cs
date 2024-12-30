using Examine;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using www.Models;
using www.Models.PublishedModels;


namespace www.ViewComponents
{
    public class MainNavViewComponent : ViewComponent
    {
        private IPublishedValueFallback PublishedValueFallback;
        private readonly ILogger<MainNavViewComponent> logger;


        public MainNavViewComponent(
            ILogger<MainNavViewComponent> _logger,
                IPublishedValueFallback _PublishedValueFallback)
        {
            logger = _logger;
            PublishedValueFallback = _PublishedValueFallback;
        }





        public async Task<IViewComponentResult> InvokeAsync(IPublishedContent ipModel, bool isMobile = false)
        {
            //Instantiate scope variables
            MainNavigation mainNavigation = new MainNavigation();

            try
            {
                //Instantiate variables
                IPublishedContent ipRoot = ipModel.Root();
                Home cmHome = new Home(ipRoot, PublishedValueFallback);


                /*                 
                 foreach (var a in MainNav)
	                if a.content.contenttype == "navigationGroup"
		                a.Link [1st level]
		
		                foreach (var b in a.GroupColumns)
			                if b.content.contenttype == "groupColumn"
			
				                foreach (var c in b.NavigationElements)
					                if (c.content.contentType == "navigationLink")
						                c.Link [2nd level]
					                else if (c.content.contenttype == "header") 
						                Title = c.content.title
                 */



                //Generate navigation from BlockList
                foreach (var _navigationGroup in cmHome.MainNavigation)
                {
                    if (_navigationGroup.Content.ContentType.Alias == Common.Doctype.NavigationGroup)
                    {
                        //Create model
                        var cmNavigationGroup = new NavigationGroup(_navigationGroup.Content, PublishedValueFallback);


                        //Create navigation group
                        NavGroup navGroup = new NavGroup();
                        mainNavigation.LstNavGroups?.Add(navGroup);


                        //Get link
                        navGroup.Link = new Link()
                        {
                            Url = cmNavigationGroup.Link.Url,
                            Title = cmNavigationGroup.Link.Name,
                            Target = cmNavigationGroup.Link.Target,
                            Level = 1,
                            NavLevel = "primary",
                            Summary = "mega-menu-" + cmNavigationGroup.Link?.Name?.ToLower().Replace(" ", "")
                        };


                        foreach (var _groupColumn in cmNavigationGroup.GroupColumns)
                        {
                            if (_groupColumn.Content.ContentType.Alias == Common.Doctype.GroupColumn)
                            {
                                //Create model
                                var _cmGroupColumn = new Models.PublishedModels.GroupColumn(_groupColumn.Content, PublishedValueFallback);


                                //Create navigation group
                                var groupColumn = new Models.GroupColumn();
                                navGroup.LstColumns.Add(groupColumn);


                                foreach (var _element in _cmGroupColumn.NavigationElements)
                                {
                                    //Create element
                                    var navElement = new NavElement();
                                    groupColumn.LstElements.Add(navElement);


                                    //Get data from element
                                    switch (_element.Content.ContentType.Alias)
                                    {
                                        case "header":
                                            var header = new Header(_element.Content, PublishedValueFallback);
                                            navElement.IsHeader = true;

                                            string navLvl3 = "tertiary";
                                            if (!string.IsNullOrEmpty(header.LevelOverride) && header.LevelOverride != "None") navLvl3 = header.LevelOverride?.ToLower();

                                            navElement.Link = new Link()
                                            {
                                                Title = header.Title,
                                                Level = 3,
                                                NavLevel = navLvl3
                                            };
                                            break;


                                        case "navigationLink":
                                            var navLink = new NavigationLink(_element.Content, PublishedValueFallback);
                                            navElement.IsHeader = false;

                                            string navLvl2 = "secondary";
                                            if (!string.IsNullOrEmpty(navLink.LevelOverride) && navLink.LevelOverride != "None") navLvl2 = navLink.LevelOverride?.ToLower();

                                            navElement.Link = new Link()
                                            {
                                                Title = navLink.Link?.Name,
                                                Url = navLink.Link?.Url,
                                                Target = navLink.Link?.Target,
                                                Level = 2,
                                                NavLevel = navLvl2
                                            };


                                            if (navLink.ChildLinks.Any())
                                            {
                                                foreach (var bliChildLink in navLink.ChildLinks)
                                                {
                                                    ChildLink childLink = new ChildLink(bliChildLink.Content, PublishedValueFallback);

                                                    string navLvl = "quaternary";
                                                    if (!string.IsNullOrEmpty(navLink.LevelOverride) && navLink.LevelOverride != "None") navLvl = navLink.LevelOverride?.ToLower();

                                                    navElement.LstChildLinks.Add(new Link()
                                                    {
                                                        Title = childLink.Link?.Name,
                                                        Url = childLink.Link?.Url,
                                                        Target = childLink.Link?.Target,
                                                        Level = 4,
                                                        NavLevel = navLvl
                                                    });
                                                }
                                            }


                                            break;

                                        default:
                                            break;
                                    }

                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }



            //Return data to partialview
            if (isMobile)
            {
                return View(Common.Partial.MobalNav, mainNavigation);
            }
            else
            {
                return View(Common.Partial.MainNav, mainNavigation);
            }
        }
    }
}
