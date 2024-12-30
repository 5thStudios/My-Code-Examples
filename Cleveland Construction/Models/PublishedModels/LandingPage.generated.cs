//------------------------------------------------------------------------------
// <auto-generated>
//   This code was generated by a tool.
//
//    Umbraco.ModelsBuilder.Embedded v13.5.1+2e068bb
//
//   Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Infrastructure.ModelsBuilder;
using Umbraco.Cms.Core;
using Umbraco.Extensions;

namespace www.Models.PublishedModels
{
	/// <summary>Landing Page</summary>
	[PublishedModel("LandingPage")]
	public partial class LandingPage : PublishedContentModel, IHero, INavigation, ISEO
	{
		// helpers
#pragma warning disable 0109 // new is redundant
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		public new const string ModelTypeAlias = "LandingPage";
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		[return: global::System.Diagnostics.CodeAnalysis.MaybeNull]
		public new static IPublishedContentType GetModelContentType(IPublishedSnapshotAccessor publishedSnapshotAccessor)
			=> PublishedModelUtility.GetModelContentType(publishedSnapshotAccessor, ModelItemType, ModelTypeAlias);
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		[return: global::System.Diagnostics.CodeAnalysis.MaybeNull]
		public static IPublishedPropertyType GetModelPropertyType<TValue>(IPublishedSnapshotAccessor publishedSnapshotAccessor, Expression<Func<LandingPage, TValue>> selector)
			=> PublishedModelUtility.GetModelPropertyType(GetModelContentType(publishedSnapshotAccessor), selector);
#pragma warning restore 0109

		private IPublishedValueFallback _publishedValueFallback;

		// ctor
		public LandingPage(IPublishedContent content, IPublishedValueFallback publishedValueFallback)
			: base(content, publishedValueFallback)
		{
			_publishedValueFallback = publishedValueFallback;
		}

		// properties

		///<summary>
		/// Main Content
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("mainContent")]
		public virtual global::Umbraco.Cms.Core.Models.Blocks.BlockGridModel MainContent => this.Value<global::Umbraco.Cms.Core.Models.Blocks.BlockGridModel>(_publishedValueFallback, "mainContent");

		///<summary>
		/// Hero Background Image: *Home pg uses 1903x435.  Projects use 1903x925. All others use 1903x350.
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("heroBackgroundImage")]
		public virtual global::Umbraco.Cms.Core.Models.MediaWithCrops HeroBackgroundImage => global::www.Models.PublishedModels.Hero.GetHeroBackgroundImage(this, _publishedValueFallback);

		///<summary>
		/// Hero Background Info
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("heroBackgroundInfo")]
		public virtual global::Newtonsoft.Json.Linq.JToken HeroBackgroundInfo => global::www.Models.PublishedModels.Hero.GetHeroBackgroundInfo(this, _publishedValueFallback);

		///<summary>
		/// Hero Background Video: *Use local .mov videos only.
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("heroBackgroundVideo")]
		public virtual global::Umbraco.Cms.Core.Models.MediaWithCrops HeroBackgroundVideo => global::www.Models.PublishedModels.Hero.GetHeroBackgroundVideo(this, _publishedValueFallback);

		///<summary>
		/// Hero Button: *Not used on a project pg.
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("heroButton")]
		public virtual global::Umbraco.Cms.Core.Models.Link HeroButton => global::www.Models.PublishedModels.Hero.GetHeroButton(this, _publishedValueFallback);

		///<summary>
		/// Hero Subtitle: *If Project Pg, if field left blank the Location field will be used.
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("heroSubtitle")]
		public virtual string HeroSubtitle => global::www.Models.PublishedModels.Hero.GetHeroSubtitle(this, _publishedValueFallback);

		///<summary>
		/// Hero Title: *If blank, the page name will be used instead.
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("heroTitle")]
		public virtual string HeroTitle => global::www.Models.PublishedModels.Hero.GetHeroTitle(this, _publishedValueFallback);

		///<summary>
		/// Optional Parameters
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("optionalParameters")]
		public virtual global::Newtonsoft.Json.Linq.JToken OptionalParameters => global::www.Models.PublishedModels.Hero.GetOptionalParameters(this, _publishedValueFallback);

		///<summary>
		/// Navigation Title Override
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("navigationTitleOverride")]
		public virtual string NavigationTitleOverride => global::www.Models.PublishedModels.Navigation.GetNavigationTitleOverride(this, _publishedValueFallback);

		///<summary>
		/// Show in Footer Main Nav: *Only 1st level pages can be in the foot main navigation.
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		[ImplementPropertyType("showInFooterMainNav")]
		public virtual bool ShowInFooterMainNav => global::www.Models.PublishedModels.Navigation.GetShowInFooterMainNav(this, _publishedValueFallback);

		///<summary>
		/// Show in Main Navigation
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		[ImplementPropertyType("showInMainNavigation")]
		public virtual bool ShowInMainNavigation => global::www.Models.PublishedModels.Navigation.GetShowInMainNavigation(this, _publishedValueFallback);

		///<summary>
		/// Show in Top Minor Nav
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		[ImplementPropertyType("showInTopMinorNav")]
		public virtual bool ShowInTopMinorNav => global::www.Models.PublishedModels.Navigation.GetShowInTopMinorNav(this, _publishedValueFallback);

		///<summary>
		/// Meta Robots
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("metaRobots")]
		public virtual global::SEOChecker.Common.DataTypeValues.SEOCheckerMetaRobotValues MetaRobots => global::www.Models.PublishedModels.SEO.GetMetaRobots(this, _publishedValueFallback);

		///<summary>
		/// Redirects
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("redirects")]
		public virtual string Redirects => global::www.Models.PublishedModels.SEO.GetRedirects(this, _publishedValueFallback);

		///<summary>
		/// SEO Checker
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("sEOChecker")]
		public virtual global::SEOChecker.Library.Models.MetaData SEochecker => global::www.Models.PublishedModels.SEO.GetSEochecker(this, _publishedValueFallback);

		///<summary>
		/// SEO Description: *optional
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("sEODescription")]
		public virtual string SEodescription => global::www.Models.PublishedModels.SEO.GetSEodescription(this, _publishedValueFallback);

		///<summary>
		/// Exclude from Sitemap
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		[ImplementPropertyType("umbracoNaviHide")]
		public virtual bool UmbracoNaviHide => global::www.Models.PublishedModels.SEO.GetUmbracoNaviHide(this, _publishedValueFallback);

		///<summary>
		/// XMLSitemap
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("xMLSitemap")]
		public virtual global::SEOChecker.Common.DataTypeValues.SEOCheckerXMLSitemapValues XMlsitemap => global::www.Models.PublishedModels.SEO.GetXMlsitemap(this, _publishedValueFallback);
	}
}