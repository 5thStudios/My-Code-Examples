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
	/// <summary>Standard</summary>
	[PublishedModel("standard")]
	public partial class Standard : PublishedContentModel, INavigation
	{
		// helpers
#pragma warning disable 0109 // new is redundant
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		public new const string ModelTypeAlias = "standard";
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		public new static IPublishedContentType GetModelContentType()
			=> PublishedModelUtility.GetModelContentType(ModelItemType, ModelTypeAlias);
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		public static IPublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<Standard, TValue>> selector)
			=> PublishedModelUtility.GetModelPropertyType(GetModelContentType(), selector);
#pragma warning restore 0109

		// ctor
		public Standard(IPublishedContent content)
			: base(content)
		{ }

		// properties

		///<summary>
		/// Content
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		[ImplementPropertyType("content")]
		public IHtmlString Content => this.Value<IHtmlString>("content");

		///<summary>
		/// Meta Robots
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		[ImplementPropertyType("metaRobots")]
		public object MetaRobots => this.Value("metaRobots");

		///<summary>
		/// Product Description
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		[ImplementPropertyType("productDescription")]
		public string ProductDescription => this.Value<string>("productDescription");

		///<summary>
		/// Product Image
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		[ImplementPropertyType("productImage")]
		public Umbraco.Core.Models.MediaWithCrops ProductImage => this.Value<Umbraco.Core.Models.MediaWithCrops>("productImage");

		///<summary>
		/// Product Name
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		[ImplementPropertyType("productName")]
		public string ProductName => this.Value<string>("productName");

		///<summary>
		/// Product Price: *Note: Do NOT add the $ symbol.
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		[ImplementPropertyType("productPrice")]
		public string ProductPrice => this.Value<string>("productPrice");

		///<summary>
		/// Redirects
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		[ImplementPropertyType("redirects")]
		public object Redirects => this.Value("redirects");

		///<summary>
		/// SEO Checker
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		[ImplementPropertyType("sEOChecker")]
		public SEOChecker.MVC.MetaData SEochecker => this.Value<SEOChecker.MVC.MetaData>("sEOChecker");

		///<summary>
		/// SEO Description
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		[ImplementPropertyType("sEODescription")]
		public string SEodescription => this.Value<string>("sEODescription");

		///<summary>
		/// Show Product Snippet
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		[ImplementPropertyType("showProductSnippet")]
		public bool ShowProductSnippet => this.Value<bool>("showProductSnippet");

		///<summary>
		/// Subtitle: *Optional: Will add as an H2 tag under the H1 title.
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		[ImplementPropertyType("subtitle")]
		public string Subtitle => this.Value<string>("subtitle");

		///<summary>
		/// Title: *Optional: Will override name as the H1 tag.
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		[ImplementPropertyType("title")]
		public string Title => this.Value<string>("title");

		///<summary>
		/// XML Sitemap
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		[ImplementPropertyType("xMLSitemap")]
		public object XMlsitemap => this.Value("xMLSitemap");

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
	}
}
