﻿using ECMC_Umbraco.Models;
using ECMC_Umbraco.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;
using UmbracoProject.Models;
using ContentModels = Umbraco.Cms.Web.Common.PublishedModels;

namespace www.Controllers
{
    public class ListLattusScholarsController : RenderController
    {
        private IPublishedValueFallback _publishedValueFallback;
        private UmbracoHelper UmbracoHelper;
        private readonly ILogger<ListLattusScholarsController> logger;
        private readonly ServiceContext _serviceContext;
        private readonly IVariationContextAccessor _variationContextAccessor;

        public ListLattusScholarsController(
                ILogger<ListLattusScholarsController> _logger,
                ICompositeViewEngine compositeViewEngine,
                IUmbracoContextAccessor umbracoContextAccessor,
                UmbracoHelper _UmbracoHelper,
                IPublishedValueFallback publishedValueFallback,
                ServiceContext context,
                IVariationContextAccessor variationContextAccessor
             )
            : base(_logger, compositeViewEngine, umbracoContextAccessor)
        {
            _publishedValueFallback = publishedValueFallback;
            UmbracoHelper = _UmbracoHelper;
            logger = _logger;
            _serviceContext = context;
            _variationContextAccessor = variationContextAccessor;
        }

        public override IActionResult Index()
        {
            //Instantiate variables
            var cmPage = new ListLattusScholars(CurrentPage, _publishedValueFallback);
            ListViewModel lstVmodel = new ListViewModel();

            try
            {
                //Loop through children
                if (cmPage.Children.Any())
                {
                    foreach (var ipChild in cmPage.Children)
                    {
                        //Instantiate 
                        ListItemViewModel item = new ListItemViewModel();

                        switch (ipChild.ContentType.Alias)
                        {
                            //case Common.Doctype.PartnerCardScholars:
                            //    item = RenderCard_PartnerLandingPg(ipChild);
                            //    break;


                            //case Common.Doctype.PersonLandingPageScholars:
                            //    item = RenderCard_PersonLandingPg(ipChild);
                            //    break;


                            //case Common.Doctype.DocumentCardScholars:
                            //    item = RenderCard_DocumentLandingPg(ipChild);
                            //    break;


                            //case Common.Doctype.NewsAndEventsLandingPageScholars:
                            //    item = RenderCard_NewsAndEventsLandingPg(ipChild);
                            //    break;


                            //case Common.Doctype.IconCardCompScholars:
                            //    item = RenderCard_IconCard(ipChild);
                            //    break;


                            case Common.Doctype.PaddedImageCardComp:
                                item = RenderCard_PaddedImg(ipChild);
                                break;



                            default:
                                break;
                        }

                        lstVmodel.LstListItems.Add(item);

                    }

                    //Obtain filters
                    lstVmodel.filterViewModel = new ListFilters().GenerateFilters(lstVmodel, CurrentPage!);

                }


                //Is search panel to be shown?
                lstVmodel.filterViewModel.ShowSearchPnl = cmPage.ShowSearchPanel;
                lstVmodel.filterViewModel.ShowViewSelector = cmPage.ShowViewSelector;
                if (lstVmodel.filterViewModel.ShowSearchPnl) lstVmodel.filterViewModel.FilterCount++;


                //Show pagination ONLY if it is needed
                if (lstVmodel.LstListItems != null)
                {
                    if (cmPage.PaginationCount > 0 && lstVmodel.LstListItems.Count() > cmPage.PaginationCount)
                    {
                        lstVmodel.ShowPagination = true;
                        lstVmodel.PaginationCount = cmPage.PaginationCount;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }


            var viewModel = new ComposedPageViewModel<ListLattusScholars, ListViewModel>
            {
                Page = cmPage,
                ViewModel = lstVmodel
            };

            return View(Common.View.ListLattusScholars, viewModel);
        }



        //private ListItemViewModel RenderCard_PartnerLandingPg(IPublishedContent ipChild)
        //{
        //    //Instantiate variables
        //    ListItemViewModel item = new ListItemViewModel();

        //    //Obtain model vsn of child node
        //    ContentModels.PartnerCardScholars cmPartnerLattice = new ContentModels.PartnerCardScholars(ipChild, new PublishedValueFallback(_serviceContext, _variationContextAccessor));

        //    //Obtain all data from model
        //    item.Id = cmPartnerLattice.Id;
        //    item.DocType = cmPartnerLattice.ContentType.Alias;
        //    item.Title = cmPartnerLattice.Title ?? "";

        //    if (cmPartnerLattice.Subtitle != null)
        //        item.Subtitle = cmPartnerLattice.Subtitle;

        //    if (cmPartnerLattice.Summary != null)
        //    {
        //        item.Summary = cmPartnerLattice.Summary;
        //        item.EncodedSummary = HttpUtility.HtmlEncode(item.Summary.ToLower());
        //    }

        //    //item.PrimaryImgUrl = cmPartnerLattice.PrimaryImage?.GetCropUrl(Common.Crop.Staff_525x500);
        //    item.PrimaryImgUrl = cmPartnerLattice.PrimaryImage?.GetCropUrl(500) + "&format=webp";

        //    if (cmPartnerLattice.AreasOfInterest != null)
        //    {
        //        foreach (var i in cmPartnerLattice.AreasOfInterest)
        //            item.LstAreasOfInterest.Add(i.Name);
        //        item.jsonAreasOfInterest = Newtonsoft.Json.JsonConvert.SerializeObject(item.LstAreasOfInterest);
        //    }

        //    if (cmPartnerLattice.Audience != null)
        //    {
        //        foreach (var i in cmPartnerLattice.Audience)
        //            item.LstAudience.Add(i.Name);
        //        item.jsonAudience = Newtonsoft.Json.JsonConvert.SerializeObject(item.LstAudience);
        //    }

        //    if (cmPartnerLattice.PartnerUrl != null)
        //    {
        //        item.Link = new Link()
        //        {
        //            Url = cmPartnerLattice.PartnerUrl?.Url ?? "",
        //            Target = cmPartnerLattice.PartnerUrl?.Target ?? "",
        //            Title = cmPartnerLattice.PartnerUrl?.Name ?? "Learn More"
        //        };
        //    }

        //    item.InViewAnimation = cmPartnerLattice.InViewAnimation;

        //    if (!string.IsNullOrWhiteSpace(cmPartnerLattice.HoverTitle))
        //    {
        //        item.HoverTitle = cmPartnerLattice.HoverTitle;
        //        item.ShowHoverContent = true;
        //    }
        //    if (cmPartnerLattice.HoverTip != null && !string.IsNullOrWhiteSpace(cmPartnerLattice.HoverTip.ToString()))
        //    {
        //        item.HoverTip = cmPartnerLattice.HoverTip;
        //        item.ShowHoverContent = true;
        //    }

        //    //Determine if card content should appear
        //    if (!String.IsNullOrEmpty(cmPartnerLattice.Title) || !String.IsNullOrEmpty(cmPartnerLattice.Subtitle))
        //        item.ShowCardContent = true;


        //    return item;
        //}

        //private ListItemViewModel RenderCard_PersonLandingPg(IPublishedContent ipChild)
        //{
        //    //Instantiate variables
        //    ListItemViewModel item = new ListItemViewModel();

        //    //Obtain model vsn of child node
        //    ContentModels.PersonLandingPageScholars cmPersonLandingPg = new ContentModels.PersonLandingPageScholars(ipChild, new PublishedValueFallback(_serviceContext, _variationContextAccessor));

        //    //Obtain all data from model
        //    item.Id = cmPersonLandingPg.Id;
        //    item.DocType = cmPersonLandingPg.ContentType.Alias;
        //    item.Title = cmPersonLandingPg.Title ?? "";

        //    item.Link = new Link()
        //    {
        //        Url = cmPersonLandingPg.Url()
        //    };

        //    if (cmPersonLandingPg.Subtitle != null)
        //        item.Subtitle = cmPersonLandingPg.Subtitle;

        //    if (cmPersonLandingPg.Summary != null)
        //    {
        //        item.Summary = cmPersonLandingPg.Summary;
        //        item.EncodedSummary = HttpUtility.HtmlEncode(item.Summary.ToLower());
        //    }

        //    item.PrimaryImgUrl = cmPersonLandingPg.PrimaryImage?.GetCropUrl(Common.Crop.Staff_525x500);

        //    if (cmPersonLandingPg.AreasOfInterest != null)
        //    {
        //        foreach (var i in cmPersonLandingPg.AreasOfInterest)
        //            item.LstAreasOfInterest.Add(i.Name);
        //        item.jsonAreasOfInterest = Newtonsoft.Json.JsonConvert.SerializeObject(item.LstAreasOfInterest);
        //    }

        //    if (cmPersonLandingPg.Staff != null)
        //    {
        //        foreach (var i in cmPersonLandingPg.Staff)
        //            item.LstStaff.Add(i.Name);
        //        item.jsonStaff = Newtonsoft.Json.JsonConvert.SerializeObject(item.LstStaff);
        //    }

        //    item.InViewAnimation = cmPersonLandingPg.InViewAnimation;

        //    if (!string.IsNullOrWhiteSpace(cmPersonLandingPg.HoverTitle))
        //    {
        //        item.HoverTitle = cmPersonLandingPg.HoverTitle;
        //        item.ShowHoverContent = true;
        //    }
        //    if (cmPersonLandingPg.HoverTip != null && !string.IsNullOrWhiteSpace(cmPersonLandingPg.HoverTip.ToString()))
        //    {
        //        item.HoverTip = cmPersonLandingPg.HoverTip;
        //        item.ShowHoverContent = true;
        //    }

        //    //Determine if card content should appear
        //    if (!String.IsNullOrEmpty(cmPersonLandingPg.Title) || !String.IsNullOrEmpty(cmPersonLandingPg.Subtitle) || !string.IsNullOrEmpty(cmPersonLandingPg.LinkedIn) || !string.IsNullOrEmpty(cmPersonLandingPg.Facebook))
        //        item.ShowCardContent = true;


        //    if (!string.IsNullOrEmpty(cmPersonLandingPg.LinkedIn))
        //        item.LinkedInUrl = cmPersonLandingPg.LinkedIn;

        //    if (!string.IsNullOrEmpty(cmPersonLandingPg.Facebook))
        //        item.FacebookUrl = cmPersonLandingPg.Facebook;



        //    return item;
        //}

        //private ListItemViewModel RenderCard_DocumentLandingPg(IPublishedContent ipChild)
        //{
        //    //Instantiate variables
        //    ListItemViewModel item = new ListItemViewModel();

        //    //Obtain model vsn of child node
        //    ContentModels.DocumentCardScholars cmDocumentLanding = new ContentModels.DocumentCardScholars(ipChild, new PublishedValueFallback(_serviceContext, _variationContextAccessor));

        //    //Obtain all data from model
        //    item.Id = cmDocumentLanding.Id;
        //    item.DocType = cmDocumentLanding.ContentType.Alias;
        //    item.Title = cmDocumentLanding.Title ?? "";

        //    if (cmDocumentLanding.Subtitle != null)
        //        item.Subtitle = cmDocumentLanding.Subtitle;

        //    if (cmDocumentLanding.Summary != null)
        //    {
        //        item.Summary = cmDocumentLanding.Summary;
        //        item.EncodedSummary = HttpUtility.HtmlEncode(item.Summary);
        //    }

        //    item.PrimaryImgUrl = cmDocumentLanding.PrimaryImage?.GetCropUrl(Common.Crop.Staff_525x500);

        //    if (cmDocumentLanding.Audience != null)
        //    {
        //        foreach (var i in cmDocumentLanding.Audience)
        //            item.LstAudience.Add(i.Name);
        //        item.jsonAudience = Newtonsoft.Json.JsonConvert.SerializeObject(item.LstAudience);
        //    }

        //    if (cmDocumentLanding.Document != null)
        //    {
        //        item.Link = new Link()
        //        {
        //            Url = cmDocumentLanding.Document.Url() ?? ""
        //        };
        //    }

        //    item.InViewAnimation = cmDocumentLanding.InViewAnimation;

        //    if (!string.IsNullOrWhiteSpace(cmDocumentLanding.HoverTitle))
        //    {
        //        item.HoverTitle = cmDocumentLanding.HoverTitle;
        //        item.ShowHoverContent = true;
        //    }
        //    if (cmDocumentLanding.HoverTip != null && !string.IsNullOrWhiteSpace(cmDocumentLanding.HoverTip.ToString()))
        //    {
        //        item.HoverTip = cmDocumentLanding.HoverTip;
        //        item.ShowHoverContent = true;
        //    }

        //    //Determine if card content should appear
        //    if (!String.IsNullOrEmpty(cmDocumentLanding.Title) || !String.IsNullOrEmpty(cmDocumentLanding.Subtitle))
        //        item.ShowCardContent = true;


        //    return item;
        //}

        //private ListItemViewModel RenderCard_NewsAndEventsLandingPg(IPublishedContent ipChild)
        //{
        //    //Instantiate variables
        //    ListItemViewModel item = new ListItemViewModel();

        //    //Obtain model vsn of child node
        //    ContentModels.NewsAndEventsLandingPageScholars cmNewsAndEventsLandingPage = new ContentModels.NewsAndEventsLandingPageScholars(ipChild, new PublishedValueFallback(_serviceContext, _variationContextAccessor));

        //    //Obtain all data from model
        //    item.Id = cmNewsAndEventsLandingPage.Id;
        //    item.DocType = cmNewsAndEventsLandingPage.ContentType.Alias;
        //    item.Title = cmNewsAndEventsLandingPage.Title ?? "";
        //    item.Link = new Link()
        //    {
        //        Url = cmNewsAndEventsLandingPage.Url()
        //    };

        //    if (cmNewsAndEventsLandingPage.Subtitle != null)
        //        item.Subtitle = cmNewsAndEventsLandingPage.Subtitle;

        //    if (cmNewsAndEventsLandingPage.Summary != null)
        //    {
        //        item.Summary = cmNewsAndEventsLandingPage.Summary;
        //        item.EncodedSummary = HttpUtility.HtmlEncode(item.Summary.ToLower());
        //    }


        //    if (cmNewsAndEventsLandingPage.PostDate != null)
        //    {
        //        item.PostDate = cmNewsAndEventsLandingPage.PostDate;
        //    }


        //    //Determine if image is cropped or not
        //    if (cmNewsAndEventsLandingPage.NoCrop)
        //    {
        //        item.PrimaryImgUrl = cmNewsAndEventsLandingPage.PrimaryImage?.Url() + "?format=webp";
        //        item.AdditionalClasses = "no-crop-vsn";
        //    }
        //    else
        //    {
        //        item.PrimaryImgUrl = cmNewsAndEventsLandingPage.PrimaryImage?.GetCropUrl(Common.Crop.LearnMore_500x550) + "&format=webp";
        //    }


        //    if (cmNewsAndEventsLandingPage.AreasOfInterest != null)
        //    {
        //        foreach (var i in cmNewsAndEventsLandingPage.AreasOfInterest)
        //            item.LstAreasOfInterest.Add(i.Name);
        //        item.jsonAreasOfInterest = Newtonsoft.Json.JsonConvert.SerializeObject(item.LstAreasOfInterest);
        //    }

        //    if (cmNewsAndEventsLandingPage.Staff != null)
        //    {
        //        foreach (var i in cmNewsAndEventsLandingPage.Staff)
        //            item.LstStaff.Add(i.Name);
        //        item.jsonStaff = Newtonsoft.Json.JsonConvert.SerializeObject(item.LstStaff);
        //    }

        //    item.InViewAnimation = cmNewsAndEventsLandingPage.InViewAnimation;

        //    if (!string.IsNullOrWhiteSpace(cmNewsAndEventsLandingPage.HoverTitle))
        //    {
        //        item.HoverTitle = cmNewsAndEventsLandingPage.HoverTitle;
        //        item.ShowHoverContent = true;
        //    }
        //    if (cmNewsAndEventsLandingPage.HoverTip != null && !string.IsNullOrWhiteSpace(cmNewsAndEventsLandingPage.HoverTip.ToString()))
        //    {
        //        item.HoverTip = cmNewsAndEventsLandingPage.HoverTip;
        //        item.ShowHoverContent = true;
        //    }

        //    //Determine if card content should appear
        //    if (!string.IsNullOrWhiteSpace(cmNewsAndEventsLandingPage.PostDate) ||
        //        !string.IsNullOrWhiteSpace(cmNewsAndEventsLandingPage.Title) ||
        //        (cmNewsAndEventsLandingPage.Summary != null && !string.IsNullOrWhiteSpace(cmNewsAndEventsLandingPage.Summary.ToString())))
        //    {
        //        item.ShowCardContent = true;
        //    }
        //    //if (!String.IsNullOrEmpty(cmNewsAndEventsLandingPage.Title) || !String.IsNullOrEmpty(cmNewsAndEventsLandingPage.Subtitle))
        //    //    item.ShowCardContent = true;


        //    return item;
        //}

        //private ListItemViewModel RenderCard_IconCard(IPublishedContent ipChild)
        //{
        //    //Instantiate variables
        //    ListItemViewModel item = new ListItemViewModel();

        //    //Obtain model vsn of child node
        //    ContentModels.IconCardCompScholars cmItem = new ContentModels.IconCardCompScholars(ipChild, new PublishedValueFallback(_serviceContext, _variationContextAccessor));

        //    //Obtain all data from model
        //    item.DocType = cmItem.ContentType.Alias;
        //    item.IconCard = new IconCard(ipChild, new PublishedValueFallback(_serviceContext, _variationContextAccessor));

        //    return item;
        //}

        private ListItemViewModel RenderCard_PaddedImg(IPublishedContent ipChild)
        {
            //Instantiate variables
            ListItemViewModel item = new ListItemViewModel();

            //Obtain model vsn of child node
            ContentModels.PaddedImageCard cmItem = new ContentModels.PaddedImageCard(ipChild, new PublishedValueFallback(_serviceContext, _variationContextAccessor));

            //Obtain all data from model
            item.DocType = cmItem.ContentType.Alias;
            item.PaddedImageCard = new PaddedImageCard(ipChild, new PublishedValueFallback(_serviceContext, _variationContextAccessor));

            return item;
        }




    }
}