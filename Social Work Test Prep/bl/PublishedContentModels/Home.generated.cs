//------------------------------------------------------------------------------
// <auto-generated>
//   This code was generated by a tool.
//
//    Umbraco.ModelsBuilder v8.1.6
//
//   Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.ModelsBuilder;
using Umbraco.ModelsBuilder.Umbraco;

namespace Umbraco.Web.PublishedModels
{
	/// <summary>Home</summary>
	[PublishedModel("home")]
	public partial class Home : PublishedContentModel, INavigation, ISEO
	{
		// helpers
#pragma warning disable 0109 // new is redundant
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		public new const string ModelTypeAlias = "home";
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		public new static IPublishedContentType GetModelContentType()
			=> PublishedModelUtility.GetModelContentType(ModelItemType, ModelTypeAlias);
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		public static IPublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<Home, TValue>> selector)
			=> PublishedModelUtility.GetModelPropertyType(GetModelContentType(), selector);
#pragma warning restore 0109

		// ctor
		public Home(IPublishedContent content)
			: base(content)
		{ }

		// properties

		///<summary>
		/// Feature 01
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		[ImplementPropertyType("feature01")]
		public IHtmlString Feature01 => this.Value<IHtmlString>("feature01");

		///<summary>
		/// Feature 02
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		[ImplementPropertyType("feature02")]
		public IHtmlString Feature02 => this.Value<IHtmlString>("feature02");

		///<summary>
		/// Feature 03
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		[ImplementPropertyType("feature03")]
		public IHtmlString Feature03 => this.Value<IHtmlString>("feature03");

		///<summary>
		/// Featured Sections
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		[ImplementPropertyType("featuredSections")]
		public Newtonsoft.Json.Linq.JToken FeaturedSections => this.Value<Newtonsoft.Json.Linq.JToken>("featuredSections");

		///<summary>
		/// Save 20% Call-to-Action Panel
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		[ImplementPropertyType("infoLabel")]
		public Newtonsoft.Json.Linq.JToken InfoLabel => this.Value<Newtonsoft.Json.Linq.JToken>("infoLabel");

		///<summary>
		/// List of Testimonials
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		[ImplementPropertyType("listOfTestimonials")]
		public Newtonsoft.Json.Linq.JToken ListOfTestimonials => this.Value<Newtonsoft.Json.Linq.JToken>("listOfTestimonials");

		///<summary>
		/// Testimonials
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		[ImplementPropertyType("testimonials")]
		public IEnumerable<Umbraco.Core.Models.MediaWithCrops> Testimonials => this.Value<IEnumerable<Umbraco.Core.Models.MediaWithCrops>>("testimonials");

		///<summary>
		/// Add Querystring: *This will add a querystring to the end of the url.  NOTE!!  Do not add a ? and the string must be a valid querystring.  Example: utm_source=googleshop&utm_medium=cpc
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		[ImplementPropertyType("addQuerystring")]
		public string AddQuerystring => Navigation.GetAddQuerystring(this);

		///<summary>
		/// Navigation Title Override: *Optional: Overrides the title in the navigation only.
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		[ImplementPropertyType("navigationTitleOverride")]
		public string NavigationTitleOverride => Navigation.GetNavigationTitleOverride(this);

		///<summary>
		/// Show in Main Navigation: *Will appear in navigation ONLY if parents are selected to show in navigation!
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		[ImplementPropertyType("showInMainNavigation")]
		public bool ShowInMainNavigation => Navigation.GetShowInMainNavigation(this);

		///<summary>
		/// Meta Robots
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		[ImplementPropertyType("metaRobots")]
		public object MetaRobots => SEO.GetMetaRobots(this);

		///<summary>
		/// Redirects
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		[ImplementPropertyType("redirects")]
		public object Redirects => SEO.GetRedirects(this);

		///<summary>
		/// SEO Checker
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		[ImplementPropertyType("sEOChecker")]
		public SEOChecker.MVC.MetaData SEochecker => SEO.GetSEochecker(this);

		///<summary>
		/// SEO Description
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		[ImplementPropertyType("sEODescription")]
		public string SEodescription => SEO.GetSEodescription(this);

		///<summary>
		/// XMLSitemap
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		[ImplementPropertyType("xMLSitemap")]
		public object XMlsitemap => SEO.GetXMlsitemap(this);
	}
}
