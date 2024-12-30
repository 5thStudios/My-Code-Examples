using System.Drawing;
using System.Text.RegularExpressions;

namespace www.Models
{
    public sealed class Common
    {
        #region "Properties"
        public struct Doctype
        {
            public const string Blog = "blog";
            public const string Category = "category";
            public const string Common = "common";
            public const string Footer = "footer";
            public const string Home = "home";
            public const string LandingPage = "landingPage";
            public const string Post = "post";
            public const string RedirectPage = "redirectPage";
            public const string SiteSettings = "siteSettings";
            public const string SocialMedia = "socialMedia";
            public const string Standard = "standard";
        }
        public struct RedirectTo
        {
            public const string RedirectToUrl = "Redirect To Url";
            public const string ToFirstChild = "To First Child";
            public const string ToParent = "To Parent";
            public const string ToHome = "To Home";
        }
        public struct Action
        {
            public const string xxx = "";
        }
        public struct Property
        {
            public const string AltText = "altText";
            public const string Author = "author";
            public const string BlogTitle = "blogTitle";
            public const string Content = "content";
            public const string Excerpt = "excerpt";
            public const string NavigationExtraClasses = "navigationExtraClasses";
            public const string NavigationTitleOverride = "navigationTitleOverride";
            public const string PostImage = "postImage";
            public const string PublishedDate = "publishedDate";
            public const string ShowInMainNavigation = "showInMainNavigation";
            public const string UmbracoNaviHide = "umbracoNaviHide";
        }
        public struct ViewComponent
        {
            public const string Footer = "Footer";
            public const string Header = "Header";
            public const string Teaser = "Teaser";
        }
        public struct View
        {
            public const string Blog = "~/Views/Blog.cshtml";
            public const string Category = "~/Views/Category.cshtml";
            public const string EfficiencySchedule = "~/Views/EfficiencySchedule.cshtml";
            public const string Home = "~/Views/Home.cshtml";
            public const string Post = "~/Views/Post.cshtml";
            public const string RedirectPage = "~/Views/RedirectPage.cshtml";
            public const string Standard = "~/Views/Standard.cshtml";
        }
        public struct Partial
        {
            public const string BvTeaser = "~/Views/Partials/blockViews/bvTeaser.cshtml";
            public const string Footer = "~/Views/Partials/Common/Footer.cshtml";
            public const string Header = "~/Views/Partials/Common/Header.cshtml";
            public const string HeroBillboard = "~/Views/Partials/Heroes/Hero-Billboard.cshtml";
            public const string HeroHome = "~/Views/Partials/Heroes/Hero-Home.cshtml";

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
            public const string Portrait_600x400 = "Portrait_600x400";
            public const string Square_500x500 = "Square_500x500";
            public const string Square_600x600 = "Square_600x600";
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
        public static string RemoveNonAlphaChar(string Text)
        {
            return Regex.Replace(Text, "[^a-zA-Z0-9]", String.Empty);
        }
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
                return prevalue.Replace(" ", "-").Replace("_", "-");
        }
        #endregion
    }
}
