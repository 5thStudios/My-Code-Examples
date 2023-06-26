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
	/// <summary>Redirect to Page</summary>
	[PublishedModel("redirectToPage")]
	public partial class RedirectToPage : PublishedContentModel, INavigation
	{
		// helpers
#pragma warning disable 0109 // new is redundant
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		public new const string ModelTypeAlias = "redirectToPage";
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		public new static IPublishedContentType GetModelContentType()
			=> PublishedModelUtility.GetModelContentType(ModelItemType, ModelTypeAlias);
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		public static IPublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<RedirectToPage, TValue>> selector)
			=> PublishedModelUtility.GetModelPropertyType(GetModelContentType(), selector);
#pragma warning restore 0109

		// ctor
		public RedirectToPage(IPublishedContent content)
			: base(content)
		{ }

		// properties

		///<summary>
		/// Info
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		[ImplementPropertyType("info")]
		public Newtonsoft.Json.Linq.JToken Info => this.Value<Newtonsoft.Json.Linq.JToken>("info");

		///<summary>
		/// Redirect Page: *Mandatory: Page to redirect to.
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder", "8.1.6")]
		[ImplementPropertyType("redirectPage")]
		public IPublishedContent RedirectPage => this.Value<IPublishedContent>("redirectPage");

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
