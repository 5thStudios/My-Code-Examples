using Umbraco.Cms.Core.Models.PublishedContent;
using www.Models.PublishedModels;
using cm = www.Models.PublishedModels;



namespace www.Models
{
    public sealed class Common
    {
        #region "Properties"
        public struct Doctype
        {
            public const string Bio = "bio";
            public const string HomePg = "homePg";
            public const string ListOfHomePgs = "listOfHomePgs";
            //public const string SiteSettings = "siteSettings";
            public const string RedirectTo = "redirectTo";
            public const string ProjectPage = "projectPage";
        }
        public struct Property
        {
            public const string DatePosted = "datePosted";
            public const string FooterLocations = "footerLocations";
            public const string FooterLogo = "footerLogo";
            public const string FooterMinorNav = "footerMinorNav";
            public const string FooterParagraph = "footerParagraph";
            public const string LoginLinks = "loginLinks";
            //public const string MetaDescription = "MetaDescription";
            //public const string MetaKeywords = "MetaKeywords";
            public const string MetaTitle = "MetaTitle";
            public const string NavigationTitleOverride = "navigationTitleOverride";
            public const string ShowInFooterMainNav = "showInFooterMainNav";
            public const string ShowInMainNavigation = "showInMainNavigation";
            public const string ShowInTopMinorNav = "showInTopMinorNav";

            public const string SeoChecker = "sEOChecker";
            public const string SeoDescription = "sEODescription";
            public const string MetaRobots = "metaRobots";
            public const string Redirects = "redirects";
            public const string XmlSitemap = "xMLSitemap";


            public const string Site = "site";
            public const string SiteCanonicalDomain = "SiteCanonicalDomain";
            public const string SiteDescription = "siteDescription";
            public const string SiteLogo = "siteLogo";
            //public const string SiteMetaTitleFormat = "SiteMetaTitleFormat";
            public const string SiteTagline = "siteTagline";
            public const string SiteTitle = "siteTitle";

            public const string SocialLinks = "socialLinks";
            public const string OpenGraphDescription = "OpenGraphDescription";
            public const string OpenGraphImage = "OpenGraphImage";
            public const string OpenGraphTitle = "OpenGraphTitle";
            public const string OpenGraphType = "OpenGraphType";
        }
        public struct ViewComponent
        {
            public const string Footer = "Footer";
            public const string Header = "Header";
            public const string Hero = "Hero";
            public const string Meta = "Meta";
        }
        public struct View
        {
            public const string Blog = "~/Views/Blog.cshtml";
            public const string BlogPost = "~/Views/BlogPost.cshtml";
            public const string Home = "~/Views/Home.cshtml";
            public const string ListOfHomePgs = "~/Views/ListOfHomePgs.cshtml";
            public const string ProjectLists = "~/Views/ProjectLists.cshtml";
            public const string ProjectPage = "~/Views/ProjectPage.cshtml";
        }
        public struct Partial
        {
            public const string Footer = "~/Views/Partials/Common/Footer.cshtml";
            public const string Header = "~/Views/Partials/Common/Header.cshtml";
            public const string Hero = "~/Views/Partials/Common/Hero.cshtml";

            public const string BioCard = "~/Views/Partials/Cards/BioCard.cshtml";
            public const string LeadGeneratorCard = "~/Views/Partials/Cards/LeadGeneratorCard.cshtml";
            public const string LocationCard = "~/Views/Partials/Cards/LocationCard.cshtml";
            public const string MetaAndAnalytics = "~/Views/Partials/Common/MetaAndAnalytics.cshtml";
            public const string NewsCard = "~/Views/Partials/Cards/NewsCard.cshtml";
            public const string ProjectCard = "~/Views/Partials/Cards/ProjectCard.cshtml";
            public const string ReadMoreCard = "~/Views/Partials/Cards/ReadMoreCard.cshtml";

            public const string GridOutline = "~/Views/Partials/Styles/GridOutline.cshtml";
            public const string ZurbTestPg = "~/Views/Partials/Styles/ZurbTestPg.cshtml";

        }
        public struct Session
        {
            //public const string Success = "Success";
        }
        public struct Query
        {
            public const string Search = "search";
            public const string Category = "category";
        }
        public struct Crop
        {
            public const string BlogThumbnail_660x435 = "BlogThumbnail_660x435";
            public const string Card_1100x600 = "Card_1100x600";
            public const string Card_1000x1000 = "Card_1000x1000";
            public const string Hero_1903x350 = "Hero_1903x350";
            public const string Hero_1903x435 = "Hero_1903x435";
            public const string Project_1903x925 = "Project_1903x925";
        }
        #endregion



        #region "Methods"
        public static string ConvertRowSpacing(string incomingValue)
        {
            switch (incomingValue)
            {
                case "Add Padding- Horizontal":
                    return "grid-padding-x";
                case "Add Margin- Horizontal":
                    return "grid-margin-x";
                case "Add Padding- Vertical":
                    return "grid-padding-y";
                case "Add Margin- Vertical":
                    return "grid-margin-y";
                default:
                    return "";
            }
        }
        public static string ConvertBgColorClass(string prevalue)
        {
            if (prevalue == "None")
                return string.Empty;
            else
                return "bg-" + prevalue.ToLower().Replace(" ", "-");
        }
        public static string ConvertToClass(string prevalue)
        {
            prevalue = prevalue.ToLower();

            if (prevalue == "none")
                return string.Empty;
            else
                return prevalue.Replace(" ", "-").Replace("_", "-").Replace("&", "-").Replace("/", "-");
        }

        public static cm.HomePg? ObtainHomePg(IPublishedContent ipCurrentPg, IPublishedValueFallback _publishedValueFallback)
        {
            if (ipCurrentPg.ContentType.Alias == Doctype.ListOfHomePgs)
                return null;
            else
                return new cm.HomePg(ipCurrentPg.AncestorOrSelf(Doctype.HomePg), _publishedValueFallback);
        }
        public static cm.SiteSettings? ObtainSettingsPg(IPublishedContent ipCurrentPg, IPublishedValueFallback _publishedValueFallback)
        {
            cm.HomePg? cmHome = ObtainHomePg(ipCurrentPg, _publishedValueFallback);

            if (cmHome == null)
                return null;
            else
            {
                return new cm.SiteSettings(cmHome, _publishedValueFallback);
            }
        }
        #endregion
    }
}
