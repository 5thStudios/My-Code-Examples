using System.Drawing;

namespace ECMC_Umbraco.Models
{
    public sealed class Common
    {
        #region "Properties"
        public struct Doctype
        {
            public const string AdministrativeSettings = "administrativeSettings";
            public const string ContentFolder = "contentFolder";
            public const string DocumentCardAR = "documentCardAR";
            public const string DocumentCardCN = "DocumentCardCN";
            public const string DocumentCardEcmc = "documentCardEcmc";
            public const string DocumentCardEG = "documentCardEG";
            public const string DocumentCardEIF = "documentCardEIF";
            public const string DocumentCardFnd = "documentCardFnd";
            public const string DocumentCardQTQ = "documentCardQTQ";
            public const string DocumentCardScholars = "documentCardScholars";
            public const string DocumentCardSol = "documentCardSol";
            public const string FullWidthImageCard = "fullWidthImageCard";
            public const string FullWidthImageCardComp = "fullWidthImageCardComp";
            public const string HomeAR = "homeAR";
            public const string HomeEIF = "homeEIF";
            public const string HomeScholars = "homeScholars";
            public const string IconCard = "iconCard";
            public const string IconCardCompAR = "iconCardCompAR";
            public const string IconCardCompCN = "IconCardCompCN";
            public const string IconCardCompEcmc = "iconCardCompEcmc";
            public const string IconCardCompEG = "iconCardCompEG";
            public const string IconCardCompEIF = "iconCardCompEIF";
            public const string IconCardCompFnd = "iconCardCompFnd";
            public const string IconCardCompQTQ = "iconCardCompQTQ";
            public const string IconCardCompScholars = "iconCardCompScholars";
            public const string IconCardCompSol = "iconCardCompSol";
            public const string NewsAndEventsLandingPageAR = "newsAndEventsLandingPageAR";
            public const string NewsAndEventsLandingPageEG = "newsAndEventsLandingPageEG";
            public const string NewsAndEventsLandingPageEIF = "newsAndEventsLandingPageEIF";
            public const string NewsAndEventsLandingPageFnd = "newsAndEventsLandingPageFnd";
            public const string NewsAndEventsLandingPageScholars = "newsAndEventsLandingPageScholars";
            public const string NewsAndEventsLandingPageSol = "newsAndEventsLandingPageSol";
            public const string PaddedImageCard = "paddedImageCard";
            public const string PaddedImageCardComp = "paddedImageCardComp";
            public const string PartnerCardAR = "partnerCardAR";
            public const string PartnerCardCN = "PartnerCardCN";
            public const string PartnerCardEcmc = "partnerCardEcmc";
            public const string PartnerCardQTQ = "partnerCardQTQ";
            public const string PartnerCardEG = "partnerCardEG";
            public const string PartnerCardEIF = "partnerCardEIF";
            public const string PartnerCardFnd = "partnerCardFnd";
            public const string PartnerCardScholars = "partnerCardScholars";
            public const string PartnerCardSol = "partnerCardSol";
            public const string PersonLandingPageAR = "personLandingPageAR";
            public const string PersonLandingPageEG = "personLandingPageEG";
            public const string PersonLandingPageEIF = "personLandingPageEIF";
            public const string PersonLandingPageFnd = "personLandingPageFnd";
            public const string PersonLandingPageScholars = "personLandingPageScholars";
            public const string PersonLandingPageSol = "personLandingPageSol";
            public const string RedirectTo = "redirectTo";
            public const string SearchAR = "searchAR";
            public const string SearchCN = "searchCN";
            public const string SearchEcmc = "searchEcmc";
            public const string SearchEG = "searchEG";
            public const string SearchQTQ = "searchQTQ";
            public const string SearchEIF = "searchEIF";
            public const string SearchFnd = "searchFnd";
            public const string SearchScholars = "searchScholars";
            public const string SearchSol = "searchSol";
            public const string SiteStyles = "siteStyles";
            public const string Tags = "tags";
            public const string Tooltips = "tooltips";
            public const string TitleOverride = "titleOverride";
            public const string PageDescription = "pageDescription";
            public const string HideFromIndexing = "hideFromIndexing";
            public const string KeywordList = "keywordList";
            public const string SeoImageOverride = "seoImageOverride";
        }
        public struct Action
        {
            public const string BarChart = "BarChart";
            public const string LineChart = "LineChart";
            public const string PieChart = "PieChart";
            public const string DoughnutChart = "DoughnutChart";
            public const string Chart = "Chart";
            public const string NewsTop5 = "NewsTop5";
            public const string RelatedNews = "RelatedNews";
        }
        public struct Property
        {
            public const string ChildNavigations = "childNavigations";
            public const string HideChildrenFromNavigation = "hideChildrenFromNavigation";
            public const string HideFromSearch = "hideFromSearch";
            public const string NavigationTitleOverride = "navigationTitleOverride";
            public const string PageSummary = "pageSummary";
            public const string RedirectToPage = "redirectToPage";
            public const string SidebarList = "sidebarList";
            public const string SiteSettings = "siteSettings";
            public const string TooltipId = "tooltipId";
            public const string TooltipTitle = "tooltipTitle";
            public const string TooltipText = "tooltipText";
            public const string UmbracoExtension = "umbracoExtension";
        }
        public struct ViewComponent
        {
            public const string Footer = "Footer";
            public const string Header = "Header";
            public const string HeaderMegaMenu = "HeaderMegaMenu";
            public const string MetaAndAnalytics = "MetaAndAnalytics";
            public const string Sidebar = "Sidebar";
            public const string Tooltips = "Tooltips";
        }
        public struct View
        {
            public const string ListLattusAR = "~/Views/ListLattusAR.cshtml";
            public const string ListLattusEG = "~/Views/ListLattusEG.cshtml";
            public const string ListLattusEIF = "~/Views/ListLattusEIF.cshtml";
            public const string ListLattusFnd = "~/Views/ListLattusFnd.cshtml";
            public const string ListLattusScholars = "~/Views/ListLattusScholars.cshtml";
            public const string Grants = "~/Views/Grants.cshtml";
            public const string HomeScholars = "~/Views/HomeScholars.cshtml";
            public const string HomeSol = "~/Views/HomeSol.cshtml";
            public const string HomeEcmc = "~/Views/HomeEcmc.cshtml";
            public const string HomeCN = "~/Views/HomeCN.cshtml";
            public const string NewsAndEventsLandingPageAR = "~/Views/NewsAndEventsLandingPageAR.cshtml";
            public const string NewsAndEventsLandingPageEG = "~/Views/NewsAndEventsLandingPageEG.cshtml";
            public const string NewsAndEventsLandingPageEIF = "~/Views/NewsAndEventsLandingPageEIF.cshtml";
            public const string NewsAndEventsLandingPageFnd = "~/Views/NewsAndEventsLandingPageFnd.cshtml";
            public const string NewsCardAR = "~/Views/NewsCardAR.cshtml";
            public const string NewsCardEG = "~/Views/NewsCardEG.cshtml";
            public const string NewsCardEIF = "~/Views/NewsCardEIF.cshtml";
            public const string NewsCardFnd = "~/Views/NewsCardFnd.cshtml";
            public const string NewsCardQTQ = "~/Views/NewsCardQTQ.cshtml";
            public const string NewsCardScholars = "~/Views/NewsCardScholars.cshtml";
            public const string NewsCardSol = "~/Views/NewsCardSol.cshtml";
            public const string NewsCardEcmc = "~/Views/NewsCardEcmc.cshtml";
            public const string NewsCardCN = "~/Views/NewsCardCN.cshtml";
            public const string NewsCategoryAR = "~/Views/NewsCategoryAR.cshtml";
            public const string NewsCategoryEG = "~/Views/NewsCategoryEG.cshtml";
            public const string NewsCategoryEIF = "~/Views/NewsCategoryEIF.cshtml";
            public const string NewsCategoryFnd = "~/Views/NewsCategoryFnd.cshtml";
            public const string NewsCategoryQTQ = "~/Views/NewsCategoryQTQ.cshtml";
            public const string NewsCategoryScholars = "~/Views/NewsCategoryScholars.cshtml";
            public const string NewsCategorySol = "~/Views/NewsCategorySol.cshtml";
            public const string NewsCategoryEcmc = "~/Views/NewsCategoryEcmc.cshtml";
            public const string NewsCategoryCN = "~/Views/NewsCategoryCN.cshtml";
            public const string NewsLattusAR = "~/Views/NewsLattusAR.cshtml";
            public const string NewsLattusEG = "~/Views/NewsLattusEG.cshtml";
            public const string NewsLattusEIF = "~/Views/NewsLattusEIF.cshtml";
            public const string NewsLattusFnd = "~/Views/NewsLattusFnd.cshtml";
            public const string NewsLattusQTQ = "~/Views/NewsLattusQTQ.cshtml";
            public const string NewsLattusScholars = "~/Views/NewsLattusScholars.cshtml";
            public const string NewsLattusSol = "~/Views/NewsLattusSol.cshtml";
            public const string NewsLattusEcmc = "~/Views/NewsLattusEcmc.cshtml";
            public const string NewsLattusCN = "~/Views/NewsLattusCN.cshtml";
            public const string PersonLandingPageAR = "~/Views/PersonLandingPageAR.cshtml";
            public const string PersonLandingPageEG = "~/Views/PersonLandingPageEG.cshtml";
            public const string PersonLandingPageEIF = "~/Views/PersonLandingPageEIF.cshtml";
            public const string PersonLandingPageFnd = "~/Views/PersonLandingPageFnd.cshtml";
            public const string SearchAR = "~/Views/SearchAR.cshtml";
            public const string SearchEG = "~/Views/SearchEG.cshtml";
            public const string SearchEIF = "~/Views/SearchEIF.cshtml";
            public const string SearchFnd = "~/Views/SearchFnd.cshtml";
            public const string SearchQTQ = "~/Views/SearchQTQ.cshtml";
            public const string SearchScholars = "~/Views/SearchScholars.cshtml";
            public const string SearchSol = "~/Views/SearchSol.cshtml";
            public const string SearchEcmc = "~/Views/SearchEcmc.cshtml";
            public const string SearchCN = "~/Views/SearchCN.cshtml";
            public const string SectionScholars = "~/Views/SectionScholars.cshtml";
            public const string SectionSol = "~/Views/SectionSol.cshtml";
            public const string SectionCN = "~/Views/SectionCN.cshtml";
            public const string SectionEcmc = "~/Views/SectionEcmc.cshtml";
        }
        public struct Partial
        {
            public const string AccordionItemA = "~/Views/Partials/ListItems/accordionItemA.cshtml";
            public const string AccordionItemB = "~/Views/Partials/ListItems/accordionItemB.cshtml";
            public const string BarChart = "~/Views/Partials/blockgrid/Components/Charts/BarChart.cshtml";
            public const string DoughnutChart = "~/Views/Partials/blockgrid/Components/Charts/DoughnutChart.cshtml";
            public const string Filter = "~/Views/Partials/Lists/Filter.cshtml";
            public const string Footer_A = "~/Views/Partials/Common/Footer-A.cshtml";
            public const string Footer_B = "~/Views/Partials/Common/Footer-B.cshtml";
            public const string Footer_AR = "~/Views/Partials/Common/Footer-AR.cshtml";
            public const string GrantsFilter = "~/Views/Partials/Lists/GrantsFilter.cshtml";
            public const string GrantsPagination = "~/Views/Partials/Lists/GrantsPagination.cshtml";
            public const string FullWidthImgCard = "~/Views/Partials/ListItems/FullWidthImgCard.cshtml";
            public const string Header = "~/Views/Partials/Common/Header.cshtml";
            public const string HeaderMbl = "~/Views/Partials/Common/Header-Mbl.cshtml";
            public const string HeaderMegaMenu = "~/Views/Partials/Common/HeaderMegaMenu.cshtml";
            public const string IconCard_GridView = "~/Views/Partials/ListItems/IconCard_GridView.cshtml";
            public const string IconCard_ListView = "~/Views/Partials/ListItems/IconCard_ListView.cshtml";
            public const string ImageCardFixedSizeWithSideText = "~/Views/Partials/ListItems/ImageCardFixedSizeWithSideText.cshtml";
            public const string ImageCardWithSideText = "~/Views/Partials/ListItems/ImageCardWithSideText.cshtml";
            public const string LattusList_GridView = "~/Views/Partials/Lists/LattusList_GridView.cshtml";
            public const string LattusList_ListView = "~/Views/Partials/Lists/LattusList_ListView.cshtml";
            public const string LineChart = "~/Views/Partials/blockgrid/Components/Charts/LineChart.cshtml";
            public const string MetaAndAnalytics = "~/Views/Partials/Common/MetaAndAnalytics.cshtml";
            public const string NewsAndEventCard_GridView = "~/Views/Partials/ListItems/NewsAndEventCard_GridView.cshtml";
            public const string NewsAndEventCard_ListView = "~/Views/Partials/ListItems/NewsAndEventCard_ListView.cshtml";
            public const string NewsCard_GridView = "~/Views/Partials/ListItems/NewsCard_GridView.cshtml";
            public const string NewsCard_ListView = "~/Views/Partials/ListItems/NewsCard_ListView.cshtml";
            public const string NewsTop5List = "~/Views/Partials/Lists/NewsTop5List.cshtml";
            public const string PaddedImgCard = "~/Views/Partials/ListItems/PaddedImgCard.cshtml";
            public const string PersonCard_GridView = "~/Views/Partials/ListItems/PersonCard_GridView.cshtml";
            public const string PersonCard_ListView = "~/Views/Partials/ListItems/PersonCard_ListView.cshtml";
            public const string PieChart = "~/Views/Partials/blockgrid/Components/Charts/PieChart.cshtml";
            public const string PolarChart = "~/Views/Partials/blockgrid/Components/Charts/PolarChart.cshtml";
            public const string RadarChart = "~/Views/Partials/blockgrid/Components/Charts/RadarChart.cshtml";
            public const string RelatedNewsList = "~/Views/Partials/Lists/RelatedNewsList.cshtml";
            public const string Sidebar = "~/Views/Partials/Lists/Sidebar.cshtml";
            public const string Tooltips = "~/Views/Partials/Common/Tooltips.cshtml";

