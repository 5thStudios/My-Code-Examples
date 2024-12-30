using Lucene.Net.Util;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using www.Models;
using www.Models.PublishedModels;
using www.ViewModels;
using static Umbraco.Cms.Core.Constants.Conventions;
using cm = www.Models.PublishedModels;


namespace www.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private IPublishedValueFallback PublishedValueFallback;
        private readonly ILogger<HeaderViewComponent> logger;

        public HeaderViewComponent(
            ILogger<HeaderViewComponent> _logger,
                IPublishedValueFallback _PublishedValueFallback)
        {
            PublishedValueFallback = _PublishedValueFallback;
            logger = _logger;
        }



        public async Task<IViewComponentResult> InvokeAsync(IPublishedContent ipModel)
        {
            //Instantiate variables
            HeaderViewModel headerVM = new HeaderViewModel();


            try
            {
                if (ipModel.ContentType.Alias == Common.Doctype.ListOfHomePgs)
                {
                    //Obtain model of pg
                    cm.ListOfHomePgs cmListOfHomePgs = new cm.ListOfHomePgs(ipModel, PublishedValueFallback);


                    //Set bool variable
                    headerVM.IsListOfHomePgs = true;


                    //Site logo
                    headerVM.SiteLogo = new Link()
                    {
                        ImgUrl = cmListOfHomePgs.SiteLogo?.Url()
                    };


                    //Tagline
                    headerVM.SiteTagline = cmListOfHomePgs.SiteTagline;


                    //Minor Nav
                    foreach (var link in cmListOfHomePgs.MinorNavigation)
                    {
                        headerVM.LstMinorNav.Add(new Link()
                        {
                            Title = link.Name,
                            Url = link.Url,
                            Target = link.Target
                        });
                    }


                    //Login links
                    foreach (var link in cmListOfHomePgs.LoginLinks)
                    {
                        headerVM.LstLoginLinks.Add(new Link()
                        {
                            Title = link.Name,
                            Url = link.Url,
                            Target = link.Target
                        });
                    }


                    //Social links
                    foreach (var link in cmListOfHomePgs.SocialLinks)
                    {
                        cm.ImageLink cmImgLink = new cm.ImageLink(link.Content, PublishedValueFallback);
                        headerVM.LstSocialLinks.Add(new Link()
                        {
                            Title = cmImgLink.Link.Name ?? string.Empty,
                            Url = cmImgLink.Link.Url ?? string.Empty,
                            Target = cmImgLink.Link.Target ?? string.Empty,
                            Class = cmImgLink.Class ?? string.Empty,
                            ImgUrl = cmImgLink.Image?.Url() ?? string.Empty
                        });
                    }





                    //Mobile Navigation
                    foreach (var link in cmListOfHomePgs.MobileNavigation)
                    {
                        headerVM.LstMainNav.Add(new Link()
                        {
                            Title = link.Name,
                            Url = link.Url,
                            Target = link.Target
                        });
                    }
                    foreach (var link in cmListOfHomePgs.MinorNavigation)
                    {
                        headerVM.LstMainNav.Add(new Link()
                        {
                            Title = link.Name,
                            Url = link.Url,
                            Target = link.Target
                        });
                    }



                    var _loginLinks = new Link();
                    _loginLinks.Title = "Login";
                    _loginLinks.Url = "#";
                    _loginLinks.LstChildLinks = new List<Link>();
                    foreach (var link in cmListOfHomePgs.LoginLinks)
                    {
                        _loginLinks.LstChildLinks.Add(new Link()
                        {
                            Title = link.Name,
                            Url = link.Url,
                            Target = link.Target
                        });
                    }
                    headerVM.LstMainNav.Add(_loginLinks);



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
                        //Site logo
                        cm.HomePg cmHomePg = Common.ObtainHomePg(ipModel, PublishedValueFallback);
                        headerVM.SiteLogo = new Link()
                        {
                            ImgUrl = cmSettings.SiteLogo?.Url(),
                            Url = cmHomePg.Url(),
                            Title = cmHomePg.Name
                        };


                        //Tagline
                        headerVM.SiteTagline = cmSettings.SiteTagline;


                        //Minor Nav
                        foreach (var ipChild in cmHomePg.Descendants().Where(x => x.HasProperty(Common.Property.ShowInTopMinorNav) && x.Value<bool>(Common.Property.ShowInTopMinorNav) == true))
                        {
                            //Variables
                            string _name = ipChild.Name;
                            string _url = ipChild.Url();
                            string _target = "_self";

                            //Get override title if applicable
                            cm.Navigation cmNav = new Navigation(ipChild, PublishedValueFallback);
                            if (!string.IsNullOrWhiteSpace(cmNav.NavigationTitleOverride))
                                _name = cmNav.NavigationTitleOverride;

                            //Get redirect to url if applicable.
                            if (ipChild.ContentType.Alias == Common.Doctype.RedirectTo)
                            {
                                cm.RedirectTo cmRedirectTo = new RedirectTo(ipChild, PublishedValueFallback);
                                _url = cmRedirectTo.RedirectToUrl.Url;
                                _target = cmRedirectTo.RedirectToUrl.Target;
                            }

                            //Create link
                            headerVM.LstMinorNav.Add(new Link()
                            {
                                Title = _name,
                                Url = _url,
                                Target = _target
                            });
                        }


                        //Login links
                        foreach (var link in cmSettings.LoginLinks)
                        {
                            headerVM.LstLoginLinks.Add(new Link()
                            {
                                Title = link.Name,
                                Url = link.Url,
                                Target = link.Target
                            });
                        }


                        //Social links
                        foreach (var link in cmSettings.SocialLinks)
                        {
                            cm.ImageLink cmImgLink = new cm.ImageLink(link.Content, PublishedValueFallback);
                            headerVM.LstSocialLinks.Add(new Link()
                            {
                                Title = cmImgLink.Link.Name ?? string.Empty,
                                Url = cmImgLink.Link.Url ?? string.Empty,
                                Target = cmImgLink.Link.Target ?? string.Empty,
                                Class = cmImgLink.Class ?? string.Empty,
                                ImgUrl = cmImgLink.Image?.Url() ?? string.Empty
                            });
                        }


                        //Main navigation
                        foreach (IPublishedContent ipParent in cmHomePg.Children)
                        {
                            //Get ip navigation
                            cm.Navigation cmParent = new Navigation(ipParent, PublishedValueFallback);


                            if (cmParent.ShowInMainNavigation)
                            {
                                //Obtain link data
                                //Link lnkParent = new Link() { Title = ipParent.Name, Url = ipParent.Url() };

                                Link lnkParent = new Link();




                                if (ipParent.ContentType.Alias == Common.Doctype.RedirectTo)
                                {
                                    cm.RedirectTo cmRedirectTo_Parent = new RedirectTo(ipParent, PublishedValueFallback);
                                    lnkParent.Title = cmRedirectTo_Parent.Name;
                                    lnkParent.Url = cmRedirectTo_Parent.RedirectToUrl?.Url;
                                    lnkParent.Target = cmRedirectTo_Parent.RedirectToUrl?.Target ?? string.Empty;
                                }
                                else
                                {
                                    lnkParent.Title = ipParent.Name;
                                    lnkParent.Url = ipParent.Url();
                                }





                                //Title override
                                if (!string.IsNullOrEmpty(cmParent.NavigationTitleOverride?.Trim()))
                                    lnkParent.Title = cmParent.NavigationTitleOverride?.Trim() ?? string.Empty;


                                if (ipParent.Children.Any())
                                {
                                    //Instantiate new child list
                                    lnkParent.LstChildLinks = new List<Link>();

                                    //Loop through all children
                                    foreach (IPublishedContent ipChild in ipParent.Children)
                                    {
                                        //Get ip navigation
                                        cm.Navigation cmChild = new Navigation(ipChild, PublishedValueFallback);


                                        if (cmChild.ShowInMainNavigation)
                                        {
                                            //Obtain link data
                                            //Link lnkChild = new Link() { Title = ipChild.Name, Url = ipChild.Url() };





                                            Link lnkChild = new Link();
                                            if (ipChild.ContentType.Alias == Common.Doctype.RedirectTo)
                                            {
                                                cm.RedirectTo cmRedirectTo_child = new RedirectTo(ipChild, PublishedValueFallback);
                                                lnkChild.Title = cmRedirectTo_child.Name;
                                                lnkChild.Url = cmRedirectTo_child.RedirectToUrl?.Url;
                                                lnkChild.Target = cmRedirectTo_child.RedirectToUrl?.Target ?? string.Empty;
                                            }
                                            else
                                            {
                                                lnkChild.Title = ipChild.Name;
                                                lnkChild.Url = ipChild.Url();
                                            }






                                            //Title override
                                            if (!string.IsNullOrEmpty(cmChild.NavigationTitleOverride?.Trim()))
                                                lnkChild.Title = cmChild.NavigationTitleOverride?.Trim() ?? string.Empty;


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
                                headerVM.LstMainNav.Add(lnkParent);
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
            return View(Common.Partial.Header, headerVM);
        }
    }
}
