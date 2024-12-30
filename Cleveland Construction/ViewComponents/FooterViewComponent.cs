using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using www.Models;
using www.Models.PublishedModels;
using www.ViewModels;
using cm = www.Models.PublishedModels;


namespace www.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private IPublishedValueFallback PublishedValueFallback;
        private readonly ILogger<FooterViewComponent> logger;

        public FooterViewComponent(
            ILogger<FooterViewComponent> _logger,
                IPublishedValueFallback _PublishedValueFallback)
        {
            PublishedValueFallback = _PublishedValueFallback;
            logger = _logger;
        }



        public async Task<IViewComponentResult> InvokeAsync(IPublishedContent ipModel)
        {
            //Instantiate variables
            FooterViewModel footerVM = new FooterViewModel();


            try
            {
                if (ipModel.ContentType.Alias == Common.Doctype.ListOfHomePgs)
                {
                    //Obtain model of pg
                    cm.ListOfHomePgs cmListOfHomePgs = new cm.ListOfHomePgs(ipModel, PublishedValueFallback);


                    //Set bool variable
                    footerVM.IsListOfHomePgs = true;


                    //Site logo
                    footerVM.Logo = new Link()
                    {
                        ImgUrl = cmListOfHomePgs.FooterLogo?.Url()
                    };


                    //Footer description
                    footerVM.Description = cmListOfHomePgs.FooterParagraph;


                    //Minor Nav
                    foreach (var _link in cmListOfHomePgs.FooterMinorNav)
                    {
                        footerVM.LstMinorNav.Add(new Link()
                        {
                            Url = _link.Url,
                            Title = _link.Name,
                            Target = _link.Target
                        });
                    }


                    //Social links
                    foreach (var link in cmListOfHomePgs.SocialLinks)
                    {
                        cm.ImageLink cmImgLink = new cm.ImageLink(link.Content, PublishedValueFallback);
                        footerVM.LstSocialLinks.Add(new Link()
                        {
                            Title = cmImgLink.Link?.Name ?? string.Empty,
                            Url = cmImgLink.Link?.Url ?? string.Empty,
                            Target = cmImgLink.Link?.Target ?? string.Empty,
                            Class = cmImgLink.Class ?? string.Empty,
                            ImgUrl = cmImgLink.Image?.Url() ?? string.Empty
                        });
                    }
                }
                else
                {

                    //Obtain settings pg
                    cm.SiteSettings? cmSettings = Common.ObtainSettingsPg(ipModel, PublishedValueFallback);
                    if (cmSettings == null)
                    {
                        //Do not show panel
                        return Content(string.Empty);
                    }
                    else
                    {
                        //Footer site logo
                        cm.HomePg cmHomePg = Common.ObtainHomePg(ipModel, PublishedValueFallback);
                        footerVM.Logo = new Link()
                        {
                            ImgUrl = cmSettings.FooterLogo?.Url(),
                            Url = cmHomePg.Url(),
                            Title = cmHomePg.Name
                        };


                        //Footer description
                        footerVM.Description = cmSettings.FooterParagraph;


                        //Minor Nav
                        foreach (var _link in cmSettings.FooterMinorNav)
                        {
                            footerVM.LstMinorNav.Add(new Link()
                            {
                                Url = _link.Url,
                                Title = _link.Name,
                                Target = _link.Target
                            });
                        }


                        //Locations
                        foreach (var ipChild in cmSettings?.FooterLocations?.Children)
                        {
                            footerVM.LstLocations.Add(new Link()
                            {
                                Url = ipChild.Url(),
                                Title = ipChild.Name
                            });
                        }


                        //Social links
                        foreach (var link in cmSettings.SocialLinks)
                        {
                            cm.ImageLink cmImgLink = new cm.ImageLink(link.Content, PublishedValueFallback);
                            footerVM.LstSocialLinks.Add(new Link()
                            {
                                Title = cmImgLink.Link?.Name ?? string.Empty,
                                Url = cmImgLink.Link?.Url ?? string.Empty,
                                Target = cmImgLink.Link?.Target ?? string.Empty,
                                Class = cmImgLink.Class ?? string.Empty,
                                ImgUrl = cmImgLink.Image?.Url() ?? string.Empty
                            });
                        }


                        //Main navigation
                        foreach (IPublishedContent ipParent in cmHomePg.Children)
                        {
                            //Get ip navigation
                            cm.Navigation cmParent = new Navigation(ipParent, PublishedValueFallback);


                            if (cmParent.ShowInFooterMainNav)
                            {
                                //Obtain link data
                                Link lnkParent = new Link() { Title = ipParent.Name, Url = ipParent.Url() };


                                //Title override
                                if (!string.IsNullOrEmpty(cmParent.NavigationTitleOverride?.Trim()))
                                    lnkParent.Title = cmParent.NavigationTitleOverride?.Trim() ?? string.Empty;


                                //if (cmParent.ContentType.Alias == Common.Doctype.RedirectToPage)
                                //{
                                //    //Change url to the redirect page
                                //    cm.RedirectToPage cmPg = new RedirectToPage(ipParent);
                                //    lnkParent.Url = cmPg.RedirectPage.Url();
                                //}



                                if (ipParent.Children.Any())
                                {
                                    //Instantiate new child list
                                    lnkParent.LstChildLinks = new List<Link>();

                                    //Loop through all children
                                    foreach (IPublishedContent ipChild in ipParent.Children)
                                    {
                                        //Get ip navigation
                                        cm.Navigation cmChild = new Navigation(ipChild, PublishedValueFallback);


                                        if (cmChild.ShowInFooterMainNav)
                                        {
                                            //Obtain link data
                                            Link lnkChild = new Link() { Title = ipChild.Name, Url = ipChild.Url() };


                                            //Title override
                                            if (!string.IsNullOrEmpty(cmChild.NavigationTitleOverride?.Trim()))
                                                lnkChild.Title = cmChild.NavigationTitleOverride?.Trim() ?? string.Empty;


                                            //
                                            //if (cmChild.ContentType.Alias == Common.Doctype.RedirectToPage)
                                            //{
                                            //    //Change url to the redirect page
                                            //    cm.RedirectToPage cmPg = new RedirectToPage(ipChild);
                                            //    lnkChild.Url = cmPg.RedirectPage.Url();
                                            //}

                                            //Add page to list
                                            lnkParent.LstChildLinks.Add(lnkChild);
                                        }
                                    }

                                    //nullify if no children were added
                                    if (!lnkParent.LstChildLinks.Any())
                                    {
                                        lnkParent.LstChildLinks = null;
                                    }
                                }

                                //Add page to list
                                footerVM.LstMainNav.Add(lnkParent);
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
            return View(Common.Partial.Footer, footerVM);
        }
    }
}