            public const string SecondaryNav = "~/Views/Partials/Common/SecondaryNav.cshtml";
            public const string SecondaryNavMbl = "~/Views/Partials/Common/SecondaryNav-Mbl.cshtml";


            public const string DocumentCard_GridView = "~/Views/Partials/ListItems/DocumentCard_GridView.cshtml";
            public const string DocumentCard_ListView = "~/Views/Partials/ListItems/DocumentCard_ListView.cshtml";
            public const string PartnerCard_GridView = "~/Views/Partials/ListItems/PartnerCard_GridView.cshtml";
            public const string PartnerCard_ListView = "~/Views/Partials/ListItems/PartnerCard_ListView.cshtml";
        }
        public struct Session
        {
            public const string Success = "Success";
        }
        public struct Query
        {
            public const string Search = "search";
        }
        public struct Crop
        {
            public const string Card_100x130 = "Card_100x130";
            public const string Card_500x253 = "Card_500x253";
            public const string Hero_1903x = "Hero_1903x";
            public const string Hero_1903x1000 = "Hero_1903x1000";
            public const string Hero_1903x500 = "Hero_1903x500";
            public const string Hero_1903x600 = "Hero_1903x600";
            public const string Hero_1903x700 = "Hero_1903x700";
            public const string Hero_1903x800 = "Hero_1903x800";
            public const string Hero_1903x900 = "Hero_1903x900";
            public const string Icon_Card_575x375 = "Icon_Card_575x375";
            public const string LearnMore_500x550 = "LearnMore_500x550";
            public const string Slide_1600x900 = "Slide_1600x900";
            public const string Staff_525x500 = "Staff_525x500";

            public const string Landscape_1440x810 = "Landscape_1440x810";
            public const string Portrait_810x1440 = "Portrait_810x1440";
            public const string Landscape_1440x1080 = "Landscape_1440x1080";
            public const string Portrait_1080x1440 = "Portrait_1080x1440";
            public const string Square_1440x1440 = "Square_1440x1440";

            public const string SEO_1200x630 = "SEO_1200x630";
        }
        public struct TagGroup
        {
            public const string AreaOfInterest = "Area of Interest";
            public const string Audience = "Audience";
            public const string Staff = "Staff";
        }
        public struct Miscellaneous
        {
            public const string DomainName = "DomainName";
        }
        #endregion


        #region "Methods"
        public static string ConvertHexToRGB(string hexValue, decimal opacity = 1)
        {
            //Insure hex string contains #
            if (!hexValue.Contains("#"))
                hexValue = "#" + hexValue;

            Color color = ColorTranslator.FromHtml(hexValue);
            int r = Convert.ToInt16(color.R);
            int g = Convert.ToInt16(color.G);
            int b = Convert.ToInt16(color.B);
            return string.Format("rgba({0}, {1}, {2}, {3})", r, g, b, opacity);
        }
        public static string ConvertRowPadding(string prevalue)
        {
            return "padding-" + prevalue.ToLower().Replace(" ", "-");
        }
        public static string ConvertRowMargin(string prevalue)
        {
            return "margin-" + prevalue.ToLower().Replace(" ", "-");
        }
        #endregion
    }
}
